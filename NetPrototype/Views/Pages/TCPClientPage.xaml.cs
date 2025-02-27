using NetPrototype.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Lógica de interacción para TCPClientPage.xaml
    /// </summary>
    public partial class TCPClientPage : UserControl, IPage
    {
        public TCPClientPage()
        {
            InitializeComponent();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            // Solo permite números (0-9)
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }
    }
}
