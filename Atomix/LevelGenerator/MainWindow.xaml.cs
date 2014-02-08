using AtomixData;
using Microsoft.Win32;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
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
using System.Windows.Threading;
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

        public int Rows { get; set; }
        public int Columns { get; set; }

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

        private ObservableCollection<BoardTile> _boardTiles = new ObservableCollection<BoardTile>();
        private ObservableCollection<BoardTile> _moleculeTiles = new ObservableCollection<BoardTile>();
        private ObservableCollection<BoardTile> _availableTiles  = new ObservableCollection<BoardTile>();
        public ObservableCollection<BoardTile> AvailableTiles
        {
            get { return _availableTiles; }
            set
            {
                _availableTiles = value;
                OnPropertyChanged("AvailableTiles");
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            foreach (var type in Enum.GetValues(typeof(TileType)).Cast<TileType>())
            {
                TilePropertiesAttribute properties = type.GetAttributeOfType<TilePropertiesAttribute>();
                bool isFixed = properties != null ? properties.IsFixed : false;

                if (properties != null && !properties.ShowInBoardEditor)
                    continue;

                _boardTiles.Add(new BoardTile() { Type = type, IsFixed = isFixed });
            }

            foreach (var type in Enum.GetValues(typeof(TileType)).Cast<TileType>())
            {
                TilePropertiesAttribute properties = type.GetAttributeOfType<TilePropertiesAttribute>();
                bool isFixed = properties != null ? properties.IsFixed : false;

                if (properties != null && !properties.ShowInMoleculeEditor)
                    continue;

                _moleculeTiles.Add(new BoardTile() { Type = type, IsFixed = isFixed });
            }

            AvailableTiles = _boardTiles;
        }

        void Load(string path)
        {
            Microsoft.Xna.Framework.Content.ContentManager cm = new Microsoft.Xna.Framework.Content.ContentManager(new DummyServiceProvider());
            cm.RootDirectory = System.IO.Path.GetDirectoryName(path);

            try
            {
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

                Rows = Level.Board.RowsCount;
                Columns = Level.Board.ColumnsCount;
            }
            catch
            {
                MessageBox.Show("Unable to load level definition.", "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Atomix level (*.xnb, *.xml)|*.xnb;*.xml|All files|*.*";

            if (dialog.ShowDialog(this) == true)
                Load(dialog.FileName);
        }

        private void handler(object sender, RoutedEventArgs e)
        {
            if (activeTile == null)
                return;

            BoardTile tile = ((ContentPresenter)ItemsControl.ContainerFromElement((ItemsControl)sender, (DependencyObject)e.OriginalSource)).Content as BoardTile;
            tile.Type = activeTile.Type;
        }

        BoardTile activeTile;

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox box = sender as ListBox;
            BoardTile tile = box.SelectedItem as BoardTile;

            activeTile = tile;
        }

        private void Button_Save(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Atomix level (*.xml)|*.xml|Atomix binary level (*.xnb)|*.xnb";
            dialog.Title = "Save Kinectomix level definition";
            if (dialog.ShowDialog(this) == true)
            {
                if (dialog.FileName == "")
                    return;

                using (Stream stream = dialog.OpenFile())
                {

                    switch (dialog.FilterIndex)
                    {
                        case 1:
                            // Save definition
                            XmlWriterSettings settings = new XmlWriterSettings();
                            settings.Indent = true;

                            using (XmlWriter writer = XmlWriter.Create(stream, settings))
                            {
                                IntermediateSerializer.Serialize(writer, Level, null);
                            }
                            break;
                        case 2:
                            // http://stackoverflow.com/questions/8856528/serialize-texture2d-programatically-in-xna
                            Type compilerType = typeof(ContentCompiler);
                            ContentCompiler cc = compilerType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0].Invoke(null) as ContentCompiler;
                            var compileMethod = compilerType.GetMethod("Compile", BindingFlags.NonPublic | BindingFlags.Instance);

                            compileMethod.Invoke(cc, new object[]{
                              stream, Level, TargetPlatform.Windows, GraphicsProfile.Reach, false/*true*/, dialog.FileName, dialog.FileName
                              });
                            break;
                    }
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl tabs = sender as TabControl;
            TabItem item = tabs.SelectedItem as TabItem;
            string tag = item.Tag as string;
            
            if (tag == "Board")
                AvailableTiles = _boardTiles;
            
            if (tag == "Molecule")
                AvailableTiles = _moleculeTiles;
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
