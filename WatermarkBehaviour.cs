namespace AttachedBehaviours
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;

    /// <summary>
    /// The watermark behavior.
    /// </summary>
    public static class WatermarkBehavior
    {
        /// <summary>
        /// The is the flag that sets the watermark on the control.
        /// </summary>
        public static readonly DependencyProperty IsWatermarkEnabledProperty =
    DependencyProperty.RegisterAttached("IsWatermarkEnabled", typeof(bool), typeof(WatermarkBehavior), new UIPropertyMetadata(false, OnIsWatermarkEnabled));

        /// <summary>
        /// The property that is tested before applying the watermark
        /// </summary>
        public static readonly DependencyProperty WatermarkPropertyProperty =
            DependencyProperty.RegisterAttached("WatermarkProperty", typeof(string), typeof(WatermarkBehavior), new UIPropertyMetadata(string.Empty));

        /// <summary>
        /// The text inserted in the watermark
        /// </summary>
        public static readonly DependencyProperty WatermarkTextProperty =
            DependencyProperty.RegisterAttached("WatermarkText", typeof(string), typeof(WatermarkBehavior), new UIPropertyMetadata(string.Empty));

        /// <summary>
        /// The watermark UI element property
        /// </summary>
        public static readonly DependencyProperty WatermarkUIElementProperty =
            DependencyProperty.RegisterAttached("WatermarkUIElement", typeof(UIElement), typeof(WatermarkBehavior), new UIPropertyMetadata(null));

        /// <summary>
        /// The original control backgrounds
        /// </summary>
        private static readonly List<ControlBackground> OriginalTextBoxBackgrounds = new List<ControlBackground>();

        /// <summary>
        /// The getter for the property IsWatermarkEnabled
        /// </summary>
        /// <param name="obj">
        /// The dependency object
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> value of the property IsWatermarkEnabled.
        /// </returns>
        public static bool GetIsWatermarkEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsWatermarkEnabledProperty);
        }

        /// <summary>
        /// Gets the watermark property.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <returns>The value of the property WatermarkProperty</returns>
        public static string GetWatermarkProperty(DependencyObject obj)
        {
            return (string)obj.GetValue(WatermarkPropertyProperty);
        }

        /// <summary>
        /// Gets the watermark text property.
        /// </summary>
        /// <param name="obj">
        /// The dependency object.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>value of the property WatermarkText.
        /// </returns>
        public static string GetWatermarkText(DependencyObject obj)
        {
            return (string)obj.GetValue(WatermarkTextProperty);
        }

        /// <summary>
        /// Gets the watermark UI element.
        /// </summary>
        /// <param name="obj">dependency object</param>
        /// <returns>he value of the property WatermarkUIElement</returns>
        public static UIElement GetWatermarkUIElement(DependencyObject obj)
        {
            return (UIElement)obj.GetValue(WatermarkUIElementProperty);
        }

        /// <summary>
        /// Sets the is watermark enabled.
        /// </summary>
        /// <param name="obj">The object</param>
        /// <param name="value">if set to <c>true</c> the control will apply watermark.</param>
        public static void SetIsWatermarkEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsWatermarkEnabledProperty, value);
        }

        /// <summary>
        /// Sets the watermark property.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The property to be checked before the watermark is applied.</param>
        public static void SetWatermarkProperty(DependencyObject obj, string value)
        {
            obj.SetValue(WatermarkPropertyProperty, value);
        }

        /// <summary>
        /// Sets the watermark text.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value to be displayed as watermark.</param>
        public static void SetWatermarkText(DependencyObject obj, string value)
        {
            obj.SetValue(WatermarkTextProperty, value);
        }

        /// <summary>
        /// Sets the watermark UI element.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value to be displayed as watermark.</param>
        public static void SetWatermarkUIElement(DependencyObject obj, UIElement value)
        {
            obj.SetValue(WatermarkUIElementProperty, value);
        }

        /// <summary>
        /// Applies the watermark to the control.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="label">The label to be applied as watermark.</param>
        private static void ApplyWatermark(Control control, Label label)
        {
            var customVisualBrush = new VisualBrush { Stretch = Stretch.None, Visual = label };
            control.Background = customVisualBrush;
        }

        /// <summary>
        /// Checks some default properties not empty or null for the control.
        /// </summary>
        /// <param name="control">The control that is verified</param>
        /// <returns>true if one property is found on the control and is empty or null; false otherwise</returns>
        private static bool CheckDefaultPropertiesEmptyOrNull(Control control)
        {
            // For Password
            var passwordPropertyInfo = control.GetType().GetProperty("Password");
            if (passwordPropertyInfo != null && passwordPropertyInfo.GetValue(control, null).ToString() == string.Empty)
            {
                return true;
            }

            // For rich textbox
            var richTextBoxPropertyInfo = control.GetType().GetProperty("Document");
            if (richTextBoxPropertyInfo != null)
            {
                var richTextBoxvalue = richTextBoxPropertyInfo.GetValue(control, null) as FlowDocument;
                if (richTextBoxvalue != null)
                {
                    var textRange = new TextRange(richTextBoxvalue.ContentStart, richTextBoxvalue.ContentEnd);
                    if (string.IsNullOrWhiteSpace(textRange.Text))
                    {
                        return true;
                    }
                }
            }

            // For Selector
            var comboboxPropertyInfo = control.GetType().GetProperty("SelectedItem");

            if (comboboxPropertyInfo != null && comboboxPropertyInfo.GetValue(control, null) == null)
            {
                return true;
            }

            // For textbox
            var textPropertyInfo = control.GetType().GetProperty("Text");
            return textPropertyInfo != null && textPropertyInfo.GetValue(control, null).ToString() == string.Empty;
        }

        /// <summary>
        /// Creates the watermark label.
        /// </summary>
        /// <param name="controlBackground">The control.</param>
        private static void CreateWatermark(ControlBackground controlBackground)
        {
            if (controlBackground.Control != null && GetWatermarkUIElement(controlBackground.Control) == null)
            {
                controlBackground.CreateWatermarkLabelWithText(GetWatermarkText(controlBackground.Control));
            }
            else
            {
                controlBackground.CreateWatermarkLabelWithUIElement(GetWatermarkUIElement(controlBackground.Control));
            }
        }

        /// <summary>
        /// Called when [is watermark enabled].
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnIsWatermarkEnabled(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var frameworkElement = d as FrameworkElement;
            if (frameworkElement == null)
            {
                return;
            }

            var isEnabled = (bool)e.NewValue;
            if (isEnabled)
            {
                // hook to the events of the control
                frameworkElement.GotFocus += ControlGotFocus;
                frameworkElement.LostFocus += ControlLostFocus;
                frameworkElement.Loaded += ControlLoaded;
            }
            else
            {
                // unhook to the events of the control
                frameworkElement.GotFocus -= ControlGotFocus;
                frameworkElement.LostFocus -= ControlLostFocus;
                frameworkElement.Loaded -= ControlLoaded;
            }
        }

        /// <summary>
        /// Handles the GotFocus event of the tb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private static void ControlGotFocus(object sender, RoutedEventArgs e)
        {
            var control = sender as Control;
            var dependencyPropertyName = GetWatermarkProperty(control);
            if (control == null)
            {
                return;
            }

            var hashCode = control.GetHashCode();
            var controlBackground = OriginalTextBoxBackgrounds.FirstOrDefault(cb => cb.Control.GetHashCode() == hashCode);

            if (string.IsNullOrWhiteSpace(dependencyPropertyName) && controlBackground != null)
            {
                control.Background = controlBackground.OriginalBackground;
            }
        }

        /// <summary>
        /// Handles the Loaded event of the Control control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private static void ControlLoaded(object sender, RoutedEventArgs e)
        {
            var control = sender as Control;
            if (control == null)
            {
                return;
            }

            // Save the original look of the control
            var newControlBackground = new ControlBackground
                                           {
                                               Control = control,
                                               OriginalBackground = control.Background
                                           };
            CreateWatermark(newControlBackground);
            OriginalTextBoxBackgrounds.Add(newControlBackground);
            var stringProperty = GetWatermarkProperty(control);
            var propertyIsEmpty = string.IsNullOrWhiteSpace(stringProperty) ? CheckDefaultPropertiesEmptyOrNull(control) : PropertyIsEmpty(control, stringProperty);
            if (propertyIsEmpty)
            {
                ApplyWatermark(control, newControlBackground.WatermarkLabel);  
            }
        }

        /// <summary>
        /// Controls the lost focus.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private static void ControlLostFocus(object sender, RoutedEventArgs e)
        {
            var control = sender as Control;
            if (control == null)
            {
                return;
            }

            var stringProperty = GetWatermarkProperty(control);
            var propertyIsEmpty = string.IsNullOrWhiteSpace(stringProperty) ? CheckDefaultPropertiesEmptyOrNull(control) : PropertyIsEmpty(control, stringProperty);

            if (!propertyIsEmpty)
            {
                return;
            }

            var controlBackground =
                OriginalTextBoxBackgrounds.FirstOrDefault(cb => cb.Control.GetHashCode() == control.GetHashCode());
            if (controlBackground != null && controlBackground.WatermarkLabel != null)
            {
                ApplyWatermark(control, controlBackground.WatermarkLabel);
            }
        }

        /// <summary>
        /// Verifies if the property sent as parameter has its default value.
        /// </summary>
        /// <param name="tb">The control.</param>
        /// <param name="stringProperty">The string property.</param>
        /// <returns>True if the property has the default value; false otherwise</returns>
        private static bool PropertyIsEmpty(Control tb, string stringProperty)
        {
            var propertyIsEmpty = false;
            var propertyInfo = tb.GetType().GetProperty(stringProperty);
            if (propertyInfo != null)
            {
                var propertyType = propertyInfo.DeclaringType;
                var defaultValue = propertyType != null && (propertyType.IsValueType && !(propertyType == typeof(void)))
                                       ? Activator.CreateInstance(propertyType)
                                       : null;
                if (propertyInfo.GetValue(tb, null) == defaultValue)
                {
                    propertyIsEmpty = true;
                }
            }

            return propertyIsEmpty;
        }
    }
}