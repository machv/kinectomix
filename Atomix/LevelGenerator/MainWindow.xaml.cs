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

namespace Kinectomix.LevelGenerator
{
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

        protected int _rows;
        public int Rows
        {
            get { return _rows; }
            set
            {
                if (value > 0)
                {
                    _rows = value;
                    OnPropertyChanged("Rows");
                }
            }
        }

        protected int _columns;
        public int Columns
        {
            get { return _columns; }
            set
            {
                if (value > 0)
                {
                    _columns = value;
                    OnPropertyChanged("Columns");
                }
            }
        }

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

        protected AtomixData.BoardTileCollection _tiles;
        public AtomixData.BoardTileCollection Tiles
        {
            get { return _tiles; }
            set
            {
                _tiles = value;
                OnPropertyChanged("Tiles");
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

                Rows = _level.Board.RowsCount;
                Columns = _level.Board.ColumnsCount;
                //TODO nastavit záložku na první
                Tiles = _level.Board;
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
            {
                if (Level != null)
                {
                    Rows = _level.Board.RowsCount;
                    Columns = _level.Board.ColumnsCount;
                    Tiles = _level.Board;
                }
                
                AvailableTiles = _boardTiles;
            }

            if (tag == "Molecule")
            {
                if (Level != null)
                {
                    Rows = _level.Molecule.RowsCount;
                    Columns = _level.Molecule.ColumnsCount;
                    Tiles = _level.Molecule;
                }
   
                AvailableTiles = _moleculeTiles;
            }
        }

        Point startPoint;
        private void List_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Store the mouse position
            startPoint = e.GetPosition(null);
        }

        private void List_MouseMove(object sender, MouseEventArgs e)
        {
            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                // Get the dragged ListViewItem
                ListBox listBox = sender as ListBox;
                //BoardTile tile = listBox.SelectedItem as BoardTile;
                ListBoxItem listBoxItem =
                    FindAnchestor<ListBoxItem>((DependencyObject)e.OriginalSource);

                if (listBoxItem == null)
                    return;

                // Find the data behind the ListViewItem
                BoardTile tile = (BoardTile)listBox.ItemContainerGenerator.
                    ItemFromContainer(listBoxItem);

                // Initialize the drag & drop operation
                DataObject dragData = new DataObject("myFormat", tile);
                DragDrop.DoDragDrop(listBoxItem, dragData, DragDropEffects.Move);
            }
        }

        // Helper to search up the VisualTree
        private static T FindAnchestor<T>(DependencyObject current)
            where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        private void DropList_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("myFormat") ||
                sender == e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void DropList_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("myFormat"))
            {
                BoardTile tile = e.Data.GetData("myFormat") as BoardTile;
                //ListView listView = sender as ListView;
                //listView.Items.Add(contact);
            }
        }

        private void Button_Resize(object sender, RoutedEventArgs e)
        {
            BoardTileCollection newTiles = new BoardTileCollection(_rows, _columns);
            for (int i = 0; i < newTiles.RowsCount; i++)
            {
                for (int j = 0; j < newTiles.ColumnsCount; j++)
                {
                    newTiles[i, j] = new BoardTile() { Type = TileType.Empty, IsFixed = true };
                }
            }


            for (int i = 0; i < Math.Min(newTiles.RowsCount, _tiles.RowsCount); i++)
            {
                for (int j = 0; j < Math.Min(newTiles.ColumnsCount, _tiles.ColumnsCount); j++)
                {
                    newTiles[i, j] = _tiles[i, j];
                }
            }

            Tiles = newTiles;
        }
    }
}
