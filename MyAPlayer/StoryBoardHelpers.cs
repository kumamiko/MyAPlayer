using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MyAPlayer
{
    public static class StoryBoardHelpers
    {
        #region StoryBoard Add Animation
        /// <summary>
        /// SlideFromRight
        /// </summary>
        /// <param name="storyboard"></param>
        /// <param name="seconds"></param>
        /// <param name="offset"></param>
        /// <param name="decelerationRatio"></param>
        public static void AddSlideFromRight(this Storyboard storyboard, float milliSeconds, double offset, float decelerationRatio = 0.9f )
        {
            var animation = new ThicknessAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(milliSeconds)),
                From = new Thickness(offset, 0, -offset, 0),
                To = new Thickness(0),
                DecelerationRatio = decelerationRatio
            };

            Storyboard.SetTargetProperty(animation, new PropertyPath("Margin"));

            storyboard.Children.Add(animation);
        }

        /// <summary>
        /// Slide (Change Margin)
        /// </summary>
        /// <param name="storyboard"></param>
        /// <param name="seconds"></param>
        /// <param name="thicknessFrom"></param>
        /// <param name="thicknessTo"></param>
        /// <param name="decelerationRatio"></param>
        public static void AddSlide(this Storyboard storyboard, float milliSeconds, Thickness thicknessFrom, Thickness thicknessTo, float decelerationRatio = 0.9f)
        {
            var animation = new ThicknessAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(milliSeconds)),
                From = thicknessFrom,
                To = thicknessTo,
                DecelerationRatio = decelerationRatio
            };

            Storyboard.SetTargetProperty(animation, new PropertyPath("Margin"));

            storyboard.Children.Add(animation);
        }

        /// <summary>
        /// Slide (Canvas)
        /// </summary>
        /// <param name="storyboard"></param>
        /// <param name="milliSeconds"></param>
        /// <param name="topFrom"></param>
        /// <param name="topTo"></param>
        /// <param name="leftFrom"></param>
        /// <param name="leftTo"></param>
        /// <param name="decelerationRatio"></param>
        public static void AddCanvasSlide(this Storyboard storyboard, float milliSeconds, double topFrom, double topTo, double leftFrom, double leftTo)
        {
            var animationTop = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(milliSeconds)),
                From = topFrom,
                To = topTo,
                //EasingFunction = new QuadraticEase()
            };

            var animationLeft = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(milliSeconds)),
                From = leftFrom,
                To = leftTo,
                //EasingFunction = new QuadraticEase()
            };

            Storyboard.SetTargetProperty(animationTop, new PropertyPath("(Canvas.Top)"));
            Storyboard.SetTargetProperty(animationLeft, new PropertyPath("(Canvas.Left)"));

            storyboard.Children.Add(animationTop);
            storyboard.Children.Add(animationLeft);
        }

        /// <summary>
        /// FadeIn
        /// </summary>
        /// <param name="storyboard"></param>
        /// <param name="seconds"></param>
        public static void AddFadeIn(this Storyboard storyboard, float milliSeconds)
        {
            var animation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(milliSeconds)),
                From = 0,
                To = 1,
            };

            Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));

            storyboard.Children.Add(animation);
        }

        /// <summary>
        /// FadeOut
        /// </summary>
        /// <param name="storyboard"></param>
        /// <param name="seconds"></param>
        public static void AddFadeOut(this Storyboard storyboard, float milliSeconds)
        {
            var animation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(milliSeconds)),
                From = 1,
                To = 0,
            };

            Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));

            storyboard.Children.Add(animation);
        }

        /// <summary>
        /// Scale
        /// </summary>
        /// <param name="storyboard"></param>
        /// <param name="milliSeconds"></param>
        /// <param name="fromX"></param>
        /// <param name="toX"></param>
        /// <param name="fromY"></param>
        /// <param name="toY"></param>
        public static void AddScale(this Storyboard storyboard, float milliSeconds, double fromX, double toX, double fromY, double toY)
        {
            var animationX = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(milliSeconds)),
                From = fromX,
                To = toY,
            };

            var animationY = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(milliSeconds)),
                From = fromY,
                To = toY,
            };

            Storyboard.SetTargetProperty(animationX, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));
            Storyboard.SetTargetProperty(animationY, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));

            storyboard.Children.Add(animationX);
            storyboard.Children.Add(animationY);
        }
        #endregion

        #region Control Display Animation Directly
        /// <summary>
        /// Move
        /// </summary>
        /// <param name="thicknessFrom"></param>
        /// <param name="thicknessTo"></param>
        /// <param name="milliSeconds"></param>
        /// <param name="elem"></param>
        /// <param name="decelerationRatio"></param>
        /// <returns></returns>
        public static async Task ElementMargin(Thickness thicknessFrom, Thickness thicknessTo, float milliSeconds, FrameworkElement elem, float decelerationRatio = 0.9f)
        {
            try
            {
                var sb = new Storyboard();

                var animation = new ThicknessAnimation
                {
                    Duration = new Duration(TimeSpan.FromMilliseconds(milliSeconds)),
                    From = thicknessFrom,
                    To = thicknessTo,
                    //DecelerationRatio = decelerationRatio,
                    EasingFunction = new QuinticEase()
                };

                Storyboard.SetTargetProperty(animation, new PropertyPath("Margin"));

                sb.Children.Add(animation);

                sb.Begin(elem);

                await Task.Delay((int)milliSeconds);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Change Canvas Top And Left
        /// </summary>
        /// <param name="topFrom"></param>
        /// <param name="topTo"></param>
        /// <param name="leftFrom"></param>
        /// <param name="leftTo"></param>
        /// <param name="milliSeconds"></param>
        /// <param name="elem"></param>
        /// <param name="decelerationRatio"></param>
        /// <returns></returns>
        public static async Task ElementCanvasTopAndLeft(double topFrom, double topTo, double leftFrom, double leftTo, float milliSeconds, FrameworkElement elem, float decelerationRatio = 0.9f)
        {
            try
            {
                var sb = new Storyboard();

                var animationTop = new DoubleAnimation
                {
                    Duration = new Duration(TimeSpan.FromMilliseconds(milliSeconds)),
                    From = topFrom,
                    To = topTo,
                    //DecelerationRatio = decelerationRatio,
                    EasingFunction = new QuinticEase()
                };

                var animationLeft = new DoubleAnimation
                {
                    Duration = new Duration(TimeSpan.FromMilliseconds(milliSeconds)),
                    From = leftFrom,
                    To = leftTo,
                    //DecelerationRatio = decelerationRatio,
                    EasingFunction = new QuinticEase()
                };

                Storyboard.SetTargetProperty(animationTop, new PropertyPath("(Canvas.Top)"));
                Storyboard.SetTargetProperty(animationLeft, new PropertyPath("(Canvas.Left)"));

                sb.Children.Add(animationTop);
                sb.Children.Add(animationLeft);

                sb.Begin(elem);

                await Task.Delay((int)milliSeconds);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Opacity
        /// </summary>
        /// <param name="opacityFrom"></param>
        /// <param name="opacityTo"></param>
        /// <param name="milliSeconds"></param>
        /// <param name="elem"></param>
        /// <param name="decelerationRatio"></param>
        /// <returns></returns>
        public static async Task ElementOpacity(double opacityFrom, double opacityTo, float milliSeconds, FrameworkElement elem, float decelerationRatio = 0.9f)
        {
            try
            {
                var sb = new Storyboard();

                var animation = new DoubleAnimation
                {
                    Duration = new Duration(TimeSpan.FromMilliseconds(milliSeconds)),
                    From = opacityFrom,
                    To = opacityTo,
                    DecelerationRatio = decelerationRatio
                };

                Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));

                sb.Children.Add(animation);

                sb.Begin(elem);

                await Task.Delay((int)milliSeconds);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Scale
        /// </summary>
        /// <param name="fromX"></param>
        /// <param name="toX"></param>
        /// <param name="fromY"></param>
        /// <param name="toY"></param>
        /// <param name="milliSeconds"></param>
        /// <param name="elem"></param>
        /// <param name="decelerationRatio"></param>
        /// <returns></returns>
        public static async Task ElementScale(double fromX, double toX, double fromY, double toY, float milliSeconds, FrameworkElement elem)
        {
            try
            {
                var sb = new Storyboard();

                var animationX = new DoubleAnimation()
                {
                    Duration = new Duration(TimeSpan.FromMilliseconds(milliSeconds)),
                    From = fromX,
                    To = toX,
                    EasingFunction = new QuinticEase()
                };

                var animationY = new DoubleAnimation()
                {
                    Duration = new Duration(TimeSpan.FromMilliseconds(milliSeconds)),
                    From = fromY,
                    To = toY,
                    EasingFunction = new QuinticEase()
                };

                var lt = new ScaleTransform(fromX, fromY );

                elem.LayoutTransform = lt;

                lt.BeginAnimation(ScaleTransform.ScaleXProperty, animationX);

                lt.BeginAnimation(ScaleTransform.ScaleYProperty, animationY);

                await Task.Delay((int)milliSeconds);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Rotate
        /// </summary>
        /// <param name="fromAngle"></param>
        /// <param name="toAngle"></param>
        /// <param name="milliSeconds"></param>
        /// <param name="elem"></param>
        /// <returns></returns>
        public static async Task ElementRotate(double fromAngle, double toAngle, float milliSeconds, FrameworkElement elem)
        {
            try
            {
                var sb = new Storyboard();

                var animation = new DoubleAnimation()
                {
                    Duration = new Duration(TimeSpan.FromMilliseconds(milliSeconds)),
                    From = fromAngle,
                    To = toAngle,
                    EasingFunction = new QuinticEase()
                };

                var lt = new RotateTransform(fromAngle);

                elem.LayoutTransform = lt;

                lt.BeginAnimation(RotateTransform.AngleProperty, animation);

                await Task.Delay((int)milliSeconds);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
