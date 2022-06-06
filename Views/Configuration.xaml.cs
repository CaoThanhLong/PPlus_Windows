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
    /// Interaction logic for TabDemo.xaml
    /// </summary>
    public partial class Configuration : Window
    {
        public Configuration()
        {
            InitializeComponent();

            PagesNavigation.Navigate(new System.Uri("Views/ConfigPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void rdSetting_Click(object sender, RoutedEventArgs e)
        {
            PagesNavigation.Navigate(new System.Uri("Views/Setting.xaml", UriKind.RelativeOrAbsolute));
        }


        private void rdPhone1_Click(object sender, RoutedEventArgs e)
        {
            PagesNavigation.Navigate(new System.Uri("Views/ConfigPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void rdPhone2_Click(object sender, RoutedEventArgs e)
        {
            PagesNavigation.Navigate(new System.Uri("Views/ConfigPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
