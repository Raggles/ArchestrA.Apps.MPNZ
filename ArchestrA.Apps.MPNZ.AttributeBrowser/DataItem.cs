using ArchestrA.Client.RuntimeData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ArchestrA.Apps.MPNZ.AttributeBrowserApp
{
    /// <summary>
    /// An individual unit of data
    /// </summary>
    public class DataItem : INotifyPropertyChanged
    {
        private string referenceString;
        private string owningObject;
        private object dataValue;
        private StatusSettingType statusSetting;
        private int mxQuality;
        private int opcuaQuality;
        private DateTime timestamp;
        private string writeStatus;

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

        /// <summary>
        /// What the purpose of this data item is, eg engunit, attribute value, inalarm etc
        /// </summary>
        public DataItemPurpose Purpose { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public DataItem() { }
    
        public DataItem(string referenceString, string owningObject)
        {
            this.referenceString = referenceString;
            this.owningObject = owningObject;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
