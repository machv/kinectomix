using AtomixData;
using Microsoft.Win32;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Xml;

namespace Kinectomix.LevelGenerator.View
{
    /// <summary>
    /// Interaction logic for LevelControl.xaml
    /// </summary>
    public partial class EditorControl : UserControl
    {
        public BoardTile SelectedTile { get; set; }

        public EditorControl()
        {
            InitializeComponent();

            // DEBUG load level
            Load(@"D:\Documents\Workspaces\TFS15\atomix\Atomix\Atomix\AtomixContent\Levels\Level1.xml");
        }

        private ObservableCollection<BoardTile> _boardTiles = new ObservableCollection<BoardTile>();
        private ObservableCollection<BoardTile> _moleculeTiles = new ObservableCollection<BoardTile>();
        private ObservableCollection<BoardTile> _availableTiles = new ObservableCollection<BoardTile>();
        public ObservableCollection<BoardTile> AvailableTiles
        {
            get { return _availableTiles; }
            set
            {
                _availableTiles = value;
                //OnPropertyChanged("AvailableTiles");
            }
        }

        protected AtomixData.Level _level;
        public AtomixData.Level Level
        {
            get { return _level; }
            set
            {
                _level = value;
                //OnPropertyChanged("Level");
            }
        }

        private void Tiles_TileSelected(object sender, TileSelectedEventArgs e)
        {
            if (activeTile == null)
                return;

            e.Tile.Type = activeTile.Type;
        }

        void Load(string path)
        {
            Microsoft.Xna.Framework.Content.ContentManager cm = new Microsoft.Xna.Framework.Content.ContentManager(new Kinectomix.LevelGenerator.DummyServiceProvider());
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

                //Rows = Level.Board.RowsCount;
                //Columns = Level.Board.ColumnsCount;
            }
            catch
            {
                MessageBox.Show("Unable to load level definition.", "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void handler(object sender, RoutedEventArgs e)
        {
            if (activeTile == null)
                return;

            BoardTile tile = ((ContentPresenter)ItemsControl.ContainerFromElement((ItemsControl)sender, (DependencyObject)e.OriginalSource)).Content as BoardTile;
            tile.Type = activeTile.Type;
        }

        BoardTile activeTile;

        private void Button_Save(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Atomix level (*.xml)|*.xml|Atomix binary level (*.xnb)|*.xnb";
            dialog.Title = "Save Kinectomix level definition";
            if (dialog.ShowDialog() == true)
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

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl tabs = sender as TabControl;
            if (tabs.SelectedItem == null) return;

            TabItem item = tabs.SelectedItem as TabItem;
            string tag = item.Tag as string;

            if (tag == "Board")
                AvailableTiles = _boardTiles;

            if (tag == "Molecule")
                AvailableTiles = _moleculeTiles;
        }
    }
}
