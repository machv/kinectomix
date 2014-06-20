using Kinectomix.LevelEditor.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Kinectomix.LevelEditor.Visual
{
    class DragableLevelsListBox : ListBox
    {
        public const string DragDataDataName = "SelectedLevelItem";

        private Point _startPoint;
        private DragAdorner _adorner;
        private AdornerLayer _layer;
        private bool _dragIsOutOfScope = false;

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            AllowDrop = true;
            DragEnter += ListBoxDragEnter;
            Drop += ListBoxDrop;
            PreviewMouseMove += ListBoxPreviewMouseMove;
            PreviewMouseLeftButtonDown += ListBoxMouseLeftButtonDown;
        }

        private void ListBoxMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
        }

        private void ListBoxPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point position = e.GetPosition(null);

                if (Math.Abs(position.X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    BeginDrag(e);
                }
            }
        }

        private void BeginDrag(MouseEventArgs e)
        {
            ListBox ListBox = this;//.ListBox;
            ListBoxItem ListBoxItem =
                FindAnchestor<ListBoxItem>((DependencyObject)e.OriginalSource);

            if (ListBoxItem == null)
                return;

            // get the data for the ListBoxItem
            LevelViewModel item = (LevelViewModel)ListBox.ItemContainerGenerator.ItemFromContainer(ListBoxItem);

            //setup the drag adorner.
            InitialiseAdorner(ListBoxItem);

            //add handles to update the adorner.
            ListBox.PreviewDragOver += ListBoxDragOver;
            ListBox.DragLeave += ListBoxDragLeave;
            ListBox.DragEnter += ListBoxDragEnter;

            DataObject data = new DataObject(DragDataDataName, item);
            DragDropEffects de = DragDrop.DoDragDrop(this, data, DragDropEffects.Move);

            //cleanup 
            ListBox.PreviewDragOver -= ListBoxDragOver;
            ListBox.DragLeave -= ListBoxDragLeave;
            ListBox.DragEnter -= ListBoxDragEnter;

            if (_adorner != null)
            {
                AdornerLayer.GetAdornerLayer(ListBox).Remove(_adorner);
                _adorner = null;
            }
        }

        private void ListBoxDragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DragDataDataName) || sender == e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }


        private void ListBoxDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DragDataDataName))
            {
                LevelViewModel item = (LevelViewModel)e.Data.GetData(DragDataDataName);
                ListBoxItem ListBoxItem = FindAnchestor<ListBoxItem>((DependencyObject)e.OriginalSource);

                if (ListBoxItem != null)
                {
                    LevelViewModel itemToReplace = (LevelViewModel)ItemContainerGenerator.ItemFromContainer(ListBoxItem);
                    int index = Items.IndexOf(itemToReplace);

                    if (index >= 0)
                    {
                        (ItemsSource as ObservableCollection<LevelViewModel>).Remove(item);
                        (ItemsSource as ObservableCollection<LevelViewModel>).Insert(index, item);
                        SelectedIndex = index;
                    }
                }
                else
                {
                    (ItemsSource as ObservableCollection<LevelViewModel>).Remove(item);
                    (ItemsSource as ObservableCollection<LevelViewModel>).Add(item);
                    SelectedIndex = Items.Count - 1;
                }
            }
        }

        private void InitialiseAdorner(ListBoxItem ListBoxItem)
        {
            VisualBrush brush = new VisualBrush(ListBoxItem);
            _adorner = new DragAdorner(ListBoxItem, ListBoxItem.RenderSize, brush);
            _adorner.Opacity = 0.5;
            _layer = AdornerLayer.GetAdornerLayer(this as System.Windows.Media.Visual);
            _layer.Add(_adorner);
        }


        void ListBoxQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (_dragIsOutOfScope)
            {
                e.Action = DragAction.Cancel;
                e.Handled = true;
            }
        }


        void ListBoxDragLeave(object sender, DragEventArgs e)
        {
            if (e.OriginalSource == this)
            {
                Point p = e.GetPosition(this);
                Rect r = VisualTreeHelper.GetContentBounds(this);
                if (!r.Contains(p))
                {
                    _dragIsOutOfScope = true;
                    e.Handled = true;
                }
            }
        }

        void ListBoxDragOver(object sender, DragEventArgs args)
        {
            if (_adorner != null)
            {
                _adorner.OffsetLeft = args.GetPosition(this).X;
                _adorner.OffsetTop = args.GetPosition(this).Y - _startPoint.Y;
            }
        }

        // Helper to search up the VisualTree
        private static T FindAnchestor<T>(DependencyObject current)
            where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }
    }
}
