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
            //Load(@"D:\Documents\Workspaces\TFS15\atomix\Atomix\Atomix\AtomixContent\Levels\Level1.xml");
        }

        private void Tiles_TileSelected(object sender, TileSelectedEventArgs e)
        {
            //if (activeTile == null)
            //    return;

            //e.Tile.Type = activeTile.Type;
        }

        //private void handler(object sender, RoutedEventArgs e)
        //{
        //    if (activeTile == null)
        //        return;

        //    BoardTile tile = ((ContentPresenter)ItemsControl.ContainerFromElement((ItemsControl)sender, (DependencyObject)e.OriginalSource)).Content as BoardTile;
        //    tile.Type = activeTile.Type;
        //}

        //BoardTile activeTile;

        //private void Button_Save(object sender, RoutedEventArgs e)
        //{
        //    SaveFileDialog dialog = new SaveFileDialog();
        //    dialog.Filter = "Atomix level (*.xml)|*.xml|Atomix binary level (*.xnb)|*.xnb";
        //    dialog.Title = "Save Kinectomix level definition";
        //    if (dialog.ShowDialog() == true)
        //    {
        //        if (dialog.FileName == "")
        //            return;

        //        using (Stream stream = dialog.OpenFile())
        //        {

        //            switch (dialog.FilterIndex)
        //            {
        //                case 1:
        //                    // Save definition
        //                    XmlWriterSettings settings = new XmlWriterSettings();
        //                    settings.Indent = true;

        //                    using (XmlWriter writer = XmlWriter.Create(stream, settings))
        //                    {
        //                        IntermediateSerializer.Serialize(writer, Level, null);
        //                    }
        //                    break;
        //                case 2:
        //                    // http://stackoverflow.com/questions/8856528/serialize-texture2d-programatically-in-xna
        //                    Type compilerType = typeof(ContentCompiler);
        //                    ContentCompiler cc = compilerType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0].Invoke(null) as ContentCompiler;
        //                    var compileMethod = compilerType.GetMethod("Compile", BindingFlags.NonPublic | BindingFlags.Instance);

        //                    compileMethod.Invoke(cc, new object[]{
        //                      stream, Level, TargetPlatform.Windows, GraphicsProfile.Reach, false/*true*/, dialog.FileName, dialog.FileName
        //                      });
        //                    break;
        //            }
        //        }
        //    }
        //}

        //private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    TabControl tabs = sender as TabControl;
        //    if (tabs.SelectedItem == null) return;

        //    TabItem item = tabs.SelectedItem as TabItem;
        //    string tag = item.Tag as string;

        //    //if (tag == "Board")
        //    //    AvailableTiles = _boardTiles;

        //    //if (tag == "Molecule")
        //    //    AvailableTiles = _moleculeTiles;
        //}
    }
}
