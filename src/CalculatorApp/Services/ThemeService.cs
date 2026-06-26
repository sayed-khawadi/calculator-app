using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CalculatorApp.Services
{
    public class ThemeService
    {
        private bool _isDarkMode = true;

        public void ToggleTheme(ResourceDictionary resources, Button themeToggleButton)
        {
            _isDarkMode = !_isDarkMode;
            ApplyTheme(resources, themeToggleButton);
        }

        private void ApplyTheme(ResourceDictionary resources, Button themeToggleButton)
        {
            if (_isDarkMode)
            {
                SetThemeBrush(resources, "WindowBackgroundBrush", "#111214");
                SetThemeBrush(resources, "PanelBackgroundBrush", "#1B1D22");
                SetThemeBrush(resources, "DisplayBackgroundBrush", "#22242A");
                SetThemeBrush(resources, "NormalButtonBackgroundBrush", "#2D2F34");
                SetThemeBrush(resources, "OperatorButtonBackgroundBrush", "#3B3F48");
                SetThemeBrush(resources, "ClearButtonBackgroundBrush", "#4A2D2D");
                SetThemeBrush(resources, "PrimaryTextBrush", "#FFFFFF");
                SetThemeBrush(resources, "SecondaryTextBrush", "#A9ADB8");
                SetThemeBrush(resources, "AccentBrush", "#F5C542");
                SetThemeBrush(resources, "DangerBrush", "#FF7777");
                SetThemeBrush(resources, "ButtonBorderBrush", "#3A3D44");

                themeToggleButton.Content = "Light Mode";
            }
            else
            {
                SetThemeBrush(resources, "WindowBackgroundBrush", "#F3F4F6");
                SetThemeBrush(resources, "PanelBackgroundBrush", "#FFFFFF");
                SetThemeBrush(resources, "DisplayBackgroundBrush", "#E5E7EB");
                SetThemeBrush(resources, "NormalButtonBackgroundBrush", "#FFFFFF");
                SetThemeBrush(resources, "OperatorButtonBackgroundBrush", "#E8EDF5");
                SetThemeBrush(resources, "ClearButtonBackgroundBrush", "#FFE5E5");
                SetThemeBrush(resources, "PrimaryTextBrush", "#111827");
                SetThemeBrush(resources, "SecondaryTextBrush", "#4B5563");
                SetThemeBrush(resources, "AccentBrush", "#F5C542");
                SetThemeBrush(resources, "DangerBrush", "#DC2626");
                SetThemeBrush(resources, "ButtonBorderBrush", "#D1D5DB");

                themeToggleButton.Content = "Dark Mode";
            }
        }

        private static void SetThemeBrush(ResourceDictionary resources, string resourceKey, string colorCode)
        {
            Color color = (Color)ColorConverter.ConvertFromString(colorCode);
            resources[resourceKey] = new SolidColorBrush(color);
        }
    }
}