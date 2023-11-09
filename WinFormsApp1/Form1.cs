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
    public enum SearchStatus { Work, Pause, Stop }

    public partial class Form1 : Form
    {
        #region Variables & Properties
        /**************************************************************************************************************/
        private static object fileNameLocker = new object();
        private static object reportFileLocker = new object();
        private static object wordsDictionarylocker = new object();

        private List<string> allTextFiles;

        // Amount of threads that search prohiboited words in founded files
        private readonly int searchThreadAmount = 20;

        private readonly string prohibitedWordReplacement = "*******";
        private readonly string reportFileName = "report.txt";
        private readonly int topWordsAmount = 10;

        // List of names of the files that are copied to destination directory. List is needed to check the uniqness of the file name and to create the unique file name if the directory is already contains file with the same name as in copied one
        private List<string> copiedFileNames = new List<string>();

        // Indexes of found files list from which each thread should start searching prohibitted words
        private List<int> startSearchIndexes = new List<int>();

        // Variable to pause, resume or stop search
        private SearchStatus searchStatus;

        private List<string> prohibitedWords;

        private Dictionary<string, int> globalProhibitedWordsDictionary;
        #endregion


        #region Constructor
        /**************************************************************************************************************/
        public Form1()
        {
            InitializeComponent();

            ExitWhenAppIsRunning();

            // Clear the status text
            statusTextBox.Text = string.Empty;
            logLabel.Text = string.Empty;
            errorMessageLabel.Text = string.Empty;
        }
        #endregion


        #region Methods
        /**************************************************************************************************************/
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
            folderBrowserDialog.InitialDirectory = Directory.GetCurrentDirectory();

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                directoryPathTextBox.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private async void startButton_Click(object sender, EventArgs e)
        {
            searchStatus = SearchStatus.Work;
            searchProgressProgressBar.Value = 0;
            globalProhibitedWordsDictionary = new Dictionary<string, int>();

            errorMessageLabel.Text = string.Empty;

            //check file copies destination path and word list
            if (!Directory.Exists(directoryPathTextBox.Text) || prohibitedWordsListBox.Items.Count == 0)
            {
                errorMessageLabel.Text = "Error: wrong directory path or prohibited words list is empty";
                return;
            }

            // Add already existing files from destination directory to list in order not to save new file with the existing name
            var files = Directory.GetFiles(directoryPathTextBox.Text, "*.txt").Select((file) => Path.GetFileNameWithoutExtension(file)).ToList();
            copiedFileNames.AddRange(files);

            // Write distinct prohibited words to list
            prohibitedWords = prohibitedWordsListBox.Items.Cast<string>().Distinct().ToList();

            // Get system drives list
            var drives = DriveInfo.GetDrives();
            // Exit if no drives were found
            if (drives.Length == 0)
            {
                errorMessageLabel.Text = "Error: no drives were found";
                return;
            }

            // Get list of all text files:
            statusTextBox.Text = "Searching files...";
            allTextFiles = await FindTextFilesAsync(drives.ToList());

            searchProgressProgressBar.Maximum = allTextFiles.Count;

            // Start search
            DateTime startTime = DateTime.Now;
            statusTextBox.Text = $"{allTextFiles.Count} files were found. Searching prohibited words...";
            // Find out position of files in the list that each thread should start search from
            FillStartSearchIndexesAsync();

            // Process files (search words, copy and save files)
            await ProcessFilesAsync();

            // Output summary info
            DateTime stopTime = DateTime.Now;
            if (searchStatus == SearchStatus.Stop)
            {
                statusTextBox.Text = $"Searching words was stopped by user. {searchProgressProgressBar.Value} files were processed. Process time: {stopTime.Subtract(startTime).TotalSeconds} seconds. See report for detailes";
            }
            else
            {
                statusTextBox.Text = $"Searching words end. {searchProgressProgressBar.Value} files were processed. Process time: {stopTime.Subtract(startTime).TotalSeconds} seconds. See report for detailes";
            }
            logLabel.Text = string.Empty;
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
                // Clean report file
                File.WriteAllText(reportFileName, "Files with prohibited words:\r\n");

                // Search worsd in threads
                Task[] processFileTasks = new Task[searchThreadAmount];
                for (int i = 0; i < searchThreadAmount; ++i)
                {
                    // Find words position in the list for each thread to search in
                    int startSearchIndex = startSearchIndexes[i];
                    int endSearchIndex = i < searchThreadAmount - 1 ? startSearchIndexes[i + 1] - 1 : allTextFiles.Count - 1;

                    // Start search words in files
                    processFileTasks[i] = Task.Run(async () => { await SearchWordsAsync(startSearchIndex, endSearchIndex); });
                }

                Task.WaitAll(processFileTasks);

                // Write most popular words to report
                var topWords = globalProhibitedWordsDictionary.OrderByDescending((i) => i.Value).Take(topWordsAmount);
                StringBuilder reportText = new StringBuilder();
                reportText.AppendLine("Most popular prohibited words:");
                foreach (var word in topWords)
                {
                    reportText.AppendLine($"{word.Key} - {word.Value} ocurences");
                }
                SaveReport(reportText.ToString());
            });
        }

        private async Task SearchWordsAsync(int startSearchIndex, int endSearchIndex)
        {
            int currentFileIndex = startSearchIndex;
            Dictionary<string, int> localProhibitedWordsDictionary = new Dictionary<string, int>();

            do
            {
                // Exit search if it is stopped
                if (searchStatus == SearchStatus.Stop)
                {
                    return;
                }

                // Read text from file
                try
                {
                    string fileContent = File.ReadAllText(allTextFiles[currentFileIndex]);
                    WriteLog($"Process file {allTextFiles[currentFileIndex]}");

                    bool prohibitedWordsFound = false;
                    string destinationFileName = string.Empty;
                    int prohibitedWordsInFileAmount = 0;

                    // Check occurence of each word from list in the file content
                    foreach (var word in prohibitedWords)
                    {
                        // Search pattern to find whole word but not a part
                        string pattern = $"\\b{word}\\b";

                        // Find all occurences of prohibited word in the file
                        var match = Regex.Matches(fileContent, pattern, RegexOptions.IgnoreCase);

                        // If words were found:
                        if (match.Count > 0)
                        {
                            // Store amount of words for report
                            prohibitedWordsInFileAmount += match.Count;

                            // Update local words dictionary
                            if (localProhibitedWordsDictionary.ContainsKey(word))
                            {
                                localProhibitedWordsDictionary[word] += match.Count;
                            }
                            else
                            {
                                localProhibitedWordsDictionary.Add(word, match.Count);
                            }

                            // When first word is found find out file name to copy and copy it without waiting for the search to complete
                            if (prohibitedWordsFound == false)
                            {
                                prohibitedWordsFound = true;

                                // Get unique file name
                                destinationFileName = GetUniqueFileName(allTextFiles[currentFileIndex]);

                                // Copy file to destination directory
                                string sourseFileName = allTextFiles[currentFileIndex];
                                await Task.Run(() =>
                                {
                                    try
                                    {
                                        File.Copy(sourseFileName, Path.Combine(directoryPathTextBox.Text, destinationFileName));
                                    }
                                    catch (Exception e) { }
                                });
                            }

                            // Replace words with asteriks
                            fileContent = Regex.Replace(fileContent, pattern, prohibitedWordReplacement, RegexOptions.IgnoreCase);
                        }
                    }

                    if (prohibitedWordsFound)
                    {
                        // Create new file name
                        string copyFileName = Path.GetFileNameWithoutExtension(destinationFileName) + " - copy" + Path.GetExtension(destinationFileName);
                        // Save file copy
                        try
                        {
                            File.WriteAllText(Path.Combine(directoryPathTextBox.Text, copyFileName), fileContent);
                        }
                        catch (Exception e) { }

                        // Create report text
                        string reportText = $"File: {allTextFiles[currentFileIndex]}\r\nSize: {new FileInfo(allTextFiles[currentFileIndex]).Length}\r\nContains prohibited words: {prohibitedWordsInFileAmount}\r\n\r\n";
                        SaveReport(reportText);
                    }
                }
                catch (Exception ex)
                {
                    // catch exception file can't be read due to access denie
                }

                ReportSearchProcessed();

                // Move to the next file in the list
                ++currentFileIndex;

                // Pause search
                while (searchStatus == SearchStatus.Pause)
                {
                }

            } while (searchStatus == SearchStatus.Work && currentFileIndex <= endSearchIndex);

            // Add words occurence statictic to global dictionary
            UpdateGlobalWordsDictionary(localProhibitedWordsDictionary);
        }

        private string GetUniqueFileName(string fullFileName)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fullFileName);

            lock (fileNameLocker)
            {
                int index = 1;

                // Change file name untill it will be unique
                while (copiedFileNames.Contains(fileNameWithoutExtension))
                {
                    fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fullFileName) + $" ({index})";
                    ++index;
                }

                copiedFileNames.Add(fileNameWithoutExtension);
            }

            return fileNameWithoutExtension + Path.GetExtension(fullFileName);
        }

        private void SaveReport(string text)
        {
            lock (reportFileLocker)
            {
                File.AppendAllText(reportFileName, text);
            }
        }

        private void UpdateGlobalWordsDictionary(Dictionary<string, int> localDictionary)
        {
            lock (wordsDictionarylocker)
            {
                foreach (var word in localDictionary)
                {
                    if (globalProhibitedWordsDictionary.ContainsKey(word.Key))
                    {
                        globalProhibitedWordsDictionary[word.Key] += word.Value;
                    }
                    else
                    {
                        globalProhibitedWordsDictionary[word.Key] = word.Value;
                    }
                }
            }
        }

        private void ReportSearchProcessed()
        {
            lock (this)
            {
                searchProgressProgressBar.Invoke(() => ++searchProgressProgressBar.Value);
            }
        }

        private void pauseButton_Click(object sender, EventArgs e)
        {
            if (searchStatus != SearchStatus.Pause)
            {
                searchStatus = SearchStatus.Pause;
            }
            else
            {
                searchStatus = SearchStatus.Work;
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            searchStatus = SearchStatus.Stop;
        }

        private void reportButton_Click(object sender, EventArgs e)
        {
            string report = File.ReadAllText(reportFileName);
            ReportForm reportForm = new ReportForm(report);
            reportForm.ShowDialog();
        }

        private void WriteLog(string message)
        {
            logLabel.Invoke(() => logLabel.Text = message);
        }
    }
    #endregion
}
