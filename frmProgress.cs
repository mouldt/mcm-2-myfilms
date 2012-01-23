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
    public partial class frmProgress : Form
    {
        //Thread stuff
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        CancellationToken cancelToken;
        Task task;

        // The asset load library
        ProgressUtil utils = null;

        //Progress Bar Values
        double progressBarStep;
        const double FivePercent = 5 / 100.0;
        const double TwoPercent = 2 / 100.0;
        const double OneHundredPercent = 100.0 / 100.0;

        // Smaller % Value give more progress updates but degrades performance
        double progressBarStepPercent = OneHundredPercent;

        public frmProgress(ProgressUtil utils)
        {
            InitializeComponent();
            this.utils = utils;
            cancelToken = cancellationTokenSource.Token;
        }

        public Action<object> action { get; set; }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //Signal a cancel request
            cancellationTokenSource.Cancel();

            // wait for the task to finish
            task.Wait();

            // Now close the window
            CloseWindow();
        }

        private void frmProgress_Load(object sender, EventArgs e)
        {
            //Create a scheduler to use to get back onto the UI Thread.
            TaskScheduler UIThreadScheduler = TaskScheduler.FromCurrentSynchronizationContext(); 
            
            // Register with the library to receive Progress Messages
            utils.ProgressInfoEvent += new ProgressUtil.ProgressInfoDelegate(utils_ProgressInfoEvent);

            // StartNew will perform the action on a new thread
            // This will free the UI thread to allow UI updates to be performed
            task = Task.Factory.StartNew((object cancel) =>
                {
                    try
                    {
                        if (action != null)
                            action(cancel);
                    }
                    catch (OperationCanceledException)
                    {
                        // Thread was cancelled by the user.
                    }

                }, (object)cancelToken, cancelToken);

            // Register the action to take when the thread completes
            // i.e. close the window
            // Using the UIThreadScheduler ensures the method is correctly called on the UI thread
            Task t2 = task.ContinueWith((Task Prevtask) =>
                {
                    // Cancel will close the window so we don't need to
                    if (!((CancellationToken)Prevtask.AsyncState).IsCancellationRequested)
                        CloseWindow();

                    // Show any raised exception
                    if (Prevtask.Exception != null)
                        HandleError(Prevtask.Exception);
                }, UIThreadScheduler);
        }

        private void HandleError(Exception ex)
        {
            String strMessage;
            if (ex is AggregateException)
                strMessage= String.Format("{0}", ex.InnerException.Message);
            else
                strMessage = ex.InnerException == null ? ex.Message : String.Format("{0}\nInner Message: {1}", ex.Message, ex.InnerException.Message);
            MessageBox.Show(strMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void CloseWindow()
        {
            utils.ProgressInfoEvent -= new ProgressUtil.ProgressInfoDelegate(utils_ProgressInfoEvent);
            this.Close();
        }

        /// <summary>
        /// Reports Progress to the UI.
        /// </summary>
        /// <param name="info"></param>
        void utils_ProgressInfoEvent(ProgressUtil.ProgressInfo info)
        {
            if (info.reason == ProgressUtil.ProgressInfo.ProgressInfoReason.update)
            {
                // Only update on progress every 'progressBarStep'
                if ((info.position % progressBarStep == 0))
                    UpdateProgressBar(info);
            }
            else
            {
                // calculate the progressBarStep value
                progressBarStep = info.useSteps? Math.Truncate((double)((info.end - info.start) * progressBarStepPercent)): 1;
                UpdateProgressBar(info);
            }
        }

        private void UpdateProgressBar(ProgressUtil.ProgressInfo info)
        {
            // Marshal to the UI thread if necessary.
            // This will be the case because we are running the 'action' on a separate thread
            if (pbProgress.InvokeRequired)
            {
                pbProgress.Invoke((ProgressUtil.ProgressInfoDelegate)UpdateProgressBar, info);
                return;
            }

            switch (info.reason)
            {
                case ProgressUtil.ProgressInfo.ProgressInfoReason.setup:
                    pbProgress.Minimum = info.start;
                    pbProgress.Maximum = info.end;
                    break;
                case ProgressUtil.ProgressInfo.ProgressInfoReason.update:
                    pbProgress.Value = info.position;
                    break;
            }
        }
    }

    public class ProgressUtil
    {
        public event ProgressInfoDelegate ProgressInfoEvent;
        public delegate void ProgressInfoDelegate(ProgressInfo info);

        public void RaiseSetupProgressInfo(int start, int end, bool useSteps)
        {
            if (ProgressInfoEvent != null)
                ProgressInfoEvent(new ProgressInfo(start, end, useSteps));
        }

        public void RaisePositionProgressInfo(int position)
        {
            if (ProgressInfoEvent != null)
                ProgressInfoEvent(new ProgressInfo(position));
        }

        public class ProgressInfo
        {
            public ProgressInfo(int start, int end, bool useSteps)
            {
                this.reason = ProgressInfoReason.setup;
                this.start = start;
                this.end = end;
                this.useSteps = useSteps;
            }
            public ProgressInfo(int position)
            {
                this.reason = ProgressInfoReason.update;
                this.position = position;
            }
            public enum ProgressInfoReason
            {
                setup,
                update
            }

            public ProgressInfoReason reason { get; set; }
            public int start { get; set; }
            public int end { get; set; }
            public bool useSteps { get; set; }
            public int position { get; set; }
        }
    }
}
