using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace FlowerFall
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            this.Background = new SolidColorBrush(Color.FromArgb(0, 34, 34, 34));

            Loaded += MainWindowLoaded;

            InitializeComponent();
        }

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            timer.Tick += (s, arg) => Fall();
            timer.Start();
        }

        readonly Random _random = new Random((int)DateTime.Now.Ticks);
        private void Fall()
        {
            var xAmount = _random.Next(-500, (int)LayoutRoot.ActualWidth - 100);
            var yAmount = -100;
            var s = _random.Next(5, 15) * 0.1;
            var rotateAmount = _random.Next(0, 270);

            RotateTransform rotateTransform = new RotateTransform(rotateAmount);
            ScaleTransform scaleTransform = new ScaleTransform(s, s);
            TranslateTransform translateTransform = new TranslateTransform(xAmount, yAmount);

            var flake = new Flake
            {
                RenderTransform = new TransformGroup
                {
                    Children = new TransformCollection { rotateTransform, scaleTransform, translateTransform }
                },

                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
            };
            LayoutRoot.Children.Add(flake);

            Duration duration = new Duration(TimeSpan.FromSeconds(_random.Next(1, 4)));

            xAmount += _random.Next(100, 500);
            var xAnimation = GenerateAnimation(xAmount, duration, flake, "RenderTransform.Children[2].X");

            yAmount += (int)(LayoutRoot.ActualHeight + 100 + 100);
            var yAnimation = GenerateAnimation(yAmount, duration, flake, "RenderTransform.Children[2].Y");

            rotateAmount += _random.Next(90, 360);
            var rotateAnimation = GenerateAnimation(rotateAmount, duration, flake, "RenderTransform.Children[0].Angle");

            Storyboard story = new Storyboard();
            story.Completed += (sender, e) => LayoutRoot.Children.Remove(flake);
            story.Children.Add(xAnimation);
            story.Children.Add(yAnimation);
            story.Children.Add(rotateAnimation);
            flake.Loaded += (sender, args) => story.Begin();

        }

        private static DoubleAnimation GenerateAnimation(int x, Duration duration, Flake flake, string propertyPath)
        {
            DoubleAnimation animation = new DoubleAnimation
            {
                To = x,
                Duration = duration
            };
            Storyboard.SetTarget(animation, flake);
            Storyboard.SetTargetProperty(animation, new PropertyPath(propertyPath));
            return animation;
        }
    }
}
