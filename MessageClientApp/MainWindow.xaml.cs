using MsgInfo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
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

namespace MessageClientApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /*const string serverAddress = "127.0.0.1";
        const short serverPort = 4040;*/
        IPEndPoint serverPoint;
        UdpClient client;
        ObservableCollection<MessageInfo> messages;
        string name;
        public MainWindow()
        {
            InitializeComponent();
            string serverAddress = ConfigurationManager.AppSettings["serverAddress"]!;
            short serverPort = short.Parse(ConfigurationManager.AppSettings["serverPort"]!);
            serverPoint = new IPEndPoint(IPAddress.Parse(serverAddress), serverPort);
            client = new UdpClient();
            messages = new ObservableCollection<MessageInfo>();
            this.DataContext = messages;
        }

        private  void SendButton_Click(object sender, RoutedEventArgs e)
        {

            MessageInfo user = new MessageInfo(msgTextBox.Text, name);
            msgTextBox.Text = "";
            SendMessage(user); ;
        }

        private void msgTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                SendButton_Click(sender, e);
            }
        }

        private void JoinButton_Click(object sender, RoutedEventArgs e)
        {
            name = nameClient.Text;
            nameClient.Visibility = Visibility.Hidden;
            this.Title = name;
            MessageInfo user = new MessageInfo(" ", name);
            user.Connection = "$<Join>";
            SendMessage(user);
            Listener();
        }
        private async void SendMessage(MessageInfo user)
        {
            string message = JsonSerializer.Serialize<MessageInfo>(user);
            byte[] data = Encoding.Unicode.GetBytes(message);
            await client.SendAsync(data, data.Length, serverPoint);
        }
        private async void Listener()
        {
            while (true)
            {
                //var res = await client.ReceiveAsync();
                //string message = Encoding.Unicode.GetString(res.Buffer);
                var data = await client.ReceiveAsync();
                MessageInfo message = JsonSerializer.Deserialize<MessageInfo>(Encoding.Unicode.GetString(data.Buffer));
                if(message.Flag)
                {
                    message.Message = "Private :: " + message.Message;
                }
                messages.Add(message);
            }
        }

        private void SelectClient(object sender, SelectionChangedEventArgs e)
        {

            var list = (ListBox)sender;
            var li = (MessageInfo)list.SelectedItem;
            //MessageBox.Show($"Name :: {li.Name} -- Msg :: {li.Message} --- {li.Flag}");
            li.Flag= true;
            SendMessage(li);
        }
    }
}
