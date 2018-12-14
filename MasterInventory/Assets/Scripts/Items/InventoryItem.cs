using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using System.Reflection;

namespace MasterInventory
{
    [CreateAssetMenu(menuName = "Inventory/Create Item")]
    [Serializable]
    public class InventoryItem : ScriptableObject
    {
        public ItemType MyItemType;
        public GameObject ItemInstancePrefab;

        public ItemAttributes InventoryItemAttributes = new ItemAttributes();

        public Dictionary<string, ItemAttribute> SerializedAttributeLookup;
        public Dictionary<string, ItemAttribute> StaticAttributeLookup;
        public Dictionary<string, ItemAttribute> AttributeLookup;

        public void Initialize()
        {
            InitializeAttributeLookupTable();
        }

        private void InitializeAttributeLookupTable()
        {
            SerializedAttributeLookup = new Dictionary<string, ItemAttribute>();
            StaticAttributeLookup = new Dictionary<string, ItemAttribute>();
            AttributeLookup = new Dictionary<string, ItemAttribute>();

            foreach (ItemAttribute attribute in InventoryItemAttributes.GetAllAttributes())
            {
                if (attribute.isStatic)
                    StaticAttributeLookup.Add(attribute.Key, attribute);
                else
                    SerializedAttributeLookup.Add(attribute.Key, attribute);

                AttributeLookup.Add(attribute.Key, attribute);
            }
        }

        public T GetAttribute<T>(string key, string guid = null) where T : ItemAttribute
        {
            if (!AttributeLookup.ContainsKey(key))
                Debug.LogErrorFormat("Key: {0} does not exist for any attribute for item: {1}", key, name, this);

            var attribute = AttributeLookup[key];
            if (!attribute.isStatic && guid != null)
                return (T)attribute.SerializedAttributeLookup[guid];
            else return (T)attribute;
        }

        public ItemAttribute AddItemAttribute(string typeString, bool isSerialized = false)
        {
            Type t = Type.GetType(typeString);
            if (!t.IsSubclassOf(typeof(ItemAttribute)))
            {
                Debug.LogErrorFormat("Trying to create new ItemAttribute with type: {0} which isn't a subclass of type: {1}", t, typeof(ItemAttribute));
                return null;
            }
            ItemAttribute attribute = (ItemAttribute)Activator.CreateInstance(t);

            attribute.isStatic = !isSerialized;
            InventoryItemAttributes.AddAttribute(attribute);

            return attribute;
        }

        public void AddSerializedReference(string guid, ItemAttribute attribute)
        {
            SerializedAttributeLookup[attribute.Key].SerializedAttributeLookup.Add(guid, attribute);
        }

        /// <summary>
        /// Refresh required attributes based on MyItemType
        /// </summary>
        public void Refresh()
        {
            Dictionary<string, ItemAttribute> lookup = new Dictionary<string, ItemAttribute>();
            foreach (ItemAttribute attribute in InventoryItemAttributes.GetAllAttributes())
            {
                lookup.Add(attribute.Key, attribute);
                attribute.isRequired = false;
            }

            if (MyItemType == null)
                return;

            foreach (ItemType.RequiredAttribute required in MyItemType.RequiredAttributes)
            {
                if (lookup.ContainsKey(required.Key))
                {
                    var attr = lookup[required.Key];
                    attr.isRequired = true;
                }
                else
                {
                    var attr = AddItemAttribute(required.TypeString, required.Serialized);
                    attr.Key = required.Key;
                    attr.isRequired = true;
                }
            }
            EditorUtility.SetDirty(this);
        }
    }
}