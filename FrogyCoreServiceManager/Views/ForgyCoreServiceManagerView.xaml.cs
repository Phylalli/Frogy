using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.ServiceProcess;
using System.Configuration.Install;
using System.Collections;

using FrogyCoreServiceManager.ViewModels;

namespace FrogyCoreServiceManager.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ForgyCoreServiceManagerView : Window
    {
        ForgyCoreServiceManagerViewModel viewModel = new ForgyCoreServiceManagerViewModel();

        public ForgyCoreServiceManagerView()
        {
            this.DataContext = viewModel;

            InitializeComponent();
        }

    }
}
