using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CountWords
{
    public class CountWords
    {
        // Some delegate functions to send data to the main form
        public delegate void ShowProgressFun(int progress);
        public delegate void DoneFun();

        /// <summary>
        /// The filename of the file that gets analyzed
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Stores the words and their occurrences
        /// </summary>
        public Dictionary<string, int> WordCount { get; private set; }

        public ShowProgressFun ShowProgress;
        public DoneFun Done;

        private Thread _t;

        private int _lastProgress;

        /// <summary>
        /// Initializes some data for later use
        /// </summary>
        /// <param name="filename">The filename of the file that gets analyzed</param>
        public CountWords(string filename)
        {
            FileName = filename;
            WordCount = new Dictionary<string, int>();
        }

        /// <summary>
        /// Starts the analyzing of the words
        /// </summary>
        public void StartCount()
        {
            _lastProgress = 0;
            _t = new Thread(startCount);
            _t.Start();
        }

        /// <summary>
        /// Cancels the analyzing of the words
        /// </summary>
        public void Abort()
        {
            _t.Abort();
            if (ShowProgress != null)
                ShowProgress(0);
        }

        /// <summary>
        /// Starts the analyzing of the words, runs in a seperate thread
        /// First reads the data from the file, then processes it
        /// </summary>
        private void startCount()
        {
            string content;
            using (StreamReader sr = new StreamReader(FileName))
            {
                content = sr.ReadToEnd();
            }
            ProcessWords(content);
            Done();
        }

        /// <summary>
        /// Processes the words in the file
        /// </summary>
        /// <param name="content">All the content of the file</param>
        public void ProcessWords(string content)
        {
            WordCount.Clear();
            string[] words = content.Split(new string[] { }, StringSplitOptions.RemoveEmptyEntries);
            double len = words.Length;
            for (int i = 0; i < len; i++)
            {
                string word = words[i];
                if (!WordCount.ContainsKey(word))
                    WordCount[word] = 1;
                else
                    WordCount[word]++;

                // Thread.Sleep(1); // Simulate some waiting time, used for testing

                TryShowProgress((int) (i / len * 100));
            }
        }

        /// <summary>
        /// Notifies the form of the changes in the progress
        /// Only updates if the percentage changed
        /// </summary>
        /// <param name="progress">The progress of the processing in %</param>
        public void TryShowProgress(int progress)
        {
            if (ShowProgress == null)
                return;

            if (progress == _lastProgress)
                return;
            
            _lastProgress = progress;

            ShowProgress(progress);
        }

    }
}
