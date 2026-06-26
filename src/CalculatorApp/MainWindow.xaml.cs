using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace CalculatorApp
{
    public partial class MainWindow : Window
    {
        private double _firstNumber = 0;
        private string _selectedOperator = "";
        private bool _isNewInput = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button)
                return;

            string value = button.Content.ToString() ?? "";

            if (DisplayText.Text == "Error" || _isNewInput)
            {
                DisplayText.Text = value == "." ? "0." : value;
                _isNewInput = false;
                return;
            }

            if (value == "." && DisplayText.Text.Contains("."))
                return;

            if (DisplayText.Text == "0" && value != ".")
            {
                DisplayText.Text = value;
            }
            else
            {
                DisplayText.Text += value;
            }
        }

        private void OperatorButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button)
                return;

            string currentOperator = button.Content.ToString() ?? "";

            if (!TryGetDisplayNumber(out double currentNumber))
            {
                ResetCalculator();
                return;
            }

            if (!string.IsNullOrEmpty(_selectedOperator) && !_isNewInput)
            {
                double? result = Calculate(_firstNumber, currentNumber, _selectedOperator);

                if (result == null)
                {
                    ShowError();
                    return;
                }

                _firstNumber = result.Value;
                DisplayText.Text = FormatNumber(_firstNumber);
            }
            else
            {
                _firstNumber = currentNumber;
            }

            _selectedOperator = currentOperator;
            _isNewInput = true;
        }

        private void EqualsButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedOperator))
                return;

            if (!TryGetDisplayNumber(out double secondNumber))
            {
                ResetCalculator();
                return;
            }

            double? result = Calculate(_firstNumber, secondNumber, _selectedOperator);

            if (result == null)
            {
                ShowError();
                return;
            }

            DisplayText.Text = FormatNumber(result.Value);

            _firstNumber = result.Value;
            _selectedOperator = "";
            _isNewInput = true;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ResetCalculator();
        }

        private void BackspaceButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isNewInput || DisplayText.Text == "Error")
            {
                DisplayText.Text = "0";
                _isNewInput = false;
                return;
            }

            if (DisplayText.Text.Length <= 1)
            {
                DisplayText.Text = "0";
                return;
            }

            DisplayText.Text = DisplayText.Text[..^1];
        }

        private double? Calculate(double firstNumber, double secondNumber, string operation)
        {
            return operation switch
            {
                "+" => firstNumber + secondNumber,
                "-" => firstNumber - secondNumber,
                "×" => firstNumber * secondNumber,
                "÷" => secondNumber == 0 ? null : firstNumber / secondNumber,
                "%" => secondNumber == 0 ? null : firstNumber % secondNumber,
                _ => null
            };
        }

        private bool TryGetDisplayNumber(out double number)
        {
            return double.TryParse(
                DisplayText.Text,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out number
            );
        }

        private string FormatNumber(double number)
        {
            return number.ToString("G15", CultureInfo.InvariantCulture);
        }

        private void ResetCalculator()
        {
            _firstNumber = 0;
            _selectedOperator = "";
            _isNewInput = false;
            DisplayText.Text = "0";
        }

        private void ShowError()
        {
            DisplayText.Text = "Error";
            _firstNumber = 0;
            _selectedOperator = "";
            _isNewInput = true;
        }
    }
}