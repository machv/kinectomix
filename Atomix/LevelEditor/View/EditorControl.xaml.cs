﻿using Kinectomix.Logic;
using Kinectomix.LevelEditor.ViewModel;
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

namespace Kinectomix.LevelEditor.View
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

            Loaded += Control_Loaded;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (AppDomain.CurrentDomain.SetupInformation.ActivationArguments != null && AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData != null && AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData.Any())
            {
                string[] activationData = AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData;
                var uri = new Uri(activationData[0]);

                (DataContext as EditorViewModel).LoadLevelsDefinition(uri.LocalPath);
            }
        }

        private void OnFileDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;

            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
                e.Effects = DragDropEffects.Copy;
        }
        private void OnFileDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);

                (DataContext as EditorViewModel).ImportLevel(paths[0]);
            }
        }
    }
}
