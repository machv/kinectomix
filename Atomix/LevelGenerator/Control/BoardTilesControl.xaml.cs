using System;
using System.Collections;
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
        public static readonly DependencyProperty SelectedTileProperty = DependencyProperty.Register("SelectedTile", typeof(BoardTileViewModel), typeof(BoardTilesControl), new PropertyMetadata(null));

        public BoardTileViewModel SelectedTile
        {
            get { return (BoardTileViewModel)GetValue(SelectedTileProperty); }
            set { SetValue(SelectedTileProperty, value); }
        }

        public static readonly DependencyProperty TilesSourceProperty = DependencyProperty.Register("TilesSource", typeof(IEnumerable), typeof(BoardTilesControl), new PropertyMetadata(null));

        public IEnumerable TilesSource
        {
            get { return (AtomixData.BoardTileCollection)GetValue(TilesSourceProperty); }
            set { SetValue(TilesSourceProperty, value); }
        }

        public static readonly DependencyProperty RowsProperty = DependencyProperty.Register("Rows", typeof(int), typeof(BoardTilesControl), new PropertyMetadata(0));

        public int Rows
        {
            get { return (int)GetValue(RowsProperty); }
            set { SetValue(RowsProperty, value); }
        }

        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register("Columns", typeof(int), typeof(BoardTilesControl), new PropertyMetadata(0));

        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        public static readonly DependencyProperty TileWidthProperty = DependencyProperty.Register("TileWidth", typeof(int), typeof(BoardTilesControl), new PropertyMetadata(49));

        public int TileWidth
        {
            get { return (int)GetValue(TileWidthProperty); }
            set { SetValue(TileWidthProperty, value); }
        }

        public static readonly DependencyProperty TileHeightProperty = DependencyProperty.Register("TileHeight", typeof(int), typeof(BoardTilesControl), new PropertyMetadata(49));

        public int TileHeight
        {
            get { return (int)GetValue(TileHeightProperty); }
            set { SetValue(TileHeightProperty, value); }
        }

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
        void RaiseTileSelectedEvent(BoardTileViewModel tile)
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

            SelectedTile = element.DataContext as BoardTileViewModel;
            RaiseTileSelectedEvent(element.DataContext as BoardTileViewModel);
        }

        private void Tile_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                FrameworkElement element = sender as FrameworkElement;

                SelectedTile = element.DataContext as BoardTileViewModel;
                RaiseTileSelectedEvent(element.DataContext as BoardTileViewModel);
            }
        }
    }
}
