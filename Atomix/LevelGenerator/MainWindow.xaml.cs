using AtomixData;
using Microsoft.Win32;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public static class EnumHelper
    {
        /// <summary>
        /// Gets an attribute on an enum field value
        /// </summary>
        /// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
        /// <param name="enumVal">The enum value</param>
        /// <returns>The attribute of type T that exists on the enum value</returns>
        public static T GetAttributeOfType<T>(this Enum enumVal) where T : System.Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return (attributes.Length > 0) ? (T)attributes[0] : null;
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            // zjistíme, zda je někdo k události přihlášen
            // musí existovat nějaký delegát, který bude event zpracovávat
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        protected AtomixData.Level _level;
        public AtomixData.Level Level
        {
            get { return _level; }
            set
            {
                _level = value;
                OnPropertyChanged("Level");
            }
        }

        public ObservableCollection<BoardTile> AvailableTiles { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            AvailableTiles = new ObservableCollection<BoardTile>();

            DataContext = this;

            foreach (var type in Enum.GetValues(typeof(TileType)).Cast<TileType>())
            {
                TilePropertiesAttribute properties = type.GetAttributeOfType<TilePropertiesAttribute>();
                bool isFixed = properties != null ? properties.IsFixed : false;

                if (properties != null && !properties.ShowInEditor)
                    continue;

                AvailableTiles.Add(new BoardTile() { Type = type, IsFixed = isFixed });
            }
        }

        void Load(string path)
        {
            Microsoft.Xna.Framework.Content.ContentManager cm = new Microsoft.Xna.Framework.Content.ContentManager(new DummyServiceProvider());
            cm.RootDirectory = System.IO.Path.GetDirectoryName(path);

            if (System.IO.Path.GetExtension(path).ToLower() == ".xnb")
            {
                Level = cm.Load<AtomixData.Level>(System.IO.Path.GetFileNameWithoutExtension(path));
            }
            else if (System.IO.Path.GetExtension(path).ToLower() == ".xml")
            {
                using (XmlReader reader = XmlReader.Create(path))
                {
                    Level = IntermediateSerializer.Deserialize<AtomixData.Level>(reader, null);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Atomix level (*.xnb, *.xml)|*.xnb;*.xml|All files|*.*";

            if (dialog.ShowDialog(this) == true)
                Load(dialog.FileName);
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
