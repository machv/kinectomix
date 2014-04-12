using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinectomix.LevelGenerator.ViewModel
{
    public class TilesViewModel : Mvvm.NotifyPropertyBase
    {
        ObservableCollection<BoardTileViewModel> _atoms = new ObservableCollection<BoardTileViewModel>();

        public int ColumnsCount
        {
            get;
            set;
        }

        public int RowsCount
        {
            get;
            set;
        }

        public ObservableCollection<BoardTileViewModel> Tiles
        {
            get { return _atoms; }
            set
            {
                _atoms = value;

                RaisePropertyChangedEvent("Tiles");
            }
        }
    }
}
