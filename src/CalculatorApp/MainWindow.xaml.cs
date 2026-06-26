using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CalculatorApp.Helpers;
using CalculatorApp.Models;
using CalculatorApp.Services;

namespace CalculatorApp
{
    public partial class MainWindow : Window
    {
        private readonly CalculatorEngine _calculatorEngine = new();
        private readonly HistoryService _historyService = new();
        private readonly ThemeService _themeService = new();

        public MainWindow()
        {
            InitializeComponent();
            RefreshDisplay();
            RefreshHistoryList();
        }

        private void ToggleThemeButton_Click(object sender, RoutedEventArgs e)
        {
            _themeService.ToggleTheme(Application.Current.Resources, ThemeToggleButton);
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!KeyboardInputHelper.TryGetCommand(e, out KeyboardInputCommand command))
                return;

            ExecuteKeyboardCommand(command);
            e.Handled = true;
        }

        private void ExecuteKeyboardCommand(KeyboardInputCommand command)
        {
            switch (command.Type)
            {
                case KeyboardCommandType.Digit:
                    _calculatorEngine.EnterDigit(command.Value);
                    break;

                case KeyboardCommandType.DecimalPoint:
                    _calculatorEngine.EnterDecimalPoint();
                    break;

                case KeyboardCommandType.Operator:
                    _calculatorEngine.EnterOperator(command.Value);
                    break;

                case KeyboardCommandType.Percent:
                    _calculatorEngine.EnterPercent();
                    break;

                case KeyboardCommandType.OpeningParenthesis:
                    _calculatorEngine.EnterOpeningParenthesis();
                    break;

                case KeyboardCommandType.ClosingParenthesis:
                    _calculatorEngine.EnterClosingParenthesis();
                    break;

                case KeyboardCommandType.Equals:
                    EvaluateExpression();
                    return;

                case KeyboardCommandType.Backspace:
                    _calculatorEngine.Backspace();
                    break;

                case KeyboardCommandType.Clear:
                    _calculatorEngine.Clear();
                    break;
            }

            RefreshDisplay();
        }

        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button)
                return;

            string value = button.Content.ToString() ?? "";

            if (value == ".")
            {
                _calculatorEngine.EnterDecimalPoint();
            }
            else
            {
                _calculatorEngine.EnterDigit(value);
            }

            RefreshDisplay();
        }

        private void OperatorButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button)
                return;

            string operation = button.Content.ToString() ?? "";

            _calculatorEngine.EnterOperator(operation);
            RefreshDisplay();
        }

        private void PercentButton_Click(object sender, RoutedEventArgs e)
        {
            _calculatorEngine.EnterPercent();
            RefreshDisplay();
        }

        private void ParenthesisButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button)
                return;

            string parenthesis = button.Content.ToString() ?? "";

            if (parenthesis == "(")
            {
                _calculatorEngine.EnterOpeningParenthesis();
            }
            else if (parenthesis == ")")
            {
                _calculatorEngine.EnterClosingParenthesis();
            }

            RefreshDisplay();
        }

        private void EqualsButton_Click(object sender, RoutedEventArgs e)
        {
            EvaluateExpression();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            _calculatorEngine.Clear();
            RefreshDisplay();
        }

        private void BackspaceButton_Click(object sender, RoutedEventArgs e)
        {
            _calculatorEngine.Backspace();
            RefreshDisplay();
        }

        private void ClearHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            _historyService.Clear();
            RefreshHistoryList();
        }

        private void EvaluateExpression()
        {
            CalculationHistoryItem? historyItem = _calculatorEngine.Evaluate();

            if (historyItem != null)
            {
                _historyService.Add(historyItem);
                RefreshHistoryList();
            }

            RefreshDisplay();
        }

        private void RefreshDisplay()
        {
            ExpressionText.Text = _calculatorEngine.ExpressionPreview;
            DisplayText.Text = _calculatorEngine.DisplayText;
        }

        private void RefreshHistoryList()
        {
            HistoryList.ItemsSource = null;
            HistoryList.ItemsSource = _historyService.GetDisplayItems();
        }
    }
}