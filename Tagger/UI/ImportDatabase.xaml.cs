using System;
using System.Windows;

namespace Tagger.UI
{
    /// <summary>
    /// Interaction logic for ImportDatabase.xaml
    /// </summary>
    public partial class ImportDatabase : Window
    {
        public ImportDatabase(string DirectoryPath)
        {
            InitializeComponent();
            SetWindowScreen(this, GetWindowScreen(App.Current.MainWindow));

            FileLocationInput.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);  //Set initial file location as My Documents
            ImageDirectoryInput.Text = DirectoryPath;                                                   //Set initial directory
            SelectedDirectory = DirectoryPath;                                                          //Set inital output directory
            Canceled = true;                                                                            //initalize canceled as true
        }

        ErrorHandling Error = new ErrorHandling();

        public string SelectedDirectory { get; set; }   //Output image directory
        public string ImportFile { get; set; }          //Output backup file Location
        public bool Canceled { get; set; }              //Output canceled or not

        private void OpenFileLocation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var FileDialog = new Microsoft.Win32.OpenFileDialog         //Open dialog to browse for backup file (only allow text files)
                {
                    DefaultExt = ".txt",
                    Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                    InitialDirectory = FileLocationInput.Text
                };

                if (FileDialog.ShowDialog() == true)                        //Retrieve file location information from file dialog
                {
                    FileLocationInput.Text = FileDialog.FileName;
                    ImportFile = FileDialog.FileName;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error getting database backup file. - " + ex.Message);
                Error.WriteToLog(ex);
            }            
        }

        private void SelectImageDirectory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var dialog = new System.Windows.Forms.FolderBrowserDialog()) //Open Folder browse dialog to browse for image directory
                {
                    dialog.ShowDialog();

                    ImageDirectoryInput.Text = dialog.SelectedPath;
                    SelectedDirectory = dialog.SelectedPath;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error getting image directory. - " + ex.Message);
                Error.WriteToLog(ex);
            }            
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FileLocationInput.Text == "" || ImageDirectoryInput.Text == "") //Verify all fields have values before submitting
                {
                    MessageBox.Show("All fields need to be filled in to import");
                }
                else
                {
                    MessageBoxResult check = MessageBox.Show("All imported directory information will be overwritten with selected directory information. Are you sure you want to continue", "Continue?", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (check == MessageBoxResult.Yes)  //Check if user wants to update directory information with new directory
                    {
                        Canceled = false;
                        this.Close();
                    }
                    else
                    {
                        Canceled = true;
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error importing backup file. - " + ex.Message);
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
