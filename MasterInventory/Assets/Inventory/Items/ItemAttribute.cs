using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MasterInventory
{
    [System.Serializable]
    public class ItemAttribute
    {
        public string Key = "";
        public bool isStatic = true;
        public bool isRequired = false;

        /// <summary>
        /// Runtime lookup, set by the inventory state, matches a GUID string to item attribute data
        /// </summary>
        public Dictionary<string, ItemAttribute> SerializedAttributeLookup = new Dictionary<string, ItemAttribute>();

        [SerializeField] private DataType dataType = DataType.Float;
        public DataType MyDataType
        {
            get { return dataType; }
            set
            {
                dataType = value;
            }
        }

        public float FloatValue;
        public int IntValue;
        public bool BoolValue;
        public string StringValue;
        public InventoryItem InventoryItemValue;

        public object GetValue(string guid = null)
        {
            if (MyDataType == DataType.Float)
            {
                return isStatic ? FloatValue : SerializedAttributeLookup[guid].FloatValue;
            }
            else if (MyDataType == DataType.Int)
                {
                    return isStatic ? IntValue : SerializedAttributeLookup[guid].IntValue;
            }
            else if (MyDataType == DataType.Bool)
            {
                return isStatic ? BoolValue : SerializedAttributeLookup[guid].BoolValue;
            }
            else if (MyDataType == DataType.String)
            {
                return isStatic ? StringValue : SerializedAttributeLookup[guid].StringValue;
            }
            else if (MyDataType == DataType.InventoryItem)
            {
                return isStatic ? InventoryItemValue : SerializedAttributeLookup[guid].InventoryItemValue;
            }
            return null;
        }
        
        public enum DataType
        {
            Float,
            Int,
            Bool,
            String,
            InventoryItem,
        }
        public System.Type GetMyType()
        {
            switch (MyDataType)
            {
                case DataType.Float: return typeof(float);
                case DataType.Int: return typeof(int);
                case DataType.Bool: return typeof(bool);
                case DataType.String: return typeof(string);
                case DataType.InventoryItem: return typeof(InventoryItem);
            }
            return null;
        }

        public ItemAttribute() { }
        public ItemAttribute(ItemAttribute copy)
        {
            Key = copy.Key;
            isStatic = copy.isStatic;
            MyDataType = copy.MyDataType;

            FloatValue = copy.FloatValue;
            IntValue = copy.IntValue;
            BoolValue = copy.BoolValue;
            StringValue = copy.StringValue;
            InventoryItemValue = copy.InventoryItemValue;
        }
    }

}