using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CountWords
{
    public partial class Form1 : Form
    {
        private CountWords.ShowProgressFun showProgress;
        private CountWords.DoneFun done;

        private CountWords countWords;

        /// <summary>
        /// Initializes some data 
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            showProgress = ShowProgress;
            done = Done;
        }

        /// <summary>
        /// Shows a file dialog box, so the user can choose the file he wants to analyze
        /// </summary>
        private void openFileButton_Click(object sender, EventArgs e)
        {
            if (!showSureMessageBox())
                return;
            if (ofd.ShowDialog() != DialogResult.OK)
                return;
            string name = ofd.FileName;

            countWords = new CountWords(name);
            countWords.ShowProgress = showProgress;
            countWords.Done = done;

            currentlyOpenFileLabel.Text = name;

            displayStartStopButtons(false);
        }

        /// <summary>
        /// Displays the previusly hidden buttons
        /// </summary>
        /// <param name="started">Indicates which buttons should be disabled</param>
        private void displayStartStopButtons(bool started)
        {
            openFileButton.Enabled = !started;

            startButton.Visible = true;
            stopButton.Visible = true;
            loadingBar.Visible = true;

            startButton.Enabled = !started;
            stopButton.Enabled = started;

            if(!started)
                loadingBar.Value = 0;
        }

        /// <summary>
        /// Starts the analyzing of the words
        /// If there is already data in the table, it will first ask the user for consent
        /// </summary>
        private void startButton_Click(object sender, EventArgs e)
        {
            if (showSureMessageBox())
            {
                displayStartStopButtons(true);
                countWords.StartCount();
            }
        }

        /// <summary>
        /// Cancels the analyzing of the words
        /// </summary>
        private void stopButton_Click(object sender, EventArgs e)
        {
            displayStartStopButtons(false);
            countWords.Abort();
        }

        /// <summary>
        /// Displays the analyzing progress on the loading bar
        /// Gets called by the worker thread
        /// </summary>
        /// <param name="progress">The progress in %</param>
        public void ShowProgress(int progress)
        {
            BeginInvoke((Action)(() =>
            {
                loadingBar.Value = progress;
            }));
        }

        /// <summary>
        /// Invokes the displaying of the analyzed words
        /// Gets called by the worker thread
        /// </summary>
        public void Done()
        {
            BeginInvoke((Action)(() =>
            {
                displayStartStopButtons(false);
                displayWords();
            }));
        }

        /// <summary>
        /// Displays the analyzed words
        /// </summary>
        private void displayWords()
        {
            wordCountTable.Rows.Clear();
            foreach (var item in countWords.WordCount.OrderByDescending(key => key.Value))
            {
                wordCountTable.Rows.Add(item.Key, item.Value);
            }
            wordCountTable.Visible = true;
        }

        /// <summary>
        /// Asks the user for consent, if he really want's to delete the data
        /// </summary>
        /// <returns>True, if the user gave consent</returns>
        private bool showSureMessageBox()
        {
            if (wordCountTable.Rows.Count > 1)
            {
                if (MessageBox.Show("Are you sure?", "Data will get lost if you do that!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    wordCountTable.Rows.Clear();
                    return true;
                }
                return false;
            }
            return true;
        }
    }
}
