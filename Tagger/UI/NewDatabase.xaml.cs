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
using System.Data.SqlClient;
using System.Data.Linq;
using System.IO;
using System.Data.Linq.Mapping;

namespace Tagger
{
    /// <summary>
    /// Interaction logic for NewDatabase.xaml
    /// </summary>
    public partial class NewDatabase : Window
    {
        DatabaseUtil DBUtilities = new DatabaseUtil();  //Databse Class
        ErrorHandling Error = new ErrorHandling();      //Error Handling Class

        public NewDatabase()
        {
            InitializeComponent();
            SetWindowScreen(this, GetWindowScreen(App.Current.MainWindow));

            CanceledFlag = true;
        }

        string DatabseExtension = ".mdf";           //Databse Extension

        public string IDirectory { get; set; }      //Image directory Location
        public string Dbfile { get; set; }          //Database file location
        public string Profilename { get; set; }     //Databse/Profile Name
        public bool CanceledFlag { get; set; }          //Boolean flag if closed without creating database

        private void CreateDatabse_Click(object sender, RoutedEventArgs e)
        {
            string DbFile = "";
            string DBLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Tagger");

            try
            {
                if (DbName.Text == "")  //If no profile name is entered skip and alert user 
                {
                    MessageBox.Show("Please fill out all fields before creating database.", "Error!");
                }
                else
                {
                    DbFile = DbName.Text + DatabseExtension;
                    if (!Directory.Exists(DBLocation))          //Create directory if it doesn't exist
                    {
                        Directory.CreateDirectory(DBLocation);
                        DBUtilities.CreateDatabase(DBLocation, DbName.Text);    //Create new database with tables preloaded
                        Profilename = DbName.Text;
                        Dbfile = Path.Combine(DBLocation, DbFile);
                        CanceledFlag = false;
                        this.Close();
                    }
                    else
                    {
                        if (!File.Exists(Path.Combine(DBLocation, DbFile)))         //Create Database file if it doesn't already exist
                        {
                            DBUtilities.CreateDatabase(DBLocation, DbName.Text);
                            Profilename = DbName.Text;
                            Dbfile = Path.Combine(DBLocation, DbFile);
                            CanceledFlag = false;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Database with that name already exits.", "Error!");
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error creating database. - " + ex.Message);
                Error.WriteToLog(ex);
            }            
        }

        private void LocationOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var dialog = new System.Windows.Forms.FolderBrowserDialog()) //Open folder browse dialog to get image directory from user
                {
                    dialog.ShowDialog();

                    ImageDirectory.Text = dialog.SelectedPath;

                    IDirectory = dialog.SelectedPath;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error retrieving image directory");
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
