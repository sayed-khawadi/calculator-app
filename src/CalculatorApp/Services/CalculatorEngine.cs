using System;
using System.Globalization;
using CalculatorApp.Models;

namespace CalculatorApp.Services
{
    public class CalculatorEngine
    {
        private string _expression = "";
        private string _expressionPreview = "";
        private bool _justCalculated = false;
        private bool _hasError = false;

        public string DisplayText
        {
            get
            {
                if (_hasError)
                    return "Error";

                return string.IsNullOrWhiteSpace(_expression) ? "0" : _expression;
            }
        }

        public string ExpressionPreview => _expressionPreview;

        public void EnterDigit(string digit)
        {
            if (_justCalculated || _hasError)
            {
                Clear();
            }

            AppendDigit(digit);
        }

        public void EnterDecimalPoint()
        {
            if (_justCalculated || _hasError)
            {
                Clear();
            }

            AppendDecimalPoint();
        }

        public void EnterOperator(string operation)
        {
            if (_hasError)
                return;

            if (_justCalculated)
            {
                _expressionPreview = "";
                _justCalculated = false;
            }

            AppendOperator(operation);
        }

        public void EnterPercent()
        {
            if (_hasError)
                return;

            if (_justCalculated)
            {
                _expressionPreview = "";
                _justCalculated = false;
            }

            AppendPercent();
        }

        public void EnterOpeningParenthesis()
        {
            if (_hasError || _justCalculated)
            {
                Clear();
            }

            AppendOpeningParenthesis();
        }

        public void EnterClosingParenthesis()
        {
            if (_hasError)
                return;

            if (_justCalculated)
            {
                _expressionPreview = "";
                _justCalculated = false;
            }

            AppendClosingParenthesis();
        }

        public CalculationHistoryItem? Evaluate()
        {
            if (string.IsNullOrWhiteSpace(_expression) || _justCalculated)
                return null;

            try
            {
                string expressionToCalculate = _expression.Trim();

                if (EndsWithInvalidCharacter(expressionToCalculate))
                {
                    ShowError();
                    return null;
                }

                double result = ExpressionParser.Evaluate(expressionToCalculate);

                if (double.IsNaN(result) || double.IsInfinity(result))
                {
                    ShowError();
                    return null;
                }

                string formattedResult = FormatNumber(result);

                _expressionPreview = expressionToCalculate + " =";
                _expression = formattedResult;
                _justCalculated = true;

                return new CalculationHistoryItem(expressionToCalculate, formattedResult);
            }
            catch
            {
                ShowError();
                return null;
            }
        }

        public void Backspace()
        {
            if (_justCalculated || _hasError)
            {
                Clear();
                return;
            }

            if (string.IsNullOrEmpty(_expression))
                return;

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
        }

        public void Clear()
        {
            _expression = "";
            _expressionPreview = "";
            _justCalculated = false;
            _hasError = false;
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

        private void ShowError()
        {
            _expression = "";
            _expressionPreview = "";
            _justCalculated = false;
            _hasError = true;
        }

        private string FormatNumber(double number)
        {
            return number.ToString("G15", CultureInfo.InvariantCulture);
        }
    }
}