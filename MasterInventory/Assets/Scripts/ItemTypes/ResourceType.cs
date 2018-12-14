using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MasterInventory
{
    [CreateAssetMenu(menuName = "Inventory/Resource Type")]
    public class ResourceType : ItemType
    {
        [Header("Config")]
        [NaughtyAttributes.OnValueChanged("SetStackable")]
        public bool IsStackable;

        public void SetStackable()
        {
            if (IsStackable)
            {
                if (!RequiredAttributes.Any(t => t.Key == "CurrentStackSize"))
                    RequiredAttributes.Add(new RequiredAttribute(typeof(IntAttribute)) { Key = "CurrentStackSize", Serialized = true});
                if (!RequiredAttributes.Any(t => t.Key == "MaxStackSize"))
                    RequiredAttributes.Add(new RequiredAttribute(typeof(IntAttribute)) { Key = "MaxStackSize", Serialized = false });
            }
            else
            {
                RequiredAttributes.RemoveAll(t => t.Key == "CurrentStackSize");
                RequiredAttributes.RemoveAll(t => t.Key == "MaxStackSize");
            }

            UpdateMyItems();
        }


    }
}