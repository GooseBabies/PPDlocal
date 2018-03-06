using System;
using System.Windows;

namespace Tagger.UI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class InputWindow : Window
    {
        public InputWindow(string Message, string textfill)
        {
            InitializeComponent();
            SetWindowScreen(this, GetWindowScreen(App.Current.MainWindow));

            InputLabel.Text = Message;
            Input.Text = textfill;
            Response = "";
        }
        public string Response { get; set; }        //User Input
        ErrorHandling Error = new ErrorHandling();

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Input.Text != "")
                {
                    Response = Input.Text;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Please fill in all fields before submitting.");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error" + " - " + ex.Message);
                Error.WriteToLog(ex);
                Response = Input.Text;
                this.Close();
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
