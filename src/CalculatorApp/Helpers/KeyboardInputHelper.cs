using System.Windows.Input;

namespace CalculatorApp.Helpers
{
    public enum KeyboardCommandType
    {
        None,
        Digit,
        DecimalPoint,
        Operator,
        Percent,
        OpeningParenthesis,
        ClosingParenthesis,
        Equals,
        Backspace,
        Clear
    }

    public readonly struct KeyboardInputCommand
    {
        public KeyboardCommandType Type { get; }
        public string Value { get; }

        public KeyboardInputCommand(KeyboardCommandType type, string value = "")
        {
            Type = type;
            Value = value;
        }
    }

    public static class KeyboardInputHelper
    {
        public static bool TryGetCommand(KeyEventArgs e, out KeyboardInputCommand command)
        {
            Key key = e.Key == Key.System ? e.SystemKey : e.Key;
            bool isShiftPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);

            command = new KeyboardInputCommand(KeyboardCommandType.None);

            if (isShiftPressed)
            {
                if (key == Key.D5)
                {
                    command = new KeyboardInputCommand(KeyboardCommandType.Percent);
                    return true;
                }

                if (key == Key.D7)
                {
                    command = new KeyboardInputCommand(KeyboardCommandType.Operator, "÷");
                    return true;
                }

                if (key == Key.D8)
                {
                    command = new KeyboardInputCommand(KeyboardCommandType.OpeningParenthesis);
                    return true;
                }

                if (key == Key.D9)
                {
                    command = new KeyboardInputCommand(KeyboardCommandType.ClosingParenthesis);
                    return true;
                }

                if (key == Key.OemPlus)
                {
                    command = new KeyboardInputCommand(KeyboardCommandType.Operator, "×");
                    return true;
                }
            }

            if (!isShiftPressed && key >= Key.D0 && key <= Key.D9)
            {
                string digit = (key - Key.D0).ToString();
                command = new KeyboardInputCommand(KeyboardCommandType.Digit, digit);
                return true;
            }

            if (key >= Key.NumPad0 && key <= Key.NumPad9)
            {
                string digit = (key - Key.NumPad0).ToString();
                command = new KeyboardInputCommand(KeyboardCommandType.Digit, digit);
                return true;
            }

            switch (key)
            {
                case Key.Add:
                case Key.OemPlus:
                    command = new KeyboardInputCommand(KeyboardCommandType.Operator, "+");
                    return true;

                case Key.Subtract:
                case Key.OemMinus:
                    command = new KeyboardInputCommand(KeyboardCommandType.Operator, "-");
                    return true;

                case Key.Multiply:
                    command = new KeyboardInputCommand(KeyboardCommandType.Operator, "×");
                    return true;

                case Key.Divide:
                case Key.Oem2:
                    command = new KeyboardInputCommand(KeyboardCommandType.Operator, "÷");
                    return true;

                case Key.Decimal:
                case Key.OemPeriod:
                case Key.OemComma:
                    command = new KeyboardInputCommand(KeyboardCommandType.DecimalPoint);
                    return true;

                case Key.Return:
                    command = new KeyboardInputCommand(KeyboardCommandType.Equals);
                    return true;

                case Key.Back:
                    command = new KeyboardInputCommand(KeyboardCommandType.Backspace);
                    return true;

                case Key.Escape:
                case Key.Delete:
                    command = new KeyboardInputCommand(KeyboardCommandType.Clear);
                    return true;

                default:
                    return false;
            }
        }
    }
}