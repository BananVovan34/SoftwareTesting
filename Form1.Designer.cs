namespace SoftwareTesting
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private Label firstNumberLabel;
        private Label secondNumberLabel;
        private Label operationLabel;
        private Label resultLabel;
        private Label errorLabel;
        private TextBox firstNumberTextBox;
        private TextBox secondNumberTextBox;
        private TextBox resultTextBox;
        private ComboBox operationComboBox;
        private Button calculateButton;
        private Button clearButton;

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

        private void InitializeComponent()
        {
            firstNumberLabel = new Label();
            secondNumberLabel = new Label();
            operationLabel = new Label();
            resultLabel = new Label();
            errorLabel = new Label();
            firstNumberTextBox = new TextBox();
            secondNumberTextBox = new TextBox();
            resultTextBox = new TextBox();
            operationComboBox = new ComboBox();
            calculateButton = new Button();
            clearButton = new Button();
            SuspendLayout();

            operationLabel.AutoSize = true;
            operationLabel.Location = new Point(24, 25);
            operationLabel.Text = "Операция";
            operationComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            operationComboBox.Name = "operationComboBox";
            operationComboBox.Items.AddRange(new object[] { "+", "-", "×", "÷" });
            operationComboBox.Location = new Point(24, 48);
            operationComboBox.Size = new Size(252, 28);
            operationComboBox.SelectedIndex = 0;
            operationComboBox.SelectedIndexChanged += InputChanged;

            firstNumberLabel.AutoSize = true;
            firstNumberLabel.Location = new Point(24, 90);
            firstNumberLabel.Text = "Первое число";
            firstNumberTextBox.Location = new Point(24, 113);
            firstNumberTextBox.Name = "firstNumberTextBox";
            firstNumberTextBox.Size = new Size(252, 27);
            firstNumberTextBox.TextChanged += InputChanged;

            secondNumberLabel.AutoSize = true;
            secondNumberLabel.Location = new Point(24, 154);
            secondNumberLabel.Text = "Второе число";
            secondNumberTextBox.Location = new Point(24, 177);
            secondNumberTextBox.Name = "secondNumberTextBox";
            secondNumberTextBox.Size = new Size(252, 27);
            secondNumberTextBox.TextChanged += InputChanged;

            calculateButton.Location = new Point(24, 222);
            calculateButton.Name = "calculateButton";
            calculateButton.Size = new Size(120, 36);
            calculateButton.Text = "Вычислить";
            calculateButton.UseVisualStyleBackColor = true;
            calculateButton.Click += CalculateButton_Click;
            clearButton.Location = new Point(156, 222);
            clearButton.Name = "clearButton";
            clearButton.Size = new Size(120, 36);
            clearButton.Text = "Очистить";
            clearButton.UseVisualStyleBackColor = true;
            clearButton.Click += ClearButton_Click;

            resultLabel.AutoSize = true;
            resultLabel.Location = new Point(24, 275);
            resultLabel.Text = "Результат";
            resultTextBox.Location = new Point(24, 298);
            resultTextBox.Name = "resultTextBox";
            resultTextBox.ReadOnly = true;
            resultTextBox.Size = new Size(252, 27);
            errorLabel.AutoSize = true;
            errorLabel.ForeColor = Color.Firebrick;
            errorLabel.Location = new Point(24, 338);
            errorLabel.Name = "errorLabel";
            errorLabel.MaximumSize = new Size(252, 0);

            AcceptButton = calculateButton;
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(300, 390);
            Controls.Add(operationLabel);
            Controls.Add(operationComboBox);
            Controls.Add(firstNumberLabel);
            Controls.Add(firstNumberTextBox);
            Controls.Add(secondNumberLabel);
            Controls.Add(secondNumberTextBox);
            Controls.Add(calculateButton);
            Controls.Add(clearButton);
            Controls.Add(resultLabel);
            Controls.Add(resultTextBox);
            Controls.Add(errorLabel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Арифметический калькулятор";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
