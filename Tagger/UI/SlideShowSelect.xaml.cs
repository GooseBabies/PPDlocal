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

namespace Tagger
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class SlideShowSelect : Window
    {
        DatabaseUtil DB;
        DatabaseUtil.DBTable Data;
        List<string> TagInfo = new List<string>();
        List<string> SearchTags = new List<string>();
        List<string> files = new List<string>();
        string[] filearray;
        XMLUtil.SaveSettings savesettings;
        string prof;

        public SlideShowSelect(DatabaseUtil db, DatabaseUtil.DBTable data, XMLUtil.SaveSettings ss, string profilename)
        {
            InitializeComponent();
            SetWindowScreen(this, GetWindowScreen(App.Current.MainWindow));
            DB = db;
            Data = data;
            prof = profilename;
            savesettings = ss;
        }

        private void Main_Click(object sender, RoutedEventArgs e)
        {
            if(MainSearch.Text == "")
            {
                MainSearch.Background = Brushes.LightPink;
                MainSearch.Text = "";
            }
            else
            {
                if (files.Count > 0)
                {
                    filearray = files.ToArray();
                    SlideShow ss = new SlideShow(filearray, prof, Data, savesettings);
                    ss.Show();
                    MainSearch.Text = "";
                    MainSearch.ClearValue(BackgroundProperty);
                }
                else
                {
                    MainSearch.Background = Brushes.LightPink;
                    MainSearch.Text = "";
                }
            }                

        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink chosenTag = (Hyperlink)sender;
            MainSearch.Text = MainSearch.Text + chosenTag.Tag.ToString() + ";";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TagInfo.Clear();
            TagInfo = DB.GetAllTags(Data);
            TagInfo.Sort();
            TagList.Children.RemoveAt(0);
            if(TagInfo.Count > 0)
            {
                foreach(string tag in TagInfo)
                {
                    
                    TagList.Children.Add(new TextBlock { Tag = tag, Margin=new Thickness(10, 2, 10, 2), FontSize=14, FontWeight=FontWeights.Bold, VerticalAlignment=VerticalAlignment.Center});
                }
                foreach (TextBlock k in TagList.Children)
                {
                    Run Tagrun = new Run(k.Tag.ToString());
                    Hyperlink linky = new Hyperlink(Tagrun) { Tag = k.Tag };
                    linky.Click += new RoutedEventHandler(Hyperlink_Click);
                    k.Inlines.Add(linky);
                }
            }
            
        }

        private void Ratingbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem rating;
            rating = (ListBoxItem)Ratingbox.SelectedItem;
            MainSearch.Text = MainSearch.Text + "Rating:" + rating.Content.ToString() + ";";
        }

        private void MainSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (MainSearch.Text == "")
            //{
            //    MainSearch.ClearValue(BackgroundProperty);
            //    RowCount.Content = "0";
            //}
            //else
            //{
            //    SearchTags = MainSearch.Text.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            //    short rating = 0;
            //    foreach (string tag in SearchTags)
            //    {
            //        if (tag.Contains("Rating:"))
            //        {
            //            rating = Convert.ToInt16(tag.Replace("Rating:", ""));
            //        }
            //    }
            //    SearchTags.Remove("Rating:" + rating.ToString());
            //    if (SearchTags.Count < 1)
            //    {
            //        files = DB.SearchForFiles(Data, rating);
            //    }
            //    else if (rating == 0)
            //    {
            //        files = DB.SearchForFiles(Data, SearchTags);
            //    }
            //    else
            //    {
            //        files = DB.SearchForFiles(Data, SearchTags, rating);
            //    }

            //    RowCount.Content = files.Count.ToString();

            //}
        }

        private void All_Click(object sender, RoutedEventArgs e)
        {
            SlideShow ss = new SlideShow(new string[0], prof, Data, savesettings);
            ss.Show();
            MainSearch.Text = "";
            MainSearch.ClearValue(BackgroundProperty);
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
