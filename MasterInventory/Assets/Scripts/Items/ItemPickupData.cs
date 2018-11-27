using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MasterInventory
{

    [System.Serializable]
    public class ItemPickupData
    {

        public ItemPickupData(InventoryItem item)
        {
            PickupItem = item;

            foreach (ItemAttribute attribute in item.SerializedAttributes)
            {
                var attributeValue = new ItemAttribute(attribute);
                SerializedAttributeValues.Add(attributeValue);
            }
        }

        public InventoryItem PickupItem;
        public List<ItemAttribute> SerializedAttributeValues = new List<ItemAttribute>();


    }

}