using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MasterInventory
{
    public abstract class ItemType : ScriptableObject
    {
        public List<RequiredAttribute> RequiredAttributes = new List<RequiredAttribute>();

        [System.Serializable]
        public class RequiredAttribute
        {
            public string Key;
            public bool Serialized;
            public string TypeString;

            public RequiredAttribute(System.Type T)
            {
                TypeString = T.FullName;
            }
        }

        public void UpdateMyItems()
        {
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(InventoryItem).Name);
            for (int i = 0; i < guids.Length; i++)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                InventoryItem item = UnityEditor.AssetDatabase.LoadAssetAtPath<InventoryItem>(path);
                if (item.MyItemType == this)
                {
                    item.Refresh();
                }
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
        
        
    }
}