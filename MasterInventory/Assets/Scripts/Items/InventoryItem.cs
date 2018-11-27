using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace MasterInventory
{
    [CreateAssetMenu(menuName = "Inventory/Create Item")]
    [System.Serializable]
    public class InventoryItem : ScriptableObject
    {
        public ItemType MyItemType;
        public GameObject ItemInstancePrefab;
        public List<ItemAttribute> StaticAttributes = new List<ItemAttribute>();
        public List<ItemAttribute> SerializedAttributes = new List<ItemAttribute>();

        private Dictionary<string, ItemAttribute> AttributeLookup;

        public void Initialize()
        {
            InitializeAttributeLookupTable();
        }

        private void InitializeAttributeLookupTable()
        {
            AttributeLookup = new Dictionary<string, ItemAttribute>();
            foreach (ItemAttribute attribute in StaticAttributes.Union(SerializedAttributes))
            {
                AttributeLookup.Add(attribute.Key, attribute);
            }
        }

        public T GetAttributeValue<T>(string key, string guid = null)
        {
            ItemAttribute attribute;
            AttributeLookup.TryGetValue(key, out attribute);
            if (attribute.GetMyType() == typeof(T))
            {
                T value = (T)attribute.GetValue(guid);
                return value;
            }
            else
            {
                Debug.LogWarning("Type mismatch for attribute with key: " + attribute.Key + " - Type: " + attribute.GetMyType(), this);
                return default(T);
            }
        }

        public ItemAttribute GetAttribute(string key, string guid = null)
        {
            var attribute = AttributeLookup[key];
            if (!attribute.isStatic)
                return attribute.SerializedAttributeLookup[guid];
            else return attribute;
        }

        public ItemAttribute AddItemAttribute(bool isSerialized = false)
        {
            ItemAttribute attribute = new ItemAttribute();
            attribute.isStatic = !isSerialized;
            if (isSerialized)
            {
                SerializedAttributes.Add(attribute);
            }
            else
            {
                StaticAttributes.Add(attribute);
            }
            return attribute;
        }

        public void AddSerializedReference(string guid, ItemAttribute attribute)
        {
            AttributeLookup[attribute.Key].SerializedAttributeLookup.Add(guid, attribute);
        }

        /// <summary>
        /// Refresh required attributes based on MyItemType
        /// </summary>
        public void Refresh()
        {
            if (MyItemType == null)
                return;

            Dictionary<string, ItemAttribute> lookup = new Dictionary<string, ItemAttribute>();
            foreach (ItemAttribute attribute in StaticAttributes.Union(SerializedAttributes))
            {
                lookup.Add(attribute.Key, attribute);
                attribute.isRequired = false;
            }

            foreach (ItemType.RequiredAttribute required in MyItemType.RequiredAttributes)
            {
                if (lookup.ContainsKey(required.Key))
                {
                    var attr = lookup[required.Key];
                    attr.isRequired = true;
                    attr.MyDataType = required.AttributeDataType;
                }
                else
                {
                    var attr = AddItemAttribute(required.Serialized);
                    attr.Key = required.Key;
                    attr.MyDataType = required.AttributeDataType;
                    attr.isRequired = true;
                }
            }
            EditorUtility.SetDirty(this);
        }
    }
}