using Mach.Kinectomix.Logic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Kinectomix.LevelEditor.Model
{
    public class ObservableTilesCollection<T> : TilesCollection<T>, INotifyCollectionChanged, INotifyPropertyChanged where T : new()
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        protected void OnCollectionChanged()
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
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
                OnPropertyChanged();
            }
        }

        public override int RowsCount
        {
            get { return base.RowsCount; }
            set
            {
                base.RowsCount = value;
                OnPropertyChanged();
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
                T previousValue = base[row, column];
                int index = row * ColumnsCount + column;

                base[row, column] = value;
                NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, previousValue, index);
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

            OnPropertyChanged("ColumnsCount");
            OnCollectionChanged();
        }

        public override void InsertRow(int rowIndex)
        {
            base.InsertRow(rowIndex);

            OnPropertyChanged("RowsCount");
            OnCollectionChanged();
        }

        public override void RemoveColumn(int columnIndex)
        {
            base.RemoveColumn(columnIndex);

            OnPropertyChanged("ColumnsCount");
            OnCollectionChanged();
        }

        public override void RemoveRow(int rowIndex)
        {
            base.RemoveRow(rowIndex);

            OnPropertyChanged("RowsCount");
            OnCollectionChanged();
        }
    }
}
