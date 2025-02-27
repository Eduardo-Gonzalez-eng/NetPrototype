using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NetPrototype.Controls.TitleBarControl
{
    public class CustomTitleBar : System.Windows.Controls.Control
    {
        public static readonly DependencyProperty CanMaximizeProperty =
            DependencyProperty.Register("CanMaximize", typeof(bool), typeof(CustomTitleBar), new PropertyMetadata(true));

        public static readonly DependencyProperty IsMaximizedProperty =
            DependencyProperty.Register("IsMaximized", typeof(bool), typeof(CustomTitleBar), new PropertyMetadata(false));

        private System.Windows.Window? _currentWindow = null!;
        public Action<CustomTitleBar, System.Windows.Window>? MinimizeActionOverride { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="Action"/> that should be executed when the Maximize button is clicked."/>
        /// </summary>
        public Action<CustomTitleBar, System.Windows.Window>? MaximizeActionOverride { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the maximize functionality is enabled. If disabled the MaximizeActionOverride action won't be called
        /// </summary>
        public bool CanMaximize
        {
            get => (bool)GetValue(CanMaximizeProperty);
            set => SetValue(CanMaximizeProperty, value);
        }

        /// <summary>
        /// Gets a value indicating whether the current window is maximized.
        /// </summary>
        public bool IsMaximized
        {
            get => (bool)GetValue(IsMaximizedProperty);
            internal set => SetValue(IsMaximizedProperty, value);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="TitleBar"/> class and sets the default <see cref="FrameworkElement.Loaded"/> event.
        /// </summary>
        public CustomTitleBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomTitleBar), new FrameworkPropertyMetadata(typeof(CustomTitleBar)));
            Loaded += CustomTitleBar_Loaded;
        }

        private void CustomTitleBar_Loaded(object sender, RoutedEventArgs e)
        {
            _currentWindow = Window.GetWindow(this);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            System.Windows.Controls.Button? closeButton = GetTemplateChild("CloseButton") as System.Windows.Controls.Button;
            System.Windows.Controls.Button? minimButton = GetTemplateChild("MinimizeButton") as System.Windows.Controls.Button;
            System.Windows.Controls.Border? titleBarBor = GetTemplateChild("TitleBarBorder") as System.Windows.Controls.Border;

            if (closeButton != null)
            {
                closeButton.Click += CloseWindow;
            }
            if (minimButton != null)
            {
                minimButton.Click += MinimizeWindow;
            }

            if (titleBarBor != null)
            {
                titleBarBor.MouseLeftButtonDown += TitleBarBorder_MouseLeftButtonDown;
            }
        }

        private void TitleBarBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_currentWindow != null)
            {
                _currentWindow.DragMove();
            }
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            _currentWindow.Close();
        }

        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            if (MinimizeActionOverride is not null)
            {
                MinimizeActionOverride(this, _currentWindow);

                return;
            }

            _currentWindow.SetCurrentValue(Window.WindowStateProperty, WindowState.Minimized);
        }
    }
}
