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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Sockets;
using System.Net;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string remoteIPAddress = "127.0.0.1";
        private static int remotePort = 8080;
        TcpClient client = new TcpClient(); 
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ConnectServer()
        {
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(remoteIPAddress), remotePort);
            client.Connect(iPEndPoint);
        }

        private void DisconnectServer()
        {
            if(!client.Close()) client.Close();
        }
    }
}
