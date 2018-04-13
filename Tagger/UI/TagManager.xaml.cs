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
    public partial class TagManager : Window
    {

        DatabaseUtil DB;            //Database Class
        DatabaseUtil.DBTable Data;  //Database Information
        string fileName = "";       //Filename of associated file (Empty string if all tags)
        List<string> TagNames;      //List for scanning through Tags
        List<string> FileNames;     //List for scanning through files associated with a selected tag
        int PageIndex = 1;          //Index of Page
        int TagsperPage = 500;
        double PageAmount = 0.0;

        public TagManager(DatabaseUtil db, DatabaseUtil.DBTable data)
        {
            InitializeComponent();
            SetWindowScreen(this, GetWindowScreen(App.Current.MainWindow));

            DB = db;
            Data = data;
        }

        public TagManager(string imgname, DatabaseUtil db, DatabaseUtil.DBTable data)
        {

            InitializeComponent();
            SetWindowScreen(this, GetWindowScreen(App.Current.MainWindow));

            fileName = imgname;
            DB = db;
            Data = data;
        }

        ErrorHandling Error = new ErrorHandling();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateUI();            
        }

        private void UpdateUI()
        {
            try
            {
                //If there is no filename we want all of the tags
                if (fileName == "")
                {                    
                    TagNames = DB.GetAllTags(Data); //Retrieve all of the tags
                    if(TagNames == null)
                    {
                        throw new Exception("Error retreiving tags from database.");
                    }
                    TagNames.Sort();                //sort all of the tags alphabetically
                }
                else  //If there is a filename get all of the tags for the associated image
                {
                    TagNames = DB.GetImageTags(Data, fileName);
                    if(TagNames == null)
                    {
                        throw new Exception("Error retreiving image tags from database.");
                    }
                }
                if (TagNames.Count > 0) //If image has at least one tag
                {
                    PageAmount = (double)TagNames.Count / TagsperPage;  //set the page amount so there are 500 rows per page
                    TagList.Children.Clear();                   //clear the grid for updating

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
                        Content = PageIndex.ToString() + "/" + Math.Ceiling(PageAmount).ToString() + " (" + TagNames.Count.ToString() + ")",
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
                    TagList.Children.Add(NavGridTop);

                    for (int p = ((PageIndex - 1) * TagsperPage); (p <= (TagsperPage * PageIndex) - 1) && (p <= TagNames.Count - 1); p++)   //loop through tag names up to 500 or end of list which ever comes first
                    {
                        Grid TagItemGrid = new Grid();
                        ColumnDefinition ColDefA = new ColumnDefinition() { Width = new GridLength(8, GridUnitType.Star) };
                        ColumnDefinition ColDefB = new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) };
                        ColumnDefinition ColDefC = new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) };
                        ColumnDefinition ColDefD = new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) };
                        TagItemGrid.ColumnDefinitions.Add(ColDefD);
                        TagItemGrid.ColumnDefinitions.Add(ColDefA);
                        TagItemGrid.ColumnDefinitions.Add(ColDefB);
                        TagItemGrid.ColumnDefinitions.Add(ColDefC);

                        TextBlock TagNumber = new TextBlock { Text = (p + 1).ToString() + ": ", Margin = new Thickness(1), VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Right };
                        TextBlock TagLabel = new TextBlock { Text = TagNames[p], Tag = TagNames[p], Margin = new Thickness(1), Width = 360, ToolTip = TagNames[p], TextWrapping = TextWrapping.Wrap }; //create label for each tag
                        Button EditButton = new Button { Content = "Edit", Tag = TagNames[p], Margin = new Thickness(1) };                              //create edit button for each tag
                        Button RemoveButton = new Button { Content = "Remove", Tag = TagNames[p], Margin = new Thickness(1) };                          //create remove button for each tag
                        EditButton.Click += new RoutedEventHandler(Edit_Click);         //create click event for edit button
                        RemoveButton.Click += new RoutedEventHandler(Remove_Click);     //create click event for remove button

                        Grid.SetColumn(TagNumber, 0);
                        Grid.SetColumn(TagLabel, 1);        //set column and row for tag label
                        Grid.SetColumn(EditButton, 2);      //set column and row for edit button
                        Grid.SetColumn(RemoveButton, 3);    //set column and row for remove button                        

                        TagItemGrid.Children.Add(TagNumber);
                        TagItemGrid.Children.Add(TagLabel);     //add tag Label, edit button, and remove button to grid
                        TagItemGrid.Children.Add(RemoveButton);
                        TagItemGrid.Children.Add(EditButton);
                        TagList.Children.Add(TagItemGrid);
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
                        Content = PageIndex.ToString() + "/" + Math.Ceiling(PageAmount).ToString() + " (" + TagNames.Count.ToString() + ")",
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

                    TagList.Children.Add(NavGridBottom);     //add navigation grid to tag list grid
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error Loading Tags - " + ex.Message);
                Error.WriteToLog(ex);
                this.Close();
            }            
        }

        private void NextPage_Clicked(object sender, RoutedEventArgs e)
        {
            int TempIndex = PageIndex;

            try
            {
                PageIndex++;    //increment page index
                if (PageIndex > PageAmount) //make sure page index didn't go over max number of pages
                {
                    PageIndex = (int)Math.Ceiling(PageAmount);    //if pag index went over max number of pages set page index back to max number of pages
                }
                else
                {
                    UpdateUI();
                    Scroller.ScrollToTop();
                }
            }
            catch (Exception ex)
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
                if (PageIndex < 1)
                {
                    PageIndex = 1;
                }
                else
                {
                    UpdateUI();
                    Scroller.ScrollToTop();
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
                if (PageIndex != 1)
                {
                    PageIndex = 1;
                    UpdateUI();
                    Scroller.ScrollToTop();
                }
            }
            catch (Exception ex)
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
                if (PageIndex != (int)Math.Ceiling(PageAmount))
                {
                    PageIndex = (int)Math.Ceiling(PageAmount);
                    UpdateUI();
                    Scroller.ScrollToTop();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error moving to first page. - " + ex.Message);
                Error.WriteToLog(ex);
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            Button senderButt = (Button)sender;                 //remove button that was clicked
            string selectedTag = senderButt.Tag.ToString();     //tag associated with button that was clicked

            try
            {
                if (fileName != "") //If We are NOT in all tag mode
                {
                    if(selectedTag != "")
                    {
                        MessageBoxResult check = MessageBox.Show("Are you sure you Want to Delete this Tag?", "Delete?", MessageBoxButton.YesNo, MessageBoxImage.Warning);  //Ask user if they really want to remove tag
                        if (check == MessageBoxResult.Yes)
                        {
                            if (!DB.RemoveImageTag(Data, selectedTag, fileName)) //if so attempt to remove tag from Tag string in ImageTags Database Table
                            {
                                throw new Exception("Error removing " + selectedTag);
                            }
                            DB.DecrementImageDataTagCount(Data, fileName);      //lower the image tag count by 1
                            DB.UpdateTagsCount(Data, selectedTag, false);       //lower the tag table tag count by one
                        }
                        if (DB.GetIndividualTagCount(Data, selectedTag) == 0)   //if tag count for tag in Tags Table is 0 there is no images it is tagged to remove tag from tags table      
                        {
                            DB.DeleteTag(Data, selectedTag);
                        }
                    }                    
                }
                else  //If we are in all tags mode
                {
                    if(selectedTag != "")
                    {
                        FileNames = DB.SearchForFiles(Data, selectedTag);   //Retrieve file names that contain selected tag
                        if (FileNames == null)
                        {
                            throw new Exception("Error determining files with tag: " + selectedTag);
                        }
                        foreach (string file in FileNames)  //for each filename attempt to remove tag from tag string in imagetags database table
                        {
                            if (!DB.RemoveImageTag(Data, selectedTag, file))
                            {
                                throw new Exception("Error removing " + selectedTag + " from " + file);
                            }
                            DB.DecrementImageDataTagCount(Data, file);  //lower the image tag count by 1
                        }
                        DB.DeleteTag(Data, selectedTag);    //Delete that selected tag from the tags table
                    }                    
                }
                UpdateUI();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error removing selected tag. - " + ex.Message);
                Error.WriteToLog(ex);
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            Button senderButt = (Button)sender;                 //edit button that was clicked
            List<string> TagsToUpdate;                          //List of tags
            string selectedTag = senderButt.Tag.ToString();     //tag associated with edit button that was clicked
            string updatedTag;                                  //new tag submitted by user in inputWindow
            string searchString = ";";                          //tag string

            try
            {
                UI.InputWindow IW = new UI.InputWindow("Enter new tag name to update", selectedTag);    //create new input window for user to input updated tag name
                IW.ShowDialog();                                //show InputWindow
                updatedTag = IW.Response;                       //collect inputted tag form InputWindow
                if (updatedTag != "" && updatedTag != null)     //as long as the updated tag isn't empty or null we can go about changing the databases
                {
                    MessageBoxResult check = MessageBox.Show("Are you sure you Want to Update this Tag?", "Update?", MessageBoxButton.YesNo, MessageBoxImage.Warning);  //verify user actually wants to update tag
                    if (check == MessageBoxResult.Yes)
                    {
                        if (fileName != "") //if we are NOT in all tags mode
                        {
                            foreach (string tag in TagNames)    //grab each tag and form the tag string with new tag in place of the old tag
                            {
                                if (tag == selectedTag)
                                {
                                    searchString = searchString + updatedTag + ";";
                                }
                                else
                                {
                                    searchString = searchString + tag + ";";
                                }

                            }
                            if(!DB.UpdateTaginImageData(Data, fileName, searchString))    //update tag string in database with updated tag string
                            {
                                throw new Exception("Error updating tag to " + updatedTag + " for file " + fileName);
                            }
                            DB.UpdateTagsCount(Data, selectedTag, false);               //decrease count of old tag by one in tags table
                            if (DB.TagExists(Data, updatedTag))                         //if updated tag already exists increase its count by one in tags table
                            {
                                DB.UpdateTagsCount(Data, updatedTag, true);
                            }
                            else  //if updated tag didn't already exist in tags table add it
                            {
                                if(!DB.AddtoTags(Data, updatedTag))
                                {
                                    throw new Exception("Error adding " + updatedTag +  " to Tags Database");
                                }
                            }
                            if (DB.GetIndividualTagCount(Data, selectedTag) == 0)   //if old tag in tags table now has a count of one delete it
                            {
                                DB.DeleteTag(Data, selectedTag);
                            }
                        }
                        else  //if we are in all tags mode
                        {
                            FileNames = DB.SearchForFiles(Data, selectedTag);       //retrieve all files tagged with selected tag
                            if(FileNames == null)
                            {
                                throw new Exception("Error retrieving files tagged with " + selectedTag);
                            }
                            foreach (string file in FileNames)                      //for each file
                            {
                                searchString = ";";
                                TagsToUpdate = DB.GetImageTags(Data, file);         //retrieve tags for file
                                if(TagsToUpdate == null)
                                {
                                    throw new Exception("Error retrieving tags to update " + selectedTag);
                                }
                                foreach (string tag in TagsToUpdate)                //update tag string for file with new tag in place of old tag
                                {
                                    if (tag == selectedTag)
                                    {
                                        searchString = searchString + updatedTag + ";";
                                    }
                                    else
                                    {
                                        searchString = searchString + tag + ";";
                                    }

                                }
                                if(!DB.UpdateTaginImageData(Data, file, searchString))    //update tag string in database with updated tags string
                                {
                                    throw new Exception("Error updating tag to " + updatedTag + " for file " + file);
                                }
                                DB.UpdateTagsCount(Data, selectedTag, false);           //decrement old tag count in tags table
                                if (DB.TagExists(Data, updatedTag))                     //if new tag exists in tags table increment its tag count
                                {
                                    DB.UpdateTagsCount(Data, updatedTag, true);
                                }
                                else  //if new tag does not exist in tags table add it
                                {
                                    if(!DB.AddtoTags(Data, updatedTag))
                                    {
                                        throw new Exception("Error adding " + updatedTag + " to Tags Database");
                                    }
                                }
                                if (DB.GetIndividualTagCount(Data, selectedTag) == 0)   //if old tag count is now zero delete tag from tags table
                                {
                                    DB.DeleteTag(Data, selectedTag);
                                }
                            }
                        }
                        UpdateUI();
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error editing tag - " + ex.Message);
                Error.WriteToLog(ex);
            }            
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)    //scrolls scroll bar to first tag in list that starts with the letter of the keyboard key hit
        {
            Label u;
            try
            {
                foreach (UIElement h in TagList.Children)   //loop through labels and buttons in tag lis
                {
                    if (h is Label) //if specific element is a label and it starts with the keyboard letter that was hit
                    {
                        u = (Label)h;
                        if (u.Tag.ToString().StartsWith(e.Key.ToString().ToUpper()) || u.Tag.ToString().StartsWith(e.Key.ToString().ToLower())) 
                        {
                            var point = h.TranslatePoint(new Point(), TagList); //grab the point in the scroll viewer associated with found element
                            Scroller.ScrollToVerticalOffset(point.Y - 3);       //scroll the scroll viewer to that element
                            return;
                        }
                    }
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

