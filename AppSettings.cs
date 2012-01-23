using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MCM2MyFilms
{
    public partial class AppSettings : Form
    {
        public AppSettings()
        {
            InitializeComponent();
            PopulateScreenFromSettings();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            PopulateSettingsFromScreen();
            Properties.Settings.Default.Save();

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void PopulateScreenFromSettings()
        {
            txtAMCatalog.Text = Properties.Settings.Default.AMCCatalog;
            txtFilmDirs.Text = Properties.Settings.Default.FilmDirs;
            txtMePoThumbsDir.Text = Properties.Settings.Default.MePoThumbsDir;
            txtFilmExtensions.Text = Properties.Settings.Default.FilmExtensions;
        }

        private void PopulateSettingsFromScreen()
        {
            Properties.Settings.Default.AMCCatalog = txtAMCatalog.Text;
            Properties.Settings.Default.FilmDirs = txtFilmDirs.Text;
            Properties.Settings.Default.MePoThumbsDir = txtMePoThumbsDir.Text;
            Properties.Settings.Default.FilmExtensions = txtFilmExtensions.Text;
        }

        private void btnFindFilmDir_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowNewFolderButton = false;
            folderBrowserDialog1.Description = "Select Film Folder";
            folderBrowserDialog1.SelectedPath = txtFilmDirs.Text;
            DialogResult res = folderBrowserDialog1.ShowDialog(this);
            if (res == System.Windows.Forms.DialogResult.OK)
                txtFilmDirs.Text = folderBrowserDialog1.SelectedPath;
        }

        private void btnFindAMCCatalog_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = false;
            openFileDialog1.Title = "Select AMC catalog file";
            if (!String.IsNullOrEmpty(txtAMCatalog.Text))
            {
                openFileDialog1.InitialDirectory = Path.GetDirectoryName(txtAMCatalog.Text);
                openFileDialog1.FileName = Path.GetFileName(txtAMCatalog.Text);
            }
            DialogResult res = openFileDialog1.ShowDialog(this);
            if (res == System.Windows.Forms.DialogResult.OK)
                txtAMCatalog.Text = openFileDialog1.FileName;
        }

        private void btnFindMePoThumbsDir_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowNewFolderButton = false;
            folderBrowserDialog1.Description = "Select MyFilms Fanart Folder";
            folderBrowserDialog1.SelectedPath = txtMePoThumbsDir.Text;
            DialogResult res = folderBrowserDialog1.ShowDialog(this);
            if (res == System.Windows.Forms.DialogResult.OK)
                txtMePoThumbsDir.Text = folderBrowserDialog1.SelectedPath;
        }
    }
}
