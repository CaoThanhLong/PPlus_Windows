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

namespace PC_Client.Views
{
    /// <summary>
    /// Interaction logic for ConfigPage.xaml
    /// </summary>
    public partial class ConfigPage : Page
    {
        public bool IsDisconnect { get; set; }
        public ConfigPage()
        {
            InitializeComponent();
        }

        private void Disconnect(object sender, RoutedEventArgs e)
        {
            IsDisconnect = true;
        }
    }
}
