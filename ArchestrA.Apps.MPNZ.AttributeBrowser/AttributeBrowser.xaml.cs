using ArchestrA.Client.RuntimeData;
using ArchestrA.Diagnostics;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ArchestrA.Apps.AttributeBrowser
{
    public partial class AttributeBrowser : UserControl, IRuntimeDataClient
    {
        private long _lastResizeTime = 0;
        public static readonly DependencyProperty ObjectSourceProperty = DependencyProperty.Register("ObjectSource", typeof(string), typeof(AttributeBrowser), new FrameworkPropertyMetadata( ObjectSourceChanged));
        private bool _assetSet = false;
        
        public static void ObjectSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj == null || args.NewValue == null)
                return;
            ((AttributeBrowser)obj).ChangeName(args.NewValue.ToString());
        }

        public string ObjectSource
        {
            get { return (string)this.GetValue(ObjectSourceProperty); }
            set { this.SetValue(ObjectSourceProperty, value); }
        }

        public bool UseCurrentAsset { get; set; } = false;
        public bool UpdateOnSourceChanged { get; set; } = false;
        
        public void SetAsset()
        {
            if (ObjectSource != "")
                _assetSet = true;
        }

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

        public AttributeBrowser()
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
            if (UseCurrentAsset)
                _model.Name = Client.MyViewApp.Navigation.CurrentAsset;
            else
                _model.Name = ObjectSource;
            _model.DataChanged += _model_DescriptionUpdated;
            _model.SubscribeAttributes();
        }

        private void _model_DescriptionUpdated()
        {
            Task ignoredAwaitableResult = AutoSizeListView();
        }

        private void Navigation_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (UseCurrentAsset)
            {
                if (!string.IsNullOrEmpty(Client.MyViewApp.Navigation.CurrentAsset))
                {
                    ChangeName(Client.MyViewApp.Navigation.CurrentAsset);
                }
            }
        }

        public void ChangeName(string name)
        {
            if (UpdateOnSourceChanged || !_assetSet)
            _model.Unsubscribe();
            _model.Name = name;
            _model.SubscribeAttributes();
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
                _model.Unsubscribe();
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

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //Logger.LogInfo(() => string.Format("Visibility change to {0}", (e.NewValue).ToString()));
        }
    }
}
