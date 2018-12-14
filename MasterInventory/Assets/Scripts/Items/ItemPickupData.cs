using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MasterInventory
{

    [System.Serializable]
    public class ItemPickupData
    {
        public InventoryItem PickupItem;
        public List<ItemAttribute> SerializedAttributeValues = new List<ItemAttribute>();


        public ItemPickupData(InventoryItem item)
        {
            PickupItem = item;

            foreach (ItemAttribute attribute in item.SerializedAttributeLookup.Values)
            {
                ItemAttribute newAttribute = (ItemAttribute)System.Activator.CreateInstance(attribute.GetType(), attribute);

                SerializedAttributeValues.Add(newAttribute);
            }
        }
    }

}