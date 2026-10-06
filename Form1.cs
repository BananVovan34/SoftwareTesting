namespace SoftwareTesting
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void CalculateButton_Click(object sender, EventArgs e)
        {
            if (CalculatorSession.TryCalculate(firstNumberTextBox.Text,
                secondNumberTextBox.Text, operationComboBox.Text,
                out string result, out string error))
            {
                resultTextBox.Text = result;
                errorLabel.Text = string.Empty;
            }
            else
            {
                resultTextBox.Clear();
                errorLabel.Text = error;
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            firstNumberTextBox.Clear();
            secondNumberTextBox.Clear();
            resultTextBox.Clear();
            errorLabel.Text = string.Empty;
            operationComboBox.SelectedIndex = 0;
            firstNumberTextBox.Focus();
        }
    }
}
