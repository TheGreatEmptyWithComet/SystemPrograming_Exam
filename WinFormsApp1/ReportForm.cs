namespace ProhibitedWordsSearchApp
{
    public partial class ReportForm : Form
    {
        public ReportForm(string text)
        {
            InitializeComponent();

            reportTextBox.Text = text;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}