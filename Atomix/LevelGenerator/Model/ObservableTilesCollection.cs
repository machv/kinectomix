using AtomixData;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Kinectomix.LevelGenerator.Model
{
    public class ObservableTilesCollection<T> : TilesCollection<T>, INotifyCollectionChanged, INotifyPropertyChanged where T : new()
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChangedEvent([CallerMemberName] string propertyName = "")
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void OnCollectionChanged()
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
                CollectionChanged(this, e);
        }

        public override int ColumnsCount
        {
            get { return base.ColumnsCount; }
            set
            {
                base.ColumnsCount = value;
                RaisePropertyChangedEvent();
            }
        }

        public override int RowsCount
        {
            get { return base.RowsCount; }
            set
            {
                base.RowsCount = value;
                RaisePropertyChangedEvent();
            }
        }

        public override T this[int row, int column]
        {
            get
            {
                return base[row, column];
            }

            set
            {
                base[row, column] = value;
                NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value);
                OnCollectionChanged(e);
            }
        }


        public ObservableTilesCollection() : base() { }
        public ObservableTilesCollection(int rows, int columns) : base(rows, columns) { }

        public override void Add(T item)
        {
            base.Add(item);

            NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item);
            OnCollectionChanged(e);
        }

        public override void Clear()
        {
            base.Clear();

            OnCollectionChanged();
        }

        public override void InsertColumn(int columnIndex)
        {
            base.InsertColumn(columnIndex);

            RaisePropertyChangedEvent("ColumnsCount");
            OnCollectionChanged();
        }

        public override void InsertRow(int rowIndex)
        {
            base.InsertRow(rowIndex);

            RaisePropertyChangedEvent("RowsCount");
            OnCollectionChanged();
        }

        public override void RemoveColumn(int columnIndex)
        {
            base.RemoveColumn(columnIndex);

            RaisePropertyChangedEvent("ColumnsCount");
            OnCollectionChanged();
        }

        public override void RemoveRow(int rowIndex)
        {
            base.RemoveRow(rowIndex);

            RaisePropertyChangedEvent("RowsCount");
            OnCollectionChanged();
        }
    }
}
