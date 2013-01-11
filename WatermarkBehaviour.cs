using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AttachedBehaviours
{
    public static class WatermarkBehaviour
    {
        public static readonly DependencyProperty IsWatermarkEnabledProperty =
    DependencyProperty.RegisterAttached("IsWatermarkEnabled", typeof(bool), typeof(WatermarkBehaviour), new UIPropertyMetadata(false, OnIsWatermarkEnabled));

        public static readonly DependencyProperty WatermarkTextProperty =
            DependencyProperty.RegisterAttached("WatermarkText", typeof(string), typeof(WatermarkBehaviour), new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty WatermarkUIElementProperty =
            DependencyProperty.RegisterAttached("WatermarkUIElement", typeof(UIElement), typeof(WatermarkBehaviour), new UIPropertyMetadata(null));

        private static Brush originalTextBoxBackground;

        private static Label watermarkLabel = new Label();

        public static bool GetIsWatermarkEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsWatermarkEnabledProperty);
        }

        public static string GetWatermarkText(DependencyObject obj)
        {
            return (string)obj.GetValue(WatermarkTextProperty);
        }

        public static UIElement GetWatermarkUIElement(DependencyObject obj)
        {
            return (UIElement)obj.GetValue(WatermarkUIElementProperty);
        }

        public static void SetIsWatermarkEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsWatermarkEnabledProperty, value);
        }

        public static void SetWatermarkText(DependencyObject obj, string value)
        {
            obj.SetValue(WatermarkTextProperty, value);
        }

        public static void SetWatermarkUIElement(DependencyObject obj, UIElement value)
        {
            obj.SetValue(WatermarkUIElementProperty, value);
        }
        private static void ApplyWatermark(TextBox tb, Label label)
        {
            var customVisualBrush = new VisualBrush { Stretch = Stretch.None, Visual = label };
            tb.Background = customVisualBrush;
        }

        private static Label CreateWaterMarkLabel(TextBox textBox, string getWatermarkText)
        {
            var watermark = new Label
                {
                    Content = getWatermarkText,
                    Background = originalTextBoxBackground,
                    Foreground = new SolidColorBrush(Colors.Gray),
                    FontStretch = textBox.FontStretch,
                    FontSize = textBox.FontSize,
                    FontFamily = textBox.FontFamily,
                    FontStyle = textBox.FontStyle,
                    Height = textBox.Height,
                    Width = textBox.Width
                };
            return watermark;
        }

        private static Label CreateWatermarkUIElement(TextBox tb, UIElement uiElement)
        {
            var watermark = new Label {Content = uiElement};
            var frameworkElement = watermark.Content as FrameworkElement;
            if (frameworkElement != null)
            {
                frameworkElement.Height = tb.Height;
                frameworkElement.Width = tb.Width;
            }
            return watermark;
        }

        private static void OnIsWatermarkEnabled(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tb = d as TextBox;
            if (tb != null)
            {
                var isEnabled = (bool)e.NewValue;
                if (isEnabled)
                {
                    //hook to the events of the control
                    tb.GotFocus += tb_GotFocus;
                    tb.LostFocus += tb_LostFocus;
                    tb.Loaded += tb_Loaded;
                }
                else
                {
                    //unhook to the events of the control
                    tb.GotFocus -= tb_GotFocus;
                    tb.LostFocus -= tb_LostFocus;
                    tb.Loaded -= tb_Loaded;
                }
            }
        }

        private static void tb_GotFocus(object sender, RoutedEventArgs e)
        {
            var tb = e.OriginalSource as TextBox;
            if (tb != null)
            {
                if (tb.Text == string.Empty)
                {
                    tb.Background = originalTextBoxBackground;
                }
            }
        }

        private static void tb_Loaded(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb != null)
            {
                //Save the original look of the control
                originalTextBoxBackground = tb.Background;

                //Create the watermark
                watermarkLabel = GetWatermarkUIElement(tb) == null ? CreateWaterMarkLabel(tb, GetWatermarkText(tb)) : CreateWatermarkUIElement(tb, GetWatermarkUIElement(tb));

                //Apply the watermark label
                ApplyWatermark(tb, watermarkLabel);
            }
        }
        private static void tb_LostFocus(object sender, RoutedEventArgs e)
        {
            var tb = e.OriginalSource as TextBox;
            if (tb != null)
            {
                if (string.IsNullOrWhiteSpace(tb.Text))
                {
                    ApplyWatermark(tb, watermarkLabel);
                }
            }
        }
    }
}