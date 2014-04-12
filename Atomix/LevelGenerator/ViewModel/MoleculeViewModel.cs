using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinectomix.LevelGenerator.ViewModel
{
    public class MoleculeViewModel : Mvvm.NotifyPropertyBase
    {
        ObservableCollection<BoardTileViewModel> _atoms = new ObservableCollection<BoardTileViewModel>();

        public ObservableCollection<BoardTileViewModel> Atoms
        {
            get { return _atoms; }
            set
            {
                _atoms = value;

                RaisePropertyChangedEvent("Atoms");
            }
        }
    }
}
