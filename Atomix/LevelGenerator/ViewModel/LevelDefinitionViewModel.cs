using Kinectomix.Logic;
using Kinectomix.LevelEditor.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinectomix.LevelEditor.ViewModel
{
    public class LevelDefinitionViewModel : NotifyPropertyBase
    {
        private LevelDefinition _levelDefinition;

        public string Name
        {
            get { return _levelDefinition.Name; }
            set
            {
                _levelDefinition.Name = value;
                OnPropertyChanged();
            }
        }

        public string FileName
        {
            get { return _levelDefinition.FileName; }
            set
            {
                _levelDefinition.FileName = value;
                OnPropertyChanged();
            }
        }

        public LevelDefinition LevelDefinition
        {
            get { return _levelDefinition; }
            set { _levelDefinition = value; }
        }

        public LevelDefinitionViewModel()
        {
            _levelDefinition = new LevelDefinition();
        }

        public LevelDefinitionViewModel(LevelDefinition definition)
        {
            _levelDefinition = definition;
        }
    }
}
