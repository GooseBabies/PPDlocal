using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Tagger
{
    class ErrorHandling
    {
        string ErrorFileLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Tagger", "TaggerErrorLog.txt");
        string BackupErrorLog = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BackUpTaggerErrorLog.txt");

        public void CreateErrorLog()
        {
            try
            {
                if (!File.Exists(ErrorFileLocation))
                {
                    File.Create(ErrorFileLocation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to create Error Log");
                if (!File.Exists(BackupErrorLog))
                {
                    File.Create(BackupErrorLog);
                }
                WriteToBackupLog(ex);
            }            
        }

        public void WriteToLog(Exception ex)
        {
            try
            {
                using (StreamWriter SW = File.AppendText(ErrorFileLocation))
                {
                    SW.WriteLine(DateTime.Now.ToString() + " - " + ex.Message + "(" + ex.StackTrace + ")");
                }
            }
            catch (Exception ex1)
            {
                if (File.Exists(BackupErrorLog))
                {
                    using (StreamWriter SW = File.AppendText(BackupErrorLog))
                    {
                        SW.WriteLine(DateTime.Now.ToString() + " - " + ex1.Message + "(" + ex1.StackTrace + ")");
                    }
                }
            }            
        }

        private void WriteToBackupLog(Exception ex)
        {
            using (StreamWriter SW = File.AppendText(BackupErrorLog))
            {
                SW.WriteLine(DateTime.Now.ToString() + " - " + ex.Message + "(" + ex.StackTrace + ")");
            }
        }

    }
}
