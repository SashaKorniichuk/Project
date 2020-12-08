using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int port = 2020;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SetFlag("Add");
            var client = new TcpClient(Dns.GetHostName(), port);
            var contact = new ContactDTO
            {
                Email = tbEmail.Text,
                Name = tbName.Text,
                Phone = tbPhone.Text

            };
            using (var stream = client.GetStream())
            {
                var serializer = new XmlSerializer(contact.GetType());
                serializer.Serialize(stream, contact);
            }
            client.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            list.Items.Clear();
            SetFlag("Load");
            var client = new TcpClient(Dns.GetHostName(), port);
            using (var stream = client.GetStream())
            {
                var serializer = new XmlSerializer(typeof(List<string>));
                var contact = (List<string>)serializer.Deserialize(stream);

                for (int i = 0; i < contact.Count; i++)
                {
                    list.Items.Add(contact[i]);
                }
            }
            client.Close();
        }
        private void SetFlag(string str)
        {
            var client = new TcpClient(Dns.GetHostName(), port);
            using (var stream = client.GetStream())
            {
                var serializer1 = new XmlSerializer(typeof(string));
                serializer1.Serialize(stream, str);
            }
            client.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (list.SelectedIndex != -1)
            {
                SetFlag("Delete");
                var client = new TcpClient(Dns.GetHostName(), port);
                using (var stream = client.GetStream())
                {
                    var serializer1 = new XmlSerializer(typeof(int));
                    serializer1.Serialize(stream, list.SelectedIndex);

                }
                client.Close();
                Button_Click_1(sender, e);

            }
        }

        private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (list.SelectedIndex != -1)
            {
                SetFlag("SelectionIndex");
                var client = new TcpClient(Dns.GetHostName(), port);
                using (var stream = client.GetStream())
                {
                    var serializer1 = new XmlSerializer(typeof(int));
                    serializer1.Serialize(stream, list.SelectedIndex);
                }
                client.Close();

                ReceiveContact();


            }
        }
        public void ReceiveContact()
        {
            SetFlag("SelectionChanged");
            var client2 = new TcpClient(Dns.GetHostName(), port);
            using (var stream = client2.GetStream())
            {
                var serializer2 = new XmlSerializer(typeof(ContactDTO));
                var contact = (ContactDTO)serializer2.Deserialize(stream);
                tbEmail.Text = contact.Email;
                tbName.Text = contact.Name;
                tbPhone.Text = contact.Phone;
            }
            client2.Close();
        }
    }
}
