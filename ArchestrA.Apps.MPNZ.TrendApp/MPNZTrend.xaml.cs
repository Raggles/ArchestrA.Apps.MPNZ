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
        aaTrendControl _trend;

        public MPNZTrend()
        {
            InitializeComponent();

            WindowsFormsHost host = new WindowsFormsHost();
            _trend = new aaTrendControl();
            _trend.TagPicker.FilterVisible = true;
            
            host.Child = _trend;
            this.grid1.Children.Add(host);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //TODO: we can apply different tags, eg if the tagname contains RTU, ION, Relay and so on
            _trend.SetDuration(System.DateTime.FromOADate(31.0));
            _trend.TagPicker.Width = 300;
            _trend.GraphStack();

            _trend.TagPicker.TagNameFilter = ArchestrA.Client.MyViewApp.Navigation.CurrentAsset + ".Current";
            _trend.TagPicker.ApplyFilter();
            _trend.TagPicker.SelectAll();
            foreach (aaTag a in _trend.TagPicker.SelectedTags)
            {
                _trend.AddAnyTag(a.Server.Name, a.Name);
            }

            _trend.TagPicker.TagNameFilter = ArchestrA.Client.MyViewApp.Navigation.CurrentAsset + ".Voltage";
            _trend.TagPicker.ApplyFilter();
            _trend.TagPicker.SelectAll();
            foreach (aaTag a in _trend.TagPicker.SelectedTags)
            {
                _trend.AddAnyTag(a.Server.Name, a.Name);
            }

            _trend.ScaleAutoAllTags();
        }
    }
}
