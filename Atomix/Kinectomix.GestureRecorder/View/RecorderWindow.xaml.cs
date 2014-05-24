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

namespace Kinectomix.GestureRecorder.View
{
    /// <summary>
    /// Interaction logic for RecorderWindow.xaml
    /// </summary>
    public partial class RecorderWindow : Window
    {
        public RecorderWindow()
        {
            InitializeComponent();

            Closing += Window_Closing;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            (DataContext as ViewModel.RecorderViewModel).OnWindowClosing();
        }
    }
}
