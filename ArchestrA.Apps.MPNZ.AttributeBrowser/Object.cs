using ArchestrA.Client.RuntimeData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Threading;

namespace ArchestrA.Apps.MPNZ.AttributeBrowserApp
{
    /// <summary>
    /// An ArchestrA object
    /// </summary>
    public class Object : INotifyPropertyChanged
    {
        private Dispatcher _d;//we need a dispatcher to update the observable collection, and for some reason Application.Current.Dispatcher doesn't work
        private List<Attribute> Attributes { get; set; } = new List<Attribute>();
        private List<DataItem> Items { get; set; } = new List<DataItem>(); //currently subscribed items

        /// <summary>
        /// The name of the object
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The attributes to show in the listview
        /// </summary>
        public ObservableRangeCollection<Attribute> FilteredAttributes { get; set; }

        /// <summary>
        /// Data subscription to be provided by the control
        /// </summary>
        public DataSubscription DataSubscription { get; set; }

        /// <summary>
        /// Filter options
        /// </summary>
        public Filter FilterOptions { get; set; } = Filter.HasInput | Filter.IsCalculation | Filter.IsProcessValue | Filter.IsStatus;

        public event PropertyChangedEventHandler PropertyChanged;
        
        public event Action DataChanged;

        /// <summary>
        /// Fetch all the attributes and subscribe according to the filter
        /// </summary>
        public void SubscribeAttributes()
        {
            //just in case we are resubscribing, clear everything
            _d.Invoke(() => FilteredAttributes.Clear());
            Attributes.Clear();
            DataReferenceSource r = new DataReferenceSource(
                Name + "._Attributes[]",
                Name,
                (i) =>
                {
                    //we need to start this on a new thread otherwise we can't make other calls to the RuntimeDataClient
                    string[] attributes = (string[])i.VTQ.Value.Array;
                    new Thread(() =>
                    {
                        Thread.CurrentThread.IsBackground = true;
                        DealWithNewAttributes(attributes.ToList<string>());
                    }).Start();

                }
                );
            //just in case
            if (DataSubscription != null)
                DataSubscription.Subscribe(new DataReferenceSource[] { r });
        }

        /// <summary>
        /// Sort the list of strings and convert it into Attribute objects
        /// </summary>
        /// <param name="attributes">The value of an objects _Attributes[] attribute</param>
        private void DealWithNewAttributes(List<string> attributes)
        {
            //unsubscribe all previous subscriptions
            DataSubscription.UnsubscribeAll();
            attributes.Sort();
            Dictionary<string, List<string>> uniqueAttributes = new Dictionary<string, List<string>>();
            string previous = "zoogabooga";//I bet nobody will call an attribute this
            List<string> currentList = new List<string>();
            for (int i = 0; i < attributes.Count; i++)
            {
                if (attributes[i].StartsWith(previous))
                {
                    currentList.Add(attributes[i]);
                }
                else
                {
                    currentList = new List<string>();
                    uniqueAttributes.Add(attributes[i], currentList);
                    previous = attributes[i];
                }

            }
            foreach (var kvp in uniqueAttributes)
            {
                Attribute a = new Attribute(kvp.Key, kvp.Value, this);
                Attributes.Add(a);
                a.PropertyChanged += Attribute_PropertyChanged;
            }
            //Filter the attributes on the GUI dispatcher
            _d.Invoke(() =>
            {
                FilteredAttributes.Clear();
                var results = from i in Attributes where (i.Flags & FilterOptions) != 0 select i;
                FilteredAttributes.AddRange(results);
            });

            //subscribe all the filtered Attributes DataItems
            foreach (var a in FilteredAttributes)
            {
                Items.AddRange(a.DataItems);
            }

            var dataSources = new List<DataReferenceSource>();
            foreach (var dataItem in Items)
            {
                dataSources.Add(
                    new DataReferenceSource(
                        dataItem.ReferenceString,
                        dataItem.OwningObject,
                        (d) =>
                        {
                            dataItem.DataReference = d;
                            dataItem.DataValue = d.VTQ.Value.RawValue;
                            dataItem.StatusSetting = d.VTQ.QualityStatus.StatusSetting;
                            dataItem.MxQuality = d.VTQ.QualityStatus.MxQuality;
                            dataItem.OpcUaQuality = d.VTQ.QualityStatus.OPCUAQuality;
                            dataItem.Timestamp = d.VTQ.Timestamp;
                        }));
            }
            DataSubscription.Subscribe(dataSources.ToArray());
        }

        /// <summary>
        /// Trigger an event on data change in case the UI wants to do something
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Attribute_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            DataChanged?.Invoke();
        }

        public void Unsubscribe()
        {
            try
            {
                //var result = Items.Select(i => i.DataReference).ToArray();
                DataSubscription.UnsubscribeAll();
                Items.Clear();
                _d.Invoke(() => FilteredAttributes.Clear());
            }
            catch { }
        }

        private void OnProptertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public Object()
        {
            try
            {
                //for some reason, the Application.Current.Dispatcher is not the thread that the GUI is running on.  so when we are created, we will assume that 
                //whatever the current dispatcher is is the one we want to use.  Seems to work.
                _d = Dispatcher.CurrentDispatcher;
                FilteredAttributes = new ObservableRangeCollection<Attribute>();
            }
            catch { }
        }

    }
}
