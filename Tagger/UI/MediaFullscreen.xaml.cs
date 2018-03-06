using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Tagger.UI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MediaFullscreen : Window
    {
        XMLUtil.SaveSettings ss;
        DispatcherTimer timer = new DispatcherTimer();
        DispatcherTimer mousewheeldonetimer = new DispatcherTimer();
        DispatcherTimer mousenomove = new DispatcherTimer();

        bool isdragging = false;
        bool mediaplaying = false;
        bool mediaopened = false;
        int mousewheelcounter = 0;
        int mousedelay;
        int eventindex = -1;
        long skipInterval = 0;
        double mediaTotalTime;        
        string MediaLength = "";
        bool SettingProgressBar = false;
        bool LessThan10Seconds = false;

        public double VolumeOut { get; set; }
        public TimeSpan returnposition { get; set; }
        public bool playing { get; set; }

        ErrorHandling Error = new ErrorHandling();

        public MediaFullscreen(string mediafile, TimeSpan position, XMLUtil.SaveSettings settings, List<string> Tags, bool mediaplay)
        {            
            InitializeComponent();            
            SetWindowScreen(this, GetWindowScreen(App.Current.MainWindow));

            try
            {
                this.Cursor = Cursors.None;
                this.Topmost = true;
                mousedelay = settings.Mousedisappeardelay;

                timer.Interval = new TimeSpan(0, 0, 1);
                timer.Tick += new EventHandler(update_progress);
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

                PreviewMedia.Source = new Uri(mediafile);
                ss = settings;
                Volume.Value = ss.Volume;
                mediaplaying = mediaplay;
                returnposition = position;

                if (mediaplaying)
                {
                    PreviewMedia.Play();
                    mediaplaying = true;
                    timer.Start();
                    Play.Visibility = Visibility.Hidden;
                    Pause.Visibility = Visibility.Visible;
                    Pause2.Visibility = Visibility.Visible;
                }
                else
                {
                    mediaplaying = false;
                    timer.Stop();
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

        private void PreviewMedia_MediaOpened(object sender, RoutedEventArgs e)
        {
            try
            {
                mediaopened = true;
                var RemainingMilliseconds = (PreviewMedia.NaturalDuration.TimeSpan.TotalMilliseconds - PreviewMedia.Position.TotalMilliseconds < 0 ? 0 : PreviewMedia.NaturalDuration.TimeSpan.TotalMilliseconds - PreviewMedia.Position.TotalMilliseconds);
                var RemainingSeconds = Math.Floor((RemainingMilliseconds / 1000) % 60);
                var RemainingMinutes = Math.Floor((RemainingMilliseconds / 60000) % 60);
                var RemainingHours = Math.Floor((RemainingMilliseconds / 3600000) % 60);
                MediaLength = RemainingHours.ToString("00.") + ":" + RemainingMinutes.ToString("00.") + ":" + RemainingSeconds.ToString("00.");

                if (PreviewMedia.NaturalDuration.HasTimeSpan)
                {
                    mediaTotalTime = PreviewMedia.NaturalDuration.TimeSpan.TotalMilliseconds;
                    VideoProgress.Maximum = mediaTotalTime;
                    if (mediaTotalTime > 30000)
                    {
                        skipInterval = (long)Math.Floor(mediaTotalTime / 30);
                    }
                    if (mediaTotalTime < 10000)
                    {
                        LessThan10Seconds = true;
                    }
                    else
                    {
                        LessThan10Seconds = false;
                    }
                }
                else
                {
                    VideoProgress.Maximum = 0;
                }

                PreviewMedia.Position = returnposition;
                VideoProgress.Value = PreviewMedia.Position.TotalMilliseconds;
                PreviewMedia.Volume = ss.Volume;
            }
            catch(Exception ex)
            {
                this.Topmost = false;
                MessageBox.Show("Error opening media. - " + ex.Message);
                Error.WriteToLog(ex);
            }                      
        }

        private void Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (mediaopened)
                {
                    PreviewMedia.Volume = Volume.Value;
                }
                if (ss != null)
                {
                    ss.Volume = Volume.Value;
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
                if (!mediaplaying)
                {
                    mediaplaying = true;
                    Play.Visibility = Visibility.Hidden;
                    Pause.Visibility = Visibility.Visible;
                    Pause2.Visibility = Visibility.Visible;
                    PreviewMedia.Play();
                    timer.Start();
                }
                else
                {
                    mediaplaying = false;
                    Pause.Visibility = Visibility.Hidden;
                    Pause2.Visibility = Visibility.Hidden;
                    Play.Visibility = Visibility.Visible;
                    PreviewMedia.Pause();
                    timer.Stop();
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
                mediaplaying = false;
                Pause.Visibility = Visibility.Hidden;
                Pause2.Visibility = Visibility.Hidden;
                Play.Visibility = Visibility.Visible;
                PreviewMedia.Stop();
                timer.Stop();
                VideoProgress.Value = 0;
                RemainingMediaTime.Content = MediaLength;
                mediatime.Content = "00:00:00";
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
            }            
        }

        private void update_progress(object sender, EventArgs e)
        {
            updateProgressBar();
        }

        private void updateProgressBar()
        {
            try
            {
                if (mediaplaying && !isdragging)
                {
                    var MediaMilliseconds = PreviewMedia.Position.TotalMilliseconds;
                    var MediaSeconds = PreviewMedia.Position.TotalSeconds;
                    var MediaMinutes = PreviewMedia.Position.TotalMinutes;
                    var MediaHours = PreviewMedia.Position.TotalHours;

                    var RemainingMilliseconds = PreviewMedia.NaturalDuration.TimeSpan.TotalMilliseconds - PreviewMedia.Position.TotalMilliseconds;
                    var RemainingSeconds = Math.Floor((RemainingMilliseconds / 1000) % 60);
                    var RemainingMinutes = Math.Floor((RemainingMilliseconds / 60000) % 60);
                    var RemainingHours = Math.Floor((RemainingMilliseconds / 3600000) % 60);

                    SettingProgressBar = true;
                    VideoProgress.Value = PreviewMedia.Position.TotalMilliseconds;
                    SettingProgressBar = false;

                    if (PreviewMedia.NaturalDuration.HasTimeSpan)
                    {
                        if (PreviewMedia.Position.TotalMilliseconds == PreviewMedia.NaturalDuration.TimeSpan.TotalMilliseconds)
                        {
                            if (LessThan10Seconds)
                            {
                                PreviewMedia.Stop();
                                PreviewMedia.Play();
                            }
                            else
                            {
                                PreviewMedia.Stop();
                                mediaplaying = false;
                                Play.Visibility = Visibility.Visible;
                                Pause.Visibility = Visibility.Hidden;
                                Pause2.Visibility = Visibility.Hidden;
                            }
                        }
                    }

                    mediatime.Content = (MediaHours >= 1 ? Math.Floor(MediaHours).ToString("00.") + ":" : "00:") + Math.Floor(MediaMinutes % 60).ToString("00.") + ":" + Math.Floor(MediaSeconds % 60).ToString("00.");
                    RemainingMediaTime.Content = RemainingHours.ToString("00.") + ":" + RemainingMinutes.ToString("00.") + ":" + RemainingSeconds.ToString("00.");
                }
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
            }            
        }

        private void VideoProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (mediaplaying && !isdragging && !SettingProgressBar)
                {
                    PreviewMedia.Position = new TimeSpan(0, 0, 0, 0, (int)VideoProgress.Value);
                }
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
            }            
        }

        private void VideoProgress_DragStarted(object sende, RoutedEventArgs e)
        {
            isdragging = true;
        }

        private void VideoProgress_DragCompleted(object sender, RoutedEventArgs e)
        {
            try
            {
                isdragging = false;
                PreviewMedia.Position = new TimeSpan(0, 0, 0, 0, (int)VideoProgress.Value);
                updateProgressBar();
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
                PreviewMedia.Position = PreviewMedia.Position.Add(new TimeSpan(0, 0, mousewheelcounter * 10));
                updateProgressBar();
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
                returnposition = PreviewMedia.Position;
                playing = mediaplaying;
                VolumeOut = Volume.Value;
                PreviewMedia.Stop();
                timer.Stop();
                this.Close();
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                PreviewMedia.Stop();
                timer.Stop();
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
                    returnposition = PreviewMedia.Position;
                    playing = mediaplaying;
                    VolumeOut = Volume.Value;
                    PreviewMedia.Stop();
                    timer.Stop();
                    this.Close();
                }
                else if (e.Key == Key.D1)
                {
                    if (EventControls.Children.Count - 1 >= 0)
                    {
                        eventindex = 0;
                        EventKey(eventindex);
                    }
                }
                else if (e.Key == Key.D2)
                {
                    if (EventControls.Children.Count - 1 >= 1)
                    {
                        eventindex = 1;
                        EventKey(eventindex);
                    }
                }
                else if (e.Key == Key.D3)
                {
                    if (EventControls.Children.Count - 1 >= 2)
                    {
                        eventindex = 2;
                        EventKey(eventindex);
                    }
                }
                else if (e.Key == Key.D4)
                {
                    if (EventControls.Children.Count - 1 >= 3)
                    {
                        eventindex = 3;
                        EventKey(eventindex);
                    }
                }
                else if (e.Key == Key.D5)
                {
                    if (EventControls.Children.Count - 1 >= 4)
                    {
                        eventindex = 4;
                        EventKey(eventindex);
                    }
                }
                else if (e.Key == Key.D6)
                {
                    if (EventControls.Children.Count - 1 >= 5)
                    {
                        eventindex = 5;
                        EventKey(eventindex);
                    }
                }
                else if (e.Key == Key.D7)
                {
                    if (EventControls.Children.Count - 1 >= 6)
                    {
                        eventindex = 6;
                        EventKey(eventindex);
                    }
                }
                else if (e.Key == Key.D8)
                {
                    if (EventControls.Children.Count - 1 >= 7)
                    {
                        eventindex = 7;
                        EventKey(eventindex);
                    }
                }
                else if (e.Key == Key.D9)
                {
                    if (EventControls.Children.Count - 1 >= 8)
                    {
                        eventindex = 8;
                        EventKey(eventindex);
                    }
                }
                else if (e.Key == Key.D0)
                {
                    if (EventControls.Children.Count - 1 >= 9)
                    {
                        eventindex = 9;
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
                PreviewMedia.Position = new TimeSpan(0, 0, 0, 0, Convert.ToInt32(jack.Tag));
                updateProgressBar();
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
                long skipAmount = 0;
                skipAmount = (long)PreviewMedia.Position.TotalMilliseconds + skipInterval;
                if (skipAmount < mediaTotalTime)
                {
                    PreviewMedia.Position = PreviewMedia.Position.Add(new TimeSpan(0, 0, 0, 0, (int)skipInterval));
                }
                updateProgressBar();
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
                long skipAmount = 0;
                skipAmount = (long)PreviewMedia.Position.TotalMilliseconds - skipInterval;
                if (skipAmount > 0)
                {
                    PreviewMedia.Position = PreviewMedia.Position.Subtract(new TimeSpan(0, 0, 0, 0, (int)skipInterval));
                }
                else
                {
                    PreviewMedia.Position = new TimeSpan(0, 0, 0, 0, 0);
                }
                updateProgressBar();
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
    }
}
