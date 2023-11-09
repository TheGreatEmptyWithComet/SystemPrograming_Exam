namespace ProhibitedWordsSearchApp
{
    partial class ReportForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            reportTextBox = new TextBox();
            closeButton = new Button();
            SuspendLayout();
            // 
            // reportTextBox
            // 
            reportTextBox.Location = new Point(12, 12);
            reportTextBox.Multiline = true;
            reportTextBox.Name = "reportTextBox";
            reportTextBox.ScrollBars = ScrollBars.Vertical;
            reportTextBox.Size = new Size(776, 397);
            reportTextBox.TabIndex = 0;
            // 
            // closeButton
            // 
            closeButton.Location = new Point(362, 415);
            closeButton.Name = "closeButton";
            closeButton.Size = new Size(75, 23);
            closeButton.TabIndex = 1;
            closeButton.Text = "Close";
            closeButton.UseVisualStyleBackColor = true;
            closeButton.Click += closeButton_Click;
            // 
            // ReportForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(closeButton);
            Controls.Add(reportTextBox);
            Name = "ReportForm";
            Text = "Report";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox reportTextBox;
        private Button closeButton;
    }
}