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
using WpfAnimatedGif;

namespace Tagger.UI
{
    /// <summary>
    /// Interaction logic for SearchResults.xaml
    /// </summary>
    public partial class SearchResults : Window
    {
        DatabaseUtil.DBTable Data;
        DatabaseUtil db = new DatabaseUtil();
        ErrorHandling Error = new ErrorHandling();
        XMLUtil.SaveSettings SaveSettings;
        string ProfileName = "";

        List<string> BooleanSearchResults = new List<string>();
        List<string> Tags = new List<string>();
        public string ReturnFile { get; set; }
        public List<string> SearchResultsField { get; set; }
        int pageindex = 1;
        int resultsperpage = 20;
        BitmapImage ImageInstance;

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
        

        public SearchResults(DatabaseUtil DB, DatabaseUtil.DBTable data, XMLUtil.SaveSettings ss, string profilename, List<string> SRS)
        {
            InitializeComponent();

            SetWindowScreen(this, GetWindowScreen(App.Current.MainWindow));
            SearchBar.AddHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent, new TextChangedEventHandler(ComboBox_TextChanged));

            Data = data;
            db = DB;
            ReturnFile = "";
            ProfileName = profilename;
            SaveSettings = ss;
            BooleanSearchResults = SRS;
            if(BooleanSearchResults != null)
            {
                if (BooleanSearchResults.Count > 0)
                {
                    AddSearchResults(BooleanSearchResults);
                }
            }                     
        }

        private void ComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (SearchBar.Text == "" || SearchBar.Text == " ")
                {
                    SearchBar.IsDropDownOpen = false;
                    Tags.Clear();
                    SearchBar.Items.Clear();
                }
                else
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
                Button b = (Button)sender;
                Grid u;
                u = (Grid)b.Parent;

                int rownum = Convert.ToInt32(u.Tag);

                switch (b.Content.ToString())
                {
                    case "And":
                        b.Content = "Or";
                        int[] y = HandleOrs();
                        HandleParens(y);
                        break;
                    case "Or":
                        b.Content = "Not";
                        int[] x = HandleOrs();
                        HandleParens(x);
                        break;
                    case "Not":
                        b.Content = "And";
                        int[] z = HandleOrs();
                        HandleParens(z);
                        break;
                    default:
                        b.Content = "And";
                        int[] zz = HandleOrs();
                        HandleParens(zz);
                        break;
                }
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
            }            
        }

        private int[] HandleOrs()
        {
            try
            {
                Grid l;
                Button k;
                int[] y = new int[SearchPanel.Children.Count];

                for (int h = 1; h <= SearchPanel.Children.Count - 1; h++)
                {
                    if (SearchPanel.Children[h] is Grid)
                    {
                        l = (Grid)SearchPanel.Children[h];
                        k = (Button)l.Children[1];
                        if (k.Content.ToString() == "Or")
                        {
                            y[h] = 1;
                        }
                        else if (k.Content.ToString() == "And")
                        {
                            y[h] = 2;
                        }
                        else if (k.Content.ToString() == "Not")
                        {
                            y[h] = 3;
                        }
                        else
                        {
                            y[h] = -1;
                        }
                    }
                }

                return y;
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
                return new int[] { };
            }
            
        }

        private void HandleParens(int[] y)
        {
            try
            {
                Grid j;
                Grid k;
                bool boolisor = false;

                for (int m = 2; m <= y.Length - 1; m++)
                {

                    if (y[m] == 1 && boolisor == false) //if current row is or and previous row is not
                    {
                        //make previous row open paren visible
                        j = (Grid)SearchPanel.Children[m - 1];
                        k = (Grid)SearchPanel.Children[m];
                        j.Children[2].Visibility = Visibility.Visible;
                        j.Children[4].Visibility = Visibility.Hidden;
                        k.Children[4].Visibility = Visibility.Visible;
                        boolisor = true;
                    }
                    else if (y[m] != 1 && boolisor == false)    //if current row isn't or and previous row is not
                    {
                        //make previous row open paren hidden
                        j = (Grid)SearchPanel.Children[m - 1];
                        k = (Grid)SearchPanel.Children[m];
                        j.Children[2].Visibility = Visibility.Hidden;
                        j.Children[4].Visibility = Visibility.Hidden;
                        k.Children[4].Visibility = Visibility.Hidden;
                        boolisor = false;
                    }
                    else if (y[m] == 1 && boolisor == true)     //if current row is or and previous row is
                    {
                        //Do Nothing
                        j = (Grid)SearchPanel.Children[m - 1];
                        k = (Grid)SearchPanel.Children[m];
                        j.Children[2].Visibility = Visibility.Hidden;
                        j.Children[4].Visibility = Visibility.Hidden;
                        k.Children[4].Visibility = Visibility.Visible;
                        boolisor = true;
                    }
                    else if (y[m] != 1 && boolisor == true)     //if current row isn't or and previous row is
                    {
                        //make prev row close paren visible
                        j = (Grid)SearchPanel.Children[m - 1];
                        k = (Grid)SearchPanel.Children[m];
                        j.Children[2].Visibility = Visibility.Hidden;
                        j.Children[4].Visibility = Visibility.Visible;
                        k.Children[4].Visibility = Visibility.Hidden;
                        boolisor = false;
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
                Button r = (Button)sender;
                Grid u;
                u = (Grid)r.Parent;
                Polygon t;
                int rownum = Convert.ToInt32(u.Tag);

                //Remove tag row
                SearchPanel.Children.RemoveAt(rownum);

                //update all rownums after removed to to reflect current row
                if (SearchPanel.Children.Count >= rownum)
                {
                    foreach (UIElement h in SearchPanel.Children)
                    {
                        if (h is Grid)
                        {
                            u = (Grid)h;
                            if (Convert.ToInt32(u.Tag) > rownum)
                            {
                                u.Tag = Convert.ToUInt32(u.Tag) - 1;
                            }
                        }
                    }
                }

                if (rownum == 1) //If removing first row
                {
                    if (SearchPanel.Children.Count > 1)
                    {
                        u = (Grid)SearchPanel.Children[1];
                        u.Children[0].IsEnabled = false;
                        u.Children[1].Visibility = Visibility.Hidden;
                        r = (Button)u.Children[0];
                        t = (Polygon)r.Content;
                        t.Fill = Brushes.LightGray;
                    }
                }

                if (rownum == SearchPanel.Children.Count) //If removing last row
                {
                    if (SearchPanel.Children.Count > 1)
                    {
                        u = (Grid)SearchPanel.Children[SearchPanel.Children.Count - 1];
                        u.Children[6].IsEnabled = false;
                        r = (Button)u.Children[6];
                        t = (Polygon)r.Content;
                        t.Fill = Brushes.LightGray;
                    }
                }

                int[] y = HandleOrs();
                HandleParens(y);
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
                Button r = (Button)sender;
                Grid u;
                u = (Grid)r.Parent;
                Polygon t;
                int rownum = Convert.ToInt32(u.Tag);

                //move tag
                SearchPanel.Children.RemoveAt(rownum);
                SearchPanel.Children.Insert(rownum - 1, u);

                //Update tag
                u = (Grid)SearchPanel.Children[rownum];
                u.Tag = Convert.ToInt32(u.Tag) + 1;

                u = (Grid)SearchPanel.Children[rownum - 1];
                u.Tag = Convert.ToInt32(u.Tag) - 1;

                //if tag to move is in last row
                if (rownum == SearchPanel.Children.Count - 1) //If removing last row
                {
                    if (SearchPanel.Children.Count > 1)
                    {
                        u = (Grid)SearchPanel.Children[SearchPanel.Children.Count - 2];
                        u.Children[6].IsEnabled = true;
                        r = (Button)u.Children[6];
                        t = (Polygon)r.Content;
                        t.Fill = Brushes.Black;

                        u = (Grid)SearchPanel.Children[SearchPanel.Children.Count - 1];
                        u.Children[6].IsEnabled = false;
                        r = (Button)u.Children[6];
                        t = (Polygon)r.Content;
                        t.Fill = Brushes.LightGray;
                    }
                }

                //if tag to move in in second row
                if (rownum == 2) //If removing last row
                {
                    if (SearchPanel.Children.Count > 1)
                    {
                        u = (Grid)SearchPanel.Children[1];
                        u.Children[0].IsEnabled = false;
                        u.Children[1].Visibility = Visibility.Hidden;
                        r = (Button)u.Children[0];
                        t = (Polygon)r.Content;
                        t.Fill = Brushes.LightGray;

                        u = (Grid)SearchPanel.Children[2];
                        u.Children[0].IsEnabled = true;
                        u.Children[1].Visibility = Visibility.Visible;
                        r = (Button)u.Children[0];
                        t = (Polygon)r.Content;
                        t.Fill = Brushes.Black;
                    }
                }

                int[] y = HandleOrs();
                HandleParens(y);
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
                Button r = (Button)sender;
                Grid u;
                u = (Grid)r.Parent;
                Polygon t;
                int rownum = Convert.ToInt32(u.Tag);

                //move tag
                SearchPanel.Children.RemoveAt(rownum);
                SearchPanel.Children.Insert(rownum + 1, u);

                //Update tag
                u = (Grid)SearchPanel.Children[rownum];
                u.Tag = Convert.ToInt32(u.Tag) - 1;

                u = (Grid)SearchPanel.Children[rownum + 1];
                u.Tag = Convert.ToInt32(u.Tag) + 1;

                //if tag to move is in second last row
                if (rownum == SearchPanel.Children.Count - 2) //If removing last row
                {
                    if (SearchPanel.Children.Count > 1)
                    {
                        u = (Grid)SearchPanel.Children[SearchPanel.Children.Count - 2];
                        u.Children[6].IsEnabled = true;
                        r = (Button)u.Children[6];
                        t = (Polygon)r.Content;
                        t.Fill = Brushes.Black;

                        u = (Grid)SearchPanel.Children[SearchPanel.Children.Count - 1];
                        u.Children[6].IsEnabled = false;
                        r = (Button)u.Children[6];
                        t = (Polygon)r.Content;
                        t.Fill = Brushes.LightGray;
                    }
                }

                //if tag to move is in first row
                if (rownum == 1) //If removing last row
                {
                    if (SearchPanel.Children.Count > 1)
                    {
                        u = (Grid)SearchPanel.Children[1];
                        u.Children[0].IsEnabled = false;
                        u.Children[1].Visibility = Visibility.Hidden;
                        r = (Button)u.Children[0];
                        t = (Polygon)r.Content;
                        t.Fill = Brushes.LightGray;

                        u = (Grid)SearchPanel.Children[2];
                        u.Children[0].IsEnabled = true;
                        u.Children[1].Visibility = Visibility.Visible;
                        r = (Button)u.Children[0];
                        t = (Polygon)r.Content;
                        t.Fill = Brushes.Black;
                    }
                }

                int[] y = HandleOrs();
                HandleParens(y);
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
                ColumnDefinition ColDef4 = new ColumnDefinition { Width = new GridLength(8, GridUnitType.Star) };
                ColumnDefinition ColDef5 = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) };
                ColumnDefinition ColDef6 = new ColumnDefinition { Width = new GridLength(24, GridUnitType.Pixel) };
                ColumnDefinition ColDef7 = new ColumnDefinition { Width = new GridLength(24, GridUnitType.Pixel) };
                TagArea.ColumnDefinitions.Add(ColDef1);
                TagArea.ColumnDefinitions.Add(ColDef2);
                TagArea.ColumnDefinitions.Add(ColDef3);
                TagArea.ColumnDefinitions.Add(ColDef4);
                TagArea.ColumnDefinitions.Add(ColDef5);
                TagArea.ColumnDefinitions.Add(ColDef6);
                TagArea.ColumnDefinitions.Add(ColDef7);
                #endregion

                //Create each element for Tag
                Button up = new Button { Content = new Polygon() { Points = UpCollection, Fill = (rowNum == 1 ? Brushes.LightGray : Brushes.Black) }, IsEnabled = (rowNum == 1 ? false : true), Margin = new Thickness(2) };
                Button BooleanOperator = new Button { Content = "And", Visibility = (rowNum == 1 ? Visibility.Hidden : Visibility.Visible), Margin = new Thickness(2), Tag = Content };
                Label OpenParen = new Label { Content = "(", Visibility = Visibility.Hidden, Margin = new Thickness(2), Tag = Content };
                Label TagDisp = new Label { Content = SearchBar.Text, Margin = new Thickness(2), Tag = Content };
                Label CloseParen = new Label { Content = ")", Visibility = Visibility.Hidden, Margin = new Thickness(2), Tag = Content };
                Button Remove = new Button { Content = new Polygon() { Points = RemoveCollection, Fill = Brushes.Red }, Margin = new Thickness(2) };
                Button down = new Button { Content = new Polygon() { Points = DownCollection, Fill = Brushes.LightGray }, IsEnabled = false, Margin = new Thickness(2) };

                //Create Button Click Events
                up.Click += new RoutedEventHandler(TagUp_Click);
                BooleanOperator.Click += new RoutedEventHandler(BooleanOperator_Click);
                Remove.Click += new RoutedEventHandler(RemoveTag_Click);
                down.Click += new RoutedEventHandler(TagDown_Click);

                //set element rows and columns
                Grid.SetColumn(up, 0);
                Grid.SetColumn(BooleanOperator, 1);
                Grid.SetColumn(OpenParen, 2);
                Grid.SetColumn(TagDisp, 3);
                Grid.SetColumn(CloseParen, 4);
                Grid.SetColumn(Remove, 5);
                Grid.SetColumn(down, 6);
                Grid.SetRow(up, 0);
                Grid.SetRow(BooleanOperator, 0);
                Grid.SetRow(OpenParen, 0);
                Grid.SetRow(TagDisp, 0);
                Grid.SetRow(CloseParen, 0);
                Grid.SetRow(Remove, 0);
                Grid.SetRow(down, 0);

                //add elements to grid
                TagArea.Children.Add(up);
                TagArea.Children.Add(BooleanOperator);
                TagArea.Children.Add(OpenParen);
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

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            int[] bools = HandleOrs();
            List<string> ands = new List<string>();
            List<string> nots = new List<string>();
            List<string[]> oars = new List<string[]>();
            string ors = "";
            Grid p;
            Label h;
            Button b;

            string search = "select * from dbo.ImageData where ";

            try
            {
                BooleanSearchResults.Clear();
                for(int i = 1; i <= SearchPanel.Children.Count - 1; i++)
                {
                    p = (Grid)SearchPanel.Children[i];
                    b = (Button)p.Children[1];
                    for (int j = 1; j <= 4; j++)
                    {                        
                        if(p.Children[j].Visibility == Visibility.Visible)
                        {
                            switch (j)
                            {
                                case 1: // Operator
                                    if (b.Content.ToString() == "Or")
                                    {
                                        search += "Or ";
                                    }
                                    else
                                    {
                                        search += "And ";
                                    }                                    
                                    break;
                                case 2: // Open Paren
                                    h = (Label)p.Children[j];
                                    search += h.Content.ToString();
                                    break;
                                case 3: // Tag
                                    h = (Label)p.Children[j];
                                    if (h.Content.ToString().StartsWith("Rating:"))
                                    {
                                        search += "Rating=" + h.Content.ToString().Replace("Rating: ", "").Replace(@"'", "''") + " ";
                                    }
                                    else if (h.Content.ToString().StartsWith("Extension:"))
                                    {
                                        search += "Filetype Like '%" + h.Content.ToString().Replace("Extension: ", "").Replace(@"'", "''") + "' ";
                                    }
                                    else if(b != null & b.Content.ToString() == "Not")
                                    {
                                        search += "Tags Not Like '%;" + h.Content.ToString().Replace(@"'", "''") + ";%' ";
                                    }
                                    else
                                    {
                                        search += "Tags Like '%;" + h.Content.ToString().Replace(@"'", "''") + ";%' ";
                                    }                                    
                                    break;
                                case 4: // Close Paren
                                    h = (Label)p.Children[j];
                                    search += h.Content.ToString() + " ";
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                AdvancedSearch.Text = search;
                BooleanSearchResults = db.MainSearch(Data, search);
                AddSearchResults(BooleanSearchResults);
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
            }

            //try
            //{
            //    BooleanSearchResults.Clear();

            //    string[] seperateors;

            //    if (bools.Length == 2)
            //    {
            //        p = (Grid)SearchPanel.Children[1];
            //        h = (Label)p.Children[3];
            //        BooleanSearchResults = db.SearchForFiles(Data, h.Content.ToString());
            //        AddSearchResults(BooleanSearchResults);
            //        return;
            //    }
            //    else if (bools.Length == 1)
            //    {
            //        BooleanSearchResults = db.SearchForFiles(Data, SearchBar.Text);
            //        AddSearchResults(BooleanSearchResults);
            //        return;
            //    }

            //    for (int n = 2; n <= bools.Length - 1; n++)
            //    {
            //        if (bools[n] == 1 && bools[n - 1] == 1)
            //        {
            //            p = (Grid)SearchPanel.Children[n - 1];
            //            h = (Label)p.Children[3];
            //            ors += h.Content.ToString() + ";";
            //        }
            //        else if (bools[n] == 1 && bools[n - 1] == 2)
            //        {
            //            p = (Grid)SearchPanel.Children[n - 1];
            //            h = (Label)p.Children[3];
            //            ors += "|" + h.Content.ToString() + ";";
            //        }
            //        else if (bools[n] == 1 && bools[n - 1] == 3)
            //        {
            //            p = (Grid)SearchPanel.Children[n - 1];
            //            h = (Label)p.Children[3];
            //            ors += "|" + h.Content.ToString() + ";";
            //        }
            //        else if (bools[n] == 2 && bools[n - 1] == 1)
            //        {
            //            p = (Grid)SearchPanel.Children[n - 1];
            //            h = (Label)p.Children[3];
            //            ors += h.Content.ToString() + ";";
            //        }
            //        else if (bools[n] == 2 && bools[n - 1] == 2)
            //        {
            //            p = (Grid)SearchPanel.Children[n - 1];
            //            h = (Label)p.Children[3];
            //            ands.Add(h.Content.ToString());
            //        }
            //        else if (bools[n] == 2 && bools[n - 1] == 3)
            //        {
            //            p = (Grid)SearchPanel.Children[n - 1];
            //            h = (Label)p.Children[3];
            //            nots.Add(h.Content.ToString());
            //        }
            //        else if (bools[n] == 3 && bools[n - 1] == 1)
            //        {
            //            p = (Grid)SearchPanel.Children[n - 1];
            //            h = (Label)p.Children[3];
            //            ors += h.Content.ToString() + ";";
            //        }
            //        else if (bools[n] == 3 && bools[n - 1] == 2)
            //        {
            //            p = (Grid)SearchPanel.Children[n - 1];
            //            h = (Label)p.Children[3];
            //            ands.Add(h.Content.ToString());
            //        }
            //        else if (bools[n] == 3 && bools[n - 1] == 3)
            //        {
            //            p = (Grid)SearchPanel.Children[n - 1];
            //            h = (Label)p.Children[3];
            //            nots.Add(h.Content.ToString());
            //        }

            //    }
            //    if (bools[bools.Length - 1] == 3)
            //    {
            //        p = (Grid)SearchPanel.Children[bools.Length - 1];
            //        h = (Label)p.Children[3];
            //        nots.Add(h.Content.ToString());
            //    }
            //    else if (bools[bools.Length - 1] == 2)
            //    {
            //        p = (Grid)SearchPanel.Children[bools.Length - 1];
            //        h = (Label)p.Children[3];
            //        ands.Add(h.Content.ToString());
            //    }
            //    else if (bools[bools.Length - 1] == 1)
            //    {
            //        p = (Grid)SearchPanel.Children[bools.Length - 1];
            //        h = (Label)p.Children[3];
            //        ors += h.Content.ToString() + ";";
            //    }
            //    //else if (bools[bools.Length - 1] == 1 && bools[bools.Length - 2] != 1)
            //    //{
            //    //    p = (Grid)SearchPanel.Children[bools.Length - 1];
            //    //    h = (Label)p.Children[3];
            //    //    ors += "|" + h.Content.ToString() + ";";
            //    //}
            //    seperateors = ors.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            //    foreach (string bitch in seperateors)
            //    {
            //        oars.Add(bitch.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries));
            //    }

            //    BooleanSearchResults = db.BooleanSearch(Data, oars, ands, nots);
            //    AddSearchResults(BooleanSearchResults);
            //}
            //catch(Exception ex)
            //{
            //    MessageBox.Show("Error with search.");
            //    Error.WriteToLog(ex);
            //}           
        }

        private void SubitSearchParameter_Click(object sender, RoutedEventArgs e)
        {
            AddSearchParameter();
        }

        private void AddSearchParameter()
        {
            try
            {
                if (SearchBar.Text != "")
                {
                    SearchPanel.Children.Add(CreateTagRow(SearchPanel.Children.Count));
                    SearchBar.Text = "";

                    //enable down button on prev row when adding row
                    if (SearchPanel.Children.Count > 2)
                    {
                        Grid l;
                        Polygon t;
                        Button r;
                        foreach (UIElement u in SearchPanel.Children)
                        {
                            if (u is Grid)
                            {
                                l = (Grid)u;
                                if (Convert.ToInt32(l.Tag) == SearchPanel.Children.Count - 2)
                                {
                                    l.Children[6].IsEnabled = true;
                                    r = (Button)l.Children[6];
                                    t = (Polygon)r.Content;
                                    t.Fill = Brushes.Black;
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
            }            
        }

        private void AddSearchResults(List<string> SR)
        {
            ResultsPanel.Children.Clear();
            GC.Collect();
            string filepath = "";
            try
            {
                double PageAmount = (double)SR.Count / resultsperpage;
                if (SR.Count > 0)
                {
                    if (Math.Ceiling(PageAmount) <= pageindex)
                    {
                        NextPage.IsEnabled = false;
                    }
                    else
                    {
                        NextPage.IsEnabled = true;

                    }
                    if (pageindex >= 2)
                    {
                        PrevPage.IsEnabled = true;
                    }
                    else
                    {
                        PrevPage.IsEnabled = false;
                    }
                    PageCount.Content = pageindex.ToString() + "/" + Math.Ceiling(PageAmount).ToString() + " (" + SR.Count.ToString() + ")";

                    for (int p = ((pageindex - 1) * resultsperpage); (p <= (resultsperpage * pageindex) - 1) && (p <= SR.Count - 1); p++)
                    {
                        filepath = System.IO.Path.Combine(db.GetImageDirectory(Data, SR[p]), SR[p]);
                        if (File.Exists(filepath))
                        {
                            Grid Result1 = new Grid() { Margin = new Thickness(2) };
                            Grid Result2 = new Grid() { Margin = new Thickness(2) };
                            RowDefinition rd1 = new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) };
                            RowDefinition rd2 = new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) };
                            RowDefinition rd3 = new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) };
                            //RowDefinition rd4 = new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) };
                            Result2.RowDefinitions.Add(rd1);
                            Result2.RowDefinitions.Add(rd2);
                            Result2.RowDefinitions.Add(rd3);
                            //Result2.RowDefinitions.Add(rd4);
                            ColumnDefinition cd1 = new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) };
                            ColumnDefinition cd2 = new ColumnDefinition() { Width = new GridLength(6, GridUnitType.Star) };
                            ColumnDefinition cd3 = new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) };
                            Result1.ColumnDefinitions.Add(cd1);
                            Result1.ColumnDefinitions.Add(cd2);
                            Result1.ColumnDefinitions.Add(cd3);

                            Label filename = new Label() { Content = SR[p] };
                            Grid.SetRow(filename, 0);
                            Result2.Children.Add(filename);

                            Label rating = new Label() { Content = "Rating: " + db.GetRating(Data, SR[p]).ToString() };
                            Grid.SetRow(rating, 1);
                            Result2.Children.Add(rating);

                            Label Dimensions = new Label() { Content = db.GetImageHeight(Data, SR[p]).ToString() + " x " + db.GetImageWidth(Data, SR[p]).ToString() };
                            Grid.SetRow(Dimensions, 2);
                            Result2.Children.Add(Dimensions);

                            Button FileGet = new Button() { Tag = SR[p], Background = Brushes.Black };
                            FileGet.Click += new RoutedEventHandler(FileSelect);

                            if ((bool)!db.IsVideo(Data, SR[p]))
                            {
                                Image thumb = new Image() { Width = 100, Height = 80 };
                                ImageInstance = BitmapImageFromFile(filepath, thumb);
                                thumb.Source = ImageInstance;
                                FileGet.Content = thumb;

                            }
                            else
                            {
                                Label thumb2 = new Label() { Content = "No Thumbnail", Foreground = Brushes.White };
                                FileGet.Content = thumb2;
                            }
                            Grid.SetColumn(FileGet, 0);
                            Result1.Children.Add(FileGet);

                            Grid.SetColumn(Result2, 1);
                            Result1.Children.Add(Result2);

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
                            Button RemoveResult = new Button() { Tag = SR[p], Content = new Polygon() { Points = RemoveCollection, Fill = Brushes.Red }, Width = 30, Height = 30, VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Right };
                            RemoveResult.Click += new RoutedEventHandler(RemoveResult_Click);

                            Grid.SetColumn(RemoveResult, 2);
                            Result1.Children.Add(RemoveResult);

                            DockPanel.SetDock(Result1, Dock.Top);

                            ResultsPanel.Children.Add(Result1);
                        }
                    }
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
                Button a = (Button)sender;
                ReturnFile = a.Tag.ToString();
                this.Close();
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
            }            
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            pageindex++;
            AddSearchResults(BooleanSearchResults);
        }

        private void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            pageindex--;
            AddSearchResults(BooleanSearchResults);
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
                pageindex = 1;
                Tags.Clear();
                ReturnFile = "";
                ResultsPanel.Children.Clear();
                SearchPanel.Children.RemoveRange(1, SearchPanel.Children.Count - 1);
                NextPage.IsEnabled = false;
                PrevPage.IsEnabled = false;
                PageCount.Content = "0/0 (0)";
                SearchBar.Text = "";
                AdvancedSearch.Text = "";
            }
            catch(Exception ex)
            {
                Error.WriteToLog(ex);
            }            
        }

        private static BitmapImage BitmapImageFromFile(string path, Image img)
        {
            BitmapImage bi = new BitmapImage() { DecodePixelHeight = 80, DecodePixelWidth = 100 };

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

        private void AdvancedSearchGo_Click(object sender, RoutedEventArgs e)
        {
            if(AdvancedSearch.Text != "")
            {
                BooleanSearchResults.Clear();
                BooleanSearchResults = db.MainSearch(Data, AdvancedSearch.Text);
                AddSearchResults(BooleanSearchResults);
            }            
        }
    }  
}
