using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestRS232
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SerialPort serialPort;
        private System.Windows.Threading.DispatcherTimer timer;
        private Thread thread;
        public MainWindow()
        {
            InitializeComponent();
            InitSerialPort();
          //  UpdateInfor();


            // check port RS232 in the computer
            foreach (var port in SerialPort.GetPortNames())
            {
                Console.WriteLine("port:" + port);
            }
            updateData();
        }
        private void InitSerialPort()
        {
            // setup info to connect RS232
            serialPort = new SerialPort
            {
                PortName = "COM3",
                BaudRate = 9600,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One,
                Handshake = Handshake.None,
                ReadTimeout = 2000,
                WriteTimeout = 2000,

            };
            try
            {
                serialPort.Open();
            }
            catch(Exception ex)
            {
                MessageBox.Show("ERROR:" + ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            serialPort.WriteLine("SYSTEM:REMOTE\r");

            if (serialPort.IsOpen)
            {
                if (double.TryParse(ChangeVal.Text.Replace(",", "."), out double voltage)&& double.TryParse(ChangeValA.Text.Replace(",", "."), out double amp))
                {
                   
                    string commandV = $"VOLT {voltage:F2}\n"; // Lệnh SCPI để đặt điện áp
                    serialPort.WriteLine(commandV);
                    string commandA = $"CURR {amp:F2}\n"; // Lệnh SCPI để đặt dòng điện
                    serialPort.WriteLine(commandA);             
                    updateData();
                    //   Thread.Sleep(100); // Chờ một chút để thiết bị phản hồi
                    //   string response = serialPort.ReadExisting(); // Đọc phản hồi từ thiết bị
                    //   MessageBox.Show($"Phản hồi từ nguồn: {response}");
                }
            }
        }

        
  //      private void UpdateInfor()
  //      {
   //         timer = new System.Windows.Threading.DispatcherTimer();
   //         timer.Interval = TimeSpan.FromSeconds(2);// Setup time to update info 2 second
    //        timer.Tick += (s, e) => updateData();// call function updateData after 2 second
    //        timer.Start();
    //    }
        
        
        private void updateData()
        {
            if (serialPort.IsOpen)
            {
                inputV.Text = Command("VOLT?") + "V";
                outputA.Text = Command("MEAS:CURR?") + "A";
            }
        }
        private string Command(String str)
        {
            try
            {
                serialPort.WriteLine(str);
                return serialPort.ReadLine().Trim();
            }
            catch(Exception ex)
            {

                MessageBox.Show("ERR:" + ex.Message);
                return "N/A";

            }
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            updateData();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            PrintDialog print = new PrintDialog();
            if (print.ShowDialog() == true)
            {
                print.PrintVisual(Info, "In thong tin");
            }
        }
    }
}
