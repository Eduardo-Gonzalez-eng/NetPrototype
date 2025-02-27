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

namespace NetPrototype.Views.Pages
{
    /// <summary>
    /// Lógica de interacción para TCPServerPage.xaml
    /// </summary>
    public partial class TCPServerPage : UserControl
    {

        public static readonly DependencyProperty ServerTitleProperty = DependencyProperty.Register("ServerTitle", typeof(string), typeof(TCPServerPage), new PropertyMetadata("Default Server Title"));

        public string ServerTitle
        {
            get { return (string)GetValue(ServerTitleProperty); }
            set { SetValue(ServerTitleProperty, value); }
        }

        public TCPServerPage()
        {
            InitializeComponent();
        }
    }
}
