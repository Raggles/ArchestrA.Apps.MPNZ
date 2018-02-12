using ArchestrA.Client.RuntimeData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ArchestrA.Apps.AttributeBrowser2
{
    public class aaObject
    {
        public string Name { get; set; }
        public List<aaAttribute> Attributes { get; set; }
        public ObservableCollection<aaAttribute> FilteredAttributes { get; set; }
        //public List<DataReferenceSource> Sources { get; set; } = new List<DataReferenceSource>();
        //public List<DataReference> References { get; set; } = new List<DataReference>();
        public List<DataItem> Items { get; set; } = new List<DataItem>(); //currently subscribed items
        public DataSubscription Subscription { get; set; }
        //enum filter
             
        public void GetAttributes()
        {
            DataItem d = new DataItem()
            {
                ReferenceString = "_Attributes[]",
                OwningObject = Name
            };
            DataReferenceSource r = new DataReferenceSource(
                "_Attributes[]",
                Name,
                (i) =>
                {
                    string[] attributes = (string[])i.VTQ.Value.Array;
                    DealWithNewAttributes(attributes.ToList<string>());
                }
                );
        }

        private void DealWithNewAttributes(List<string> attributes)
        {
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
                Attributes.Add(new aaAttribute(kvp.Key, kvp.Value));
            }
            ApplyFilter();
            Subscribe();

        }

        public void ApplyFilter()
        {

        }

        public void Subscribe()
        {
            foreach (var a in FilteredAttributes)
            {
                Items.AddRange(a.DataItems);
            }

            var dataSources = new List<DataReferenceSource>();
            foreach (var dataItem in Items)
            {
                // Popuplate the list of DataReferenceSource
                // The call back action of each source will directly update the data item collection to update the UI
                dataSources.Add(
                    new DataReferenceSource(
                        dataItem.ReferenceString,
                        dataItem.OwningObject,
                        (d) =>
                        {
                            dataItem.DataReference = d;
                            dataItem.DataValue = d.VTQ.Value.ValueAsString;
                            dataItem.StatusSetting = d.VTQ.QualityStatus.StatusSetting;
                            dataItem.MxQuality = d.VTQ.QualityStatus.MxQuality;
                            dataItem.OpcUaQuality = d.VTQ.QualityStatus.OPCUAQuality;
                            dataItem.Timestamp = d.VTQ.Timestamp;
                        }));
            }

            // Call Runtime Data SDK API to subscribe all data references in bulk
            Subscription.Subscribe(dataSources.ToArray());
        }

        public void Unsubscribe()
        {
            //var result = Items.Select(i => i.DataReference).ToArray();
            Subscription.UnsubscribeAll();
        }
    }

    public class aaAttribute
    {
        public string Name { get; set; }
        public string Value
        {
            get
            {
                Type t = _value.GetType();
                if (t == typeof(bool))
                {
                    if ((bool)_value)
                    {
                        return OnLabel;
                    }
                    else
                    {
                        return OffLabel;
                    }
                }
                if (t == typeof(int))
                {
                    return _value.ToString() + " " + EngUnit;
                }
                if (t == typeof(float))
                {
                    double v = ((float)_value);
                    return Math.Round(v, 2).ToString() + " " + EngUnit;
                }
                if (t == typeof(double))
                {
                    return Math.Round((double)_value, 2).ToString() + " " + EngUnit;

                }
                return _value.ToString();
            }

            set
            {
                _value = value;
            }
        }

        public string Description { get; set; }
        public int Quality { get; set; }
        public bool InAlarm { get; set; }
        public DateTime TimeStamp { get; set; }
        public aaObject ParentObject { get; set; }
        public string OnLabel { get; set; }
        public string OffLabel { get; set; }
        public Type Type { get; set; }
        public string EngUnit { get; set; }
        private object _value;
        //enum typeflags;
        //TODO: how do we get the object type???

        public aaAttribute(string name, List<string> items)
        {
            OnLabel = "True";
            OffLabel = "False";
            //if items contains InputSource then and so on

            DataItem d = new DataItem();
            d.ReferenceString = "bla";
            d.Purpose = aaLMXValuePurpose.Normal;
            d.OwningObject = ParentObject.Name;
            d.PropertyChanged += DataItem_PropertyChanged;
            DataItems.Add(d);


        }

        private void DataItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var v = sender as DataItem;
            switch (v.Purpose)
            {
                case aaLMXValuePurpose.Description:
                    Description = v.DataValue.ToString();
                    break;
                case aaLMXValuePurpose.EngUnit:
                    EngUnit = v.DataValue.ToString();
                    break;
                case aaLMXValuePurpose.InAlarm:
                    InAlarm = (bool)v.DataValue;
                    break;
                case aaLMXValuePurpose.Normal:
                    _value = v.DataValue;
                    TimeStamp = v.Timestamp;
                    Quality = v.MxQuality;
                    break;
            }
        }

        public void SetValue(object obj)
        {
            _value = obj;
        }

        public List<DataItem> DataItems { get; set; } = new List<DataItem>();


    }

    public enum aaLMXValuePurpose
    {
        AttributeList,
        OnLabel,
        OffLabel,
        Description,
        EngUnit,
        Normal,
        InAlarm,
    }

    /// <summary>
    /// This class holds the data item to be displayed in UI
    /// </summary>
    public class DataItem : INotifyPropertyChanged
    {
        private aaObject _parentObject;
        private aaAttribute _parentAttribute;
        private string referenceString;
        private string owningObject;
        private object dataValue;
        private StatusSettingType statusSetting;
        private int mxQuality;
        private int opcuaQuality;
        private DateTime timestamp;
        private string writeStatus;

        public DataItem()
        {
        }

        public DataItem(string referenceString, string owningObject)
        {
            this.referenceString = referenceString;
            this.owningObject = owningObject;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public aaLMXValuePurpose Purpose { get; set; }
        public bool OnceOnly { get; set; }

        /// <summary>
        /// Gets or sets the reference string of the data reference
        /// </summary>
        public string ReferenceString
        {
            get
            {
                return this.referenceString;
            }

            set
            {
                this.referenceString = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the owning object of the data reference. Can be empty
        /// </summary>
        public string OwningObject
        {
            get
            {
                return this.owningObject;
            }

            set
            {
                this.owningObject = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the runtime data value
        /// </summary>
        public object DataValue
        {
            get
            {
                return this.dataValue;
            }

            set
            {
                this.dataValue = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the data status of the data reference
        /// </summary>
        public StatusSettingType StatusSetting
        {
            get
            {
                return this.statusSetting;
            }

            set
            {
                this.statusSetting = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the MxQuality of the data reference
        /// </summary>
        public int MxQuality
        {
            get
            {
                return this.mxQuality;
            }

            set
            {
                this.mxQuality = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the OPC UA quality of the data reference
        /// </summary>
        public int OpcUaQuality
        {
            get
            {
                return this.opcuaQuality;
            }

            set
            {
                this.opcuaQuality = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the timestamp of the data reference
        /// </summary>
        public DateTime Timestamp
        {
            get
            {
                return this.timestamp;
            }

            set
            {
                this.timestamp = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the write status of the data reference
        /// </summary>
        public string WriteStatus
        {
            get
            {
                return this.writeStatus;
            }

            set
            {
                this.writeStatus = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the DataReference instance which is returned from Runtime Data SDK api
        /// </summary>
        public DataReference DataReference { get; set; }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}