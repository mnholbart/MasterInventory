using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MasterInventory
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Inventory/EquipmentSlot")]
    public class EquipmentSlot : ScriptableObject
    {
        public List<EquippableType> ValidEquipmentTypes = new List<EquippableType>();

        public InventoryState.ItemData EquippedItemData;
        public UnityAction<InventoryState.ItemData> UnequippedItem;
        public UnityAction<InventoryState.ItemData> EquippedNewItem;


        public bool UnequipItem(out InventoryState.ItemData unequippedItem) 
        {
            unequippedItem = null;
            if (EquippedItemData != null)
            {
                unequippedItem = EquippedItemData;
                ResetSlot();
                return true;
            }

            return false;
        }

        public bool EquipNewItem(InventoryState.ItemData item)
        {
            if (EquippedItemData != null)
                return false;

            EquippedItemData = item;
            if (EquippedNewItem != null)
                EquippedNewItem.Invoke(EquippedItemData);

            return true;
        }

        public void ResetSlot()
        {
            if (UnequippedItem != null)
                UnequippedItem.Invoke(EquippedItemData);
            EquippedItemData = null;
        }
    }

}
