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
using System.Windows.Shapes;

namespace PC_Client.Views
{
    /// <summary>
    /// Interaction logic for Alert.xaml
    /// </summary>
    public partial class Alert : Window
    {
        public bool Confirm { get; set; }
        public Alert(string mac, string name)
        {
            InitializeComponent();
            mac_label.Text = mac;
            name_label.Text = name;
        }

        private void Reject(object sender, RoutedEventArgs e)
        {
            Confirm = false;
            this.Close();
        }

        private void Accept(object sender, RoutedEventArgs e)
        {
            Confirm = true;
            this.Close();
        }
    }
}
