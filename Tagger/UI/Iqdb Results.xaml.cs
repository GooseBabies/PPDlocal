using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;

namespace Tagger
{
    /// <summary>
    /// Interaction logic for Iqdb_Results.xaml
    /// </summary>
    public partial class Iqdb_Results : Window
    {

        IqdbApi.Models.SearchResult results;
        DatabaseUtil _db;
        DatabaseUtil.DBTable _data;       

        string _DirectoryPath;
        FileInfo _currentImage;
        string currentImageName;
        int _currentImageIndex;
        short _currentImageHeight;
        short _currentImageWidth;        
        
        bool BetterOnBooru = false;
        bool AlreadyHasTag;

        List<string> DisplayTags = new List<string>();

        public Iqdb_Results()
        {
            InitializeComponent();
        }

        public bool Tagged { get; set; }
        bool NewTags { get; set; }

        ErrorHandling Error = new ErrorHandling();

        public Iqdb_Results(IqdbApi.Models.SearchResult _results, DatabaseUtil db, DatabaseUtil.DBTable data, bool imagetagged, string currentimgname, string directoryPath, short currentimgheight, short currentimgwidth, FileInfo currentimg, int currentimageindex)
        {
            InitializeComponent();
            SetWindowScreen(this, GetWindowScreen(App.Current.MainWindow));

            results = _results;
            _db = db;
            _data = data;
            currentImageName = currentimgname;
            _DirectoryPath = directoryPath;
            _currentImageHeight = currentimgheight;
            _currentImageWidth = currentimgwidth;
            _currentImage = currentimg;
            _currentImageIndex = currentimageindex;            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResizeMode = ResizeMode.NoResize;

            try
            {
                for (int index = 0; index <= results.Matches.Count - 1; index++)
                {
                    if(results.Matches[index].Similarity > 60)
                    {
                        var rowdef = new RowDefinition { Height = GridLength.Auto };
                        ExpanderGrid.RowDefinitions.Add(rowdef);
                        Expander Match = new Expander { Header = results.Matches[index].Source.ToString() + ": " + results.Matches[index].Similarity.ToString() + " - " + results.Matches[index].Resolution.Height + "x" + results.Matches[index].Resolution.Width + (results.Matches[index].MatchType == IqdbApi.Enums.MatchType.Best ? " - Best" : ""), Margin = new Thickness(3), MaxHeight = 860 - (index * 30) };
                        Grid.SetRow(Match, index);
                        Grid.SetColumn(Match, 0);
                        ExpanderGrid.Children.Add(Match);

                        Grid TagListGrid = new Grid { Margin = new Thickness(2) };
                        ScrollViewer Scoller = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Visible };
                        var rowdef1 = new RowDefinition { Height = new GridLength(120, GridUnitType.Pixel) };
                        var rowdef2 = new RowDefinition { Height = GridLength.Auto };
                        TagListGrid.RowDefinitions.Add(rowdef1);
                        TagListGrid.RowDefinitions.Add(rowdef2);

                        Image ImageThumb = new Image();
                        BitmapImage bi = new BitmapImage();
                        bi.BeginInit();
                        bi.UriSource = new Uri("http://iqdb.org" + results.Matches[index].PreviewUrl, UriKind.Absolute);
                        bi.EndInit();
                        ImageThumb.Source = bi;
                        ImageThumb.Stretch = Stretch.Uniform;
                        ImageThumb.Tag = "Http:" + results.Matches[index].Url;
                        ImageThumb.MouseUp += new MouseButtonEventHandler(ImageThumb_clicked);

                        Grid.SetRow(ImageThumb, 0);
                        Grid.SetColumn(ImageThumb, 0);

                        Button AddAllTags = new Button { Content = "Add All Tags", Margin = new Thickness(3), Tag = index };
                        AddAllTags.Click += new RoutedEventHandler(AddAllTags_Clicked);

                        Grid.SetRow(AddAllTags, 1);
                        Grid.SetColumn(AddAllTags, 0);

                        TagListGrid.Children.Add(AddAllTags);
                        TagListGrid.Children.Add(ImageThumb);

                        for (int Index2 = 0; Index2 <= results.Matches[index].Tags.Count - 1; Index2++)
                        {
                            var rowdef3 = new RowDefinition { Height = GridLength.Auto };
                            TagListGrid.RowDefinitions.Add(rowdef3);

                            Button AddIndividualTag = new Button { Content = results.Matches[index].Tags[Index2], Margin = new Thickness(3), Tag = index };
                            Grid.SetRow(AddIndividualTag, Index2 + 2);
                            Grid.SetColumn(AddIndividualTag, 0);

                            AddIndividualTag.Click += new RoutedEventHandler(AddIndividualTag_clicked);
                            TagListGrid.Children.Add(AddIndividualTag);
                        }

                        Match.Content = Scoller;
                        Scoller.Content = TagListGrid;
                    }                    
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error Initializing IQDB results");
                Error.WriteToLog(ex);
            }            
        }

        private void AddIndividualTag_clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                Button TagButt = (Button)sender;
                int BooruIdentifier = Convert.ToInt32(TagButt.Tag);
                AddTag(TagButt.Content.ToString().Replace('_', ' ').Replace('|', 'l').Replace(Environment.NewLine, ""));
                string BooruURL = _db.GetBooruURL(_data, currentImageName);

                if(BooruURL != null)
                {
                    if (BooruURL == "")
                    {
                        if (_currentImageHeight < results.Matches[BooruIdentifier].Resolution.Height || _currentImageWidth < results.Matches[BooruIdentifier].Resolution.Width)
                        {
                            BetterOnBooru = true;
                        }
                        else
                        {
                            BetterOnBooru = false;
                        }
                        if(!_db.UpdateBooruInfo(_data, currentImageName, "Http:" + results.Matches[BooruIdentifier].Url, BetterOnBooru, Convert.ToInt32(results.Matches[BooruIdentifier].Similarity)))
                        {
                            throw new Exception("Error adding booru information to database.");
                        }
                    }
                }
                else
                {
                    throw new Exception("Error determining if image already contains booru information.");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error adding tag to image. - " + ex.Message);
                Error.WriteToLog(ex);
            }            
        }

        private void AddAllTags_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                Button TaggButt = (Button)sender;
                int BooruIdentifier = Convert.ToInt32(TaggButt.Tag);
                for (int index3 = 0; index3 <= results.Matches[BooruIdentifier].Tags.Count - 1; index3++)
                {
                    AddTag(results.Matches[BooruIdentifier].Tags[index3].Replace('_', ' ').Replace('|', 'l').Replace(Environment.NewLine, ""));
                }
                if (_currentImageHeight < results.Matches[BooruIdentifier].Resolution.Height || _currentImageWidth < results.Matches[BooruIdentifier].Resolution.Width)
                {
                    BetterOnBooru = true;
                }
                else
                {
                    BetterOnBooru = false;
                }
                if(!_db.UpdateBooruInfo(_data, currentImageName, "Http:" + results.Matches[BooruIdentifier].Url, BetterOnBooru, Convert.ToInt32(results.Matches[BooruIdentifier].Similarity)))
                {
                    throw new Exception("Error adding booru information to database.");
                }
                this.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error adding tags to image. - " + ex.Message);
                Error.WriteToLog(ex);
            }            
        }

        private void ImageThumb_clicked(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Image keep = (Image)sender;
                System.Diagnostics.Process.Start(keep.Tag.ToString());
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error opening link to image. - " + ex.Message);
                Error.WriteToLog(ex);
            }            
        }

        private void ImageTag(string TagName)
        {
            try
            {
                //Check if File has already had at least one tag added
                if ((bool)_db.IsFileinImageData(_data, currentImageName))
                {
                    if (!_db.AddTagtoImageData(_data, currentImageName, TagName))
                    {
                        throw new Exception("Error adding new tag to ImageData table.");
                    }
                }
                else
                {
                    if (!_db.AddtoImageData(_data, currentImageName, _DirectoryPath, 0, TagName, 1, _currentImageWidth, _currentImageHeight, _currentImage.LastWriteTime, _currentImage.Extension, _currentImage.Length))
                    {
                        throw new Exception("Error adding info to ImageData table");
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error adding tag to ImageTags table. - " + ex.Message);
                Error.WriteToLog(ex);
            }            
        }

        private void AddTag(string TagName)
        {
            try
            {
                //check if Tags table has tag if not add it and increment count            
                if (_db.TagExists(_data, TagName))
                {
                    AddExistingTag(TagName);
                }
                else
                {
                    //Add Tag to Tag Table
                    if(!_db.AddtoTags(_data, TagName))
                    {
                        throw new Exception("Error adding tag to Tags - table");
                    }

                    //add Tag to new tag column in image table
                    ImageTag(TagName);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error adding tag to Tags table. - " + ex.Message);
                Error.WriteToLog(ex);
            }
        }

        private void AddExistingTag(string TagName)
        {
            try
            {
                //Check if tag is already added for image, make textbox background red if so
                DisplayTags.Clear();
                DisplayTags = _db.GetImageTags(_data, currentImageName);
                if(DisplayTags != null)
                {
                    if (DisplayTags.Count > 0)
                    {
                        foreach (string tag in DisplayTags)
                        {
                            if (tag == TagName)
                            {
                                AlreadyHasTag = true;
                                break;
                            }
                            else
                            {
                                AlreadyHasTag = false;
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception("Error retrieving image tags.");
                }

                if (!AlreadyHasTag)
                {
                    //increment tag count                
                    _db.UpdateTagsCount(_data, TagName, true);

                    //add Tag to new tag column in image table
                    ImageTag(TagName);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error adding tag to ImageTags table. - " + ex.Message);
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
                window.Left = workingArea.Left + 60;
                window.Top = workingArea.Top + 60;
            }
        }

        public System.Windows.Forms.Screen GetWindowScreen(Window window)
        {
            return System.Windows.Forms.Screen.FromHandle(new System.Windows.Interop.WindowInteropHelper(window).Handle);
        }
    }
}
