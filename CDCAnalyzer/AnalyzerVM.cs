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
                    return "Disconnect";
                }
                else
                {
                    return "Connect";
                }
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
    }
}
