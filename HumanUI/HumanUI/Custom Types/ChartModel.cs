using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanUI.Components
{
    internal class SeriesModel
    {
         public ObservableCollection<ChartItem> Chart { get;  set; }

            public SeriesModel(List<string> categories, List<double> values,string seriesTitle)
            {
                Chart = new ObservableCollection<ChartItem>();
                for (int i = 0; i < categories.Count; i++)
                {
                    Chart.Add(new ChartItem() { Category = categories[i], Number = (float)values[i], ClusterCategory=seriesTitle });
                }
            }

            public SeriesModel(List<string> categories, List<double> values)
            {
                Chart = new ObservableCollection<ChartItem>();
                for (int i = 0; i < categories.Count; i++)
                {
                    Chart.Add(new ChartItem() { Category = categories[i], Number = (float)values[i] });
                }
            }

            public SeriesModel()
            {
                Chart = new ObservableCollection<ChartItem>();
            }
    }

    internal class ChartItem : INotifyPropertyChanged
    {
        public string ClusterCategory { get; set; }
        private string _category;

        public string Category
        {
            get => _category;

            set
            {
                _category = value;
                if (PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Category"));
                }
            }
        }

        private float _number;
        public float Number
        {
            get => _number;

            set
            {
                _number = value;
                if (PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Number"));
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
    }


    internal class MultiChartModel
    {
        public ObservableCollection<SeriesModel> Series { get; set; }
    }
}
