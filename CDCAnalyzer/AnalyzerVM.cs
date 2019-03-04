using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using System.IO.Ports;

namespace CDCAnalyzer
{
    public class AnalyzerVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Analyzer analyzer;
        DispatcherTimer timer;
        private UiCommand ButtonConnectCommand;
        private UiCommand ButtonClearCommand;
        private UiCommand ButtonSelectFileCommand;

        public bool ElementsEnabled { get; set; }

        public List<string> PortListVM
        {
            get
            {
                return analyzer.PortList;
            }
        }
        public string SelectedPortVM
        {
            set
            {
                if (value != null)
                {
                    analyzer.SelectedPort = value;
                }
            }
        }
        //public string BaudVM
        //{
        //    get
        //    {
        //        return analyzer.Baud.ToString();
        //    }
        //    set
        //    {
        //        analyzer.Baud = int.Parse(value);
        //        PropertyChanged(this, new PropertyChangedEventArgs("BaudVM"));
        //    }
        //}

        public string BytesReceivedVM
        {
            get
            {
                return analyzer.ReceivedCnt.ToString();
            }
        }
        public string CurrentSpeedVM
        {
            get
            {
                return analyzer.CurrentSpeed.ToString();
            }

        }
        public string AverageSpeedVM
        {
            get
            {
                return analyzer.AverageSpeed.ToString();
            }

        }
        public string ConnectionStateVM
        {
            get
            {
                if (analyzer.ConnectionState == true)
                {
                    ElementsEnabled = false;
                    PropertyChanged(this, new PropertyChangedEventArgs("ElementsEnabled"));
                    return "Disconnect";
                }
                else
                {
                    ElementsEnabled = true;
                    PropertyChanged(this, new PropertyChangedEventArgs("ElementsEnabled"));
                    return "Connect";          
                }
                
            }
        }

        public string FilePathVM
        {
            get
            {
                return analyzer.FilePath;
            }
            set
            {
                analyzer.FilePath = value;
            }
        }

        public bool SaveFileEnabledVM
        {
            set
            {
                analyzer.SaveFileEnabled = value;
            }
        }


        public ICommand ButtonConnect_Click
        {
            get
            {
                if (ButtonConnectCommand == null)
                {
                    ButtonConnectCommand = new UiCommand((obj) => this.ConnectRequest(obj));
                }
                return ButtonConnectCommand;
            }
        }

        public ICommand ButtonClear_Click
        {
            get
            {
                if (ButtonClearCommand == null)
                {
                    ButtonClearCommand = new UiCommand((obj) => this.ClearRequest(obj));
                }
                return ButtonClearCommand;
            }
        }

        public ICommand ButtonSelectFile_Click
        {
            get
            {
                if (ButtonSelectFileCommand == null)
                {
                    ButtonSelectFileCommand = new UiCommand(obj => this.SelectFileRequest(obj));
                }
                return ButtonSelectFileCommand;
            }
        }


        public AnalyzerVM ()
        {
            analyzer = new Analyzer();

            //Configure and start timer
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(50);
            timer.Tick += TimerTickHandler;
            timer.Start();

        }

        // Update window with received data
        public void TimerTickHandler(object sender, EventArgs e)
        {
            PropertyChanged(this, new PropertyChangedEventArgs("BytesReceivedVM"));
            PropertyChanged(this, new PropertyChangedEventArgs("CurrentSpeedVM"));
            PropertyChanged(this, new PropertyChangedEventArgs("AverageSpeedVM"));
        }

        public void ConnectRequest (object parameter)
        {
            if (analyzer.ConnectionState == true)
            {
                analyzer.ChangeConnectionState(false);
            }
            else
            {
                analyzer.ChangeConnectionState(true);
            }
            PropertyChanged(this, new PropertyChangedEventArgs("ConnectionStateVM"));
        }

        public void ClearRequest (object parameter)
        {
            analyzer.AverageSpeed = 0;
            analyzer.CurrentSpeed = 0;
            analyzer.ReceivedCnt = 0;
        }

        public void SelectFileRequest (object parameter)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();

            ofd.DefaultExt = ".txt";
            ofd.Filter = "TXT Files (*.txt)|*.txt|All Files (*.*)|*.*";
            ofd.InitialDirectory = @"C:\Users\User\Desktop\";
            ofd.CheckFileExists = false;
            ofd.CheckPathExists = true;
            ofd.Title = "Select File";
            ofd.FileName = "data.txt";
            ofd.RestoreDirectory = true;
            

            Nullable<bool> result = ofd.ShowDialog();

            if (result == true)
            {
                analyzer.FilePath = ofd.FileName;
                PropertyChanged(this, new PropertyChangedEventArgs("FilePathVM"));
            }
        }
    }
}
