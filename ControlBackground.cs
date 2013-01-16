namespace AttachedBehaviours
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// Class that holds the control logic of the background of a control
    /// </summary>
    public class ControlBackground
    {
        /// <summary>
        /// Gets or sets the control which background it is changed.
        /// </summary>
        /// <value>
        /// The control.
        /// </value>
        public Control Control { get; set; }

        /// <summary>
        /// Gets or sets the original background of the control.
        /// </summary>
        /// <value>
        /// The original background.
        /// </value>
        public Brush OriginalBackground { get; set; }

        /// <summary>
        /// Gets or sets the watermark label that is used as background for the control.
        /// </summary>
        /// <value>
        /// The watermark label.
        /// </value>
        public Label WatermarkLabel { get; set; }

        /// <summary>
        /// Creates the watermark label.
        /// </summary>
        /// <param name="textForWatermark">The text for watermark.</param>
        public void CreateWatermarkLabelWithText(string textForWatermark)
        {
            var fontStrech = this.Control.GetType().GetProperty("FontStretch");
            var fontSize = this.Control.GetType().GetProperty("FontSize");
            var fontFamily = this.Control.GetType().GetProperty("FontFamily");
            var fontStyle = this.Control.GetType().GetProperty("FontStyle");
            var height = this.Control.GetType().GetProperty("Height");
            var width = this.Control.GetType().GetProperty("Width");
            var watermark = new Label
                                {
                                    Content = textForWatermark,
                                    Foreground = new SolidColorBrush(Colors.Gray),
                                    Background = this.OriginalBackground,
                                };

            if (fontStrech != null)
            {
                var value = fontStrech.GetValue(this.Control, null);
                watermark.FontStretch = value is FontStretch ? (FontStretch)value : new FontStretch();
            }

            if (fontSize != null)
            {
                var value = fontSize.GetValue(this.Control, null);
                watermark.FontSize = value is double ? (double)value : 0;
            }

            if (fontFamily != null)
            {
                var value = fontFamily.GetValue(this.Control, null);
                watermark.FontFamily = value is FontFamily ? (FontFamily)value : new FontFamily();
            }

            if (fontStyle != null)
            {
                var value = fontStyle.GetValue(this.Control, null);
                watermark.FontStyle = value is FontStyle ? (FontStyle)value : new FontStyle();
            }

            if (height != null)
            {
                var value = height.GetValue(this.Control, null);
                watermark.Height = value is double ? (double)value : 0;
            }

            if (width != null)
            {
                var value = width.GetValue(this.Control, null);
                watermark.Width = value is double ? (double)value : 0;
            }

            this.WatermarkLabel = watermark;
        }

        /// <summary>
        /// Creates the watermark label with UI element.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        public void CreateWatermarkLabelWithUIElement(UIElement uiElement)
        {
            var height = this.Control.GetType().GetProperty("Height");
            var width = this.Control.GetType().GetProperty("Width");

            var watermark = new Label { Content = uiElement };
            var frameworkElement = watermark.Content as FrameworkElement;
            if (frameworkElement != null)
            {
                if (height != null)
                {
                    var value = height.GetValue(this.Control, null);
                    frameworkElement.Height = value is double ? (double)value : 0;
                }

                if (width != null)
                {
                    var value = width.GetValue(this.Control, null);
                    frameworkElement.Width = value is double ? (double)value : 0;
                }
            }

            this.WatermarkLabel = watermark;
        }
    }
}