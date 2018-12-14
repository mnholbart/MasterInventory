using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MasterInventory
{
    public class InventoryState : ScriptableObject
    {
        private InventorySlots InventorySlotsReference;

        public List<ItemData> OwnedItemData = new List<ItemData>();
        public Dictionary<string, ItemData> GUIDLookup;

        public void Clear()
        {
            OwnedItemData.Clear();
            GUIDLookup.Clear();
        }

        public IEnumerator EquipNewItem(ItemData newItem)
        {
            var equippable = (newItem.Item.MyItemType as EquippableType);

            //Unequip anything currently equipped and cache the item 
            var unequippedItem = GetEquippedItemFromSlot(equippable.DefaultSlot);

            yield return new WaitForSeconds(equippable.EquipTime);

            //Equip the new item before returning the unequipped item to inventory 
            EquipItem(newItem);

            //Add the cached item back to inventory
            InventorySlotsReference.GiveItem(unequippedItem);
        }

        public ItemData GetEquippedItemFromSlot(EquipmentSlot slot)
        {
            if (slot == null)
            {
                Debug.Log("InventoryState - null slot in UnequipItemSlot()");
            }

            ItemData unequippedItem = null;
            slot.UnequipItem(out unequippedItem);
            return unequippedItem;
        }

        public void EquipItem(ItemData item)
        {
            var equippableType = item.Item.MyItemType as EquippableType;

            if (equippableType.DefaultSlot.EquipNewItem(item))
            {
                InventorySlotsReference.RemoveItem(item);
                item.EquipmentSlot = equippableType.DefaultSlot;
            }
        }

        public void ResetInventoryState()
        {
            OwnedItemData.Clear();
            GUIDLookup = new Dictionary<string, ItemData>();
        }

        public void GiveItem(ItemPickupData item, string newGUID)
        {
            
            if (item.PickupItem.MyItemType is ResourceType && (item.PickupItem.MyItemType as ResourceType).IsStackable)
            {
                IntAttribute amountAttribute = (IntAttribute)item.SerializedAttributeValues.Find(t => t.Key == "CurrentStackSize");
                int amountBackfilled = BackfillResources(item.PickupItem, amountAttribute.GetValue());

                int newAmount = amountAttribute.GetValue() - amountBackfilled;
                amountAttribute.SetValue(newAmount);

                if (amountAttribute.GetValue() <= 0)
                    return;
            }

            var itemData = CreateNewItemData(item, newGUID);
            InventorySlotsReference.GiveItem(itemData);
        }

        public int BackfillResources(InventoryItem resource, int amount)
        {
            int amountBackfilled = 0;
            foreach (ItemData data in OwnedItemData)
            {
                if (data.Item == resource)
                {
                    //Get the stack size attributes
                    var maxStackSize = data.Item.GetAttribute<IntAttribute>("MaxStackSize");
                    var currentStackSize = data.Item.GetAttribute<IntAttribute>("CurrentStackSize", data.guid);

                    //Find out how much we need to fill the current stack
                    int maxNeeded = maxStackSize.GetValue() - currentStackSize.GetValue();
                    int amountUsed = Mathf.Min(maxNeeded, amount);

                    //Fill the stack with as much as we can
                    var newStackSize = currentStackSize.GetValue() + amountUsed;
                    currentStackSize.SetValue(newStackSize);

                    //Update the slot and amount from the new stack we used
                    amountBackfilled += amountUsed;
                    if (amountUsed > 0)
                        InventorySlotsReference.UpdateInventorySlot(data.InventorySlot);
                }
            }

            return amountBackfilled;
        }

        private ItemData CreateNewItemData(ItemPickupData pickup, string guid)
        {
            var newData = new ItemData()
            {
                EquipmentSlot = null,
                InventorySlot = -1,
                Item = pickup.PickupItem,
                guid = guid,
            };
            newData.InitializeSerializedAttributes(pickup.SerializedAttributeValues);
            OwnedItemData.Add(newData);
            GUIDLookup.Add(guid, newData);

            return newData;
        }

        public ItemData GetInstanceFromGUID(string guid)
        {
            var instance = OwnedItemData.FirstOrDefault(t => t.guid == guid);
            return instance != null ? instance : null;
        }

        public void PreSerialize(Serializer serializer)
        {
            foreach (ItemData data in OwnedItemData)
            {
                data.PreSerialize();
            }
        }

        public void PostDeserialize(Serializer serializer, InventorySlots slotsReference)
        {
            InventorySlotsReference = slotsReference;

            GUIDLookup = new Dictionary<string, ItemData>();
            foreach (ItemData data in OwnedItemData)
            {
                GUIDLookup.Add(data.guid, data);
                data.PostDeserialize();

                if (data.InventorySlot > -1)
                    InventorySlotsReference.AssignItemInstance(data.InventorySlot, data);
                else if (data.Item.MyItemType is EquippableType)
                {
                    var type = data.Item.MyItemType as EquippableType;
                    type.DefaultSlot.EquipNewItem(data);
                }
            }
        }

        [System.Serializable]
        public class ItemData
        {
            public string guid;
            public InventoryItem Item;

            public EquipmentSlot EquipmentSlot = null;
            public int InventorySlot = -1;

            public ItemAttributes ItemAttributeData;
            public Dictionary<string, ItemAttribute> AttributeLookup;

            public void PreSerialize()
            {
                foreach (ItemAttribute a in ItemAttributeData.GetAllAttributes())
                {
                    a.SaveSerializedValue();
                }
            }

            public void PostDeserialize()
            {
                AttributeLookup = new Dictionary<string, ItemAttribute>();
                foreach (ItemAttribute a in ItemAttributeData.GetAllAttributes())
                {
                    a.InitializeSerializedValue();
                    Item.AddSerializedReference(guid, a);
                    AttributeLookup.Add(a.Key, a);
                }
            }

            public void InitializeSerializedAttributes(List<ItemAttribute> defaultAttributes)
            {
                ItemAttributeData = new ItemAttributes();
                foreach (ItemAttribute attribute in Item.SerializedAttributeLookup.Values)
                {

                    ItemAttribute copyAttribute = attribute;
                    if (defaultAttributes.Any(t => t.Key == attribute.Key))
                    {
                        copyAttribute = defaultAttributes.First(t => t.Key == attribute.Key);
                    }
                    
                    ItemAttribute attributeReference = (ItemAttribute)System.Activator.CreateInstance(attribute.GetType(), copyAttribute);

                    ItemAttributeData.AddAttribute(attributeReference);
                    Item.AddSerializedReference(guid, attributeReference);
                }
            }
        }
    }

}