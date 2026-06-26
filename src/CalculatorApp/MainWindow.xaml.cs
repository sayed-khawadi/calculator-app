using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CalculatorApp
{
    public partial class MainWindow : Window
    {
        private string _expression = "";
        private bool _justCalculated = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key key = e.Key == Key.System ? e.SystemKey : e.Key;
            bool isShiftPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);

            if (DisplayText.Text == "Error" && key != Key.Escape && key != Key.Delete)
            {
                ResetCalculator();
            }

            if (isShiftPressed)
            {
                if (key == Key.D5)
                {
                    AppendPercentFromKeyboard();
                    e.Handled = true;
                    return;
                }

                if (key == Key.D7)
                {
                    AppendOperatorFromKeyboard("÷");
                    e.Handled = true;
                    return;
                }

                if (key == Key.D8)
                {
                    AppendParenthesisFromKeyboard("(");
                    e.Handled = true;
                    return;
                }

                if (key == Key.D9)
                {
                    AppendParenthesisFromKeyboard(")");
                    e.Handled = true;
                    return;
                }

                if (key == Key.OemPlus)
                {
                    AppendOperatorFromKeyboard("×");
                    e.Handled = true;
                    return;
                }
            }

            if (!isShiftPressed && key >= Key.D0 && key <= Key.D9)
            {
                AppendDigitFromKeyboard((key - Key.D0).ToString());
                e.Handled = true;
                return;
            }

            if (key >= Key.NumPad0 && key <= Key.NumPad9)
            {
                AppendDigitFromKeyboard((key - Key.NumPad0).ToString());
                e.Handled = true;
                return;
            }

            switch (key)
            {
                case Key.Add:
                case Key.OemPlus:
                    AppendOperatorFromKeyboard("+");
                    e.Handled = true;
                    break;

                case Key.Subtract:
                case Key.OemMinus:
                    AppendOperatorFromKeyboard("-");
                    e.Handled = true;
                    break;

                case Key.Multiply:
                    AppendOperatorFromKeyboard("×");
                    e.Handled = true;
                    break;

                case Key.Divide:
                case Key.Oem2:
                    AppendOperatorFromKeyboard("÷");
                    e.Handled = true;
                    break;

                case Key.Decimal:
                case Key.OemPeriod:
                case Key.OemComma:
                    AppendDigitFromKeyboard(".");
                    e.Handled = true;
                    break;

                case Key.Return:
                    EqualsButton_Click(sender, e);
                    e.Handled = true;
                    break;

                case Key.Back:
                    BackspaceButton_Click(sender, e);
                    e.Handled = true;
                    break;

                case Key.Escape:
                case Key.Delete:
                    ResetCalculator();
                    e.Handled = true;
                    break;
            }
        }

        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button)
                return;

            string value = button.Content.ToString() ?? "";

            if (_justCalculated || DisplayText.Text == "Error")
            {
                _expression = "";
                ExpressionText.Text = "";
                _justCalculated = false;
            }

            if (value == ".")
            {
                AppendDecimalPoint();
            }
            else
            {
                AppendDigit(value);
            }

            UpdateDisplay();
        }

        private void OperatorButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button)
                return;

            string operation = button.Content.ToString() ?? "";

            if (DisplayText.Text == "Error")
                return;

            if (_justCalculated)
            {
                ExpressionText.Text = "";
                _justCalculated = false;
            }

            AppendOperator(operation);
            UpdateDisplay();
        }

        private void PercentButton_Click(object sender, RoutedEventArgs e)
        {
            if (DisplayText.Text == "Error")
                return;

            if (_justCalculated)
            {
                ExpressionText.Text = "";
                _justCalculated = false;
            }

            AppendPercent();
            UpdateDisplay();
        }

        private void ParenthesisButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button)
                return;

            string parenthesis = button.Content.ToString() ?? "";

            if (DisplayText.Text == "Error")
                return;

            if (_justCalculated)
            {
                _expression = "";
                ExpressionText.Text = "";
                _justCalculated = false;
            }

            if (parenthesis == "(")
            {
                AppendOpeningParenthesis();
            }
            else if (parenthesis == ")")
            {
                AppendClosingParenthesis();
            }

            UpdateDisplay();
        }

        private void EqualsButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_expression))
                return;

            try
            {
                string expressionToCalculate = _expression.Trim();

                if (EndsWithInvalidCharacter(expressionToCalculate))
                {
                    ShowError();
                    return;
                }

                double result = ExpressionParser.Evaluate(expressionToCalculate);

                if (double.IsNaN(result) || double.IsInfinity(result))
                {
                    ShowError();
                    return;
                }

                ExpressionText.Text = expressionToCalculate + " =";
                DisplayText.Text = FormatNumber(result);

                _expression = FormatNumber(result);
                _justCalculated = true;
            }
            catch
            {
                ShowError();
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ResetCalculator();
        }

        private void BackspaceButton_Click(object sender, RoutedEventArgs e)
        {
            if (_justCalculated || DisplayText.Text == "Error")
            {
                ResetCalculator();
                return;
            }

            if (string.IsNullOrEmpty(_expression))
            {
                UpdateDisplay();
                return;
            }

            if (_expression.EndsWith(" "))
            {
                string trimmed = _expression.TrimEnd();

                if (trimmed.Length > 0)
                    trimmed = trimmed[..^1].TrimEnd();

                _expression = trimmed;
            }
            else
            {
                _expression = _expression[..^1];
            }

            UpdateDisplay();
        }

        private void AppendDigitFromKeyboard(string value)
        {
            if (_justCalculated || DisplayText.Text == "Error")
            {
                _expression = "";
                ExpressionText.Text = "";
                _justCalculated = false;
            }

            if (value == ".")
            {
                AppendDecimalPoint();
            }
            else
            {
                AppendDigit(value);
            }

            UpdateDisplay();
        }

        private void AppendOperatorFromKeyboard(string operation)
        {
            if (DisplayText.Text == "Error")
                return;

            if (_justCalculated)
            {
                ExpressionText.Text = "";
                _justCalculated = false;
            }

            AppendOperator(operation);
            UpdateDisplay();
        }

        private void AppendPercentFromKeyboard()
        {
            if (DisplayText.Text == "Error")
                return;

            if (_justCalculated)
            {
                ExpressionText.Text = "";
                _justCalculated = false;
            }

            AppendPercent();
            UpdateDisplay();
        }

        private void AppendParenthesisFromKeyboard(string parenthesis)
        {
            if (DisplayText.Text == "Error")
                return;

            if (_justCalculated)
            {
                _expression = "";
                ExpressionText.Text = "";
                _justCalculated = false;
            }

            if (parenthesis == "(")
            {
                AppendOpeningParenthesis();
            }
            else if (parenthesis == ")")
            {
                AppendClosingParenthesis();
            }

            UpdateDisplay();
        }

        private void AppendDigit(string digit)
        {
            char lastChar = GetLastNonSpaceChar();

            if (lastChar == ')' || lastChar == '%')
            {
                _expression += " × " + digit;
                return;
            }

            if (CurrentNumberIsZero())
            {
                _expression = _expression[..^1] + digit;
            }
            else
            {
                _expression += digit;
            }
        }

        private void AppendDecimalPoint()
        {
            if (CurrentNumberContainsDecimalPoint())
                return;

            char lastChar = GetLastNonSpaceChar();

            if (string.IsNullOrEmpty(_expression) || IsBinaryOperator(lastChar) || lastChar == '(')
            {
                _expression += "0.";
            }
            else if (char.IsDigit(lastChar))
            {
                _expression += ".";
            }
            else if (lastChar == ')' || lastChar == '%')
            {
                _expression += " × 0.";
            }
        }

        private void AppendOperator(string operation)
        {
            if (string.IsNullOrWhiteSpace(_expression))
            {
                if (operation == "-")
                    _expression = "-";

                return;
            }

            string trimmedExpression = _expression.TrimEnd();
            char lastChar = trimmedExpression[^1];

            if (lastChar == '(')
            {
                if (operation == "-")
                    _expression = trimmedExpression + "-";

                return;
            }

            if (IsBinaryOperator(lastChar))
            {
                if (operation == "-" && lastChar != '-')
                {
                    _expression = trimmedExpression + " -";
                }
                else
                {
                    _expression = trimmedExpression[..^1].TrimEnd() + $" {operation} ";
                }

                return;
            }

            if (lastChar == '.')
                return;

            _expression = trimmedExpression + $" {operation} ";
        }

        private void AppendPercent()
        {
            string trimmedExpression = _expression.TrimEnd();
            char lastChar = GetLastNonSpaceChar();

            if (char.IsDigit(lastChar) || lastChar == ')')
            {
                _expression = trimmedExpression + "%";
            }
        }

        private void AppendOpeningParenthesis()
        {
            char lastChar = GetLastNonSpaceChar();

            if (char.IsDigit(lastChar) || lastChar == ')' || lastChar == '%')
            {
                _expression += " × ";
            }

            _expression += "(";
        }

        private void AppendClosingParenthesis()
        {
            if (!CanCloseParenthesis())
                return;

            _expression += ")";
        }

        private bool CanCloseParenthesis()
        {
            int openCount = 0;
            int closeCount = 0;

            foreach (char character in _expression)
            {
                if (character == '(')
                    openCount++;

                if (character == ')')
                    closeCount++;
            }

            if (openCount <= closeCount)
                return false;

            char lastChar = GetLastNonSpaceChar();

            return char.IsDigit(lastChar) || lastChar == ')' || lastChar == '%';
        }

        private bool CurrentNumberContainsDecimalPoint()
        {
            int startIndex = FindCurrentNumberStartIndex();

            for (int i = startIndex; i < _expression.Length; i++)
            {
                if (_expression[i] == '.')
                    return true;
            }

            return false;
        }

        private bool CurrentNumberIsZero()
        {
            int startIndex = FindCurrentNumberStartIndex();

            if (startIndex >= _expression.Length)
                return false;

            string currentNumber = _expression[startIndex..];

            return currentNumber == "0";
        }

        private int FindCurrentNumberStartIndex()
        {
            int index = _expression.Length - 1;

            while (index >= 0 && (char.IsDigit(_expression[index]) || _expression[index] == '.'))
            {
                index--;
            }

            return index + 1;
        }

        private bool EndsWithInvalidCharacter(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return true;

            char lastChar = expression[^1];

            return IsBinaryOperator(lastChar) || lastChar == '(' || lastChar == '.';
        }

        private char GetLastNonSpaceChar()
        {
            for (int i = _expression.Length - 1; i >= 0; i--)
            {
                if (!char.IsWhiteSpace(_expression[i]))
                    return _expression[i];
            }

            return '\0';
        }

        private bool IsBinaryOperator(char character)
        {
            return character == '+' ||
                   character == '-' ||
                   character == '×' ||
                   character == '÷';
        }

        private void UpdateDisplay()
        {
            DisplayText.Text = string.IsNullOrWhiteSpace(_expression) ? "0" : _expression;
        }

        private void ResetCalculator()
        {
            _expression = "";
            _justCalculated = false;
            ExpressionText.Text = "";
            DisplayText.Text = "0";
        }

        private void ShowError()
        {
            _expression = "";
            _justCalculated = false;
            ExpressionText.Text = "";
            DisplayText.Text = "Error";
        }

        private string FormatNumber(double number)
        {
            return number.ToString("G15", CultureInfo.InvariantCulture);
        }

        private class ExpressionParser
        {
            private readonly string _text;
            private int _position;

            private ExpressionParser(string text)
            {
                _text = text;
            }

            public static double Evaluate(string expression)
            {
                ExpressionParser parser = new ExpressionParser(expression);
                return parser.Parse();
            }

            private double Parse()
            {
                ParsedValue result = ParseExpression();

                SkipSpaces();

                if (_position < _text.Length)
                    throw new InvalidOperationException("Invalid expression.");

                return ConvertPercentToNumber(result);
            }

            private ParsedValue ParseExpression()
            {
                ParsedValue left = ParseMultiplicationDivision();
                double leftNumber = ConvertPercentToNumber(left);

                while (true)
                {
                    SkipSpaces();

                    if (Match('+'))
                    {
                        ParsedValue right = ParseMultiplicationDivision();

                        double rightNumber = right.IsPercent
                            ? leftNumber * right.Value / 100.0
                            : ConvertPercentToNumber(right);

                        leftNumber += rightNumber;
                    }
                    else if (Match('-'))
                    {
                        ParsedValue right = ParseMultiplicationDivision();

                        double rightNumber = right.IsPercent
                            ? leftNumber * right.Value / 100.0
                            : ConvertPercentToNumber(right);

                        leftNumber -= rightNumber;
                    }
                    else
                    {
                        break;
                    }
                }

                return new ParsedValue(leftNumber, false);
            }

            private ParsedValue ParseMultiplicationDivision()
            {
                ParsedValue left = ParseUnary();

                while (true)
                {
                    SkipSpaces();

                    if (Match('×') || Match('*'))
                    {
                        ParsedValue right = ParseUnary();

                        double leftNumber = ConvertPercentToNumber(left);
                        double rightNumber = ConvertPercentToNumber(right);

                        left = new ParsedValue(leftNumber * rightNumber, false);
                    }
                    else if (Match('÷') || Match('/'))
                    {
                        ParsedValue right = ParseUnary();

                        double leftNumber = ConvertPercentToNumber(left);
                        double rightNumber = ConvertPercentToNumber(right);

                        if (rightNumber == 0)
                            throw new DivideByZeroException();

                        left = new ParsedValue(leftNumber / rightNumber, false);
                    }
                    else
                    {
                        break;
                    }
                }

                return left;
            }

            private ParsedValue ParseUnary()
            {
                SkipSpaces();

                if (Match('+'))
                    return ParseUnary();

                if (Match('-'))
                {
                    ParsedValue value = ParseUnary();
                    return new ParsedValue(-value.Value, value.IsPercent);
                }

                return ParsePostfix();
            }

            private ParsedValue ParsePostfix()
            {
                ParsedValue value = ParsePrimary();

                while (true)
                {
                    SkipSpaces();

                    if (Match('%'))
                    {
                        double numberValue = ConvertPercentToNumber(value);
                        value = new ParsedValue(numberValue, true);
                    }
                    else
                    {
                        break;
                    }
                }

                return value;
            }

            private ParsedValue ParsePrimary()
            {
                SkipSpaces();

                if (Match('('))
                {
                    ParsedValue value = ParseExpression();

                    SkipSpaces();

                    if (!Match(')'))
                        throw new InvalidOperationException("Missing closing parenthesis.");

                    return new ParsedValue(ConvertPercentToNumber(value), false);
                }

                return ParseNumber();
            }

            private ParsedValue ParseNumber()
            {
                SkipSpaces();

                int startPosition = _position;
                bool hasDecimalPoint = false;

                while (_position < _text.Length)
                {
                    char current = _text[_position];

                    if (char.IsDigit(current))
                    {
                        _position++;
                    }
                    else if (current == '.' && !hasDecimalPoint)
                    {
                        hasDecimalPoint = true;
                        _position++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (startPosition == _position)
                    throw new InvalidOperationException("Number expected.");

                string numberText = _text[startPosition.._position];

                double number = double.Parse(
                    numberText,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture
                );

                return new ParsedValue(number, false);
            }

            private bool Match(char expectedCharacter)
            {
                SkipSpaces();

                if (_position >= _text.Length)
                    return false;

                if (_text[_position] != expectedCharacter)
                    return false;

                _position++;
                return true;
            }

            private void SkipSpaces()
            {
                while (_position < _text.Length && char.IsWhiteSpace(_text[_position]))
                {
                    _position++;
                }
            }

            private static double ConvertPercentToNumber(ParsedValue value)
            {
                return value.IsPercent ? value.Value / 100.0 : value.Value;
            }

            private readonly struct ParsedValue
            {
                public double Value { get; }
                public bool IsPercent { get; }

                public ParsedValue(double value, bool isPercent)
                {
                    Value = value;
                    IsPercent = isPercent;
                }
            }
        }
    }
}