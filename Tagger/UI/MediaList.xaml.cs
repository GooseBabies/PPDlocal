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
using System.IO;

namespace Tagger.UI
{
    /// <summary>
    /// Interaction logic for MediaList.xaml
    /// </summary>
    public partial class MediaList : Window
    {
        Button Butt;
        List<FileInfo> AllFiles = new List<FileInfo>();
        List<FileInfo> AllFilesFilter = new List<FileInfo>();
        List<FileInfo> VideoFiles = new List<FileInfo>();
        List<FileInfo> VideoFilesFilter = new List<FileInfo>();
        int PageIndex = 1;
        double PageAmount = 0.0;
        int FilePageAmount = 100;
        int MaxPageCount = 0;

        public string FileName { get; set;}

        ErrorHandling Error = new ErrorHandling();

        public MediaList(List<FileInfo> FI)
        {
            InitializeComponent();
            SetWindowScreen(this, GetWindowScreen(App.Current.MainWindow));

            try
            {
                AllFiles = FI.OrderBy(x => x.Name).ToList();
                MaxPageCount = (int)Math.Ceiling((double)AllFiles.Count / FilePageAmount);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error initializing media list. - " + ex.Message);
                Error.WriteToLog(ex);
                FileName = "";
                this.Close();
            }            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (FileInfo File in AllFiles)
                {
                    if (File.Extension == ".mp4" || File.Extension == ".mpg" || File.Extension == ".mkv" || File.Extension == ".wmv" || File.Extension == ".avi")
                    {
                        VideoFiles.Add(File);
                    }
                }
                ChooseFiles();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing media list. - " + ex.Message);
                Error.WriteToLog(ex);
                FileName = "";
                this.Close();
            }
        }

        private void UpdateUI(List<FileInfo> FileList)
        {
            try
            {
                PageAmount = (double)FileList.Count / FilePageAmount;
                Main.Children.RemoveRange(1, Main.Children.Count - 1);

                Grid NavGridTop = new Grid();
                ColumnDefinition ColDef1 = new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) };
                ColumnDefinition ColDef2 = new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) };
                ColumnDefinition ColDef3 = new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) };
                ColumnDefinition ColDef4 = new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) };
                ColumnDefinition ColDef5 = new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) };

                NavGridTop.ColumnDefinitions.Add(ColDef1);
                NavGridTop.ColumnDefinitions.Add(ColDef2);
                NavGridTop.ColumnDefinitions.Add(ColDef3);
                NavGridTop.ColumnDefinitions.Add(ColDef4);
                NavGridTop.ColumnDefinitions.Add(ColDef5);

                Button FirstPageTop = new Button()
                {
                    Content = "First"
                };
                FirstPageTop.Click += new RoutedEventHandler(FirstPage_Clicked);

                Button PrevPageTop = new Button()
                {
                    Content = "Previous"
                };
                PrevPageTop.Click += new RoutedEventHandler(PrevPage_Clicked);

                Label PageCountTop = new Label()
                {
                    Content = PageIndex.ToString() + "/" + Math.Ceiling(PageAmount).ToString() + " (" + FileList.Count.ToString() + ")",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                Button NextPageTop = new Button()
                {
                    Content = "Next"
                };
                NextPageTop.Click += new RoutedEventHandler(NextPage_Clicked);

                Button LastPageTop = new Button()
                {
                    Content = "Last Page"
                };
                LastPageTop.Click += new RoutedEventHandler(LastPage_Clicked);

                Grid.SetColumn(FirstPageTop, 0);
                Grid.SetColumn(PrevPageTop, 1);
                Grid.SetColumn(PageCountTop, 2);
                Grid.SetColumn(NextPageTop, 3);
                Grid.SetColumn(LastPageTop, 4);

                NavGridTop.Children.Add(FirstPageTop);
                NavGridTop.Children.Add(PrevPageTop);
                NavGridTop.Children.Add(PageCountTop);
                NavGridTop.Children.Add(NextPageTop);
                NavGridTop.Children.Add(LastPageTop);
                Main.Children.Add(NavGridTop);

                for (int p = ((PageIndex - 1) * FilePageAmount); (p <= (FilePageAmount * PageIndex) - 1) && (p <= FileList.Count - 1); p++)
                {
                    Butt = new Button { Content = FileList[p].Name, Tag = FileList[p].Extension };
                    Butt.Click += new RoutedEventHandler(Media_Clicked);
                    Main.Children.Insert(Main.Children.Count, Butt);
                }

                Grid NavGridBottom = new Grid();
                ColumnDefinition ColDef6 = new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) };
                ColumnDefinition ColDef7 = new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) };
                ColumnDefinition ColDef8 = new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) };
                ColumnDefinition ColDef9 = new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) };
                ColumnDefinition ColDef10 = new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) };

                NavGridBottom.ColumnDefinitions.Add(ColDef6);
                NavGridBottom.ColumnDefinitions.Add(ColDef7);
                NavGridBottom.ColumnDefinitions.Add(ColDef8);
                NavGridBottom.ColumnDefinitions.Add(ColDef9);
                NavGridBottom.ColumnDefinitions.Add(ColDef10);

                Button FirstPageBottom = new Button()
                {
                    Content = "First"
                };
                FirstPageTop.Click += new RoutedEventHandler(FirstPage_Clicked);

                Button PrevPageBottom = new Button()
                {
                    Content = "Previous"
                };
                PrevPageBottom.Click += new RoutedEventHandler(PrevPage_Clicked);

                Label PageCountBottom = new Label()
                {
                    Content = PageIndex.ToString() + "/" + Math.Ceiling(PageAmount).ToString() + " (" + FileList.Count.ToString() + ")",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                Button NextPageBottom = new Button()
                {
                    Content = "Next"
                };
                NextPageBottom.Click += new RoutedEventHandler(NextPage_Clicked);

                Button LastPageBottom = new Button()
                {
                    Content = "Last Page"
                };
                LastPageBottom.Click += new RoutedEventHandler(LastPage_Clicked);

                Grid.SetColumn(FirstPageBottom, 0);
                Grid.SetColumn(PrevPageBottom, 1);
                Grid.SetColumn(PageCountBottom, 2);
                Grid.SetColumn(NextPageBottom, 3);
                Grid.SetColumn(LastPageBottom, 4);
                NavGridBottom.Children.Add(FirstPageBottom);
                NavGridBottom.Children.Add(PrevPageBottom);
                NavGridBottom.Children.Add(PageCountBottom);
                NavGridBottom.Children.Add(NextPageBottom);
                NavGridBottom.Children.Add(LastPageBottom);
                Main.Children.Add(NavGridBottom);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing media list. - " + ex.Message);
                Error.WriteToLog(ex);
                FileName = "";
                this.Close();
            }
        }

        private void NextPage_Clicked(object sender, RoutedEventArgs e)
        {
            int TempIndex = PageIndex;

            try
            {
                PageIndex++;    //increment page index
                if (PageIndex > MaxPageCount) //make sure page index didn't go over max number of pages
                {
                    PageIndex = MaxPageCount;    //if pag index went over max number of pages set page index back to max number of pages
                }
                else
                {
                    ChooseFiles();
                    scroller.ScrollToTop();
                }                
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error moving to next page. - " + ex.Message);
                Error.WriteToLog(ex);
                PageIndex = TempIndex;
            }
        }

        private void PrevPage_Clicked(object sender, RoutedEventArgs e)
        {
            int TempIndex = PageIndex;

            try
            {
                PageIndex--;
                if(PageIndex < 1)
                {
                    PageIndex = 1;
                }
                else
                {
                    ChooseFiles();
                    scroller.ScrollToTop();
                }                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error moving to next page. - " + ex.Message);
                Error.WriteToLog(ex);
                PageIndex = TempIndex;
            }
        }

        private void FirstPage_Clicked(object sender, RoutedEventArgs e)
        {
            int TempIndex = PageIndex;
            try
            {
                if(PageIndex != 1)
                {
                    PageIndex = 1;
                    ChooseFiles();
                    scroller.ScrollToTop();
                }                
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error moving to first page. - " + ex.Message);
                Error.WriteToLog(ex);
            }
        }

        private void LastPage_Clicked(object sender, RoutedEventArgs e)
        {
            int TempIndex = PageIndex;
            try
            {
                if(PageIndex != MaxPageCount)
                {
                    PageIndex = MaxPageCount;
                    ChooseFiles();
                    scroller.ScrollToTop();
                }                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error moving to first page. - " + ex.Message);
                Error.WriteToLog(ex);
            }
        }

        private void Media_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                Button Hutt = (Button)sender;
                FileName = Hutt.Content.ToString();
                this.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error opening file. - " + ex.Message);
                Error.WriteToLog(ex);
                FileName = "";
                this.Close();
            }
        }

        private void SwitchContent_Click(object sender, RoutedEventArgs e)
        {
            PageIndex = 1;
            ChooseFiles();
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            PageIndex = 1;
            ChooseFiles();
        }

        private void ChooseFiles()
        {
            try
            {
                if (SwitchContent.IsChecked == true)
                {
                    if (Search.Text == "")
                    {
                        UpdateUI(VideoFiles);
                        MaxPageCount = (int)Math.Ceiling((double)VideoFiles.Count / FilePageAmount);
                    }
                    else
                    {
                        DisplayMediaSearch();
                    }

                }
                else
                {
                    if (Search.Text == "")
                    {
                        UpdateUI(AllFiles);
                        MaxPageCount = (int)Math.Ceiling((double)AllFiles.Count / FilePageAmount);
                    }
                    else
                    {
                        DisplaySearch();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error ching what files are displayed. - " + ex.Message);
                Error.WriteToLog(ex);
            }
        }
        
        private void DisplayMediaSearch()
        {
            try
            {
                VideoFilesFilter = VideoFiles.Where(v => v.Name.ToUpper().Contains(Search.Text.ToUpper())).ToList();
                MaxPageCount = (int)Math.Ceiling((double)VideoFilesFilter.Count / FilePageAmount);
                UpdateUI(VideoFilesFilter);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error displaying video files that match search criteria. - " + ex.Message);
                Error.WriteToLog(ex);
            }
        }

        private void DisplaySearch()
        {
            try
            {
                AllFilesFilter = AllFiles.Where(a => a.Name.ToUpper().Contains(Search.Text.ToUpper())).ToList();
                MaxPageCount = (int)Math.Ceiling((double)AllFilesFilter.Count / FilePageAmount);
                UpdateUI(AllFilesFilter);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error displaying all files that meet the search criteria. - " + ex.Message);
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
