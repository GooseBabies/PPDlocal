using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Tagger.UI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MediaFullscreen : Window
    {
        XMLUtil.SaveSettings ss;
        DispatcherTimer mousewheeldonetimer = new DispatcherTimer();
        DispatcherTimer mousenomove = new DispatcherTimer();
        
        int mousewheelcounter = 0;
        int mousedelay;
        int eventindex = -1;

        public double VolumeOut { get; set; }
        public float Returnposition { get; set; }
        public bool Playing { get; set; }

        ErrorHandling Error = new ErrorHandling();

        public MediaFullscreen(string mediafile, float position, XMLUtil.SaveSettings settings, List<string> Tags, bool mediaplay)
        {            
            InitializeComponent();            
            SetWindowScreen(this, GetWindowScreen(App.Current.MainWindow));

            try
            {
                this.Cursor = Cursors.None;
                this.Topmost = true;
                mousedelay = settings.Mousedisappeardelay;
                
                mousewheeldonetimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
                mousewheeldonetimer.Tick += new EventHandler(MouseWheel_Done);
                mousenomove.Interval = new TimeSpan(0, 0, mousedelay);
                mousenomove.Tick += new EventHandler(Mouse_nomove);

                if(Tags != null)
                {
                    if(Tags.Count > 0)
                    {
                        foreach (string tag in Tags)
                        {
                            if (tag.Contains("Event: "))
                            {
                                EventControls.Visibility = Visibility.Visible;
                                string tagname = tag.Remove(0, 7);
                                string[] others = tagname.Split('|');
                                Button event1 = new Button
                                {
                                    Content = others[0],
                                    HorizontalContentAlignment = HorizontalAlignment.Center,
                                    Tag = others[1],
                                    Background = Brushes.LightGray,
                                    BorderBrush = Brushes.Transparent,
                                    Foreground = Brushes.Black,
                                    FontWeight = FontWeights.Bold,
                                    FontSize = 12,
                                    Height = 20
                                };
                                event1.Click += new RoutedEventHandler(Event1_Click);
                                EventControls.Children.Add(event1);
                            }
                        }
                    }                    
                }                

                PreviewMedia.LoadMedia(new Uri(mediafile));
                ss = settings;                
                Returnposition = position;
                PreviewMedia.EndBehavior = Meta.Vlc.Wpf.EndBehavior.Repeat;

                if (mediaplay)
                {
                    PreviewMedia.Play();
                    Play.Visibility = Visibility.Hidden;
                    Pause.Visibility = Visibility.Visible;
                    Pause2.Visibility = Visibility.Visible;
                }
                else
                {
                    //mediaplaying = false;
                    Play.Visibility = Visibility.Visible;
                    Pause.Visibility = Visibility.Hidden;
                    Pause2.Visibility = Visibility.Hidden;
                }
            }
            catch(Exception ex)
            {
                this.Topmost = false;
                MessageBox.Show("Error initializing media fullcreen. - " + ex.Message);
                Error.WriteToLog(ex);
            }
        }        

        private void PreviewMedia_LengthChanged(object sender, EventArgs e)
        {
            PreviewMedia.Position = Returnposition;
            PreviewMedia.Volume = ss.Volume;
        }

        private void PreviewMedia_TimeChanged(object sender, EventArgs e)
        {
            var timeElapsed = PreviewMedia.Length.Subtract(PreviewMedia.Time);
            RemainingMediaTime.Text = timeElapsed.ToString("hh\\:mm\\:ss");
        }

        private void Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (ss != null && PreviewMedia.Length.TotalMilliseconds != 0)
                {
                    ss.Volume = (int)Volume.Value;
                }
                VolumeOut = Volume.Value;
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
            }            
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            PlayControl();               
        }

        private void PlayControl()
        {
            try
            {
                if (PreviewMedia.State != Meta.Vlc.Interop.Media.MediaState.Playing)
                {
                    Play.Visibility = Visibility.Hidden;
                    Pause.Visibility = Visibility.Visible;
                    Pause2.Visibility = Visibility.Visible;
                    PreviewMedia.Play();
                }
                else
                {
                    Pause.Visibility = Visibility.Hidden;
                    Pause2.Visibility = Visibility.Hidden;
                    Play.Visibility = Visibility.Visible;
                    PreviewMedia.Pause();
                }
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
            }            
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Pause.Visibility = Visibility.Hidden;
                Pause2.Visibility = Visibility.Hidden;
                Play.Visibility = Visibility.Visible;
                PreviewMedia.Stop();
                VideoProgress.Value = 0;
                RemainingMediaTime.Text = "00:00:00";
                mediatime.Text = "00:00:00";
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
            }            
        }
        
        private void MouseWheel_Done(object sender, EventArgs e)
        {
            try
            {
                mousewheeldonetimer.Stop();
                PreviewMedia.Position += (mousewheelcounter * 0.001F);
                mousewheelcounter = 0;
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
            }                       
        }

        private void Fullscren_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Returnposition = PreviewMedia.Position;
                Playing = (PreviewMedia.State == Meta.Vlc.Interop.Media.MediaState.Playing ? true : false);
                VolumeOut = Volume.Value;
                PreviewMedia.Stop();
                this.Close();
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                PreviewMedia.Stop();
                this.Close();
            }            
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Arrow;
                MediaControls.Visibility = Visibility.Visible;
                mousenomove.Stop();
                mousenomove.Interval = new TimeSpan(0, 0, mousedelay);
                mousenomove.Start();
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                this.Cursor = Cursors.Arrow;
                mousenomove.Stop();
            }            
        }

        private void Mouse_nomove(object sender, EventArgs e)
        {
            try
            {
                mousenomove.Stop();
                this.Cursor = Cursors.None;
                MediaControls.Visibility = Visibility.Collapsed;
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Escape)
                {
                    Returnposition = PreviewMedia.Position;
                    Playing = (PreviewMedia.State == Meta.Vlc.Interop.Media.MediaState.Playing ? true : false);
                    VolumeOut = Volume.Value;
                    PreviewMedia.Stop();
                    this.Close();
                }
                else if (e.Key == Key.F1)
                {
                    if (EventControls.Children.Count - 1 >= 0)
                    {
                        eventindex = 0;
                        EventKey(eventindex);
                    }
                }
                else if (e.Key == Key.F2)
                {
                    if (EventControls.Children.Count - 1 >= 1)
                    {
                        eventindex = 1;
                        EventKey(eventindex);
                    }
                }
                else if (e.Key == Key.F3)
                {
                    if (EventControls.Children.Count - 1 >= 2)
                    {
                        eventindex = 2;
                        EventKey(eventindex);
                    }
                }
                else if (e.Key == Key.F4)
                {
                    if (EventControls.Children.Count - 1 >= 3)
                    {
                        eventindex = 3;
                        EventKey(eventindex);
                    }
                }
                else if (e.Key == Key.F5)
                {
                    if (EventControls.Children.Count - 1 >= 4)
                    {
                        eventindex = 4;
                        EventKey(eventindex);
                    }
                }
                else if (e.Key == Key.F6)
                {
                    if (EventControls.Children.Count - 1 >= 5)
                    {
                        eventindex = 5;
                        EventKey(eventindex);
                    }
                }
                else if (e.Key == Key.F7)
                {
                    if (EventControls.Children.Count - 1 >= 6)
                    {
                        eventindex = 6;
                        EventKey(eventindex);
                    }
                }
                else if (e.Key == Key.F8)
                {
                    if (EventControls.Children.Count - 1 >= 7)
                    {
                        eventindex = 7;
                        EventKey(eventindex);
                    }
                }
                else if (e.Key == Key.F9)
                {
                    if (EventControls.Children.Count - 1 >= 8)
                    {
                        eventindex = 8;
                        EventKey(eventindex);
                    }
                }
                else if (e.Key == Key.F10)
                {
                    if (EventControls.Children.Count - 1 >= 9)
                    {
                        eventindex = 9;
                        EventKey(eventindex);
                    }
                }
                else if (e.Key == Key.F11)
                {
                    if (EventControls.Children.Count - 1 >= 10)
                    {
                        eventindex = 10;
                        EventKey(eventindex);
                    }
                }
                else if (e.Key == Key.F12)
                {
                    if (EventControls.Children.Count - 1 >= 11)
                    {
                        eventindex = 11;
                        EventKey(eventindex);
                    }
                }
                else if (e.Key == Key.Right)
                {
                    eventindex++;
                    if (eventindex > EventControls.Children.Count - 1)
                    {
                        eventindex = 0;
                    }
                    EventKey(eventindex);
                }
                else if (e.Key == Key.Left)
                {
                    eventindex--;
                    if (eventindex < 0)
                    {
                        eventindex = EventControls.Children.Count - 1;
                    }
                    EventKey(eventindex);
                }
                else if (e.Key == Key.Space)
                {
                    PlayControl();
                }
                else if (e.Key == Key.OemComma)
                {
                    SkipB();
                }
                else if (e.Key == Key.OemPeriod)
                {
                    SkipF();
                }
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
            }            
        }

        private void EventKey(int index)
        {
            try
            {
                if (EventControls.Children.Count > index && index >= 0)
                {
                    Event1_Click(EventControls.Children[index], new RoutedEventArgs());
                }
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
            }           
        }

        private void Event1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button jack = (Button)sender;
                for (int y = 0; y < EventControls.Children.Count - 1; y++)
                {
                    if (jack == EventControls.Children[y])
                    {
                        eventindex = y;
                    }
                }
                PreviewMedia.Position = float.Parse(jack.Tag.ToString());
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
            }
        }

        private void SkipF_Click(object sender, RoutedEventArgs e)
        {
            SkipF();
        }

        private void SkipB_Click(object sender, RoutedEventArgs e)
        {
            SkipB();
        }

        private void SkipF()
        {
            try
            {
                float skipAmount = 0;
                if(PreviewMedia.State == Meta.Vlc.Interop.Media.MediaState.Playing)
                {
                    skipAmount = PreviewMedia.Position + 0.0333F;
                    if (skipAmount < 1)
                    {
                        PreviewMedia.Position = skipAmount;
                    }
                }                
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
            }
        }

        private void SkipB()
        {
            try
            {
                float skipAmount = 0;
                if (PreviewMedia.State == Meta.Vlc.Interop.Media.MediaState.Playing)
                {
                    skipAmount = PreviewMedia.Position - 0.0333F;
                    if (skipAmount > 0)
                    {
                        PreviewMedia.Position = skipAmount;
                    }
                    else
                    {
                        PreviewMedia.Position = 0;
                    }
                }
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.WindowState = WindowState.Maximized;
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
            }
        }

        private void Window_MouseWheel_1(object sender, MouseWheelEventArgs e)
        {
            try
            {
                mousewheeldonetimer.Stop();
                mousewheeldonetimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
                mousewheeldonetimer.Start();

                if (e.Delta > 0)
                {
                    mousewheelcounter++;
                }
                else
                {
                    mousewheelcounter--;
                }
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
            }
        }

        public void SetWindowScreen(Window window, System.Windows.Forms.Screen screen)
        {
            if (screen != null)
            {
                if (!window.IsLoaded)
                {
                    window.WindowStartupLocation = WindowStartupLocation.Manual;
                }

                var workingArea = screen.WorkingArea;
                window.Left = workingArea.Left;
                window.Top = workingArea.Top;
            }
        }

        public System.Windows.Forms.Screen GetWindowScreen(Window window)
        {
            return System.Windows.Forms.Screen.FromHandle(new System.Windows.Interop.WindowInteropHelper(window).Handle);
        }

        private void RepeatButton_Click(object sender, RoutedEventArgs e)
        {
            if(PreviewMedia.EndBehavior == Meta.Vlc.Wpf.EndBehavior.Repeat)
            {
                PreviewMedia.EndBehavior = Meta.Vlc.Wpf.EndBehavior.Stop;
            }
            else
            {
                PreviewMedia.EndBehavior = Meta.Vlc.Wpf.EndBehavior.Repeat;
            }
        }

        private void MuteButton_Click(object sender, RoutedEventArgs e)
        {
            if (PreviewMedia.IsMute)
            {
                PreviewMedia.IsMute = false;
                bar1sym.Visibility = Visibility.Visible;
                bar2sym.Visibility = Visibility.Visible;
                mutesym.Visibility = Visibility.Hidden;
            }
            else
            {
                PreviewMedia.IsMute = true;
                bar1sym.Visibility = Visibility.Hidden;
                bar2sym.Visibility = Visibility.Hidden;
                mutesym.Visibility = Visibility.Visible;
            }
        }
    }
}
