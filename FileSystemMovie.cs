using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MediaInfoLib;

namespace MCM2MyFilms
{
    public class FileSystemMovie
    {
        private static MediaInfo MI;
        public FileSystemMovie(string FileName)
        {
            this.FileName = FileName;
            this.Name = Path.GetFileNameWithoutExtension(FileName);
            FileInfo fi = new FileInfo(FileName);
            CreatedTime = fi.CreationTime;
            Size = Convert.ToInt32(Math.Round((Convert.ToDouble(fi.Length) / 1024.0) / 1024.0));

            LastUpdated = DateTime.MinValue;
            String filmDir = Path.GetDirectoryName(FileName);
            foreach (String infofilename in FindMovieDataFiles(Path.GetDirectoryName(FileName), new String[] {"mymovies.xml", "backdrop*.jpg"}))
	        {
                DateTime fileLastUpdated = File.GetLastWriteTime(infofilename);
                if (fileLastUpdated > LastUpdated)
                    LastUpdated = fileLastUpdated;
	        }
        }
        private IEnumerable<string> FindMovieDataFiles(string path, string[] searchPatterns, SearchOption searchOption = SearchOption.AllDirectories)
        {
            return searchPatterns.AsParallel().SelectMany(searchPattern => Directory.EnumerateFiles(path, searchPattern, searchOption));
        } 

        public void FillMovieInfo()
        {
            if (MI == null)
            {
                String ToDisplay;
                MI = new MediaInfo();

                ToDisplay = MI.Option("Info_Version", "0.7.5;MCM2MyFilms;1.0beta1");
                if (ToDisplay.Length == 0)
                    throw new InvalidProgramException("MediaInfo.Dll: this version of the DLL is not compatible");
            }


            MI.Open(FileName);
            String info = MI.Inform();
            String[] lines = info.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            String category = String.Empty;
            String propName;
            String propValue;
            foreach (var line in lines)
            {
                propName = String.Empty;
                propValue = String.Empty;

                if (!line.Contains(" :"))
                    category = line.Trim();
                else
                {
                    String[] parts = line.Split(new String[] { " :" }, StringSplitOptions.RemoveEmptyEntries);
                    propName = parts[0].Trim();
                    propValue = parts[1].Trim();
                }

                if (category == "General")
                {
                    if (propName == "Duration") // format eg = 1h 29mn
                    {
                        string[] hours = propValue.Split(new string[] { "h" }, StringSplitOptions.RemoveEmptyEntries);
                        string[] mins = hours[1].Split(new string[] { "mn" }, StringSplitOptions.RemoveEmptyEntries);
                        Length = Convert.ToInt32(hours[0]) * 60 + Convert.ToInt32(mins[0]);
                    }
                }
                if (category == "Video")
                {
                    if (propName == "Bit rate") // format eg = 1234 kbps
                    {
                        string[] tmp = propValue.Split(new string[] { " Kbps", " Mbps" }, StringSplitOptions.RemoveEmptyEntries);
                        if (propValue.Contains("Mbps"))
                            VideoBitrate = Convert.ToInt32(Convert.ToDouble(tmp[0].Replace(" ", String.Empty)) * 1000);
                        else
                            VideoBitrate = Convert.ToInt32(tmp[0].Replace(" ", String.Empty));
                    }
                    if (propName == "Format") // format eg = AVC
                        VideoFormat = propValue;
                    if (propName == "Width") // format eg = 1920x1080
                    {
                        string tmp = propValue.Split(new string[] { " pixels" }, StringSplitOptions.RemoveEmptyEntries)[0].Replace(" ", String.Empty);
                        Resolution = !String.IsNullOrEmpty(Resolution) ? tmp + Resolution : tmp;
                    }
                    if (propName == "Height") // format eg = 1920x1080
                    {
                        string tmp = propValue.Split(new string[] { " pixels" }, StringSplitOptions.RemoveEmptyEntries)[0].Replace(" ", String.Empty);
                        Resolution = (!String.IsNullOrEmpty(Resolution) ? Resolution : "") + "x" + tmp;
                    }
                    if (propName == "Frame rate") // format eg = 25.000 fps
                        Framerate = Convert.ToSingle(propValue.Split(new string[] { " fps" }, StringSplitOptions.RemoveEmptyEntries)[0].Replace(" ", String.Empty));
                }
                if (category == "Audio")
                {
                    if (propName == "Bit rate" && AudioBitrate == 0) // format eg = 1234 kbps
                        AudioBitrate = Convert.ToInt32(propValue.Split(new string[] { " Kbps" }, StringSplitOptions.RemoveEmptyEntries)[0].Replace(" ", String.Empty));
                    if (propName == "Format" && String.IsNullOrEmpty(AudioFormat)) // format eg = AC-3
                        AudioFormat = ConvertAudioFormat(propValue);
                    if (propName == "Language") // format eg = English
                        Languages = (!String.IsNullOrEmpty(Languages) ? Languages + "," : "") + propValue;
                }
                if (category == "Text")
                {
                    if (propName == "Language") // format eg = 1234 kbps
                        Subtitles = (!String.IsNullOrEmpty(Subtitles) ? Subtitles + "," : "") + propValue;
                }
            }

        }

        public String FileName { get; set; }
        public String Name { get; set; }
        public int Length { get; set; }
        public String VideoFormat { get; set; }
        public int VideoBitrate { get; set; }
        public String AudioFormat { get; set; }
        public int AudioBitrate { get; set; }
        public String Resolution { get; set; }
        public float Framerate { get; set; }
        public String Languages { get; set; }
        public String Subtitles { get; set; }
        public int Size { get; set; }
        public DateTime CreatedTime { get; set; }
        public bool IsInCatalog { get; set; }
        public DateTime LastUpdated { get; set; }

        public String FrameRateFormatted
        {
            get
            {
                return String.Format("{0:0.000}", Framerate);
            }
        }

        private String ConvertAudioFormat(String format)
        {
            if (format == "AC-3")
                return "AC3";
            return "";
        }
    }
}
