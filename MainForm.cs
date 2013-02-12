using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TheMovieDb;
using System.Xml.Linq;
using System.IO;
using MediaInfoLib;
using System.Threading;

namespace MCM2MyFilms
{
    public partial class MainForm : Form
    {
        List<AMCMovie> AMCmovies = null;
        IEnumerable<FileSystemMovie> fileSystemMovies;
        uint nMaxMovieSeqNumber = 0;
        ProgressUtil progressUtil = new ProgressUtil();
        Cursor savedCursor;
        SortStyle sortStyle = SortStyle.ByStatusFirst;
        SortColumns sortColumn = SortColumns.Title;
        SortDirection sortDirection = SortDirection.Ascending;

        private enum SortDirection
        {
            Ascending,
            Descending
        }
        private enum SortColumns
        {
            Number,
            Title
        }
        private enum SortStyle
        {
            ByStatusFirst,
            ByColumnOnly
        }

        public MainForm()
        {
            InitializeComponent();
            InitializeUI();
        }


        #region UI Behaviour

        private void InitializeUI()
        {
            moviesGrid_SelectionChanged(this, EventArgs.Empty);
        }

        private void SetUIAsBusy()
        {
            savedCursor = this.Cursor;
            Cursor = Cursors.WaitCursor;
            this.UseWaitCursor = true;
            this.Enabled = false;
        }
        private void SetUIAsAvailable()
        {
            Cursor = savedCursor;
            this.UseWaitCursor = false;
            this.Enabled = true;
        }

        private void moviesGrid_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            AMCMovie.MovieStatus currentStatus = ((AMCMovie)(moviesListBindingSource.List[e.RowIndex])).Status;
            switch (currentStatus)
            {
                case AMCMovie.MovieStatus.Unknown:
                    break;
                case AMCMovie.MovieStatus.NoMovieFile:
                    this.moviesGrid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
                    break;
                case AMCMovie.MovieStatus.NotInCatalog:
                    this.moviesGrid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Gold;
                    break;
                case AMCMovie.MovieStatus.StaleData:
                    this.moviesGrid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                    break;
                case AMCMovie.MovieStatus.OK:
                    this.moviesGrid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.DarkOliveGreen;
                    break;
                default:
                    break;
            }
        }

        private void moviesGrid_SelectionChanged(object sender, EventArgs e)
        {
            this.SuspendLayout();
            if (moviesGrid.SelectedRows.Count == 1)
            {
                AMCMovie currentMovie = moviesGrid.SelectedRows[0].DataBoundItem as AMCMovie;
                if (currentMovie.Status == AMCMovie.MovieStatus.NotInCatalog)
                {
                    btnAddEdit.Text = "Add";
                    btnAddEdit.Enabled = true;
                    btnDelete.Enabled = false;
                    btnUndo.Enabled = (currentMovie.Action != AMCMovie.ActionType.Unchanged && currentMovie.Action != AMCMovie.ActionType.Unset);
                }
                else if (currentMovie.Status == AMCMovie.MovieStatus.NoMovieFile)
                {
                    btnAddEdit.Text = "Add/Refresh";
                    btnAddEdit.Enabled = false;
                    btnDelete.Enabled = true;
                    btnUndo.Enabled = (currentMovie.Action != AMCMovie.ActionType.Unchanged && currentMovie.Action != AMCMovie.ActionType.Unset);
                }
                else if (currentMovie.Status == AMCMovie.MovieStatus.OK || currentMovie.Status == AMCMovie.MovieStatus.StaleData)
                {
                    btnAddEdit.Text = "Refresh";
                    btnAddEdit.Enabled = true;
                    btnDelete.Enabled = true;
                    btnUndo.Enabled = (currentMovie.Action != AMCMovie.ActionType.Unchanged && currentMovie.Action != AMCMovie.ActionType.Unset);
                }
            }
            else
            {
                if (moviesGrid.SelectedRows.Count == 0)
                {
                    btnAddEdit.Text = "Add/Edit";
                    btnAddEdit.Enabled = false;
                    btnDelete.Enabled = false;
                    btnUndo.Enabled = false;
                }
                else
                {
                    if (!btnUndo.Enabled)
                    {
                        AMCMovie currentMovie = moviesGrid.SelectedRows[0].DataBoundItem as AMCMovie;
                        btnUndo.Enabled = (currentMovie.Action != AMCMovie.ActionType.Unchanged && currentMovie.Action != AMCMovie.ActionType.Unset);
                    }
                }
            }
            this.ResumeLayout();
        }

        #endregion

        #region UI Actions

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmHelp frm = new frmHelp();
            frm.ShowDialog();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppSettings frm = new AppSettings();
            DialogResult res = frm.ShowDialog(this);
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAddEdit_Click(object sender, EventArgs e)
        {
            this.SuspendLayout();
            if (moviesGrid.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow currentRow in moviesGrid.SelectedRows)
                {
                    AMCMovie currentMovie = (AMCMovie)currentRow.DataBoundItem;
                    if (currentMovie.Status == AMCMovie.MovieStatus.NotInCatalog)
                        currentMovie.Action = AMCMovie.ActionType.Added;
                    else
                        currentMovie.Action = AMCMovie.ActionType.Changed;                    
                }
            }
            this.ResumeLayout();
            return;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            this.SuspendLayout();
            if (moviesGrid.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow currentRow in moviesGrid.SelectedRows)
                {
                    AMCMovie currentMovie = (AMCMovie)currentRow.DataBoundItem;
                    currentMovie.Action = AMCMovie.ActionType.Deleted;
                }
            }
            this.ResumeLayout();
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            this.SuspendLayout();
            if (moviesGrid.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow currentRow in moviesGrid.SelectedRows)
                {
                    AMCMovie currentMovie = (AMCMovie)currentRow.DataBoundItem;
                    currentMovie.Action = AMCMovie.ActionType.Unchanged;
                }
            }
            this.ResumeLayout();
        }

        private void rescanFoldersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                SetUIAsBusy();

                AMCmovies = LoadMovieCatalog();
                if (AMCmovies != null)
                    fileSystemMovies = LoadMoviesFromDirs();

                if (fileSystemMovies != null)
                {
                    nMaxMovieSeqNumber = 0;
                    foreach (AMCMovie movie in AMCmovies)
                    {
                        var filemoviematches = from filemovie in fileSystemMovies
                                               where String.Compare(filemovie.FileName, movie.Source, true) == 0
                                               select filemovie;
                        if (filemoviematches.Count() > 0)
                        {
                            movie.fsMovie = filemoviematches.First();
                            movie.fsMovie.IsInCatalog = true;

                            if (movie.fsMovie.LastUpdated > GetAMCBackdropsFolderLastUpdated(movie))
                                movie.Status = AMCMovie.MovieStatus.StaleData;
                            else
                                movie.Status = AMCMovie.MovieStatus.OK;
                        }
                        else // not found so must be removed
                        {
                            movie.Status = AMCMovie.MovieStatus.NoMovieFile;
                        }
                        nMaxMovieSeqNumber = movie.Number > nMaxMovieSeqNumber ? movie.Number : nMaxMovieSeqNumber;
                        movie.Action = AMCMovie.ActionType.Unchanged;
                    }

                    var newMovies = from filemovie in fileSystemMovies
                                    where filemovie.IsInCatalog == false
                                    select filemovie;
                    foreach (var filemovie in newMovies)
                    {
                        AMCMovie newmovie = new AMCMovie(filemovie.Name);
                        AMCmovies.Add(newmovie);
                        newmovie.fsMovie = filemovie;
                    }

                    SortMovies();
                    BindingList<AMCMovie> boundList = new BindingList<AMCMovie>(AMCmovies);

                    this.moviesListBindingSource.DataSource = null;
                    moviesListBindingSource.Position = -1;
                    this.moviesListBindingSource.DataSource = boundList;
                }
            }
            finally
            {
                SetUIAsAvailable();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.StartUpdates();
            return;
        }

        private void moviesGrid_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            switch (e.ColumnIndex)
            {
                case 1:
                    if (sortColumn == SortColumns.Number)
                        // Change Direction
                        sortDirection = sortDirection == SortDirection.Ascending ? SortDirection.Descending : SortDirection.Ascending;
                    else
                    {
                        sortColumn = SortColumns.Number;
                        sortDirection = SortDirection.Ascending;
                    }
                    break;
                case 2:
                    if (sortColumn == SortColumns.Title)
                        // Change Direction
                        sortDirection = sortDirection == SortDirection.Ascending ? SortDirection.Descending : SortDirection.Ascending;
                    else
                    {
                        sortColumn = SortColumns.Title;
                        sortDirection = SortDirection.Ascending;
                    }
                    break;
            }

            SortMovies();
        }

        private void moviesGrid_ColumnHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            switch (sortStyle)
            {
                case SortStyle.ByStatusFirst:
                    sortStyle = SortStyle.ByColumnOnly;
                    break;
                case SortStyle.ByColumnOnly:
                    sortStyle = SortStyle.ByStatusFirst;
                    break;
            }

            switch (e.ColumnIndex)
            {
                case 1:
                    if (sortColumn != SortColumns.Number)
                    {
                        sortColumn = SortColumns.Number;
                        sortDirection = SortDirection.Ascending;
                    }
                    break;
                case 2:
                    if (sortColumn != SortColumns.Title)
                    {
                        sortColumn = SortColumns.Title;
                        sortDirection = SortDirection.Ascending;
                    }
                    break;
            }

            SortMovies();
        }

        #endregion

        #region Movie Loading

        private IEnumerable<FileSystemMovie> LoadMoviesFromDirs()
        {
            if (String.IsNullOrEmpty(Properties.Settings.Default.FilmDirs))
            {
                HandleError(new Exception("Film directory must be configured in settings."));
                return null;
            }

            String[] movieExtensions = Properties.Settings.Default.FilmExtensions.Split(new char[] {','});

            List<FileSystemMovie> filemovies = new List<FileSystemMovie>();
            foreach (String fileName in FindMovieFiles(Properties.Settings.Default.FilmDirs, movieExtensions))
            {
                filemovies.Add(new FileSystemMovie(fileName));
            }
            return filemovies;
        }

        private IEnumerable<string> FindMovieFiles(string path, string[] searchPatterns, SearchOption searchOption = SearchOption.AllDirectories)
        {
            return searchPatterns.AsParallel().SelectMany(searchPattern => Directory.EnumerateFiles(path, searchPattern, searchOption));
        } 

        private MCMMovie LoadMCMMovie(string Filename)
        {
            String movieDir = Path.GetDirectoryName(Filename);
            return new MCMMovie(movieDir);
        }

        private List<AMCMovie> LoadMovieCatalog()
        {
            if (String.IsNullOrEmpty(Properties.Settings.Default.AMCCatalog))
            {
                HandleError(new Exception("AMC Catalog file must be configured in settings."));
                return null;
            }
            XDocument catalog = XDocument.Load(Properties.Settings.Default.AMCCatalog);
            var movies = from movie in catalog.Descendants("Movie")
                         select new AMCMovie(
                                                (string)movie.Attribute("Number"),
                                                (string)movie.Attribute("Checked"),
                                                (string)movie.Attribute("MediaLabel"),
                                                (string)movie.Attribute("MediaType"),
                                                (string)movie.Attribute("Source"),
                                                (string)movie.Attribute("Date"),
                                                (string)movie.Attribute("Borrower"),
                                                (string)movie.Attribute("Rating"),
                                                (string)movie.Attribute("OriginalTitle"),
                                                (string)movie.Attribute("TranslatedTitle"),
                                                (string)movie.Attribute("FormattedTitle"),
                                                (string)movie.Attribute("Director"),
                                                (string)movie.Attribute("Producer"),
                                                (string)movie.Attribute("Country"),
                                                (string)movie.Attribute("Category"),
                                                (string)movie.Attribute("Year"),
                                                (string)movie.Attribute("Length"),
                                                (string)movie.Attribute("Actors"),
                                                (string)movie.Attribute("URL"),
                                                (string)movie.Attribute("Description"),
                                                (string)movie.Attribute("Comments"),
                                                (string)movie.Attribute("VideoFormat"),
                                                (string)movie.Attribute("VideoBitrate"),
                                                (string)movie.Attribute("AudioFormat"),
                                                (string)movie.Attribute("AudioBitrate"),
                                                (string)movie.Attribute("Resolution"),
                                                (string)movie.Attribute("Framerate"),
                                                (string)movie.Attribute("Languages"),
                                                (string)movie.Attribute("Subtitles"),
                                                (string)movie.Attribute("Size"),
                                                (string)movie.Attribute("Disks"),
                                                movie.Element("CustomFields") != null ? (string)movie.Element("CustomFields").Attribute("LastUpdated") : String.Empty,
                                                movie.Element("CustomFields") != null ? (string)movie.Element("CustomFields").Attribute("Certification") : String.Empty,
                                                movie.Element("CustomFields") != null ? (string)movie.Element("CustomFields").Attribute("Fanart") : String.Empty,
                                                movie.Element("CustomFields") != null ? (string)movie.Element("CustomFields").Attribute("Studio") : String.Empty,
                                                movie.Element("CustomFields") != null ? (string)movie.Element("CustomFields").Attribute("TagLine") : String.Empty,
                                                movie.Element("CustomFields") != null ? (string)movie.Element("CustomFields").Attribute("Writer") : String.Empty,
                                                movie.Element("CustomFields") != null ? (string)movie.Element("CustomFields").Attribute("MultiUserState") : String.Empty
                                            );
            return movies.ToList<AMCMovie>();
        }

        #endregion

        #region UpdateProcess

        private void StartUpdates()
        {
            if (String.IsNullOrEmpty(Properties.Settings.Default.MePoThumbsDir))
            {
                HandleError(new Exception("MyFilms Fanart directory must be configured in settings."));
                return;
            }

            SetUIAsBusy();
            try
            {
                frmProgress dlg = new frmProgress(progressUtil);
                dlg.action = (object cancel) =>
                {
                    DoUpdates((CancellationToken)cancel);
                };
                dlg.ShowDialog(this);
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
            finally
            {
                SetUIAsAvailable();
            }

            rescanFoldersToolStripMenuItem_Click(this, EventArgs.Empty);
        }

        private void DoUpdates(CancellationToken cancellationToken)
        {
            AMCmovies.Sort(SortMoviesByActionDesc); // Ensures we do deletes first
            List<AMCMovie> ChangedMovies = new List<AMCMovie>();
            foreach (AMCMovie movie in AMCmovies)
            {
                if (movie.Action == AMCMovie.ActionType.Added || 
                    movie.Action == AMCMovie.ActionType.Changed || 
                    movie.Action == AMCMovie.ActionType.Deleted)
                    ChangedMovies.Add(movie);
            }

            progressUtil.RaiseSetupProgressInfo(0, ChangedMovies.Count, false);
            int RowCount = 0;
            BackupAMCCatalog();

            foreach (AMCMovie movie in ChangedMovies)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Perform Action
                switch (movie.Action)
                {
                    case AMCMovie.ActionType.Added:
                        PerformAddition(movie);
                        break;
                    case AMCMovie.ActionType.Changed:
                        PerformRefresh(movie);
                        break;
                    case AMCMovie.ActionType.Deleted:
                        PerformDeletion(movie);
                        break;
                }

                progressUtil.RaisePositionProgressInfo(++RowCount);
            }

        }

        private void HandleError(Exception ex)
        {
            String strMessage = ex.InnerException == null ? ex.Message : String.Format("{0}\nInner Message: {1}", ex.Message, ex.InnerException.Message);
            MessageBox.Show(strMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private XElement CreatexMovie(AMCMovie movie)
        {
            XElement xMovie = new XElement("Movie",
                        new XAttribute("Number", movie.Number),
                        new XAttribute("Checked", movie.Checked),
                        new XAttribute("MediaLabel", GetSafeString(movie.MediaLabel)),
                        new XAttribute("MediaType", GetSafeString(movie.MediaType)),
                        new XAttribute("Source", GetSafeString(movie.Source)),
                        new XAttribute("Date", GetSafeString(movie.Date.ToString("dd/MM/yyyy"))),
                        new XAttribute("Borrower", GetSafeString(movie.Borrower)),
                        new XAttribute("Rating", movie.Rating),
                        new XAttribute("OriginalTitle", GetSafeString(movie.OriginalTitle)),
                        new XAttribute("TranslatedTitle", GetSafeString(movie.TranslatedTitle)),
                        new XAttribute("FormattedTitle", GetSafeString(movie.FormattedTitle)),
                        new XAttribute("Director", GetSafeString(movie.Director)),
                        new XAttribute("Producer", GetSafeString(movie.Producer)),
                        new XAttribute("Country", GetSafeString(movie.Country)),
                        new XAttribute("Category", GetSafeString(movie.Category)),
                        new XAttribute("Year", movie.Year),
                        new XAttribute("Length", movie.Length),
                        new XAttribute("Actors", GetSafeString(movie.Actors)),
                        new XAttribute("URL", GetSafeString(movie.URL)),
                        new XAttribute("Description", GetSafeString(movie.Description)),
                        new XAttribute("Comments", GetSafeString(movie.Comments)),
                        new XAttribute("VideoFormat", GetSafeString(movie.VideoFormat)),
                        new XAttribute("VideoBitrate", movie.VideoBitrate),
                        new XAttribute("AudioFormat", GetSafeString(movie.AudioFormat)),
                        new XAttribute("AudioBitrate", movie.AudioBitrate),
                        new XAttribute("Resolution", GetSafeString(movie.Resolution)),
                        new XAttribute("Framerate", GetSafeString(movie.FrameRateFormatted)),
                        new XAttribute("Languages", GetSafeString(movie.Languages)),
                        new XAttribute("Subtitles", GetSafeString(movie.Subtitles)),
                        new XAttribute("Size", movie.Size),
                        new XAttribute("Disks", movie.Disks),
                        new XAttribute("Picture", movie.Picture),
                        new XElement("CustomFields", 
                                        new XAttribute("LastUpdated", DateTime.Now.ToString("u")),
                                        new XAttribute("Certification", movie.Certification),
                                        new XAttribute("Studio", movie.Studio),
                                        new XAttribute("Writer", movie.Writer),
                                        new XAttribute("Fanart", movie.Fanart),
                                        new XAttribute("TagLine", movie.Tagline),
                                        new XAttribute("MultiUserState", movie.MultiUserState)
                                    )
                        );
            return xMovie;
        }

        private XDocument GetAMCCatalog()
        {
            String AMCCatDir = Path.GetDirectoryName(Properties.Settings.Default.AMCCatalog);
            XDocument catalog = XDocument.Load(Properties.Settings.Default.AMCCatalog);
            return catalog;
        }

        private XElement GetAMCCatalogContents(XDocument catalog)
        {
            XElement contents = catalog.Descendants("Contents").First();
            return contents;
        }

        private void BackupAMCCatalog()
        {
            String AMCCatDir = Path.GetDirectoryName(Properties.Settings.Default.AMCCatalog);
            String backupfilename = Path.Combine(AMCCatDir, Path.GetFileNameWithoutExtension(Properties.Settings.Default.AMCCatalog) + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".xml");
            File.Copy(Properties.Settings.Default.AMCCatalog, backupfilename);
        }

        private void SaveAMCCatalog(XDocument AMCcatalog)
        {
            // Check for LastUpdated Custom field
            var customfieldsquery = from customfield in AMCcatalog.Descendants()
                               where customfield.Name.LocalName == "CustomFieldsProperties"
                               select customfield;
            var lastupdatedquery = from customfield in AMCcatalog.Descendants("CustomField")
                                   where (string)customfield.Attribute("Tag") == "LastUpdated"
                                   select customfield;
            XElement customfields = customfieldsquery.SingleOrDefault();
            XElement lastupdated = lastupdatedquery.SingleOrDefault();
            if (customfields == null || lastupdated == null)
            {
                XElement catalog = AMCcatalog.Descendants("Catalog").Single();
                if (customfields == null)
                {
                    customfields = new XElement("CustomFieldsProperties");
                    catalog.Add(customfields);
                }

                if (lastupdated == null)
                {
                    lastupdated = new XElement("CustomField", 
                                                new XAttribute("Tag", "LastUpdated"),
                                                new XAttribute("Name", "LastUpdated"),
                                                new XAttribute("Type", "ftString")
                                                );
                    customfields.Add(lastupdated);
                }
            }

            AMCcatalog.Save(Properties.Settings.Default.AMCCatalog);
        }

        private void DeleteBackdrops(AMCMovie movie)
        {
            // cleanup existing details
            String backdropsDir = Path.Combine(Properties.Settings.Default.MePoThumbsDir, "{" + FormatAsThumbsDirectoryName(movie.OriginalTitle) + "}");
            try { Directory.Delete(backdropsDir, true); }
            catch (DirectoryNotFoundException) { }
        }

        private void CreateBackdrops(AMCMovie movie)
        {
            // cleanup existing details
            DeleteBackdrops(movie);

            String backdropsDir = Path.Combine(Properties.Settings.Default.MePoThumbsDir, "{" + FormatAsThumbsDirectoryName(movie.OriginalTitle) + "}");
            Directory.CreateDirectory(backdropsDir);
            string[] backdrops = Directory.GetFiles(Path.GetDirectoryName(movie.fsMovie.FileName), "backdrop*.jpg");
            foreach (string file in backdrops)
            {
                string newFile = Path.Combine(backdropsDir, Path.GetFileName(file));
                File.Copy(file, newFile);
                File.SetLastWriteTime(newFile, DateTime.Now);
            }
        }

        private void PerformAddition(AMCMovie movie)
        {
            FillMovieMetaData(movie);

            this.nMaxMovieSeqNumber++;
            movie.Number = this.nMaxMovieSeqNumber;

            XDocument catalog = GetAMCCatalog();
            XElement contents = GetAMCCatalogContents(catalog);
            XElement xMovie = CreatexMovie(movie);
            contents.Add(xMovie);

            // Now Deal with ArtWork
            CreateBackdrops(movie);

            // Finally Save the Catalog
            SaveAMCCatalog(catalog);
        }

        private void PerformRefresh(AMCMovie movie)
        {
            FillMovieMetaData(movie);

            XDocument catalog = GetAMCCatalog();
            XElement contents = GetAMCCatalogContents(catalog);
            XElement xMovie = CreatexMovie(movie);
            contents.Descendants("Movie").Where(x => uint.Parse(x.Attribute("Number").Value) == movie.Number).Remove();
            contents.Add(xMovie);

            // Now Deal with ArtWork
            CreateBackdrops(movie);

            // Finally Save the Catalog
            SaveAMCCatalog(catalog);
        }

        private void PerformDeletion(AMCMovie movie)
        {
            XDocument catalog = GetAMCCatalog();
            XElement contents = GetAMCCatalogContents(catalog);
            //XElement xMovie = CreatexMovie(movie);
            contents.Descendants("Movie").Where(x => uint.Parse(x.Attribute("Number").Value) == movie.Number).Remove();

            // Now Deal with ArtWork
            DeleteBackdrops(movie);

            // Finally Save the Catalog
            SaveAMCCatalog(catalog);
        }

        private void FillMovieMetaData(AMCMovie movie)
        {
            movie.fsMovie.FillMovieInfo();
            movie.Source = movie.fsMovie.FileName;
            movie.Length = movie.fsMovie.Length;
            movie.VideoFormat = movie.fsMovie.VideoFormat;
            movie.VideoBitrate = movie.fsMovie.VideoBitrate;
            movie.AudioFormat = movie.fsMovie.AudioFormat;
            movie.AudioBitrate = movie.fsMovie.AudioBitrate;
            movie.Resolution = movie.fsMovie.Resolution;
            movie.Framerate = movie.fsMovie.Framerate;
            movie.Languages = movie.fsMovie.Languages;
            movie.Subtitles = movie.fsMovie.Subtitles;
            movie.Size = movie.fsMovie.Size;
            movie.Date = movie.fsMovie.CreatedTime;

            movie.MediaLabel = "HDD";
            movie.MediaType = "MCM2MF";

            MCMMovie mcmMovie = LoadMCMMovie(movie.fsMovie.FileName);

            movie.Rating = mcmMovie.IMDBrating;
            movie.OriginalTitle = mcmMovie.LocalTitle;
            movie.TranslatedTitle = mcmMovie.OriginalTitle;
            movie.FormattedTitle = movie.OriginalTitle + " (" + movie.TranslatedTitle + ")";
            movie.Country = mcmMovie.Country;

            movie.Category = String.Empty;
            foreach (string genre in mcmMovie.Genres)
            {
                if (movie.Category.Length > 0)
                    movie.Category = movie.Category + ", ";
                movie.Category = movie.Category + genre;
            }

            movie.Year = mcmMovie.ProductionYear;
            movie.Actors = String.Empty;
            movie.Director = String.Empty;
            movie.Producer = String.Empty;
            foreach (MCMMovie.Person actor in mcmMovie.Persons)
            {
                if (actor.Type == MCMMovie.Person.PersonType.Actor)
                {
                    if (movie.Actors.Length > 0)
                        movie.Actors = movie.Actors + ", ";
                    movie.Actors = movie.Actors + actor.Name + " (" + actor.Role + ")";
                }
                if (actor.Type == MCMMovie.Person.PersonType.Director)
                {
                    if (movie.Director.Length > 0)
                        movie.Director = movie.Director + ", ";
                    movie.Director = movie.Director + actor.Name;
                }
                
                if (actor.Type == MCMMovie.Person.PersonType.Producer)
                {
                    if (movie.Producer.Length > 0)
                        movie.Producer = movie.Producer + ", ";
                    movie.Producer = movie.Producer + actor.Name;
                }
                
            }
            //if (!String.IsNullOrWhiteSpace(mcmMovie.Tagline))
            //    movie.Description = "\"" + mcmMovie.Tagline + "\"" + Environment.NewLine + mcmMovie.Description;
            //else
            movie.Description = mcmMovie.Description;
            movie.Comments = "";
            movie.URL = "http://akas.imdb.com/title/" + mcmMovie.IMDBId;
            movie.Disks = 1;
            movie.Certification = mcmMovie.Certification;
            movie.Studio = string.Join(", ", mcmMovie.Studios);
            movie.Writer = mcmMovie.WritersList;
            movie.Tagline = mcmMovie.Tagline;
            movie.Fanart = mcmMovie.BackdropURL;

            //movie.Picture = RelativePath(Path.Combine(Path.GetDirectoryName(movie.fsMovie.FileName), "folder.jpg"), Path.GetDirectoryName(Properties.Settings.Default.AMCCatalog));
            movie.Picture = Path.Combine(Path.GetDirectoryName(movie.fsMovie.FileName), "folder.jpg"); 

        }

        #endregion

        #region Utilities

        private DateTime GetAMCBackdropsFolderLastUpdated(AMCMovie movie)
        {
            DateTime LastUpdated = DateTime.MinValue;
            String backdropsDir = Path.Combine(Properties.Settings.Default.MePoThumbsDir, "{" + FormatAsThumbsDirectoryName(movie.OriginalTitle) + "}");

            if (Directory.Exists(backdropsDir))
            {
                foreach (String infofilename in FindMovieDataFiles(backdropsDir, new String[] { "backdrop*.jpg" }))
                {
                    DateTime fileLastUpdated = File.GetLastWriteTime(infofilename);
                    if (fileLastUpdated > LastUpdated)
                        LastUpdated = fileLastUpdated;
                }
            }
            return LastUpdated;
        }

        private IEnumerable<string> FindMovieDataFiles(string path, string[] searchPatterns, SearchOption searchOption = SearchOption.AllDirectories)
        {
            return searchPatterns.AsParallel().SelectMany(searchPattern => Directory.EnumerateFiles(path, searchPattern, searchOption));
        } 

        private String SafeFileName(String name)
        {
            return name.Replace('\\', '_')
            .Replace('/', '_')
            .Replace('*', '_')
            .Replace(':', '_')
            .Replace('?', '_')
            .Replace('"', '_')
            .Replace('<', '_')
            .Replace('>', '_')
            .Replace('|', '_');
        }

        private String FormatAsThumbsDirectoryName(String name)
        {
            return name.Replace(' ', '.')
            .Replace('\\', '_')
            .Replace('/', '_')
            .Replace('*', '_')
            .Replace(':', '_')
            .Replace('?', '_')
            .Replace('"', '_')
            .Replace('<', '_')
            .Replace('>', '_')
            .Replace('|', '_')
            .ToLower();
        }

        private string GetSafeString(String value)
        {
            return String.IsNullOrEmpty(value) ? String.Empty : value;
        }

        private string RelativePath(string absolutePath, string relativeTo)        
        {            
            string[] absoluteDirectories = absolutePath.Split('\\');            
            string[] relativeDirectories = relativeTo.Split('\\');            
            //Get the shortest of the two paths            
            int length = absoluteDirectories.Length < relativeDirectories.Length ? absoluteDirectories.Length : relativeDirectories.Length;            
            
            //Use to determine where in the loop we exited            
            int lastCommonRoot = -1;            
            int index;            
            
            //Find common root            
            for (index = 0; index < length; index++)                
                if (absoluteDirectories[index] == relativeDirectories[index])                    
                    lastCommonRoot = index;                
                else                    
                    break;            

            //If we didn't find a common prefix then throw            
            if (lastCommonRoot == -1)
                return absolutePath;           
            
            //Build up the relative path            
            StringBuilder relativePath = new StringBuilder();            
            
            //Add on the ..            
            for (index = lastCommonRoot + 1; index < relativeDirectories.Length; index++)
                if (relativeDirectories[index].Length > 0)                    
                    relativePath.Append("..\\");  
          
            //Add on the folders            
            for (index = lastCommonRoot + 1; index < absoluteDirectories.Length - 1; index++)
                relativePath.Append(absoluteDirectories[index] + "\\");
            relativePath.Append(absoluteDirectories[absoluteDirectories.Length - 1]);            
            return relativePath.ToString();        
        }

        #endregion

        #region Movie Sorting
        private int CompareMoviesByStatus(AMCMovie x, AMCMovie y)
        {
            int cmpVal = 0;
            switch (x.Status)
            {
                case AMCMovie.MovieStatus.NotInCatalog:
                    if (y.Status == AMCMovie.MovieStatus.NotInCatalog)
                        cmpVal = 0;
                    if (y.Status == AMCMovie.MovieStatus.NoMovieFile)
                        cmpVal = -1;
                    if (y.Status == AMCMovie.MovieStatus.StaleData)
                        cmpVal = -1;
                    if( y.Status == AMCMovie.MovieStatus.OK)
                        cmpVal = -1;
                    break;
                case AMCMovie.MovieStatus.NoMovieFile:
                    if (y.Status == AMCMovie.MovieStatus.NotInCatalog)
                        cmpVal = 1;
                    if (y.Status == AMCMovie.MovieStatus.NoMovieFile)
                        cmpVal = 0;
                    if (y.Status == AMCMovie.MovieStatus.StaleData)
                        cmpVal = -1;
                    if (y.Status == AMCMovie.MovieStatus.OK)
                        cmpVal = -1;
                    break;
                case AMCMovie.MovieStatus.StaleData:
                    if (y.Status == AMCMovie.MovieStatus.NotInCatalog)
                            cmpVal = 1;
                    if (y.Status == AMCMovie.MovieStatus.NoMovieFile)
                        cmpVal = 1;
                    if (y.Status == AMCMovie.MovieStatus.StaleData)
                        cmpVal = 0;
                    if (y.Status == AMCMovie.MovieStatus.OK)
                        cmpVal = -1;
                    break;
                case AMCMovie.MovieStatus.OK:
                    if (y.Status == AMCMovie.MovieStatus.NotInCatalog)
                        cmpVal = 1;
                    if (y.Status == AMCMovie.MovieStatus.NoMovieFile)
                        cmpVal = 1;
                    if (y.Status == AMCMovie.MovieStatus.StaleData)
                        cmpVal = 1;
                    if (y.Status == AMCMovie.MovieStatus.OK)
                        cmpVal = 0;
                    break;
            }
            return cmpVal;
        }

        private int SortMoviesByStatusThenNumberAsc(AMCMovie x, AMCMovie y)
        {
            int cmpVal = 0;
            cmpVal = CompareMoviesByStatus(x, y);

            // secondary sort
            if (cmpVal == 0)
                cmpVal = x.Number > y.Number ? 1 : x.Number == y.Number ? 0 : -1;

            return cmpVal;
        }

        private int SortMoviesByStatusThenNumberDesc(AMCMovie x, AMCMovie y)
        {
            int cmpVal = 0;
            cmpVal = CompareMoviesByStatus(x, y);

            // secondary sort
            if (cmpVal == 0)
                cmpVal = x.Number > y.Number ? -1 : x.Number == y.Number ? 0 : 1;

            return cmpVal;
        }

        private int SortMoviesByNumberAsc(AMCMovie x, AMCMovie y)
        {
            int cmpVal = 0;
            if (cmpVal == 0)
                cmpVal = x.Number > y.Number ? 1 : x.Number == y.Number ? 0 : -1;

            return cmpVal;
        }

        private int SortMoviesByNumberDesc(AMCMovie x, AMCMovie y)
        {
            int cmpVal = 0;
            if (cmpVal == 0)
                cmpVal = x.Number > y.Number ? -1 : x.Number == y.Number ? 0 : 1;

            return cmpVal;
        }

        private int SortMoviesByStatusThenTitleAsc(AMCMovie x, AMCMovie y)
        {
            int cmpVal = 0;
            cmpVal = CompareMoviesByStatus(x, y);

            // secondary sort
            if (cmpVal == 0)
                cmpVal = String.Compare(x.OriginalTitle, y.OriginalTitle);

            return cmpVal;
        }

        private int SortMoviesByStatusThenTitleDesc(AMCMovie x, AMCMovie y)
        {
            int cmpVal = 0;
            cmpVal = CompareMoviesByStatus(x, y);

            // secondary sort
            if (cmpVal == 0)
                cmpVal = String.Compare(y.OriginalTitle, x.OriginalTitle);

            return cmpVal;
        }

        private int SortMoviesByTitleAsc(AMCMovie x, AMCMovie y)
        {
            int cmpVal = 0;
            if (cmpVal == 0)
                cmpVal = String.Compare(x.OriginalTitle, y.OriginalTitle);

            return cmpVal;
        }

        private int SortMoviesByTitleDesc(AMCMovie x, AMCMovie y)
        {
            int cmpVal = 0;
            if (cmpVal == 0)
                cmpVal = String.Compare(y.OriginalTitle, x.OriginalTitle);

            return cmpVal;
        }

        private int SortMoviesByActionDesc(AMCMovie x, AMCMovie y)
        {
            int cmpVal = 0;
            if (cmpVal == 0)
                cmpVal = x.Action > y.Action ? -1 : x.Action == y.Action ? 0 : 1; ;

            return cmpVal;
        }

        private void SortMovies()
        {
            Comparison<AMCMovie> comparator = null;

            switch (sortStyle)
            {
                case SortStyle.ByStatusFirst:
                    switch (sortColumn)
                    {
                        case SortColumns.Number:
                            switch (sortDirection)
                            {
                                case SortDirection.Ascending:
                                    comparator = SortMoviesByStatusThenNumberAsc;
                                    break;
                                case SortDirection.Descending:
                                    comparator = SortMoviesByStatusThenNumberDesc;
                                    break;
                            }
                            break;
                        case SortColumns.Title:
                            switch (sortDirection)
                            {
                                case SortDirection.Ascending:
                                    comparator = SortMoviesByStatusThenTitleAsc;
                                    break;
                                case SortDirection.Descending:
                                    comparator = SortMoviesByStatusThenTitleDesc;
                                    break;
                            }
                            break;
                    }
                    break;
                case SortStyle.ByColumnOnly:
                    switch (sortColumn)
                    {
                        case SortColumns.Number:
                            switch (sortDirection)
                            {
                                case SortDirection.Ascending:
                                    comparator = SortMoviesByNumberAsc;
                                    break;
                                case SortDirection.Descending:
                                    comparator = SortMoviesByNumberDesc;
                                    break;
                            }
                            break;
                        case SortColumns.Title:
                            switch (sortDirection)
                            {
                                case SortDirection.Ascending:
                                    comparator = SortMoviesByTitleAsc;
                                    break;
                                case SortDirection.Descending:
                                    comparator = SortMoviesByTitleDesc;
                                    break;
                            }
                            break;
                    }
                    break;
            }

            AMCmovies.Sort(comparator);
            moviesListBindingSource.ResetBindings(false);
        }

        #endregion


    }
}
