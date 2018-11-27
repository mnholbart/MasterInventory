using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MasterInventory
{
    [CreateAssetMenu(menuName = "Inventory/InventorySlotsSO")]
    public class InventorySlots : ScriptableObject
    {
        public class SlotEvent : UnityEvent<Slot>
        {

        }

        public SlotEvent OnSlotChange = new SlotEvent();
        public UnityEvent OnAnySlotChange = new UnityEvent();
        private List<Slot> Slots = new List<Slot>();

        public void AssignItemInstance(int index, InventoryState.ItemData instance)
        {
            Slot s = Slots[index];

            s.GUID = instance.guid;
            s.Item = instance.Item;
            instance.InventorySlot = index;
            instance.EquipmentSlot = null;

            OnSlotChange.Invoke(s);
            OnAnySlotChange.Invoke();
        }

        public int GiveItem(InventoryState.ItemData instance)
        {
            if (instance == null)
                return -1;

            for (int i = 0; i < Slots.Count; i++)
            {
                if (Slots[i].Item == null)
                {
                    instance.InventorySlot = i;
                    AssignItemInstance(i, instance);
                    OnSlotChange.Invoke(Slots[i]);
                    OnAnySlotChange.Invoke();
                    return i;
                }
            }
            return -1;
        }

        public void RemoveItem(InventoryState.ItemData instance)
        {
            var slot = Slots.Find(t => t.GUID == instance.guid);
            slot.GUID = null;
            slot.Item = null;
            instance.InventorySlot = -1;

            OnSlotChange.Invoke(slot);
            OnAnySlotChange.Invoke();
        }

        public void ResetInventory(int numSlots)
        {
            while (Slots.Count < numSlots)
                Slots.Add(new Slot());
            foreach (Slot slot in Slots)
            {
                if (slot.GUID != null || slot.Item != null)
                {
                    slot.GUID = null;
                    slot.Item = null;
                    OnSlotChange.Invoke(slot);
                }
            }
            
            OnAnySlotChange.Invoke();
        }

        public void ForceUpdateAllInventory()
        {
            OnAnySlotChange.Invoke();
        }

        public void UpdateInventorySlot(int slotNumber)
        {
            OnSlotChange.Invoke(Slots[slotNumber]);
        }

        public List<Slot> GetInventory()
        {
            return Slots;
        }

        public class Slot
        {
            public string GUID;
            public InventoryItem Item;
        }
    }

}