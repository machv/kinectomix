using KinectomixLogic;
using Kinectomix.LevelGenerator.ViewModel;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;

namespace Kinectomix.LevelGenerator
{
    public class BondsAdorner : Adorner
    {
        private VisualCollection _visualChildren;
        private FrameworkElement _content;

        public BondsAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            string buttonTemplateXml =
@"<ControlTemplate
    xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
    xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
    TargetType='{x:Type Button}'>
    <Ellipse Width='7' Height='7' Fill='Black' Opacity='0.8' />
</ControlTemplate>";
            ControlTemplate buttonTemplate;
            using (StringReader stringReader = new StringReader(buttonTemplateXml))
            using (XmlReader xmlReader = XmlReader.Create(stringReader))
                buttonTemplate = (ControlTemplate)XamlReader.Load(xmlReader);

            _visualChildren = new VisualCollection(this);
            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            var btn = new Button();
            btn.Click += Btn_Click;
            btn.Template = buttonTemplate;
            btn.Tag = "TopLeft";
            Grid.SetRow(btn, 0);
            Grid.SetColumn(btn, 0);
            grid.Children.Add(btn);

            btn = new Button();
            btn.Click += Btn_Click;
            btn.Template = buttonTemplate;
            btn.Tag = "Top";
            Grid.SetRow(btn, 0);
            Grid.SetColumn(btn, 1);
            grid.Children.Add(btn);

            btn = new Button();
            btn.Click += Btn_Click;
            btn.Template = buttonTemplate;
            btn.Tag = "TopRight";
            Grid.SetRow(btn, 0);
            Grid.SetColumn(btn, 2);
            grid.Children.Add(btn);

            btn = new Button();
            btn.Click += Btn_Click;
            btn.Template = buttonTemplate;
            btn.Tag = "Left";
            Grid.SetRow(btn, 1);
            Grid.SetColumn(btn, 0);
            grid.Children.Add(btn);

            btn = new Button();
            btn.Click += Btn_Click;
            btn.Template = buttonTemplate;
            btn.Tag = "Right";
            Grid.SetRow(btn, 1);
            Grid.SetColumn(btn, 2);
            grid.Children.Add(btn);

            btn = new Button();
            btn.Click += Btn_Click;
            btn.Template = buttonTemplate;
            btn.Tag = "BottomLeft";
            Grid.SetRow(btn, 2);
            Grid.SetColumn(btn, 0);
            grid.Children.Add(btn);

            btn = new Button();
            btn.Click += Btn_Click;
            btn.Template = buttonTemplate;
            btn.Tag = "Bottom";
            Grid.SetRow(btn, 2);
            Grid.SetColumn(btn, 1);
            grid.Children.Add(btn);

            btn = new Button();
            btn.Click += Btn_Click;
            btn.Template = buttonTemplate;
            btn.Tag = "BottomRight";
            Grid.SetRow(btn, 2);
            Grid.SetColumn(btn, 2);
            grid.Children.Add(btn);

            _content = grid;
            _visualChildren.Add(_content);
        }

        protected override Visual GetVisualChild(int index)
        {
            return _visualChildren[index];
        }

        protected override int VisualChildrenCount
        {
            get { return _visualChildren.Count; }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            return AdornedElement.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _content.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            return _content.RenderSize;
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            var element = AdornedElement as FrameworkElement;
            var tile = element.DataContext as BoardTileViewModel;
            switch ((sender as Button).Tag as string)
            {
                case "TopLeft":
                    tile.TopLeftBond = RotateBond(tile.TopLeftBond);
                    break;
                case "Top":
                    tile.TopBond = RotateBond(tile.TopBond);
                    break;
                case "TopRight":
                    tile.TopRightBond = RotateBond(tile.TopRightBond);
                    break;
                case "Right":
                    tile.RightBond = RotateBond(tile.RightBond);
                    break;
                case "BottomRight":
                    tile.BottomRightBond = RotateBond(tile.BottomRightBond);
                    break;
                case "Bottom":
                    tile.BottomBond = RotateBond(tile.BottomBond);
                    break;
                case "BottomLeft":
                    tile.BottomLeftBond = RotateBond(tile.BottomLeftBond);
                    break;
                case "Left":
                    tile.LeftBond = RotateBond(tile.LeftBond);
                    break;
                default:
                    throw new IndexOutOfRangeException("Specified bond direction is not valid.");
                    break;
            }
        }

        private BondType RotateBond(BondType currentBond)
        {
            int current = (int)currentBond;
            current = ++current % Enum.GetNames(typeof(BondType)).Length;

            return (BondType)current;
        }
    }
}
