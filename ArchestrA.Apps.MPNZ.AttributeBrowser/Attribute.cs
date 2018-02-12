using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ArchestrA.Apps.MPNZ.AttributeBrowserApp
{
    /// <summary>
    /// Represents an ArchestrA attribute.
    /// </summary>
    public class Attribute : INotifyPropertyChanged
    {
        private string _name;
        private string _description;
        private int _quality;
        private bool _inAlarm;
        private DateTime _timeStamp;
        private Object _parentObject;
        private Type _type = typeof(string);
        private string _engUnit;
        private object _value = "";

        /// <summary>
        /// The name of the attribute
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnProptertyChanged();
            }
        }

        /// <summary>
        /// The value of the attribute as a string
        /// </summary>
        public string Value
        {
            get
            {
                Type t = _value.GetType();
                if (t == typeof(bool))
                {
                    return _value.ToString();
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
                OnProptertyChanged();
            }
        }

        /// <summary>
        /// The description of the attribute
        /// </summary>
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                OnProptertyChanged();
            }
        }

        /// <summary>
        /// The MxQuality of the value
        /// </summary>
        public int MxQuality
        {
            get
            {
                return _quality;
            }
            set
            {
                _quality = value;
                OnProptertyChanged();
            }
        }

        /// <summary>
        /// Whether the attribute is in alarm or not.  For Analog attributes this means any alarm (LoLo, Lo, Hi, HiHi)
        /// </summary>
        public bool InAlarm
        {
            get
            {
                return _inAlarm;
            }
            set
            {
                _inAlarm = value;
                OnProptertyChanged();
            }
        }

        /// <summary>
        /// The time stamp of the last recieved value
        /// </summary>
        public DateTime TimeStamp
        {
            get
            {
                return _timeStamp;
            }
            set
            {
                _timeStamp = value;
                OnProptertyChanged();
            }
        }
        
        /// <summary>
        /// The object that the attribute belongs to
        /// </summary>
        private Object ParentObject
        {
            get
            {
                return _parentObject;
            }
            set
            {
                _parentObject = value;
                OnProptertyChanged();
            }
        }
        
        /// <summary>
        /// The type of the value data
        /// </summary>
        private Type Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
                OnProptertyChanged();
            }
        }

        /// <summary>
        /// The Engineering Unit for the attribute
        /// </summary>
        private string EngUnit
        {
            get
            {
                return _engUnit;
            }
            set
            {
                _engUnit = value;
                OnProptertyChanged();
            }
        }

        /// <summary>
        /// This is for debugging purposes
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            try
            {
                return Name + ":" + Value;
            }
            catch
            {
                return base.ToString();
            }
        }

        /// <summary>
        /// The flags for the object to use for filtering
        /// </summary>
        public Filter Flags { get; set; } = 0;

        /// <summary>
        /// The associated data items for the attribute
        /// </summary>
        public List<DataItem> DataItems { get; set; } = new List<DataItem>();

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Constructor - Given a list of sub attributes, determines what kind of attribute this is eg input, output, has alarms, engineering units and so on
        /// </summary>
        /// <param name="name">The name of the attribute</param>
        /// <param name="items">A list of all the sub-attributes that belong to this attribute</param>
        /// <param name="parent">The parent object for the attribute</param>
        public Attribute(string name, List<string> items, Object parent)
        {
            Name = name;
            ParentObject = parent;
            string a;
            if (items.Contains(name + ".InputSource"))
            {
                Flags = Flags | Filter.HasInput;
            }
            if (items.Contains(name + ".OutputDest"))
            {
                Flags = Flags | Filter.HasOutput;
            }
            if (name.StartsWith("Calc."))
            {
                Flags = Flags | Filter.IsCalculation;
            }
            if (name.EndsWith(".PV"))
            {
                Flags = Flags | Filter.IsProcessValue;
            }
            if (name.StartsWith("Status."))
            {
                Flags = Flags | Filter.IsStatus;
            }
            if (name.StartsWith("Cfg."))
            {
                Flags = Flags | Filter.IsConfiguration;
            }
            if (name.StartsWith("Cmd."))
            {
                Flags = Flags | Filter.IsCommand;
            }
            if (name.StartsWith("_"))
            {
                Flags = Flags | Filter.IsHidden;
            }
            if (items.Contains(a = name + ".Description"))
            {
                AddDataItem(ParentObject.Name + "." + a, DataItemPurpose.Description);
            }
            if (items.Contains(a = name + ".Msg"))
            {
                AddDataItem(ParentObject.Name + "." + a, DataItemPurpose.Value);
                AddDataItem(ParentObject.Name + "." + name, DataItemPurpose.Status);
            }
            else
            {
                AddDataItem(ParentObject.Name + "." + name, DataItemPurpose.StatusAndValue);
            }
            if (items.Contains(a = name + ".EngUnits"))
            {
                AddDataItem(ParentObject.Name + "." + a, DataItemPurpose.EngUnit);
            }
            if (items.Contains(a = name + ".InAlarm"))
            {
                AddDataItem(ParentObject.Name + "." + a, DataItemPurpose.InAlarm);
            }
            if (items.Contains(a = name + ".AlarmMostUrgentInAlarm"))
            {
                AddDataItem(ParentObject.Name + "." + a, DataItemPurpose.InAlarm);
            }
        }

        /// <summary>
        /// Adds a data item (for subscribing later), with the specified purpose
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="purpose"></param>
        private void AddDataItem(string reference, DataItemPurpose purpose)
        {
            DataItem d = new DataItem();
            d.ReferenceString = reference;
            d.Purpose = purpose;
            d.OwningObject = ParentObject.Name;
            d.PropertyChanged += DataItem_PropertyChanged;
            DataItems.Add(d);
        }

        /// <summary>
        /// Update the Attribute values when a DataItem changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var v = sender as DataItem;
            if (v.DataValue == null)
                return;
            switch (v.Purpose)
            {
                case DataItemPurpose.Description:
                    Description = v.DataValue.ToString();
                    break;
                case DataItemPurpose.EngUnit:
                    EngUnit = v.DataValue.ToString();
                    break;
                case DataItemPurpose.InAlarm:
                    InAlarm = (bool)v.DataValue;
                    break;
                case DataItemPurpose.Value:
                    Type = v.DataValue.GetType();
                    _value = v.DataValue;
                    OnProptertyChanged(nameof(Value));
                    TimeStamp = v.Timestamp;
                    break;
                case DataItemPurpose.StatusAndValue:
                    Type = v.DataValue.GetType();
                    _value = v.DataValue;
                    OnProptertyChanged(nameof(Value));
                    TimeStamp = v.Timestamp;
                    MxQuality = v.MxQuality;
                    break;
                case DataItemPurpose.Status:
                    MxQuality = v.MxQuality;
                    break;
            }
        }

        private void OnProptertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
