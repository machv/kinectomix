using AtomixData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinectomix.LevelGenerator
{
    public class BoardViewModel : Mvvm.NotifyPropertyBase
    {
        ObservableCollection<BoardTileViewModel> _tiles = new ObservableCollection<BoardTileViewModel>();

        public ObservableCollection<BoardTileViewModel> Tiles
        {
            get { return _tiles; }
            set
            {
                _tiles = value;

                RaisePropertyChangedEvent("Tiles");
            }
        }
    }
}
