﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace Tagger.UI
{
    /// <summary>
    /// Interaction logic for SearchResults.xaml
    /// </summary>
    public partial class SearchResults : Window
    {
        private static string thumbnaillocation = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Tagger", "Thumbs");

        DatabaseUtil.DBTable Data;
        DatabaseUtil db = new DatabaseUtil();
        ErrorHandling Error = new ErrorHandling();
        XMLUtil.SaveSettings SaveSettings;        

        List<string> BooleanSearchResults = new List<string>();
        List<string> Tags = new List<string>();
        public List<string> SearchResultsField { get; set; }

        public string ReturnFile { get; set; }

        string ProfileName = "";
        double PageAmount = 0.0;
        int PageIndex = 1;
        int ResultsPerPage = 20;

        Point Up1 = new Point(0, 10);
        Point Up2 = new Point(8, 0);
        Point Up3 = new Point(16, 10);
        Point Up4 = new Point(12, 10);
        Point Up5 = new Point(8, 5);
        Point Up6 = new Point(4, 10);        

        Point Remove1 = new Point(0, 0);
        Point Remove2 = new Point(4, 0);
        Point Remove3 = new Point(8, 8);
        Point Remove4 = new Point(12, 0);
        Point Remove5 = new Point(16, 0);
        Point Remove6 = new Point(11, 10);
        Point Remove7 = new Point(16, 20);
        Point Remove8 = new Point(12, 20);
        Point Remove9 = new Point(8, 12);
        Point Remove10 = new Point(4, 20);
        Point Remove11 = new Point(0, 20);
        Point Remove12 = new Point(5, 10);        

        Point Down1 = new Point(0, 0);
        Point Down2 = new Point(4, 0);
        Point Down3 = new Point(8, 5);
        Point Down4 = new Point(12, 0);
        Point Down5 = new Point(16, 0);
        Point Down6 = new Point(8, 10);

        string[] keywords = new string[] { "!Rating:", "!Rating>:", "!Rating<:", "!Rating>=:", "!Rating<=:", "!Extension:", "!Height:", "!Height>:", "!Height<:", "!Width:", "!Width>:", "!Width<:", "!Name:", "!Image", "!Video", "!Size:", "!Size>:", "!Size<:", "!TagCount:", "!TagCount>:", "!TagCount<:" };
        

        public SearchResults(DatabaseUtil DB, DatabaseUtil.DBTable data, XMLUtil.SaveSettings ss, string profilename, List<string> SRS)
        {
            InitializeComponent();

            SetWindowScreen(this, GetWindowScreen(App.Current.MainWindow));
            SearchBar.AddHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent, new TextChangedEventHandler(ComboBox_TextChanged));

            //Handle input variables
            Data = data;
            db = DB;            
            ProfileName = profilename;
            SaveSettings = ss;
            BooleanSearchResults = SRS;

            //Update Search Results 
            if(BooleanSearchResults != null)
            {
                if (BooleanSearchResults.Count > 0)
                {
                    AddSearchResults(BooleanSearchResults);
                }
            }

            ReturnFile = "";
        }

        private void ComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (SearchBar.Text == "" || SearchBar.Text == " ")  //Clear dropdown if empty search
                {
                    SearchBar.IsDropDownOpen = false;
                    Tags.Clear();
                    SearchBar.Items.Clear();
                }
                else if (SearchBar.Text.StartsWith("!"))    //fill drop down with serach keywords
                {
                    SearchBar.IsDropDownOpen = true;
                    if(SearchBar.SelectedIndex == -1)
                    {
                        foreach(string keyboi in keywords)
                        {
                            SearchBar.Items.Add(keyboi);
                        }
                    }                    
                }
                else    //fill dropdown with similar tags
                {
                    SearchBar.IsDropDownOpen = true;
                    if (SearchBar.SelectedIndex == -1)
                    {
                        Tags.Clear();
                        SearchBar.Items.Clear();
                        Tags = db.GetTagsContaining(Data, SearchBar.Text, 8);
                        if(Tags != null)
                        {
                            foreach (string tag in Tags)
                            {
                                SearchBar.Items.Add(tag);
                            }
                        }
                        else
                        {
                            throw new Exception("Error retrieving tags that contain matching text with search parameter.");
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
            }            
        }

        private void Searchbar_DropDownOpened(object sender, EventArgs e)
        {
            try
            {
                TextBox textBox = (TextBox)((ComboBox)sender).Template.FindName("PART_EditableTextBox", (ComboBox)sender);
                textBox.SelectionStart = ((ComboBox)sender).Text.Length;
                textBox.SelectionLength = 0;
            }
            catch (Exception ex)
            {
                Error.WriteToLog(ex);
            }            
        }

        private void BooleanOperator_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button BooleanButton = (Button)sender;
                Grid SearchParameterGrid = (Grid)BooleanButton.Parent;

                int rownum = Convert.ToInt32(SearchParameterGrid.Tag);

                switch (BooleanButton.Content.ToString())
                {
                    case "And":
                        BooleanButton.Content = "Or";
                        break;
                    case "Or":
                        BooleanButton.Content = "And";
                        break;
                    default:
                        BooleanButton.Content = "And";                        
                        break;
                }
                HandleParens(HandleOrs());

                string search = CreateSearchString();
                ResultsCountLabel.Content = "Results Count: " + db.SearchCount(Data, search);
                AdvancedSearch.Text = search;
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
            }            
        }

        private void LikeNot_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button NotButton = (Button)sender;
                Grid SearchParameterGrid = (Grid)NotButton.Parent;

                int rownum = Convert.ToInt32(SearchParameterGrid.Tag);

                switch (NotButton.Content.ToString())
                {
                    case "":
                        NotButton.Content = "Not";
                        break;
                    case "Not":
                        NotButton.Content = "";
                        break;
                    default:
                        NotButton.Content = "";
                        break;
                }
                string search = CreateSearchString();
                ResultsCountLabel.Content = "Results Count: " + db.SearchCount(Data, search);
                AdvancedSearch.Text = search;
            }
            catch (Exception ex)
            {
                Error.WriteToLog(ex);
            }
        }

        private int[] HandleOrs()
        {
            try
            {
                Grid SearchParameterGrid;
                Button BooleanButton;
                int[] BooleanSearchOrArray = new int[SearchPanel.Children.Count];

                for (int SearchParameter = 1; SearchParameter <= SearchPanel.Children.Count - 1; SearchParameter++)
                {
                    if (SearchPanel.Children[SearchParameter] is Grid)
                    {
                        SearchParameterGrid = (Grid)SearchPanel.Children[SearchParameter];
                        BooleanButton = (Button)SearchParameterGrid.Children[1];
                        if (BooleanButton.Content.ToString() == "Or")
                        {
                            BooleanSearchOrArray[SearchParameter] = 1;
                        }
                        else if (BooleanButton.Content.ToString() == "And")
                        {
                            BooleanSearchOrArray[SearchParameter] = 2;
                        }
                        else
                        {
                            BooleanSearchOrArray[SearchParameter] = -1;
                        }
                    }
                }

                return BooleanSearchOrArray;
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                return new int[] { };
            }            
        }

        private void HandleParens(int[] BooleanSearchOrArray)
        {
            try
            {
                Grid PreviousSearchParameterGrid;
                Grid SearchParameterGrid;
                bool BoolisOr = false;

                if(BooleanSearchOrArray.Length < 3)
                {
                    SearchParameterGrid = (Grid)SearchPanel.Children[1];
                    SearchParameterGrid.Children[2].Visibility = Visibility.Hidden;
                    SearchParameterGrid.Children[5].Visibility = Visibility.Hidden;
                }
                else
                {
                    for (int OrArrayIndex = 2; OrArrayIndex <= BooleanSearchOrArray.Length - 1; OrArrayIndex++)
                    {

                        if (BooleanSearchOrArray[OrArrayIndex] == 1 && BoolisOr == false) //if current row is or and previous row is not
                        {
                            //make previous row open paren visible
                            PreviousSearchParameterGrid = (Grid)SearchPanel.Children[OrArrayIndex - 1];
                            SearchParameterGrid = (Grid)SearchPanel.Children[OrArrayIndex];
                            PreviousSearchParameterGrid.Children[2].Visibility = Visibility.Visible;
                            PreviousSearchParameterGrid.Children[5].Visibility = Visibility.Hidden;
                            SearchParameterGrid.Children[5].Visibility = Visibility.Visible;
                            BoolisOr = true;
                        }
                        else if (BooleanSearchOrArray[OrArrayIndex] != 1 && BoolisOr == false)    //if current row isn't or and previous row is not
                        {
                            //make previous row open paren hidden
                            PreviousSearchParameterGrid = (Grid)SearchPanel.Children[OrArrayIndex - 1];
                            SearchParameterGrid = (Grid)SearchPanel.Children[OrArrayIndex];
                            PreviousSearchParameterGrid.Children[2].Visibility = Visibility.Hidden;
                            PreviousSearchParameterGrid.Children[5].Visibility = Visibility.Hidden;
                            SearchParameterGrid.Children[5].Visibility = Visibility.Hidden;
                            BoolisOr = false;
                        }
                        else if (BooleanSearchOrArray[OrArrayIndex] == 1 && BoolisOr == true)     //if current row is or and previous row is
                        {
                            //Do Nothing
                            PreviousSearchParameterGrid = (Grid)SearchPanel.Children[OrArrayIndex - 1];
                            SearchParameterGrid = (Grid)SearchPanel.Children[OrArrayIndex];
                            PreviousSearchParameterGrid.Children[2].Visibility = Visibility.Hidden;
                            PreviousSearchParameterGrid.Children[5].Visibility = Visibility.Hidden;
                            SearchParameterGrid.Children[5].Visibility = Visibility.Visible;
                            BoolisOr = true;
                        }
                        else if (BooleanSearchOrArray[OrArrayIndex] != 1 && BoolisOr == true)     //if current row isn't or and previous row is
                        {
                            //make prev row close paren visible
                            PreviousSearchParameterGrid = (Grid)SearchPanel.Children[OrArrayIndex - 1];
                            SearchParameterGrid = (Grid)SearchPanel.Children[OrArrayIndex];
                            PreviousSearchParameterGrid.Children[2].Visibility = Visibility.Hidden;
                            PreviousSearchParameterGrid.Children[5].Visibility = Visibility.Visible;
                            SearchParameterGrid.Children[5].Visibility = Visibility.Hidden;
                            BoolisOr = false;
                        }
                    }
                }                
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
            }            
        }

        private void RemoveTag_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button RemoveButton = (Button)sender;
                Grid SearchParameterGrid = (Grid)RemoveButton.Parent;
                Polygon RemovePolygon;
                int rownum = Convert.ToInt32(SearchParameterGrid.Tag);

                //Remove tag row
                SearchPanel.Children.RemoveAt(rownum);

                //update all rownums after removed to to reflect current row
                if (SearchPanel.Children.Count >= rownum)
                {
                    foreach (UIElement SearchParameterIndex in SearchPanel.Children)
                    {
                        if (SearchParameterIndex is Grid)
                        {
                            SearchParameterGrid = (Grid)SearchParameterIndex;
                            if (Convert.ToInt32(SearchParameterGrid.Tag) > rownum)
                            {
                                SearchParameterGrid.Tag = Convert.ToUInt32(SearchParameterGrid.Tag) - 1;
                            }
                        }
                    }
                }

                if (rownum == 1) //If removing first row
                {
                    if (SearchPanel.Children.Count > 1)
                    {
                        SearchParameterGrid = (Grid)SearchPanel.Children[1];
                        SearchParameterGrid.Children[0].IsEnabled = false;
                        SearchParameterGrid.Children[1].Visibility = Visibility.Hidden;
                        RemoveButton = (Button)SearchParameterGrid.Children[0];
                        RemovePolygon = (Polygon)RemoveButton.Content;
                        RemovePolygon.Fill = Brushes.LightGray;
                    }
                }

                if (rownum == SearchPanel.Children.Count) //If removing last row
                {
                    if (SearchPanel.Children.Count > 1)
                    {
                        SearchParameterGrid = (Grid)SearchPanel.Children[SearchPanel.Children.Count - 1];
                        SearchParameterGrid.Children[7].IsEnabled = false;
                        RemoveButton = (Button)SearchParameterGrid.Children[7];
                        RemovePolygon = (Polygon)RemoveButton.Content;
                        RemovePolygon.Fill = Brushes.LightGray;
                    }
                }
                
                HandleParens(HandleOrs());
                string search = CreateSearchString();
                ResultsCountLabel.Content = "Results Count: " + db.SearchCount(Data, search);
                AdvancedSearch.Text = search;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error removing search parameter.");
                Error.WriteToLog(ex);
            }            
        }

        private void TagUp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button TagUpButton = (Button)sender;
                Grid SearchParameterGrid = (Grid)TagUpButton.Parent;
                Polygon TagUpPolygon;
                int rownum = Convert.ToInt32(SearchParameterGrid.Tag);

                //move tag
                SearchPanel.Children.RemoveAt(rownum);
                SearchPanel.Children.Insert(rownum - 1, SearchParameterGrid);

                //Update tag
                SearchParameterGrid = (Grid)SearchPanel.Children[rownum];
                SearchParameterGrid.Tag = Convert.ToInt32(SearchParameterGrid.Tag) + 1;

                SearchParameterGrid = (Grid)SearchPanel.Children[rownum - 1];
                SearchParameterGrid.Tag = Convert.ToInt32(SearchParameterGrid.Tag) - 1;

                //if tag to move is in last row
                if (rownum == SearchPanel.Children.Count - 1) //If removing last row
                {
                    if (SearchPanel.Children.Count > 1)
                    {
                        SearchParameterGrid = (Grid)SearchPanel.Children[SearchPanel.Children.Count - 2];
                        SearchParameterGrid.Children[7].IsEnabled = true;
                        TagUpButton = (Button)SearchParameterGrid.Children[7];
                        TagUpPolygon = (Polygon)TagUpButton.Content;
                        TagUpPolygon.Fill = Brushes.Black;

                        SearchParameterGrid = (Grid)SearchPanel.Children[SearchPanel.Children.Count - 1];
                        SearchParameterGrid.Children[7].IsEnabled = false;
                        TagUpButton = (Button)SearchParameterGrid.Children[7];
                        TagUpPolygon = (Polygon)TagUpButton.Content;
                        TagUpPolygon.Fill = Brushes.LightGray;
                    }
                }

                //if tag to move in in second row
                if (rownum == 2) //If removing last row
                {
                    if (SearchPanel.Children.Count > 1)
                    {
                        SearchParameterGrid = (Grid)SearchPanel.Children[1];
                        SearchParameterGrid.Children[0].IsEnabled = false;
                        SearchParameterGrid.Children[1].Visibility = Visibility.Hidden;
                        TagUpButton = (Button)SearchParameterGrid.Children[0];
                        TagUpPolygon = (Polygon)TagUpButton.Content;
                        TagUpPolygon.Fill = Brushes.LightGray;

                        SearchParameterGrid = (Grid)SearchPanel.Children[2];
                        SearchParameterGrid.Children[0].IsEnabled = true;
                        SearchParameterGrid.Children[1].Visibility = Visibility.Visible;
                        TagUpButton = (Button)SearchParameterGrid.Children[0];
                        TagUpPolygon = (Polygon)TagUpButton.Content;
                        TagUpPolygon.Fill = Brushes.Black;
                    }
                }
                
                HandleParens(HandleOrs());
                string search = CreateSearchString();
                ResultsCountLabel.Content = "Results Count: " + db.SearchCount(Data, search);
                AdvancedSearch.Text = search;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error moving row up.");
                Error.WriteToLog(ex);
            }            
        }

        private void TagDown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button TagDownButton = (Button)sender;
                Grid SearchParameterGrid = (Grid)TagDownButton.Parent;
                Polygon TagDownPolygon;
                int rownum = Convert.ToInt32(SearchParameterGrid.Tag);

                //move tag
                SearchPanel.Children.RemoveAt(rownum);
                SearchPanel.Children.Insert(rownum + 1, SearchParameterGrid);

                //Update tag
                SearchParameterGrid = (Grid)SearchPanel.Children[rownum];
                SearchParameterGrid.Tag = Convert.ToInt32(SearchParameterGrid.Tag) - 1;

                SearchParameterGrid = (Grid)SearchPanel.Children[rownum + 1];
                SearchParameterGrid.Tag = Convert.ToInt32(SearchParameterGrid.Tag) + 1;

                //if tag to move is in second last row
                if (rownum == SearchPanel.Children.Count - 2) //If removing last row
                {
                    if (SearchPanel.Children.Count > 1)
                    {
                        SearchParameterGrid = (Grid)SearchPanel.Children[SearchPanel.Children.Count - 2];
                        SearchParameterGrid.Children[7].IsEnabled = true;
                        TagDownButton = (Button)SearchParameterGrid.Children[7];
                        TagDownPolygon = (Polygon)TagDownButton.Content;
                        TagDownPolygon.Fill = Brushes.Black;

                        SearchParameterGrid = (Grid)SearchPanel.Children[SearchPanel.Children.Count - 1];
                        SearchParameterGrid.Children[7].IsEnabled = false;
                        TagDownButton = (Button)SearchParameterGrid.Children[7];
                        TagDownPolygon = (Polygon)TagDownButton.Content;
                        TagDownPolygon.Fill = Brushes.LightGray;
                    }
                }

                //if tag to move is in first row
                if (rownum == 1) //If removing last row
                {
                    if (SearchPanel.Children.Count > 1)
                    {
                        SearchParameterGrid = (Grid)SearchPanel.Children[1];
                        SearchParameterGrid.Children[0].IsEnabled = false;
                        SearchParameterGrid.Children[1].Visibility = Visibility.Hidden;
                        TagDownButton = (Button)SearchParameterGrid.Children[0];
                        TagDownPolygon = (Polygon)TagDownButton.Content;
                        TagDownPolygon.Fill = Brushes.LightGray;

                        SearchParameterGrid = (Grid)SearchPanel.Children[2];
                        SearchParameterGrid.Children[0].IsEnabled = true;
                        SearchParameterGrid.Children[1].Visibility = Visibility.Visible;
                        TagDownButton = (Button)SearchParameterGrid.Children[0];
                        TagDownPolygon = (Polygon)TagDownButton.Content;
                        TagDownPolygon.Fill = Brushes.Black;
                    }
                }
                
                HandleParens(HandleOrs());
                string search = CreateSearchString();
                ResultsCountLabel.Content = "Results Count: " + db.SearchCount(Data, search);
                AdvancedSearch.Text = search;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error moving row down.");
                Error.WriteToLog(ex);
            }
        }

        private Grid CreateTagRow(int rowNum)
        {
            Grid TagArea = new Grid { Tag = rowNum };
            try
            {
                #region Button Icon Creation     

                PointCollection UpCollection = new PointCollection
            {
                Up1,
                Up2,
                Up3,
                Up4,
                Up5,
                Up6
            };
                PointCollection RemoveCollection = new PointCollection
            {
                Remove1,
                Remove2,
                Remove3,
                Remove4,
                Remove5,
                Remove6,
                Remove7,
                Remove8,
                Remove9,
                Remove10,
                Remove11,
                Remove12
            };
                PointCollection DownCollection = new PointCollection
            {
                Down1,
                Down2,
                Down3,
                Down4,
                Down5,
                Down6
            };

                //Setup Grid Layout for Tag
                
                ColumnDefinition ColDef1 = new ColumnDefinition { Width = new GridLength(24, GridUnitType.Pixel) };
                ColumnDefinition ColDef2 = new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) };
                ColumnDefinition ColDef3 = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) };
                ColumnDefinition ColDefN = new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) };
                ColumnDefinition ColDef4 = new ColumnDefinition { Width = new GridLength(8, GridUnitType.Star) };
                ColumnDefinition ColDef5 = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) };
                ColumnDefinition ColDef6 = new ColumnDefinition { Width = new GridLength(24, GridUnitType.Pixel) };
                ColumnDefinition ColDef7 = new ColumnDefinition { Width = new GridLength(24, GridUnitType.Pixel) };
                TagArea.ColumnDefinitions.Add(ColDef1);
                TagArea.ColumnDefinitions.Add(ColDef2);
                TagArea.ColumnDefinitions.Add(ColDef3);
                TagArea.ColumnDefinitions.Add(ColDefN);
                TagArea.ColumnDefinitions.Add(ColDef4);
                TagArea.ColumnDefinitions.Add(ColDef5);
                TagArea.ColumnDefinitions.Add(ColDef6);
                TagArea.ColumnDefinitions.Add(ColDef7);
                #endregion

                //Create each element for Tag
                Button up = new Button { Content = new Polygon() { Points = UpCollection, Fill = (rowNum == 1 ? Brushes.LightGray : Brushes.Black) }, IsEnabled = (rowNum == 1 ? false : true), Margin = new Thickness(2) };
                Button BooleanOperator = new Button { Content = "And", Visibility = (rowNum == 1 ? Visibility.Hidden : Visibility.Visible), Height = 20, Margin = new Thickness(2) };
                Label OpenParen = new Label { Content = "(", Visibility = Visibility.Hidden };
                Button LikeNot = new Button { Content = "", Height = 20, Margin = new Thickness(2) };
                Label TagDisp = new Label { Content = SearchBar.Text, Margin = new Thickness(2), Tag = Content };
                Label CloseParen = new Label { Content = ")", Visibility = Visibility.Hidden };
                Button Remove = new Button { Content = new Polygon() { Points = RemoveCollection, Fill = Brushes.Red }, Margin = new Thickness(2) };
                Button down = new Button { Content = new Polygon() { Points = DownCollection, Fill = Brushes.LightGray }, IsEnabled = false, Margin = new Thickness(2) };

                //Create Button Click Events
                up.Click += new RoutedEventHandler(TagUp_Click);
                BooleanOperator.Click += new RoutedEventHandler(BooleanOperator_Click);
                LikeNot.Click += new RoutedEventHandler(LikeNot_Click);
                Remove.Click += new RoutedEventHandler(RemoveTag_Click);
                down.Click += new RoutedEventHandler(TagDown_Click);

                //set element rows and columns
                Grid.SetColumn(up, 0);
                Grid.SetColumn(BooleanOperator, 1);
                Grid.SetColumn(OpenParen, 2);
                Grid.SetColumn(LikeNot, 3);
                Grid.SetColumn(TagDisp, 4);
                Grid.SetColumn(CloseParen, 5);
                Grid.SetColumn(Remove, 6);
                Grid.SetColumn(down, 7);
                Grid.SetRow(up, 0);
                Grid.SetRow(BooleanOperator, 0);
                Grid.SetRow(OpenParen, 0);
                Grid.SetRow(LikeNot, 0);
                Grid.SetRow(TagDisp, 0);
                Grid.SetRow(CloseParen, 0);
                Grid.SetRow(Remove, 0);
                Grid.SetRow(down, 0);

                //add elements to grid
                TagArea.Children.Add(up);
                TagArea.Children.Add(BooleanOperator);
                TagArea.Children.Add(OpenParen);
                TagArea.Children.Add(LikeNot);
                TagArea.Children.Add(TagDisp);
                TagArea.Children.Add(CloseParen);
                TagArea.Children.Add(Remove);
                TagArea.Children.Add(down);
                
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error Adding search parameter");
                Error.WriteToLog(ex);
            }
            return TagArea;
        }

        private string CreateSearchString()
        {
            Grid SearchParameterGrid;
            Label SearchContentLabel;
            Button BooleanButton;
            Button NotButton;
            string inputTag;
            string like = "";

            string search = "select * from dbo.ImageData where ";

            for (int SearchParameterIndex = 1; SearchParameterIndex <= SearchPanel.Children.Count - 1; SearchParameterIndex++)
            {
                SearchParameterGrid = (Grid)SearchPanel.Children[SearchParameterIndex];
                BooleanButton = (Button)SearchParameterGrid.Children[1];
                for (int GridIndex = 1; GridIndex <= 5; GridIndex++)
                {
                    if (SearchParameterGrid.Children[GridIndex].Visibility == Visibility.Visible)
                    {
                        switch (GridIndex)
                        {
                            case 1: // Operator
                                if (BooleanButton.Content.ToString() == "Or")
                                {
                                    search += "Or ";
                                }
                                else
                                {
                                    search += "And ";
                                }
                                break;

                            case 2: // Open Paren
                                SearchContentLabel = (Label)SearchParameterGrid.Children[GridIndex];
                                search += SearchContentLabel.Content.ToString();
                                break;

                            case 3: // Not
                                NotButton = (Button)SearchParameterGrid.Children[GridIndex];
                                if (NotButton.Content.ToString() == "")
                                {
                                    like = " Like ";
                                }
                                else
                                {
                                    like = " Not Like ";
                                }
                                break;

                            case 4: // Tag                                
                                SearchContentLabel = (Label)SearchParameterGrid.Children[GridIndex];
                                inputTag = SearchContentLabel.Content.ToString();
                                if (inputTag.ToLower().StartsWith("!rating:"))  //Rating Equals
                                {
                                    search += "Rating = " + inputTag.ToLower().Replace(" ","").Replace("!rating:", "").Replace(@"'", "''") + " ";
                                }
                                else if (inputTag.ToLower().StartsWith("!rating>:"))    //Rating Greater than
                                {
                                    search += "Rating > " + inputTag.ToLower().Replace(" ", "").Replace("!rating>:", "").Replace(@"'", "''") + " ";
                                }
                                else if (inputTag.ToLower().StartsWith("!rating<:"))    //Rating less than
                                {
                                    search += "Rating < " + inputTag.ToLower().Replace(" ", "").Replace("!rating<:", "").Replace(@"'", "''") + " ";
                                }
                                else if (inputTag.ToLower().StartsWith("!rating>=:"))   //Rating greater than or equal to
                                {
                                    search += "Rating >= " + inputTag.ToLower().Replace(" ", "").Replace("!rating>=:", "").Replace(@"'", "''") + " ";
                                }
                                else if (inputTag.ToLower().StartsWith("!rating<=:"))   //Rating less than or equal to
                                {
                                    search += "Rating <= " + inputTag.ToLower().Replace(" ", "").Replace("!rating<=:", "").Replace(@"'", "''") + " ";
                                }
                                else if (inputTag.ToLower().StartsWith("!extension:"))   //Extension equals
                                {
                                    search += "Filetype" + like + "'%" + inputTag.ToLower().Replace(" ","").Replace("!extension:", "").Replace(@"'", "''") + "' ";
                                }
                                else if(inputTag.ToLower().StartsWith("!height:"))      //Image Height equals
                                {
                                    search += "Height = " + inputTag.ToLower().Replace(" ", "").Replace("!height:", "").Replace(@"'", "''") + " ";
                                }
                                else if (inputTag.ToLower().StartsWith("!height>:"))    //Image Height Greater than
                                {
                                    search += "Height > " + inputTag.ToLower().Replace(" ", "").Replace("!height>:", "").Replace(@"'", "''") + " ";
                                }
                                else if (inputTag.ToLower().StartsWith("!rating<:"))    //Image Height less than
                                {
                                    search += "Height < " + inputTag.ToLower().Replace(" ", "").Replace("!height<:", "").Replace(@"'", "''") + " ";
                                }
                                else if (inputTag.ToLower().StartsWith("!width:"))      //Image Width Equals
                                {
                                    search += "Width = " + inputTag.ToLower().Replace(" ", "").Replace("!width:", "").Replace(@"'", "''") + " ";
                                }
                                else if (inputTag.ToLower().StartsWith("!width>:"))    //Image Width Greater than
                                {
                                    search += "Width > " + inputTag.ToLower().Replace(" ", "").Replace("!width>:", "").Replace(@"'", "''") + " ";
                                }
                                else if (inputTag.ToLower().StartsWith("!width<:"))    //Image Width less than
                                {
                                    search += "Width < " + inputTag.ToLower().Replace(" ", "").Replace("!width<:", "").Replace(@"'", "''") + " ";
                                }
                                else if (inputTag.ToLower().StartsWith("!name:"))      //Image Width Equals
                                {
                                    search += "FileName" + like + "'%" + inputTag.ToLower().Replace(" ", "").Replace("!width:", "").Replace(@"'", "''") + "%' ";
                                }
                                else if (inputTag.ToLower().StartsWith("!video"))    //Image Width Greater than
                                {
                                    search += "(Filetype = '.mp4' or Filetype = '.mpg' or Filetype= '.mkv' or Filetype = '.wmv' or Filetype = '.avi' or Filetype = '.flv' or Filetype = '.webm') ";
                                }
                                else if (inputTag.ToLower().StartsWith("!image"))    //Image Width less than
                                {
                                    search += "(Filetype = '.jpg' or Filetype = '.jpeg' or Filetype= '.png' or Filetype = '.bmp' or Filetype = '.gif') ";
                                }
                                else if (inputTag.ToLower().StartsWith("!size:"))      //Image Width Equals
                                {
                                    search += "Filesize = " + inputTag.ToLower().Replace(" ", "").Replace("!size:", "").Replace(@"'", "''") + " ";
                                }
                                else if (inputTag.ToLower().StartsWith("!size>:"))    //Image Width Greater than
                                {
                                    search += "Filesize > " + inputTag.ToLower().Replace(" ", "").Replace("!size>:", "").Replace(@"'", "''") + " ";
                                }
                                else if (inputTag.ToLower().StartsWith("!size<:"))    //Image Width less than
                                {
                                    search += "Filesize < " + inputTag.ToLower().Replace(" ", "").Replace("!size<:", "").Replace(@"'", "''") + " ";
                                }
                                else if (inputTag.ToLower().StartsWith("!tagcount:"))      //Image Width Equals
                                {
                                    search += "ImageTagCount = " + inputTag.ToLower().Replace(" ", "").Replace("!tagcount:", "").Replace(@"'", "''") + " ";
                                }
                                else if (inputTag.ToLower().StartsWith("!tagcount>:"))    //Image Width Greater than
                                {
                                    search += "ImageTagCount > " + inputTag.ToLower().Replace(" ", "").Replace("!tagcount>:", "").Replace(@"'", "''") + " ";
                                }
                                else if (inputTag.ToLower().StartsWith("!tagcount<:"))    //Image Width less than
                                {
                                    search += "ImageTagCount < " + inputTag.ToLower().Replace(" ", "").Replace("!tagcount<:", "").Replace(@"'", "''") + " ";
                                }
                                else    //Default Tag search
                                {
                                    search += "Tags" + like + "'%;" + inputTag.Replace(@"'", "''") + ";%' ";
                                }
                                break;

                            case 5: // Close Paren
                                SearchContentLabel = (Label)SearchParameterGrid.Children[GridIndex];
                                search += SearchContentLabel.Content.ToString() + " ";
                                break;

                            default:
                                break;
                        }
                    }
                }
            }
            return search;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string search = "";            

            try
            {
                BooleanSearchResults.Clear();

                search = CreateSearchString();
                AdvancedSearch.Text = search;
                BooleanSearchResults = db.MainSearch(Data, search);
                AddSearchResults(BooleanSearchResults);
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
            }          
        }

        private void SubmitSearchParameter_Click(object sender, RoutedEventArgs e)
        {
            AddSearchParameter();
        }

        private void AddSearchParameter()
        {
            try
            {
                if (SearchBar.Text != "" && SearchBar.Text != " ")
                {
                    SearchPanel.Children.Add(CreateTagRow(SearchPanel.Children.Count));
                    SearchBar.Text = "";

                    //enable down button on prev row when adding row
                    if (SearchPanel.Children.Count > 2)
                    {
                        Grid SearchParameterGrid;
                        Polygon TagDownPolygon;
                        Button TagDownButton;
                        foreach (UIElement SearchParameterIndex in SearchPanel.Children)
                        {
                            if (SearchParameterIndex is Grid)
                            {
                                SearchParameterGrid = (Grid)SearchParameterIndex;
                                if (Convert.ToInt32(SearchParameterGrid.Tag) == SearchPanel.Children.Count - 2)
                                {
                                    SearchParameterGrid.Children[7].IsEnabled = true;
                                    TagDownButton = (Button)SearchParameterGrid.Children[7];
                                    TagDownPolygon = (Polygon)TagDownButton.Content;
                                    TagDownPolygon.Fill = Brushes.Black;
                                }
                            }
                        }
                    }
                    string search = CreateSearchString();
                    ResultsCountLabel.Content = "Results Count: " + db.SearchCount(Data, search);
                    AdvancedSearch.Text = search;
                }
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
            }            
        }

        private void AddSearchResults(List<string> ResultsList)
        {
            ResultsPanel.Children.Clear();
            GC.Collect();
            string filepath = "";
            try
            {
                PageAmount = (double)ResultsList.Count / ResultsPerPage;
                if (ResultsList.Count > 0)
                {
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
                        Content = PageIndex.ToString() + "/" + Math.Ceiling(PageAmount).ToString() + " (" + ResultsList.Count.ToString() + ")",
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
                    ResultsPanel.Children.Add(NavGridTop);

                    for (int ResultIndex = ((PageIndex - 1) * ResultsPerPage); (ResultIndex <= (ResultsPerPage * PageIndex) - 1) && (ResultIndex <= ResultsList.Count - 1); ResultIndex++)
                    {
                        filepath = System.IO.Path.Combine(db.GetImageDirectory(Data, ResultsList[ResultIndex]), ResultsList[ResultIndex]);
                        if (File.Exists(filepath))
                        {
                            Grid ResultGrid = new Grid() { Margin = new Thickness(2) };
                            Grid ResultDataGrid = new Grid() { Margin = new Thickness(2) };

                            RowDefinition rd1 = new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) };  //filename
                            RowDefinition rd2 = new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) };  //rating      
                            RowDefinition rd3 = new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) };  //Dimensions
                            ResultDataGrid.RowDefinitions.Add(rd1);
                            ResultDataGrid.RowDefinitions.Add(rd2);
                            ResultDataGrid.RowDefinitions.Add(rd3);

                            ColumnDefinition cd1 = new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) }; //Thumbnail
                            ColumnDefinition cd2 = new ColumnDefinition() { Width = new GridLength(6, GridUnitType.Star) }; //Data
                            ColumnDefinition cd3 = new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }; //RemoveButton
                            ResultGrid.ColumnDefinitions.Add(cd1);
                            ResultGrid.ColumnDefinitions.Add(cd2);
                            ResultGrid.ColumnDefinitions.Add(cd3);

                            Label filename = new Label() { Content = ResultsList[ResultIndex] };
                            Grid.SetRow(filename, 0);
                            ResultDataGrid.Children.Add(filename);

                            Label rating = new Label() { Content = "Rating: " + db.GetRating(Data, ResultsList[ResultIndex]).ToString() };
                            Grid.SetRow(rating, 1);
                            ResultDataGrid.Children.Add(rating);

                            Label Dimensions = new Label() { Content = db.GetImageHeight(Data, ResultsList[ResultIndex]).ToString() + " x " + db.GetImageWidth(Data, ResultsList[ResultIndex]).ToString() };
                            Grid.SetRow(Dimensions, 2);
                            ResultDataGrid.Children.Add(Dimensions);

                            Button FileGet = new Button() { Tag = ResultsList[ResultIndex], Background = Brushes.Black };
                            FileGet.Click += new RoutedEventHandler(FileSelect);

                            if (File.Exists(System.IO.Path.Combine(thumbnaillocation, ResultsList[ResultIndex])))
                            {
                                Image thumb = new Image() { Width = 100, Height = 80 };
                                thumb.Source = BitmapImageFromFile(System.IO.Path.Combine(thumbnaillocation, ResultsList[ResultIndex]), thumb);                                
                                FileGet.Content = thumb;
                            }
                            else
                            {
                                Label AbsentThumb = new Label() { Content = "No Thumbnail", Foreground = Brushes.White };
                                FileGet.Content = AbsentThumb;
                            }
                            Grid.SetColumn(FileGet, 0);
                            ResultGrid.Children.Add(FileGet);

                            Grid.SetColumn(ResultDataGrid, 1);
                            ResultGrid.Children.Add(ResultDataGrid);

                            PointCollection RemoveCollection = new PointCollection
                            {
                                Remove1,
                                Remove2,
                                Remove3,
                                Remove4,
                                Remove5,
                                Remove6,
                                Remove7,
                                Remove8,
                                Remove9,
                                Remove10,
                                Remove11,
                                Remove12
                            };
                            Button RemoveResult = new Button() { Tag = ResultsList[ResultIndex], Content = new Polygon() { Points = RemoveCollection, Fill = Brushes.Red }, Width = 30, Height = 30, VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Right };
                            RemoveResult.Click += new RoutedEventHandler(RemoveResult_Click);

                            Grid.SetColumn(RemoveResult, 2);
                            ResultGrid.Children.Add(RemoveResult);

                            ResultsPanel.Children.Add(ResultGrid);
                        }
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
                        Content = PageIndex.ToString() + "/" + Math.Ceiling(PageAmount).ToString() + " (" + ResultsList.Count.ToString() + ")",
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

                    ResultsPanel.Children.Add(NavGridBottom);
                }
                else
                {
                    //No Results Found
                    ResultsPanel.Children.Add(new Label { Content = "No Results Found" });
                }
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
            }            
        }

        private void FileSelect(object sender, RoutedEventArgs e)
        {
            try
            {
                Button ThumbnailButton = (Button)sender;
                ReturnFile = ThumbnailButton.Tag.ToString();
                this.Close();
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
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
                    AddSearchResults(BooleanSearchResults);
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
                    AddSearchResults(BooleanSearchResults);
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
                if(PageIndex != 1)
                {
                    PageIndex = 1;
                    AddSearchResults(BooleanSearchResults);
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
                if(PageIndex != (int)Math.Ceiling(PageAmount))
                {
                    PageIndex = (int)Math.Ceiling(PageAmount);
                    AddSearchResults(BooleanSearchResults);
                    Scroller.ScrollToTop();
                }                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error moving to first page. - " + ex.Message);
                Error.WriteToLog(ex);
            }
        }

        private void RemoveResult_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button n = (Button)sender;
                BooleanSearchResults.Remove(n.Tag.ToString());
                AddSearchResults(BooleanSearchResults);
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
            }            
        }

        private void ClearResults_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BooleanSearchResults.Clear();
                PageIndex = 1;
                Tags.Clear();
                ReturnFile = "";
                ResultsPanel.Children.Clear();
                SearchPanel.Children.RemoveRange(1, SearchPanel.Children.Count - 1);
                SearchBar.Text = "";
                AdvancedSearch.Text = "";
                ResultsCountLabel.Content = "Results Count: 0";
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
            }            
        }

        private static BitmapImage BitmapImageFromFile(string path, Image img)
        {
            BitmapImage bi = new BitmapImage() { DecodePixelHeight = 80 };

            try
            {
                using (var fs = new FileStream(path, FileMode.Open))
                {
                    bi.BeginInit();
                    bi.StreamSource = fs;
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.EndInit();
                }

                bi.Freeze();

                return bi;
            }
            catch (Exception)
            {
                //Error.WriteToLog(ex);
                return bi;                
            }           
        }

        private void Slideshow_Click(object sender, RoutedEventArgs e)
        {
            List<string> filepaths = new List<string>();
            try
            {                
                SlideShow SS = new SlideShow(BooleanSearchResults.ToArray(), ProfileName, Data, SaveSettings);
                SS.ShowDialog();
                if(SS.ReturnFile != "")
                {
                    ReturnFile = SS.ReturnFile;
                    this.Close();
                }
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
            }            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SearchResultsField = BooleanSearchResults;
        }

        private void AdvancedSearchGo_Click(object sender, RoutedEventArgs e)
        {
            if (AdvancedSearch.Text != "")
            {
                BooleanSearchResults.Clear();
                BooleanSearchResults = db.MainSearch(Data, AdvancedSearch.Text);
                AddSearchResults(BooleanSearchResults);
            }
        }

        private void SearchBar_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Return)
                {
                    AddSearchParameter();
                }
            }
            catch (Exception ex)
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
