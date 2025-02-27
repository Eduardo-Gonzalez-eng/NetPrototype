using NetPrototype.Interfaces;
using NetPrototype.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public partial class TCPServerPage : UserControl, IPage
    {
        public TCPServerPage()
        {
            InitializeComponent();
            this.Unloaded += View_Unloaded;
        }

        private void View_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is TCPServerViewModel viewModel)
            {
                viewModel.Cleanup();
                Debug.WriteLine("CleanUP Server View.");
            }
        }
    }
}
