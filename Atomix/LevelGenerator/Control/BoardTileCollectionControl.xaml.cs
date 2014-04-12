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

namespace Kinectomix.LevelGenerator.Control
{
    /// <summary>
    /// Interaction logic for BoardTileCollectionControl.xaml
    /// </summary>
    public partial class BoardTilesControl : UserControl
    {
        public AtomixData.BoardTile SelectedItem
        {
            get { return (AtomixData.BoardTile)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(AtomixData.BoardTile), typeof(BoardTilesControl), new PropertyMetadata(null));


        public AtomixData.BoardTileCollection Tiles
        {
            get { return (AtomixData.BoardTileCollection)GetValue(TilesProperty); }
            set { SetValue(TilesProperty, value); }
        }

        public static readonly DependencyProperty TilesProperty =
            DependencyProperty.Register("Tiles", typeof(AtomixData.BoardTileCollection), typeof(BoardTilesControl), new PropertyMetadata(null));

        public int TileWidth
        {
            get { return (int)GetValue(TileWidthProperty); }
            set { SetValue(TileWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TileWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TileWidthProperty =
            DependencyProperty.Register("TileWidth", typeof(int), typeof(BoardTilesControl), new PropertyMetadata(49));


        public int TileHeight
        {
            get { return (int)GetValue(TileHeightProperty); }
            set { SetValue(TileHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TileHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TileHeightProperty =
            DependencyProperty.Register("TileHeight", typeof(int), typeof(BoardTilesControl), new PropertyMetadata(49));

        // Based on http://msdn.microsoft.com/en-us/library/ms752288(v=vs.110).aspx

        // This event uses the bubbling routing strategy 
        public static readonly RoutedEvent TileSelectedEvent = EventManager.RegisterRoutedEvent(
            "TileSelected", RoutingStrategy.Bubble, typeof(TileSelectedEventHandler), typeof(BoardTilesControl));

        public delegate void TileSelectedEventHandler(object sender, TileSelectedEventArgs e);

        // Provide CLR accessors for the event 
        public event TileSelectedEventHandler TileSelected
        {
            add { AddHandler(TileSelectedEvent, value); }
            remove { RemoveHandler(TileSelectedEvent, value); }
        }

        // This method raises the Tap event 
        void RaiseTileSelectedEvent(AtomixData.BoardTile tile)
        {
            RoutedEventArgs newEventArgs = new TileSelectedEventArgs(BoardTilesControl.TileSelectedEvent, tile);
            RaiseEvent(newEventArgs);
        }

        public BoardTilesControl()
        {
            InitializeComponent();
        }

        private void Tile_MouseDown(object sender, MouseEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;

            SelectedItem = element.DataContext as AtomixData.BoardTile;
            RaiseTileSelectedEvent(element.DataContext as AtomixData.BoardTile);
        }

        private void Tile_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                FrameworkElement element = sender as FrameworkElement;

                SelectedItem = element.DataContext as AtomixData.BoardTile;
                RaiseTileSelectedEvent(element.DataContext as AtomixData.BoardTile);
            }
        }
    }
}
