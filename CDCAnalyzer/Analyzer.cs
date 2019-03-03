using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Windows.Threading;

namespace CDCAnalyzer
{
    public class Analyzer
    {

        SerialPort ComPort { get; set; }
        public int Baud { get; set; }
        public int ReceivedCnt { get; set; }
        public List<string> PortList { get; set; }
        public string SelectedPort { get;  set; }
        public bool ConnectionState { get; set; }

        public int CurrentSpeed { get; set; }
        public float AverageSpeed { get; set; }

        private int LastReceivedCnt = 0;
        private DateTime FirstTimestamp;
        private DateTime LastTimestamp;

        private byte[] SerialBuffer = new byte[1000000];
        private int SerialCnt = 0;
        private int SerialPos = 0;

        private DispatcherTimer Timer;

        public Analyzer()
        {

            ComPort = new SerialPort();
            Baud = 115200;
            ReceivedCnt = 0;
            PortList = SerialPort.GetPortNames().ToList<string>();
            ConnectionState = false;

            CurrentSpeed = 0;
            AverageSpeed = 0;

            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromMilliseconds(250);
            Timer.Tick += this.TimerTickHandler;
            Timer.Start();

        }

    public void ChangeConnectionState (bool state)
        {
            if (ComPort.IsOpen == false && state == true && SelectedPort != null)
            {
                try
                {
                    ComPort.PortName = SelectedPort;
                    ComPort.Open();
                    ComPort.DataReceived += DataReceivedHandler;
                    ConnectionState = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: {0}", ex.Message);
                }
            }
            else if (ComPort.IsOpen == true && state == false)
            {
                ComPort.Close();
                ComPort.DataReceived -= DataReceivedHandler;
                ConnectionState = false;
            }
            else if (SelectedPort != null)
            {
                ConnectionState = state;
            }
        }

        void DataReceivedHandler (object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort) sender;
            int cnt = sp.BytesToRead;

            if (FirstTimestamp.Ticks == 0)
            {
                FirstTimestamp = DateTime.Now;
            }

            sp.Read(SerialBuffer, 0, cnt);
            
            ReceivedCnt += cnt;

            LastTimestamp = DateTime.Now;

            if (FirstTimestamp.Ticks != LastTimestamp.Ticks)
                AverageSpeed = (ReceivedCnt) *  ((float) TimeSpan.TicksPerSecond / (LastTimestamp.Ticks - FirstTimestamp.Ticks));
        }

        void TimerTickHandler(object sender, EventArgs e)
        {
            DispatcherTimer dt = (DispatcherTimer)sender;

            CurrentSpeed = (ReceivedCnt - LastReceivedCnt) * 4;

            LastReceivedCnt = ReceivedCnt;
        }

    }
}
