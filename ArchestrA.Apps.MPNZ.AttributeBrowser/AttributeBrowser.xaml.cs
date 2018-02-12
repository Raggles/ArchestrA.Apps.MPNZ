using ArchestrA.Client.RuntimeData;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ArchestrA.Apps.AttributeBrowser
{
    public partial class aaAttributeBrowserApp2 : UserControl, IRuntimeDataClient
    {
        private long _lastResizeTime = 0;
        
        private DataSubscription _dataSubscription;

        /// <summary>
        /// Gets or sets the DataSubscription instance
        /// Framework will set the value when the control is loaded in View
        /// </summary>
        public DataSubscription DataSubscription
        {
            get
            {
                return _dataSubscription;
            }

            set
            {
                _dataSubscription = value;
                // Push the DataSubscription to view model
                _model.DataSubscription = value;
            }
        }

        public aaAttributeBrowserApp2()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                ArchestrA.Client.MyViewApp.Navigation.PropertyChanged += Navigation_PropertyChanged;
            }
            catch { }
            _model.Name  = Client.MyViewApp.Navigation.CurrentAsset;
            _model.DataChanged += _model_DescriptionUpdated;
            _model.SubscribeAttributes();
        }

        private void _model_DescriptionUpdated()
        {
            Task ignoredAwaitableResult = AutoSizeListView();
        }

        private void Navigation_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Client.MyViewApp.Navigation.CurrentAsset))
            {
                _model.Unsubscribe();
                _model.Name =  Client.MyViewApp.Navigation.CurrentAsset;
                _model.SubscribeAttributes();
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                try
                {
                    ArchestrA.Client.MyViewApp.Navigation.PropertyChanged -= Navigation_PropertyChanged;
                }
                catch { }
                //it seems that if we unsubscribe after the control is closed then its too late
                //_model.Unsubscribe();
            }
            catch { }
        }
        
        private async Task AutoSizeListView()
        {
            //this is a dirt hack - need to work out how to do it properly
            if (DateTime.Now.Ticks - _lastResizeTime < 10000 * 500)
                return;
            await Task.Delay(100);
            Dispatcher.Invoke(() =>
            {
                GridViewColumn desc = null;
                double sum = 0;
                foreach (GridViewColumn c in gridView.Columns)
                {
                    var ch = c.Header as GridViewColumnHeader;
                    if (ch?.Content?.ToString() == "Description")
                    {
                        desc = c;
                    }
                    else if (c.Width.Equals(double.NaN))
                    {
                        c.Width = c.ActualWidth;
                        c.Width = double.NaN;
                        sum += c.ActualWidth;
                    }
                    else
                    {
                        sum += c.ActualWidth;
                    }                       
                }
                if (desc != null)
                    desc.Width = listView.ActualWidth - sum - SystemParameters.VerticalScrollBarWidth;               
            });
            //for some reason when we set a column to autosize, there is a delay until is actually resize
            //need to understand this better to tidy this ugly code up
            await Task.Delay(100);
            Dispatcher.Invoke(() =>
            {
                GridViewColumn desc = null;
                double sum = 0;
                foreach (GridViewColumn c in gridView.Columns)
                {
                    var ch = c.Header as GridViewColumnHeader;
                    if (ch?.Content?.ToString() == "Description")
                    {
                        desc = c;
                    }
                    else if (c.Width.Equals(double.NaN))
                    {
                        c.Width = c.ActualWidth;
                        c.Width = double.NaN;
                        sum += c.ActualWidth;
                    }
                    else
                    {
                        sum += c.ActualWidth;
                    }
                }
                if (desc != null)
                    desc.Width = listView.ActualWidth - sum - SystemParameters.VerticalScrollBarWidth;
            });
        }
    }
}
