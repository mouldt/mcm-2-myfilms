using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

namespace MCM2MyFilms
{
    /// <summary>
    /// A Dialog to show operation progress and allow cancel.
    /// It's worth noting that Showing Progress can kill performance as each progress notification needs 
    /// marshalling to the UI thread and this is expensive.
    /// To improve the performance 
    /// Only Update the progressBar at necessary intervals
    /// or
    /// remove progressBar updates entirely by commenting the line in frmProgress_Load():
    /// utils.ProgressInfoEvent += new AssetLoadUtil.ProgressInfoDelegate(utils_ProgressInfoEvent);
    /// </summary>
    public partial class frmHelp : Form
    {
        public frmHelp()
        {
            InitializeComponent();
        }
    }
}
