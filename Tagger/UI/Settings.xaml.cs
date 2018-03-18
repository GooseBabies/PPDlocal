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
using System.IO;

namespace Tagger
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        XMLUtil XMLUtilities = new XMLUtil();
        DatabaseUtil db = new DatabaseUtil();
        DatabaseUtil.DBTable Data;
        public XMLUtil.SaveSettings Savesettings;
        public string defaultprofile;
        public bool DeletedDefaultProfile { get; set; }
        public string DeletedProfile { get; set; }
        public bool Deleted { get; set; }
        public bool DeletedOnlyProfile { get; set; }
        public string DeletedDatabasepath { get; set; }

        public Settings(DatabaseUtil.DBTable data)
        {
            InitializeComponent();

            SetWindowScreen(this, GetWindowScreen(App.Current.MainWindow));

            Savesettings = XMLUtilities.LoadDefaultProfile();
            UpdateControls();

            UpdateProfiles();
            defaultprofile = XMLUtilities.GetDefaultProfile();
            Profiles.SelectedItem = defaultprofile;
            Deleted = false;
            DeletedOnlyProfile = false;
            Data = data;
        }

        private void UpdateProfiles()
        {
            Profiles.Items.Clear();
            foreach (string prof in XMLUtilities.GetProfiles())
            {
                Profiles.Items.Add(prof);
            }
        }

        private void UpdateControls()            
        {
            Shuffle.IsChecked = Savesettings.Shuffle;
            ShuffleRepeat.IsEnabled = (Shuffle.IsChecked == true ? true : false);
            ShuffleRepeat.IsChecked = (ShuffleRepeat.IsEnabled == true ? Savesettings.RepeatShuffle : false);
            SlideshowInterval.Text = Savesettings.Slideshowinterval.ToString();
            ViewCategories.IsChecked = Savesettings.ViewCat;
            png.IsChecked = Savesettings.ImageFileTypes.Contains(".png");
            jpeg.IsChecked = Savesettings.ImageFileTypes.Contains(".jpeg");
            jpg.IsChecked = Savesettings.ImageFileTypes.Contains(".jpg");
            bmp.IsChecked = Savesettings.ImageFileTypes.Contains(".bmp");
            gif.IsChecked = Savesettings.ImageFileTypes.Contains(".gif");
            mp4.IsChecked = Savesettings.ImageFileTypes.Contains(".mp4");
            mpg.IsChecked = Savesettings.ImageFileTypes.Contains(".mpg");
            mkv.IsChecked = Savesettings.ImageFileTypes.Contains(".mkv");
            avi.IsChecked = Savesettings.ImageFileTypes.Contains(".avi");
            wmv.IsChecked = Savesettings.ImageFileTypes.Contains(".wmv");
            MouseDelay.Text = Savesettings.Mousedisappeardelay.ToString();
            VisibleNav.IsChecked = Savesettings.VisibleNav;
            Booru.SelectedIndex = Savesettings.BooruSource;
            BooruThreshold.Text = Savesettings.BooruThreshold.ToString();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            Savesettings.RepeatShuffle = (bool)ShuffleRepeat.IsChecked && ShuffleRepeat.IsEnabled;
            Savesettings.Shuffle = (bool)Shuffle.IsChecked;
            Savesettings.Slideshowinterval = Convert.ToInt32(SlideshowInterval.Text);
            Savesettings.ViewCat = (bool)ViewCategories.IsChecked;
            Savesettings.ImageFileTypes = ((bool)png.IsChecked ? ".png," : "") + ((bool)jpeg.IsChecked ? ".jpeg," : "") + ((bool)jpg.IsChecked ? ".jpg," : "") + ((bool)bmp.IsChecked ? ".bmp," : "") + ((bool)gif.IsChecked ? ".gif," : "");
            Savesettings.VideoFileTypes = ((bool)mp4.IsChecked ? ".mp4," : "") + ((bool)mpg.IsChecked ? ".mpg," : "") + ((bool)mkv.IsChecked ? ".mkv," : "") + ((bool)avi.IsChecked ? ".avi," : "") + ((bool)wmv.IsChecked ? ".wmv," : "");
            //Savesettings.Png = (bool)png.IsChecked;
            //Savesettings.Jpeg = (bool)jpeg.IsChecked;
            //Savesettings.Jpg = (bool)jpg.IsChecked;
            //Savesettings.Bmp = (bool)bmp.IsChecked;
            //Savesettings.Gif = (bool)gif.IsChecked;
            //Savesettings.Mp4 = (bool)mp4.IsChecked;
            //Savesettings.Mpg = (bool)mpg.IsChecked;
            //Savesettings.Mkv = (bool)mkv.IsChecked;
            //Savesettings.Avi = (bool)avi.IsChecked;
            //Savesettings.Wmv = (bool)wmv.IsChecked;
            Savesettings.Mousedisappeardelay = Convert.ToInt32(MouseDelay.Text);
            Savesettings.VisibleNav = (bool)VisibleNav.IsChecked;
            Savesettings.BooruSource = Booru.SelectedIndex;
            Savesettings.BooruThreshold = Convert.ToInt32(BooruThreshold.Text.ToString());

            XMLUtilities.SaveFile(Savesettings, Profiles.SelectedItem.ToString());
            this.Close();
        }

        private void DefaultSettings_Click(object sender, RoutedEventArgs e)
        {
            Shuffle.IsChecked = true;
            ShuffleRepeat.IsEnabled = true;
            ShuffleRepeat.IsChecked = false;
            SlideshowInterval.Text = "3";
            ViewCategories.IsChecked = false;
            png.IsChecked = true;
            jpeg.IsChecked = true;
            jpg.IsChecked = true;
            bmp.IsChecked = true;
            gif.IsChecked = true;
            mp4.IsChecked = true;
            mpg.IsChecked = true;
            mkv.IsChecked = true;
            avi.IsChecked = true;
            wmv.IsChecked = true;
            MouseDelay.Text = "4";
            VisibleNav.IsChecked = true;
            Booru.SelectedIndex = 0;
            BooruThreshold.Text = "60";
            
            Savesettings.RepeatShuffle = (bool)ShuffleRepeat.IsChecked && (bool)Shuffle.IsEnabled;
            Savesettings.Shuffle = (bool)Shuffle.IsChecked;
            Savesettings.Slideshowinterval = Convert.ToInt32(SlideshowInterval.Text);
            Savesettings.ViewCat = (bool)ViewCategories.IsChecked;
            Savesettings.ImageFileTypes = ((bool)png.IsChecked ? ".png," : "") + ((bool)jpeg.IsChecked ? ".jpeg," : "") + ((bool)jpg.IsChecked ? ".jpg," : "") + ((bool)bmp.IsChecked ? ".bmp," : "") + ((bool)gif.IsChecked ? ".gif," : "");
            Savesettings.VideoFileTypes = ((bool)mp4.IsChecked ? ".mp4," : "") + ((bool)mpg.IsChecked ? ".mpg," : "") + ((bool)mkv.IsChecked ? ".mkv," : "") + ((bool)avi.IsChecked ? ".avi," : "") + ((bool)wmv.IsChecked ? ".wmv," : "");
            //Savesettings.Png = (bool)png.IsChecked;
            //Savesettings.Jpeg = (bool)jpeg.IsChecked;
            //Savesettings.Jpg = (bool)jpg.IsChecked;
            //Savesettings.Bmp = (bool)bmp.IsChecked;
            //Savesettings.Gif = (bool)gif.IsChecked;
            //Savesettings.Mp4 = (bool)mpg.IsChecked;
            //Savesettings.Mpg = (bool)mpg.IsChecked;
            //Savesettings.Mkv = (bool)mkv.IsChecked;
            //Savesettings.Avi = (bool)avi.IsChecked;
            //Savesettings.Wmv = (bool)wmv.IsChecked;
            Savesettings.Mousedisappeardelay = Convert.ToInt32(MouseDelay.Text);
            Savesettings.VisibleNav = (bool)VisibleNav.IsChecked;
            Savesettings.BooruSource = Booru.SelectedIndex;
            Savesettings.BooruThreshold = Convert.ToInt32(BooruThreshold.Text.ToString());

            XMLUtilities.SaveFile(Savesettings, Profiles.SelectedItem.ToString());
            this.Close();
        }

        private void Profiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!Deleted)
            {
                Savesettings = XMLUtilities.LoadProfile(Profiles.SelectedItem.ToString());
                UpdateControls();
            }
            
        }

        private void RenameProfile_Click(object sender, RoutedEventArgs e)
        {
            UI.InputWindow get = new UI.InputWindow("Enter New Profile Name", "");
            if(get.Response != "")
            {
                XMLUtilities.RenameProfile(Profiles.SelectedItem.ToString(), get.Response);
                UpdateProfiles();
            }          
        }

        private void DeleteProfile_Click(object sender, RoutedEventArgs e)
        {          
            
            Deleted = true;
            DeletedDatabasepath = XMLUtilities.DeleteProfile(Profiles.SelectedItem.ToString());
            if (Profiles.SelectedItem.ToString() == defaultprofile)
            {
                DeletedProfile = defaultprofile;
                Profiles.Items.Remove(Profiles.SelectedItem);
                if(Profiles.Items.Count < 1)
                {
                    DeletedOnlyProfile = true;
                }
                else
                {
                    XMLUtilities.ChangeDefaultprofile(Profiles.Items[0].ToString());
                }                
                DeletedDefaultProfile = true;
                
            }
            else
            {
                DeletedProfile = Profiles.SelectedItem.ToString();
                Profiles.Items.Remove(Profiles.SelectedItem);
                DeletedDefaultProfile = false;                
            }            
            this.Close();
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

        private void Shuffle_Click(object sender, RoutedEventArgs e)
        {
            if (Shuffle.IsChecked == true)
            {
                ShuffleRepeat.IsEnabled = true;
            }
            else
            {
                ShuffleRepeat.IsEnabled = false;
            }
        
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            db.Flush(Data);
        }
    }
}
