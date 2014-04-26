﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

// http://www.markwithall.com/programming/2013/03/01/worlds-simplest-csharp-wpf-mvvm-example.html
namespace Kinectomix.LevelGenerator.Mvvm
{
    public class NotifyPropertyBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChangedEvent([CallerMemberName] string propertyName = "")
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
