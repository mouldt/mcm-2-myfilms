using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace MCM2MyFilms
{
    public class AMCMovie : INotifyPropertyChanged
    {
        private static System.Windows.Forms.ImageList ActionImages = new System.Windows.Forms.ImageList();
        static AMCMovie()
        {
            ActionImages.Images.Add("Unset", new Bitmap(128,128));
            ActionImages.Images.Add("Unchanged", new Bitmap(128, 128));
            ActionImages.Images.Add("Added", new Bitmap(MCM2MyFilms.Properties.Resources.ActionAdd));
            ActionImages.Images.Add("Deleted", new Bitmap(MCM2MyFilms.Properties.Resources.ActionDelete));
            ActionImages.Images.Add("Changed", new Bitmap(MCM2MyFilms.Properties.Resources.ActionRefresh));
        }

        public enum MovieStatus
        {
            Unknown,
            NoMovieFile,
            NotInCatalog,
            StaleData,
            OK
        }

        public enum ActionType
        {
            Unset,
            Unchanged,
            Changed,
            Added,
            Deleted
        }

        List<Image> backdrops = new List<Image>();
        public AMCMovie(string OriginalTitle)
        {
            Initialize();

            this.OriginalTitle = OriginalTitle;
            this.Status = MovieStatus.NotInCatalog;
        }

        public void Update(AMCMovie movie)
        {
            CopyAllValues(movie);
        }
        public AMCMovie(AMCMovie movie)
        {
            CopyAllValues(movie);
        }
        private void CopyAllValues(AMCMovie movie)
        {
            Number = movie.Number;
            Checked = movie.Checked;
            MediaLabel = movie.MediaLabel;
            MediaType = movie.MediaType;
            Source = movie.Source;
            Date = movie.Date;
            Borrower = movie.Borrower;
            Rating = movie.Rating;
            OriginalTitle = movie.OriginalTitle;
            TranslatedTitle = movie.TranslatedTitle;
            FormattedTitle = movie.FormattedTitle;
            Director = movie.Director;
            Producer = movie.Producer;
            Country = movie.Country;
            Category = movie.Category;
            Year = movie.Year;
            Length =movie.Length;
            Actors = movie.Actors;
            URL = movie.URL;
            Description = movie.Description;
            Comments = movie.Comments;
            VideoFormat = movie.VideoFormat;
            VideoBitrate = movie.VideoBitrate;
            AudioFormat = movie.AudioFormat;
            AudioBitrate = movie.AudioBitrate;
            Resolution = movie.Resolution;
            Framerate = movie.Framerate;
            Languages = movie.Languages;
            Subtitles = movie.Subtitles;
            Size = movie.Size;
            Disks = movie.Disks;

            Status = movie.Status;
            Action = movie.Action;
            fsMovie = movie.fsMovie;

            Certification = movie.Certification;
            Fanart = movie.Fanart;
            Studio = movie.Studio;
            Tagline = movie.Tagline;
            Writer = movie.Writer;
        }

        public AMCMovie(
                        string Number,
                        string Checked,
                        string MediaLabel,
                        string MediaType,
                        string Source,
                        string Date,
                        string Borrower,
                        string Rating,
                        string OriginalTitle,
                        string TranslatedTitle,
                        string FormattedTitle,
                        string Director,
                        string Producer,
                        string Country,
                        string Category,
                        string Year,
                        string Length,
                        string Actors,
                        string URL,
                        string Description,
                        string Comments,
                        string VideoFormat,
                        string VideoBitrate,
                        string AudioFormat,
                        string AudioBitrate,
                        string Resolution,
                        string Framerate,
                        string Languages,
                        string Subtitles,
                        string Size,
                        string Disks,
                        string LastUpdated,
                        string Certification,
                        string Fanart,
                        string Studio,
                        string Tagline,
                        string Writer
                       )
        {
            Initialize();

            this.Number = uint.Parse(Number);
            this.Checked = Boolean.Parse(Checked);
            this.MediaLabel = MediaLabel;
            this.MediaType = MediaType;
            this.Source = Source;
            this.Date = DateTime.Parse(Date);
            this.Borrower = Borrower;
            this.Rating = float.Parse(Rating);
            this.OriginalTitle = OriginalTitle;
            this.TranslatedTitle = TranslatedTitle;
            this.FormattedTitle = FormattedTitle;
            this.Director = Director;
            this.Producer = Producer;
            this.Country = Country;
            this.Category = Category;
            this.Year = int.Parse(Year);
            this.Length = int.Parse(Length);
            this.Actors = Actors;
            this.URL = URL;
            this.Description = Description;
            this.Comments = Comments;
            this.VideoFormat = VideoFormat;
            this.VideoBitrate = int.Parse(VideoBitrate);
            this.AudioFormat = AudioFormat;
            this.AudioBitrate = int.Parse(AudioBitrate);
            this.Resolution = Resolution;
            this.Framerate = float.Parse(Framerate);
            this.Languages = Languages;
            this.Subtitles = Subtitles;
            this.Size = long.Parse(Size);
            this.Disks = int.Parse(Disks);
            DateTime parsedDate;
            if (DateTime.TryParse(LastUpdated, out parsedDate))
                this.LastUpdated = parsedDate;
            this.Certification = Certification;
            this.Fanart = Fanart;
            this.Studio = Studio;
            this.Tagline = Tagline;
            this.Writer = Writer;
        }

        private void Initialize()
        {
            Number = 0;
            Checked = false;
            MediaLabel = String.Empty;
            MediaType = String.Empty;
            Source = String.Empty;
            Date = DateTime.Now;
            Borrower = String.Empty;
            Rating = 0;
            OriginalTitle = String.Empty;
            TranslatedTitle = String.Empty;
            FormattedTitle = String.Empty;
            Director = String.Empty;
            Producer = String.Empty;
            Country = String.Empty; 
            Category = String.Empty;
            Year = 0;
            Length = 0;
            Actors = String.Empty;
            URL = String.Empty;
            Description = String.Empty;
            Comments = String.Empty;
            VideoFormat = String.Empty;
            VideoBitrate = 0;
            AudioFormat = String.Empty; 
            AudioBitrate = 0;
            Resolution = String.Empty;
            Framerate = 0;
            Languages = String.Empty;
            Subtitles = String.Empty;
            Size = 0;
            Disks = 0;
            Picture = String.Empty;
            PictureImage = null;
            LastUpdated = DateTime.MinValue;

            Status = MovieStatus.Unknown;
            Action = ActionType.Unset;
            fsMovie = null;
            Certification = String.Empty;
            Fanart = String.Empty;
            Studio = String.Empty;
            Tagline = String.Empty;
            Writer = String.Empty;

        }
        private String _OriginalTitle;
        private uint _Number;
        private ActionType _Action;
        private Image _ActionImage;

        public uint Number
        {
            get { return _Number; }
            set
            {
                _Number = value;
                RaisePropertyChanged("Number");
            }
        }

        public String OriginalTitle
        {
            get { return _OriginalTitle; }
            set
            {
                _OriginalTitle = value;
                RaisePropertyChanged("OriginalTitle");
            }
        }
        public ActionType Action
        {
            get { return _Action; }
            set
            {
                _Action = value;
                ActionImage = ActionImages.Images[_Action.ToString()];
                RaisePropertyChanged("Action");
            }
        }

        public Image ActionImage
        {
            get { return _ActionImage; }
            private set
            {
                _ActionImage = value;
                RaisePropertyChanged("ActionImage");
            }
        }

        public bool Checked { get; set; }
        public String MediaLabel { get; set; }
        public String MediaType { get; set; }
        public String Source { get; set; }
        public DateTime Date { get; set; }
        public String Borrower { get; set; }
        public float Rating { get; set; }
        public String TranslatedTitle { get; set; }
        public String FormattedTitle { get; set; }
        public String Director { get; set; }
        public String Producer { get; set; }
        public String Country { get; set; }
        public String Category { get; set; }
        public int Year { get; set; }
        public int Length { get; set; }
        public String Actors { get; set; }
        public String URL { get; set; }
        public String Description { get; set; }
        public String Comments { get; set; }
        public String VideoFormat { get; set; }
        public int VideoBitrate { get; set; }
        public String AudioFormat { get; set; }
        public int AudioBitrate { get; set; }
        public String Resolution { get; set; }
        public float Framerate { get; set; }
        public String Languages { get; set; }
        public String Subtitles { get; set; }
        public long Size { get; set; }
        public int Disks { get; set; }
        public String Picture { get; set; }
        public Image PictureImage { get; set; }
        public List<Image> Backdrops { get { return backdrops; } }

        public MovieStatus Status { get; set; }
        public FileSystemMovie fsMovie { get; set; }
        public DateTime LastUpdated { get; set; }

        public String Certification { get; set; }
        public String Fanart { get; set; }
        public String Studio { get; set; }
        public String Tagline { get; set; }
        public String Writer { get; set; }

        public String NumberFormatted
        {
            get
            {
                return Number == 0 ? "" : Number.ToString();
            }
        }

        public String FrameRateFormatted
        {
            get
            {
                return String.Format("{0:0.000}", Framerate);
            }
        }

        private void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
