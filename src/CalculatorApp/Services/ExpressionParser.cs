using System;
using System.Globalization;

namespace CalculatorApp.Services
{
    public class ExpressionParser
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