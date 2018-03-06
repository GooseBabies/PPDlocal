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
        int pageindex = 1;          //Index of Page

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
            short Counter = 0;
            double PageAmount = 0.0;

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
                    PageAmount = (double)TagNames.Count / 500;  //set the page amount so there are 500 rows per page
                    TagList.Children.Clear();                   //clear the grid for updating
                    for (int p = ((pageindex - 1) * 500); (p <= (500 * pageindex) - 1) && (p <= TagNames.Count - 1); p++)   //loop through tag names up to 500 or end of list which ever comes first
                    {
                        var rowdef = new RowDefinition() { Height = GridLength.Auto };  //create auto height row
                        TagList.RowDefinitions.Add(rowdef);                             //add row to grid
                        Label TagLabel = new Label { Content = (p + 1).ToString() + ": " + TagNames[p], Tag = TagNames[p], Margin = new Thickness(1) }; //create label for each tag
                        Button EditButton = new Button { Content = "Edit", Tag = TagNames[p], Margin = new Thickness(1) };                              //create edit button for each tag
                        Button RemoveButton = new Button { Content = "Remove", Tag = TagNames[p], Margin = new Thickness(1) };                          //create remove button for each tag
                        EditButton.Click += new RoutedEventHandler(Edit_Click);         //create click event for edit button
                        RemoveButton.Click += new RoutedEventHandler(Remove_Click);     //create click event for remove button

                        Grid.SetColumn(TagLabel, 0);        //set column and row for tag label                                
                        Grid.SetRow(TagLabel, Counter);

                        Grid.SetColumn(RemoveButton, 2);    //set column and row for remove button
                        Grid.SetRow(RemoveButton, Counter);

                        Grid.SetColumn(EditButton, 1);      //set column and row for edit button
                        Grid.SetRow(EditButton, Counter);

                        TagList.Children.Add(TagLabel);     //add tag Label, edit button, and remove button to grid
                        TagList.Children.Add(RemoveButton);
                        TagList.Children.Add(EditButton);
                        Counter++;                          //increment counter
                    }
                    Grid navpanel = new Grid();             //create new grid for next and previous buttons
                    var coldef1 = new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) };  //create 3 column definitions all the same width
                    var coldef2 = new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) };
                    var coldef3 = new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) };
                    navpanel.ColumnDefinitions.Add(coldef1);    //add each column definition to the grid
                    navpanel.ColumnDefinitions.Add(coldef2);
                    navpanel.ColumnDefinitions.Add(coldef3);
                    Label PageCount = new Label() { Content = pageindex.ToString() + "/" + Math.Ceiling(PageAmount).ToString() + " (" + TagNames.Count.ToString() + ")" };  //create page number label
                    Button Next = new Button() { Content = "Next" };        //create next button
                    Next.Click += new RoutedEventHandler(NextPage_Click);   //create click event for next button
                    Button Prev = new Button() { Content = "Previous" };    //create previous button
                    Prev.Click += new RoutedEventHandler(PrevPage_Click);   //create click event for previous button

                    Grid.SetColumn(Prev, 0);    //set columns for page number label, previous button, and next button
                    Grid.SetColumn(Next, 2);
                    Grid.SetColumn(PageCount, 1);

                    Grid.SetRow(navpanel, Counter); //set row and column of the navigation grid
                    Grid.SetColumn(navpanel, 0);
                    navpanel.Children.Add(Prev);        //add page number label, next button, and previous button to navigation grid
                    navpanel.Children.Add(PageCount);
                    navpanel.Children.Add(Next);

                    TagList.Children.Add(navpanel);     //add navigation grid to tag list grid
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error Loading Tags - " + ex.Message);
                Error.WriteToLog(ex);
                this.Close();
            }            
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            int tempindex = pageindex;  //record current page index

            try
            {
                pageindex++;    //increment page index
                if (pageindex > Math.Ceiling((double)TagNames.Count / 500)) //make sure page index didn't go over max number of pages
                {
                    pageindex = (int)Math.Ceiling((double)TagNames.Count / 500);    //if pag index went over max number of pages set page index back to max number of pages
                }
                else
                {
                    UpdateUI();
                }
            }
            catch(Exception ex) //If error don't increment page index
            {
                MessageBox.Show("Error moving to next Page. - " + ex.Message);
                Error.WriteToLog(ex);
                pageindex = tempindex;
            }           
        }

        private void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            int tempindex = pageindex;  //record current page index

            try
            {
                pageindex--;        //decrement page index
                if (pageindex < 1)  //make sure page index didnt go below first page;
                {
                    pageindex = 1;  //set page index to first page if lower than first page
                }
                else
                {
                    UpdateUI();
                }
            }
            catch(Exception ex) //if error don't decrement page index
            {
                MessageBox.Show("Error moving to previous page. - " + ex.Message);
                Error.WriteToLog(ex);
                pageindex = tempindex;
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
                            scroller.ScrollToVerticalOffset(point.Y - 3);       //scroll the scroll viewer to that element
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

