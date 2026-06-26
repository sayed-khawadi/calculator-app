using System.Windows;
using System.Windows.Controls;

namespace CalculatorApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button)
                return;

            string value = button.Content.ToString() ?? "";

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

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            DisplayText.Text = "0";
        }

        private void BackspaceButton_Click(object sender, RoutedEventArgs e)
        {
            if (DisplayText.Text.Length <= 1)
            {
                DisplayText.Text = "0";
                return;
            }

            DisplayText.Text = DisplayText.Text[..^1];
        }
    }
}