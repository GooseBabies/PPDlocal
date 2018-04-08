﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.IO;
using XamlAnimatedGif;

namespace Tagger
{
    /// <summary>
    /// Interaction logic for SlideShow.xaml
    /// </summary>
    public partial class SlideShow : Window
    {

        private DispatcherTimer timerImageChange;
        Random r = new Random();
        string[] main;
        int IntervalTimer = 2;
        int Counter = 0;
        bool paused = false;
        BitmapImage Dispic;
        bool randomrepeat = false;
        DatabaseUtil db = new DatabaseUtil();
        DatabaseUtil.DBTable data;
        XMLUtil.SaveSettings savesettings;
        bool shuffle = true;
        List<FileInfo> FI = new List<FileInfo>();
        List<string> Directories = new List<string>();
        Animator _animator;
        string currentImage = "";
        List<string> filepaths = new List<string>();
        string[] currentimagpath;
        Stack<int> ImageHistory = new Stack<int>();
        string[] allTypes;
        string[] videoTypes;
        string[] imageTypes;

        public string ReturnFile {get; set;}

        public SlideShow(string[] ops, string profilename, DatabaseUtil.DBTable _data, XMLUtil.SaveSettings ss)
        {
            InitializeComponent();

            SetWindowScreen(this, GetWindowScreen(App.Current.MainWindow));

            savesettings = ss;
            randomrepeat = savesettings.RepeatShuffle;
            IntervalTimer = savesettings.Slideshowinterval;
            shuffle = savesettings.Shuffle;
            data = _data;

            string fileTypes = ss.ImageFileTypes + ss.VideoFileTypes;
            allTypes = fileTypes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            videoTypes = ss.VideoFileTypes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            imageTypes = ss.ImageFileTypes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string file in ops)
            {
                filepaths.Add(System.IO.Path.Combine(db.GetImageDirectory(data, file), file));
            }
            main = filepaths.ToArray();

            Directories = db.GetDirectories(data, profilename);

            LoadImageFolder(Directories);

            timerImageChange = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, IntervalTimer)
            };
            timerImageChange.Tick += new EventHandler(TimerImageChange_Tick);

            if (shuffle)
            {
                Randomize(ref FI);
                Randomize(ref main);
            }

            ReturnFile = "";

        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.None;
            this.Topmost = true;
            PlaySlideShow();
            timerImageChange.IsEnabled = true;            

        }
        private void TimerImageChange_Tick(object sender, EventArgs e)
        {
            PlaySlideShow();
        }

        private void LoadImageFolder(List<string> Directories)
        {
            
            DirectoryInfo DI;
            List<FileInfo> files;
            foreach (string folder in Directories)
            {
                if (!System.IO.Directory.Exists(folder))
                {
                    return;
                }

                DI = new DirectoryInfo(folder);
                files = DI.GetFiles("*.*").Where(s => allTypes.Contains(s.Extension)).ToList();
                var FI_SortedCreationtime = files.OrderBy(f => f.LastWriteTime);
                files = FI_SortedCreationtime.ToList();
                FI.AddRange(files);
            }
            

        }

        private static BitmapImage BitmapImageFromFile(string path, Image img, int height)
        {
            var bi = new BitmapImage();

            try
            {
                using (var fs = new FileStream(path, FileMode.Open))
                {
                    bi.BeginInit();
                    bi.StreamSource = fs;
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.DecodePixelHeight = height;
                    bi.EndInit();
                    AnimationBehavior.SetSourceUri(img, bi.BaseUri);
                }

                bi.Freeze();
                
            }
            catch(FileFormatException ex)
            {
                
            }
            return bi;         
        }

        private void Randomize(ref string[] bitch)
        {
            bitch = bitch.OrderBy(x => r.Next()).ToArray();
        }

        private void Randomize(ref List<FileInfo> bitches)
        {
            bitches = bitches.OrderBy(x => r.Next()).ToList();
        }

        private void PlaySlideShow()
        {
            GC.Collect();
            timerImageChange.Interval = new TimeSpan(0, 0, IntervalTimer);
            timerImageChange.Stop();
            if (main.Length == 0) //If All! is clicked
            {
                if (shuffle) //If shuffle is on
                {
                    if (!randomrepeat) //If Randomrepeat is off
                    {
                        if (Counter > FI.Count - 1)
                        {
                            Counter = 0;
                            Randomize(ref FI);
                        }
                        string ext = "." + FI[Counter].Extension;
                        if (videoTypes.Contains(ext) || !File.Exists(FI[Counter].FullName))
                        {
                            Counter++;
                            PlaySlideShow();
                            timerImageChange.Start();
                        }
                        else
                        {
                            currentImage = FI[Counter].Name;
                            ImageHistory.Push(Counter);
                            LoadImage(FI, Counter, ext);

                            label.Content = (Counter+1).ToString() + "/" + FI.Count.ToString();
                            Counter++;
                        }
                    }
                    else //If random repeat is on
                    {
                        Counter = r.Next(FI.Count - 1);
                        string ext = "." + FI[Counter].Extension;
                        if (videoTypes.Contains(ext) || !File.Exists(FI[Counter].FullName))
                        {
                            PlaySlideShow();
                            timerImageChange.Start();
                        }
                        else
                        {
                            currentImage = FI[Counter].Name;
                            ImageHistory.Push(Counter);
                            LoadImage(FI, Counter, ext);

                            label.Content = (Counter+1).ToString() + "/" + FI.Count.ToString();
                        }
                    }
                    
                                      
                }
                else //If shuffle if off
                {
                    if(Counter > FI.Count - 1)
                    {
                        Counter = 0;
                    }
                    string ext = "." + FI[Counter].Extension;
                    if (videoTypes.Contains(ext) || !File.Exists(FI[Counter].FullName))
                    {
                        Counter++;
                        PlaySlideShow();
                        timerImageChange.Start();
                    }
                    else
                    {
                        currentImage = FI[Counter].Name;
                        ImageHistory.Push(Counter);
                        LoadImage(FI, Counter, ext);

                        label.Content = (Counter+1).ToString() + "/" + FI.Count.ToString();
                        Counter++;
                    }
                    
                }                
            }
            else //If Tag Filter is chosen
            {
                if (shuffle) //if shuffle is on
                {
                    if (!randomrepeat) //If Random Repeat is off
                    {
                        if (Counter > main.Length - 1)
                        {
                            Counter = 0;
                            Randomize(ref main);
                        }
                        string ext = "." + main[Counter].Split('.').Last();
                        if (videoTypes.Contains(ext) || !File.Exists(main[Counter]))
                        {
                            Counter++;
                            PlaySlideShow();
                            timerImageChange.Start();
                        }
                        else
                        {
                            currentImage = main[Counter];
                            ImageHistory.Push(Counter);
                            LoadImage(main, Counter, ext);

                            label.Content = (Counter + 1).ToString() + "/" + main.Length.ToString();
                            Counter++;
                        }                        
                    }
                    else //If Random repeat is on
                    {
                        Counter = r.Next(0, main.Length - 1);
                        string ext = "." + main[Counter].Split('.').Last();
                        if (videoTypes.Contains(ext) || !File.Exists(main[Counter]))
                        {
                            PlaySlideShow();
                            timerImageChange.Start();
                        }
                        else
                        {
                            currentImage = main[Counter];
                            ImageHistory.Push(Counter);
                            LoadImage(main, Counter, ext);

                            label.Content = (Counter + 1).ToString() + "/" + main.Length.ToString();
                        }                        
                    }
                }
                else //If Shuffle is off
                {
                    if(Counter > main.Length - 1)
                    {
                        Counter = 0;
                    }
                    string ext = "." + main[Counter].Split('.').Last();
                    if (videoTypes.Contains(ext) || !File.Exists(main[Counter]))
                    {
                        Counter++;
                        PlaySlideShow();
                        timerImageChange.Start();
                    }
                    else
                    {
                        currentImage = main[Counter];
                        ImageHistory.Push(Counter);
                        LoadImage(main, Counter, ext);
                        
                        label.Content = (Counter + 1).ToString() + "/" + main.Length.ToString();
                        Counter++;
                    }                    
                }                
            }            
        }

        private void LoadImage(string[] files, int index, string extension)
        {
            if (extension == "gif")
            {
                myImage.Source = null;
                AnimationBehavior.SetSourceUri(myImage, new Uri(files[index]));
            }
            else
            {
                Dispic = BitmapImageFromFile(files[index], myImage, (int)myImage.ActualHeight);
                myImage.Source = Dispic;
                timerImageChange.Start();
            }
        }

        private void LoadImage(List<FileInfo> files, int index, string extension)
        {
            if (extension == "gif")
            {
                myImage.Source = null;
                AnimationBehavior.SetSourceUri(myImage, new Uri(files[index].FullName));
            }
            else
            {
                Dispic = BitmapImageFromFile(files[index].FullName, myImage, (int)myImage.ActualHeight);
                myImage.Source = Dispic;
                timerImageChange.Start();
            }
        }

        private void AnimationBehavior_OnLoaded(object sender, RoutedEventArgs e)
        {
            timerImageChange.Stop();
            GC.Collect();
            _animator = AnimationBehavior.GetAnimator(myImage);

            if (_animator != null)
            {
                int Frames = _animator.FrameCount;
                int gifInterval = (int)Math.Ceiling((decimal)(Frames / 24));
                if(gifInterval == 0)
                {
                    gifInterval = 1;
                }
                if(gifInterval < 3)
                {
                    gifInterval = gifInterval * 3;
                }
                timerImageChange.Interval = new TimeSpan(0, 0, gifInterval);
            }
            timerImageChange.Start();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                timerImageChange.Stop();
                this.Close();
            }
            else if (e.Key == Key.Space)
            {
                PlaySlideShow();
                timerImageChange.Interval = new TimeSpan(0, 0, IntervalTimer);
            }
            else if (e.Key == Key.Pause)
            {
                if (paused == false)
                {
                    timerImageChange.Stop();
                    paused = true;
                }
                else
                {
                    timerImageChange.Interval = new TimeSpan(0, 0, IntervalTimer);
                    timerImageChange.Start();
                    paused = false;
                }
            }
            else if (e.Key == Key.F12)
            {
                currentimagpath = currentImage.Split('\\');
                currentImage = currentimagpath[currentimagpath.Length - 1];
                timerImageChange.Stop();
                ReturnFile = currentImage;
                this.Close();
            }
            else if (e.Key == Key.Left)
            {
                //Goto Previous Image
                timerImageChange.Stop();
                Counter = ImageHistory.Pop();
                Counter = ImageHistory.Pop();
                PlaySlideShow();
                timerImageChange.Start();
            }
            else if (e.Key == Key.Right)
            {
                //Goto Next Image
                if (!randomrepeat) //If Randomrepeat is off
                {
                    timerImageChange.Stop();
                    Counter++;
                    PlaySlideShow();
                    timerImageChange.Start();
                }
                else
                {
                    timerImageChange.Stop();
                    PlaySlideShow();
                    timerImageChange.Start();
                }
            }
            else if(e.Key == Key.Up)
            {
                IntervalTimer++;
                timerImageChange.Interval = new TimeSpan(0, 0, IntervalTimer);
            }
            else if(e.Key == Key.Down)
            {
                IntervalTimer--;
                if (IntervalTimer < 1)
                {
                    IntervalTimer = 1;
                }
                timerImageChange.Interval = new TimeSpan(0, 0, IntervalTimer);
            }
            else if(e.Key == Key.Back)
            {
                IntervalTimer = savesettings.Slideshowinterval;
                timerImageChange.Interval = new TimeSpan(0, 0, IntervalTimer);
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
        }
    }
}
