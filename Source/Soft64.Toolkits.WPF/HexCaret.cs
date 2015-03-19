using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Soft64.Toolkits.WPF
{
    public sealed class HexCaret : UserControl
    {
        public static readonly DependencyProperty ControlTargetProperty =
            DependencyProperty.Register("ControlTarget", typeof(FrameworkElement), typeof(HexCaret),
            new PropertyMetadata(TargetChanged));

        public HexCaret()
        {
        }

        private static void TargetChanged(DependencyObject o, DependencyPropertyChangedEventArgs a)
        {
            HexCaret caret = o as HexCaret;
            caret.Visibility = Visibility.Visible;

            if (caret != null && a.NewValue != null)
            {
                FrameworkElement control = a.NewValue as FrameworkElement;

                if (control != null)
                {
                    if (a.OldValue != null)
                    {
                        ((FrameworkElement)a.OldValue).Unloaded -= caret.ControlUnloaded;
                    }

                    ((FrameworkElement)control).Unloaded += caret.ControlUnloaded;

                    Visual parentVisual = VisualTreeHelper.GetParent(caret) as Visual;

                    try
                    {
                        Point targetPoints = new Point(
                              control.TransformToVisual(parentVisual).Transform(new Point(0, 0)).X,
                              control.TransformToVisual(parentVisual).Transform(new Point(0, 0)).Y
                              );

                        Canvas.SetLeft(caret, targetPoints.X);
                        Canvas.SetTop(caret, targetPoints.Y);
                        caret.Width = control.ActualWidth;
                        caret.Height = control.ActualHeight;
                    }
                    catch (InvalidOperationException)
                    {
                        return;
                    }
                }
            }
        }

        private void ControlUnloaded(object sender, RoutedEventArgs e)
        {
            Visibility = System.Windows.Visibility.Hidden;
        }

        public FrameworkElement TargetControl
        {
            get { return (FrameworkElement)GetValue(ControlTargetProperty); }
            set { SetValue(ControlTargetProperty, value); }
        }
    }
}