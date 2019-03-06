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
        public string FilePath { get; set; }
        public bool SaveFileEnabled { get; set; } 

        public float CurrentSpeed { get; set; }
        public float AverageSpeed { get; set; }

        private int LastReceivedCnt = 0;
        private DateTime FirstTimestamp;
        private DateTime LastTimestamp;

        private CircularBuffer<byte> circularBuffer;
        private int WriteCnt = 0;

        private DispatcherTimer Timer;

        public Analyzer()
        {

            ComPort = new SerialPort();
            Baud = 921600;
            ReceivedCnt = 0;
            PortList = SerialPort.GetPortNames().ToList<string>();
            ConnectionState = false;
            FilePath = @"C:\Users\User\Desktop\data.txt";

            CurrentSpeed = 0;
            AverageSpeed = 0;

            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromMilliseconds(250);
            Timer.Tick += this.TimerTickHandler;
            Timer.Start();

            circularBuffer = new CircularBuffer<byte>(10000000);
           
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
            byte[] tmp_buf = new byte[cnt];

            if (FirstTimestamp.Ticks == 0)
            {
                FirstTimestamp = DateTime.Now;
            }

            sp.Read(tmp_buf, 0, cnt);
            circularBuffer.Enqueue(tmp_buf, cnt);
            ReceivedCnt += cnt;
  
            LastTimestamp = DateTime.Now;
            if (FirstTimestamp.Ticks != LastTimestamp.Ticks)
                AverageSpeed = (ReceivedCnt) * ((float)TimeSpan.TicksPerSecond / (LastTimestamp.Ticks - FirstTimestamp.Ticks)) / 1000;
        }

        void TimerTickHandler(object sender, EventArgs e)
        {
            DispatcherTimer dt = (DispatcherTimer)sender;

            PortList = SerialPort.GetPortNames().ToList<string>();

            CurrentSpeed = (float)(ReceivedCnt - LastReceivedCnt) * 4 / 1000;
            LastReceivedCnt = ReceivedCnt;
            if (ConnectionState == true && SaveFileEnabled == true)
            {
                using (StreamWriter sw = new StreamWriter(FilePath, true, System.Text.Encoding.Default))
                {
                    for (int i = 0; i < ReceivedCnt - WriteCnt; i++)
                    {
                        sw.Write(string.Format("0x{0:X} ", circularBuffer.Dequeue()));
                    }
                    WriteCnt = ReceivedCnt;
                }
            }
        }

    }
}
