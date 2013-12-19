using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using MarkPad.Helpers;

namespace MarkPad.Document.Controls
{
    public class FloatingToolBar : Popup
    {
        public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register("CommandTarget",
            typeof (ICommandSource), typeof (FloatingToolBar), new UIPropertyMetadata(null));

        Window window;

        public FloatingToolBar()
        {
            AllowsTransparency = true;
            Loaded += ControlLoaded;
            Unloaded += ControlUnloaded;
            StaysOpen = true;
            FocusManager.SetIsFocusScope(this, true);
        }

        public ICommandSource CommandTarget
        {
            get { return (ICommandSource) GetValue(CommandTargetProperty); }
            set { SetValue(CommandTargetProperty, value); }
        }

        public FrameworkElement Content
        {
            get
            {
                var content = Child as FrameworkElement;
                if (content == null)
                {
                    throw new Exception("The FloatingToolBar requires a FrameworkElement to be its content");
                }

                return content;
            }
        }

        void ControlLoaded(object sender, RoutedEventArgs e)
        {
            window = Window.GetWindow(this);
            Attach();
        }

        void Attach()
        {
            if (PlacementTarget == null)
                return;

            PlacementTarget.LostFocus += Hide;

            if (window != null)
            {
                window.LocationChanged += LocationChanged;
                window.PreviewMouseMove += MouseMoved;
                window.Deactivated += WindowDeactivated;
            }
        }

        public void Hide()
        {
            Content.Opacity = 0;
            IsOpen = false;
        }

        public void Show()
        {
            Hide();

            Placement = PlacementMode.Mouse;

            UpdateOpacity();
            IsOpen = true;
            UpdateOpacity();
        }

        public void Show(UIElement target, Point point)
        {
            Hide();

            Placement = PlacementMode.Relative;
            PlacementTarget = target;
            HorizontalOffset = point.X;
            VerticalOffset = point.Y;

            UpdateOpacity();
            IsOpen = true;
            UpdateOpacity();
        }

        void MouseMoved(object sender, MouseEventArgs e)
        {
            UpdateOpacity();
        }

        void UpdateOpacity()
        {
            if (Content.IsMouseDirectlyOver)
            {
                Opacity = 1;
                return;
            }

            var position = Mouse.GetPosition(window);
            var distance = Content.DistanceFromPoint(position, window);

            if (distance < 2)
            {
                Content.Opacity = 1;
            }
            else if (distance > 30)
            {
                Content.Opacity = 0.2;
            }
            else
            {
                Content.Opacity = ((30 - distance)/30.00) + 0.2;
            }

            if (Content.Opacity < 0.2)
            {
                Content.Opacity = 0.2;
            }
        }

        void WindowDeactivated(object sender, EventArgs e)
        {
            Hide();
        }

        void LocationChanged(object sender, EventArgs e)
        {
            if (IsOpen)
            {
                Hide();
            }
        }

        void ControlUnloaded(object sender, RoutedEventArgs e)
        {
            Detach();
        }

        void Hide(object sender, RoutedEventArgs e)
        {
            IsOpen = false;
        }

        void Detach()
        {
            if (PlacementTarget == null)
                return;

            PlacementTarget.LostFocus -= Hide;

            if (window != null)
            {
                window.PreviewMouseMove -= MouseMoved;
                window.LocationChanged -= LocationChanged;
                window.Deactivated -= WindowDeactivated;
            }
        }
    }
}