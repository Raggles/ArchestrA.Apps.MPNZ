using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ArchestrA.HistClient.UI;
using ArchestrA.HistClient.Database;

namespace ArchestrA.Apps.MPNZ.TrendApp
{
    /// <summary>
    /// Interaction logic for MPNZTrend.xaml
    /// </summary>
    public partial class MPNZTrend : UserControl
    {
        public static readonly DependencyProperty ObjectSourceProperty = DependencyProperty.Register("ObjectSource", typeof(string), typeof(MPNZTrend), new FrameworkPropertyMetadata(ObjectSourceChanged));
        private bool _assetSet = false;

        public static void ObjectSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj == null || args.NewValue == null)
                return;
            if ((string)args.NewValue != "")
                ((MPNZTrend)obj).UpdateGraph();
        }

        private void UpdateGraph()
        {
            if (_assetSet == false)
            {
                //TODO: we can apply different tags, eg if the tagname contains RTU, ION, Relay and so on
                _trend.SetDuration(System.DateTime.FromOADate(31.0));
                _trend.TagPicker.Width = 300;
                _trend.GraphStack();

                _trend.TagPicker.TagNameFilter = ObjectSource + ".Current";
                _trend.TagPicker.ApplyFilter();
                _trend.TagPicker.SelectAll();
                foreach (aaTag a in _trend.TagPicker.SelectedTags)
                {
                    if (!a.Name.Contains("THD"))
                        _trend.AddAnyTag(a.Server.Name, a.Name);
                }

                _trend.TagPicker.TagNameFilter = ObjectSource + ".Voltage";
                _trend.TagPicker.ApplyFilter();
                _trend.TagPicker.SelectAll();
                foreach (aaTag a in _trend.TagPicker.SelectedTags)
                {
                    if (!a.Name.Contains("THD"))
                        _trend.AddAnyTag(a.Server.Name, a.Name);
                }

                _trend.ScaleAutoAllTags();
                //reset the filter to the object in case the user wants to add other thigns
                _trend.TagPicker.TagNameFilter = ObjectSource + ".";
                _trend.TagPicker.ApplyFilter();
            }
            _assetSet = true;
        }

        public string ObjectSource
        {
            get { return (string)this.GetValue(ObjectSourceProperty); }
            set { this.SetValue(ObjectSourceProperty, value); }
        }

        private aaTrendControl _trend;

        public MPNZTrend()
        {
            InitializeComponent();

            WindowsFormsHost host = new WindowsFormsHost();
            _trend = new aaTrendControl();
            _trend.TagPicker.FilterVisible = true;
            
            host.Child = _trend;
            this.grid1.Children.Add(host);
        }
    }
}
