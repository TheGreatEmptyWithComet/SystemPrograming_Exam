using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProhibitedWordsSearchApp
{
    public enum SearchStatus { Start, Pause, Stop }

    public partial class Form1 : Form
    {
        private List<string> allTextFiles;

        // Amount of threads that search prohiboited words in founded files
        private readonly int searchThreadAmount = 10;

        private readonly string prohibitedWordSubstitute = "*******";

        // List of fole names of files that are copied to destination directory. List is needed to check the uniqness of the file name and to create the unique file name if the directory is already contains file with the same name as in copied one
        private List<string> copiedFileNames = new List<string>();

        // Indexes of found files list from which each thread should start searching prohibitted words
        private List<int> startSearchIndexes = new List<int>();

        // Variable to pause, resume or stop search
        private SearchStatus searchStatus;

        private List<string> prohibetedLowercaseWords;

        public Form1()
        {
            InitializeComponent();
            ExitWhenAppIsRunning();

            // Clear the status text
            statusLabel.Text = string.Empty;
            errorMessageLabel.Text = string.Empty;
        }


        private void ExitWhenAppIsRunning()
        {
            // Create named mutex
            Mutex mutex = new Mutex(true, "AppMutex", out bool mutexWasCreated);
            // If mutex wasn't created than another copy ao app is already running. Than close app
            if (mutexWasCreated == false)
            {
                MessageBox.Show("App is already running!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void addWordButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(prohibitedWordTextBox.Text))
            {
                prohibitedWordsListBox.Items.Add(prohibitedWordTextBox.Text);
                prohibitedWordTextBox.Text = string.Empty;
            }
        }

        private void deleteWordButton_Click(object sender, EventArgs e)
        {
            if (prohibitedWordsListBox.SelectedIndex >= 0)
            {
                prohibitedWordsListBox.Items.RemoveAt(prohibitedWordsListBox.SelectedIndex);
            }
        }

        private void loadWordButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select text file";
            openFileDialog.Filter = "All text files|*.txt";
            // file with prohibited words already exists in project current directory:
            openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var wordsFromFile = File.ReadAllLines(openFileDialog.FileName).ToList();

                foreach (var word in wordsFromFile)
                {
                    if (!string.IsNullOrEmpty(word))
                    {
                        prohibitedWordsListBox.Items.Add(word);
                    }
                }
            }
        }

        private void selectDirectoryButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                directoryPathTextBox.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private async void startButton_Click(object sender, EventArgs e)
        {
            searchStatus = SearchStatus.Start;

            errorMessageLabel.Text = string.Empty;

            // check file copies destination path and word list
            //if (!Directory.Exists(directoryPathTextBox.Text)|| prohibitedWordsListBox.Items.Count == 0)
            //{
            //    errorMessageLabel.Text = "Error: wrong directory path or prohibited words list is empty";
            //    return;
            //}

            // Rewrite prohibited words to list and lowercase them to simplify serach
            foreach (var word in prohibitedWordsListBox.Items)
            {
                prohibetedLowercaseWords.Add(word.ToString()!.ToLower());
            }

            // Get system drives list
            var drives = DriveInfo.GetDrives();
            // Exit if no drives were found
            if (drives.Length == 0)
            {
                errorMessageLabel.Text = "Error: no drives were found";
                return;
            }

            // Get list of all text files:
            statusLabel.Text = "Searching files...";
            allTextFiles = await FindTextFilesAsync(drives.ToList());

            // Start search
            statusLabel.Text = $"{allTextFiles.Count} files were found. Searching prohibited words...";
            // Find out position of file in the list each thread should start search from
            FillStartSearchIndexesAsync();

            // Start process files (search words, copy and save files)
            await ProcessFilesAsync();
            if (searchStatus == SearchStatus.Stop)
            {
                statusLabel.Text = $"Searching words was stopped by user. {searchProgressProgressBar.Value} files were processed. See report for detailes";
            }
            else
            {
                statusLabel.Text = $"Searching words end. {searchProgressProgressBar.Value} files were processed. See report for detailes";
            }
        }

        private async Task<List<string>> FindTextFilesAsync(List<DriveInfo> drives)
        {
            List<string> allFoundFiles = new List<string>();

            await Task.Run(async () =>
            {
                // Create task list to search files - one task for each drive
                List<Task<List<string>>> searchFilesTasks = new List<Task<List<string>>>();

                foreach (var drive in drives)
                {
                    // Save drive name to avoid it change when task run, as fas as task runs with some dalay
                    string path = drive.Name;
                    // Search text files
                    Task<List<string>> task = Task.Run(() =>
                    {
                        var allfiles = Directory.EnumerateFiles(path, "*.txt", new EnumerationOptions
                        {
                            IgnoreInaccessible = true,
                            RecurseSubdirectories = true
                        });
                        return allfiles.ToList();
                    });

                    searchFilesTasks.Add(task);
                }

                // Wait all search task finish their work:
                do
                {
                    int index = Task.WaitAny(searchFilesTasks.ToArray());
                    if (index >= 0)
                    {
                        allFoundFiles.AddRange(await searchFilesTasks[index]);
                        searchFilesTasks.RemoveAt(index);
                    }
                } while (searchFilesTasks.Count != 0);
            });

            return allFoundFiles;
        }

        private async Task FillStartSearchIndexesAsync()
        {
            await Task.Run(() =>
            {
                int filesAmountForEachThread = allTextFiles.Count / searchThreadAmount;
                int remainingFilesAmount = allTextFiles.Count % searchThreadAmount;

                // First thread starts search from the first file
                startSearchIndexes.Add(0);

                int filesToSearch;
                int index;

                // Find start search indexes for remaining threads 
                for (int i = 0; i < searchThreadAmount - 1; ++i)
                {
                    filesToSearch = filesAmountForEachThread;

                    // The remaining files are evenly distributed among the threads
                    if (remainingFilesAmount > 0)
                    {
                        ++filesToSearch;
                        --remainingFilesAmount;
                    }

                    // Start search index for current thread = start index for previous thread + amount of files to search in
                    index = startSearchIndexes[i] + filesToSearch;
                    startSearchIndexes.Add(index);
                }
            });
        }

        private async Task ProcessFilesAsync()
        {
            await Task.Run(() =>
            {
                List<Task> processFileTasks = new List<Task>();
                // Start search worsd in threads
                for (int i = 0; i < searchThreadAmount; ++i)
                {
                    int startSearchIndex = startSearchIndexes[i];
                    int endSearchIndex = i <= searchThreadAmount - 1 ? startSearchIndexes[i + 1] - 1 : allTextFiles.Count - 1;

                    var task = Task.Run(() => { SearchWords(startSearchIndex, endSearchIndex); });
                    processFileTasks.Add(task);
                }

                // Wait for all threads are complite work or were stopped
                Task.WaitAll(processFileTasks.ToArray());

                // update a report with TOP 10 most popular words
            });
        }

        private void SearchWords(int startSearchIndex, int endSearchIndex)
        {
            int currentFileIndex = startSearchIndex;

            do
            {
                // Exit search
                if (searchStatus == SearchStatus.Stop)
                {
                    return;
                }


                // Read text from file and lowercase it to simplify search
                string fileContent = File.ReadAllText(allTextFiles[currentFileIndex]).ToLower();

                bool prohibitedWordsFound = false;

                foreach (var word in prohibetedLowercaseWords)
                {
                    // Find all occurances of prohibited word in the file
                    var match = Regex.Matches(fileContent, word);

                    // If words were found:
                    if (match.Count > 0)
                    {
                        if (prohibitedWordsFound == false)
                        {
                            prohibitedWordsFound = true;
                            // get file name
                            // copy file
                        }

                        fileContent.Replace(word, prohibitedWordSubstitute);
                    }
                }

                if (prohibitedWordsFound)
                {
                    // save file as copy
                    // write a report
                    // - file name
                    // - file size
                    // - replacement amount in the file
                    // update TOP 10 most popular words
                }


                // Pause search
                while (searchStatus == SearchStatus.Pause)
                {
                }

            } while (searchStatus == SearchStatus.Start && currentFileIndex <= endSearchIndex);
        }







    }
}
