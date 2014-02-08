using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using Microsoft.Xna.Framework.Graphics;
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
using System.Xml;

namespace LevelGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IntPtr _handle;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string path = @"d:\TEMP\Level1.xml";
            Microsoft.Xna.Framework.Content.ContentManager cm = new Microsoft.Xna.Framework.Content.ContentManager(new DummyServiceProvider());
            cm.RootDirectory = System.IO.Path.GetDirectoryName(path);
            AtomixData.Level level = null;
            if (System.IO.Path.GetExtension(path).ToLower() == ".xnb")
            {
                level = cm.Load<AtomixData.Level>(System.IO.Path.GetFileNameWithoutExtension(path));
            }
            else if (System.IO.Path.GetExtension(path).ToLower() == ".xml")
            {
                using (XmlReader reader = XmlReader.Create(path))
                {
                    level = IntermediateSerializer.Deserialize < AtomixData.Level>(reader, null);
                }
            }
        }

        public class DummyServiceProvider : IServiceProvider
        {
            public object GetService(Type serviceType)
            {
                return null;
            }
        }
    }
}
