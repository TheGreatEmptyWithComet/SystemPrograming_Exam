namespace ProhibitedWordsSearchApp
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            prohibitedWordsListBox = new ListBox();
            label1 = new Label();
            addWordButton = new Button();
            deleteWordButton = new Button();
            loadWordButton = new Button();
            toolTip1 = new ToolTip(components);
            prohibitedWordTextBox = new TextBox();
            selectDirectoryButton = new Button();
            directoryPathTextBox = new TextBox();
            startButton = new Button();
            pauseButton = new Button();
            stopButton = new Button();
            reportButton = new Button();
            label2 = new Label();
            label3 = new Label();
            searchProgressProgressBar = new ProgressBar();
            errorMessageLabel = new Label();
            logLabel = new Label();
            statusTextBox = new TextBox();
            SuspendLayout();
            // 
            // prohibitedWordsListBox
            // 
            prohibitedWordsListBox.FormattingEnabled = true;
            prohibitedWordsListBox.ItemHeight = 16;
            prohibitedWordsListBox.Location = new Point(17, 46);
            prohibitedWordsListBox.Margin = new Padding(4);
            prohibitedWordsListBox.Name = "prohibitedWordsListBox";
            prohibitedWordsListBox.Size = new Size(239, 372);
            prohibitedWordsListBox.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(17, 16);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(117, 17);
            label1.TabIndex = 1;
            label1.Text = "Prohibited words:";
            // 
            // addWordButton
            // 
            addWordButton.Location = new Point(17, 455);
            addWordButton.Name = "addWordButton";
            addWordButton.Size = new Size(75, 25);
            addWordButton.TabIndex = 2;
            addWordButton.Text = "Add";
            toolTip1.SetToolTip(addWordButton, "Add word to list");
            addWordButton.UseVisualStyleBackColor = true;
            addWordButton.Click += addWordButton_Click;
            // 
            // deleteWordButton
            // 
            deleteWordButton.Location = new Point(99, 455);
            deleteWordButton.Name = "deleteWordButton";
            deleteWordButton.Size = new Size(75, 25);
            deleteWordButton.TabIndex = 3;
            deleteWordButton.Text = "Delete";
            toolTip1.SetToolTip(deleteWordButton, "Delete word from list");
            deleteWordButton.UseVisualStyleBackColor = true;
            deleteWordButton.Click += deleteWordButton_Click;
            // 
            // loadWordButton
            // 
            loadWordButton.Location = new Point(181, 455);
            loadWordButton.Name = "loadWordButton";
            loadWordButton.Size = new Size(75, 25);
            loadWordButton.TabIndex = 4;
            loadWordButton.Text = "Load";
            toolTip1.SetToolTip(loadWordButton, "Load words from file");
            loadWordButton.UseVisualStyleBackColor = true;
            loadWordButton.Click += loadWordButton_Click;
            // 
            // prohibitedWordTextBox
            // 
            prohibitedWordTextBox.Location = new Point(17, 426);
            prohibitedWordTextBox.Name = "prohibitedWordTextBox";
            prohibitedWordTextBox.Size = new Size(239, 23);
            prohibitedWordTextBox.TabIndex = 5;
            toolTip1.SetToolTip(prohibitedWordTextBox, "Write word here to add it to list");
            // 
            // selectDirectoryButton
            // 
            selectDirectoryButton.Location = new Point(275, 46);
            selectDirectoryButton.Name = "selectDirectoryButton";
            selectDirectoryButton.Size = new Size(115, 25);
            selectDirectoryButton.TabIndex = 7;
            selectDirectoryButton.Text = "Select directory";
            toolTip1.SetToolTip(selectDirectoryButton, "Load words from file");
            selectDirectoryButton.UseVisualStyleBackColor = true;
            selectDirectoryButton.Click += selectDirectoryButton_Click;
            // 
            // directoryPathTextBox
            // 
            directoryPathTextBox.Location = new Point(406, 46);
            directoryPathTextBox.Name = "directoryPathTextBox";
            directoryPathTextBox.Size = new Size(649, 23);
            directoryPathTextBox.TabIndex = 8;
            toolTip1.SetToolTip(directoryPathTextBox, "Write word here to add it to list");
            // 
            // startButton
            // 
            startButton.BackColor = Color.YellowGreen;
            startButton.Location = new Point(275, 455);
            startButton.Name = "startButton";
            startButton.Size = new Size(115, 25);
            startButton.TabIndex = 9;
            startButton.Text = "Start";
            toolTip1.SetToolTip(startButton, "Load words from file");
            startButton.UseVisualStyleBackColor = false;
            startButton.Click += startButton_Click;
            // 
            // pauseButton
            // 
            pauseButton.BackColor = Color.Gold;
            pauseButton.Location = new Point(396, 455);
            pauseButton.Name = "pauseButton";
            pauseButton.Size = new Size(115, 25);
            pauseButton.TabIndex = 10;
            pauseButton.Text = "Pause/Resume";
            toolTip1.SetToolTip(pauseButton, "Load words from file");
            pauseButton.UseVisualStyleBackColor = false;
            pauseButton.Click += pauseButton_Click;
            // 
            // stopButton
            // 
            stopButton.BackColor = Color.OrangeRed;
            stopButton.Location = new Point(517, 455);
            stopButton.Name = "stopButton";
            stopButton.Size = new Size(115, 25);
            stopButton.TabIndex = 11;
            stopButton.Text = "Stop";
            toolTip1.SetToolTip(stopButton, "Load words from file");
            stopButton.UseVisualStyleBackColor = false;
            stopButton.Click += stopButton_Click;
            // 
            // reportButton
            // 
            reportButton.Location = new Point(638, 455);
            reportButton.Name = "reportButton";
            reportButton.Size = new Size(115, 25);
            reportButton.TabIndex = 12;
            reportButton.Text = "Report";
            toolTip1.SetToolTip(reportButton, "Load words from file");
            reportButton.UseVisualStyleBackColor = true;
            reportButton.Click += reportButton_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(273, 101);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(52, 17);
            label2.TabIndex = 13;
            label2.Text = "Status:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(273, 156);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(117, 17);
            label3.TabIndex = 15;
            label3.Text = "Search progress:";
            // 
            // searchProgressProgressBar
            // 
            searchProgressProgressBar.Location = new Point(275, 190);
            searchProgressProgressBar.Name = "searchProgressProgressBar";
            searchProgressProgressBar.Size = new Size(780, 23);
            searchProgressProgressBar.TabIndex = 16;
            // 
            // errorMessageLabel
            // 
            errorMessageLabel.AutoSize = true;
            errorMessageLabel.ForeColor = Color.Red;
            errorMessageLabel.Location = new Point(275, 265);
            errorMessageLabel.Margin = new Padding(4, 0, 4, 0);
            errorMessageLabel.Name = "errorMessageLabel";
            errorMessageLabel.Size = new Size(151, 17);
            errorMessageLabel.TabIndex = 17;
            errorMessageLabel.Text = "<Error message label>";
            // 
            // logLabel
            // 
            logLabel.AutoSize = true;
            logLabel.Location = new Point(273, 232);
            logLabel.Margin = new Padding(4, 0, 4, 0);
            logLabel.Name = "logLabel";
            logLabel.Size = new Size(82, 17);
            logLabel.TabIndex = 18;
            logLabel.Text = "<Log label>";
            // 
            // statusTextBox
            // 
            statusTextBox.BackColor = SystemColors.Control;
            statusTextBox.BorderStyle = BorderStyle.None;
            statusTextBox.Location = new Point(406, 101);
            statusTextBox.Multiline = true;
            statusTextBox.Name = "statusTextBox";
            statusTextBox.ReadOnly = true;
            statusTextBox.Size = new Size(649, 49);
            statusTextBox.TabIndex = 19;
            statusTextBox.Text = "<Status TextBox>";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1067, 497);
            Controls.Add(statusTextBox);
            Controls.Add(logLabel);
            Controls.Add(errorMessageLabel);
            Controls.Add(searchProgressProgressBar);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(reportButton);
            Controls.Add(stopButton);
            Controls.Add(pauseButton);
            Controls.Add(startButton);
            Controls.Add(directoryPathTextBox);
            Controls.Add(selectDirectoryButton);
            Controls.Add(prohibitedWordTextBox);
            Controls.Add(loadWordButton);
            Controls.Add(deleteWordButton);
            Controls.Add(addWordButton);
            Controls.Add(label1);
            Controls.Add(prohibitedWordsListBox);
            Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point);
            Margin = new Padding(4);
            Name = "Form1";
            Text = "Prohibited words search";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label label1;
        private ListBox prohibitedWordsListBox;
        private Button addWordButton;
        private Button deleteWordButton;
        private Button loadWordButton;
        private ToolTip toolTip1;
        private TextBox prohibitedWordTextBox;
        private Button selectDirectoryButton;
        private TextBox directoryPathTextBox;
        private Button startButton;
        private Button pauseButton;
        private Button stopButton;
        private Button reportButton;
        private Label label2;
        private Label label3;
        private ProgressBar searchProgressProgressBar;
        private Label errorMessageLabel;
        private Label logLabel;
        private TextBox statusTextBox;
    }
}

