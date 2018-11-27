using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MasterInventory
{
    [CreateAssetMenu(menuName = "Inventory/InventoryDatabase")]
    public class Inventory : ScriptableObject
    {
        [Header("References")]
        public Serializer serializer;
        public InventorySlots InventorySlotsReference;

        [Header("Config")]
        public int NumInventorySlots = 20;

        [Header("Debug")]
        public List<EquipmentSlot> EquipmentSlots;
        public List<ItemType> ItemTypes;
        public List<InventoryItem> ItemDatabase = new List<InventoryItem>();

        [Header("Events")]
        public UnityEvent OnInventoryLoaded;

        private InventoryState CurrentInventoryState;
        private MonoBehaviour caller;
        private Coroutine EquippingWeapon = null;

        public void InitializeInventory(MonoBehaviour mbc)
        {
            foreach (InventoryItem item in ItemDatabase)
            {
                item.Initialize();
            }

            caller = mbc;
        }

        public void DestroyInventory()
        {
            CurrentInventoryState.Clear();
        }

        public void TryUseQuickslot(int index)
        {

        }

        public string GetLabelFromGUID(string guid)
        {
            InventoryState.ItemData data = CurrentInventoryState.GetInstanceFromGUID(guid);
            var itemType = data.Item.MyItemType;

            if (itemType is EquippableType)
            {
                return data.Item.name;
            }
            else if (itemType is ConsumableType)
            {
                return data.Item.name;
            }
            else if (itemType is ResourceType)
            {
                var resourceType = itemType as ResourceType;
                string label = data.Item.name;

                if (resourceType.IsStackable)
                {
                    label += ": " + data.Item.GetAttribute("CurrentStackSize", data.guid).IntValue;
                }
                return label;
            }

            return "Invalid Label";
        }

        public void TryUseInventorySlot(InventorySlots.Slot slot)
        {
            var item = CurrentInventoryState.GetInstanceFromGUID(slot.GUID);
            if (item == null)
                return;
            var itemType = item.Item.MyItemType;

            if (itemType is ConsumableType)
            {

            }
            else if (itemType is EquippableType)
            {
                var equipSlot = itemType as EquippableType;
                if (equipSlot.DefaultSlot == null || !EquipmentSlots.Contains(equipSlot.DefaultSlot))
                    return;
                
                if (EquippingWeapon == null)
                    EquippingWeapon = caller.StartCoroutine(EquipNewWeapon(item));
            } 
            else if (itemType is ResourceType)
            {

            }
        }

        private IEnumerator EquipNewWeapon(InventoryState.ItemData data)
        {
            yield return caller.StartCoroutine(CurrentInventoryState.EquipNewItem(data));
            EquippingWeapon = null;
        }

        public void GiveItem(ItemPickupData item)
        {
            string guid = Random.Range(0, 1000000).ToString();
            while (CurrentInventoryState.GetInstanceFromGUID(guid) != null)
                guid = Random.Range(0, 1000000).ToString();
            CurrentInventoryState.GiveItem(item, guid);
        }

        public void Save()
        {
            CurrentInventoryState.PreSerialize(serializer);
            serializer.Serialize("inventory", CurrentInventoryState);


            //Debug.Log("Saved Inventory");
        }

        public void Load()
        {
            bool success;
            serializer.Deserialize("inventory", CurrentInventoryState, out success);

            if (!success)
                Debug.LogError("Failed to deserialize inventory");

            foreach (EquipmentSlot slot in EquipmentSlots)
                slot.ResetSlot();
            InventorySlotsReference.ResetInventory(NumInventorySlots);
            CurrentInventoryState.PostDeserialize(serializer, InventorySlotsReference);

            OnInventoryLoaded.Invoke();
            //Debug.Log("Loaded Inventory");
        }

        public void ResetSave()
        {
            CurrentInventoryState.ResetInventoryState();
            Save();
        }

        [ContextMenu("Validate Inventory")]
        public void RecheckInventory()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            ValidateInventoryState();
            UpdateItemDatabase();
            UpdateEquipmentSlots();
            UpdateItemTypes();
        }

        private void ValidateInventoryState()
        {
            InventoryState state = UnityEditor.AssetDatabase.LoadAssetAtPath<InventoryState>(UnityEditor.AssetDatabase.GetAssetPath(this));
            if (state == null)
            {
                state = ScriptableObject.CreateInstance<InventoryState>();
                state.name = "InventoryState";
                UnityEditor.AssetDatabase.AddObjectToAsset(state, this);
                UnityEditor.AssetDatabase.SaveAssets();
            }
            CurrentInventoryState = state;
        }

        private void UpdateItemTypes()
        {
            ItemTypes = new List<ItemType>();

            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(ItemType).Name);
            for (int i = 0; i < guids.Length; i++)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                ItemType type = UnityEditor.AssetDatabase.LoadAssetAtPath<ItemType>(path);
                ItemTypes.Add(type);
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }

        private void UpdateItemDatabase()
        {
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(InventoryItem).Name);
            ItemDatabase.Clear();
            for (int i = 0; i < guids.Length; i++)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                InventoryItem item = UnityEditor.AssetDatabase.LoadAssetAtPath<InventoryItem>(path);
                item.Refresh();
                ItemDatabase.Add(item);
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }

        private void UpdateEquipmentSlots()
        {
            EquipmentSlots = new List<EquipmentSlot>();

            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(EquipmentSlot).Name);
            for (int i = 0; i < guids.Length; i++)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                EquipmentSlot slot = UnityEditor.AssetDatabase.LoadAssetAtPath<EquipmentSlot>(path);
                EquipmentSlots.Add(slot);
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
    }

}