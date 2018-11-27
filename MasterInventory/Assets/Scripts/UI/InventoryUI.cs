using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MasterInventory;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    public Transform InventorySlotsPanel;
    public Transform InventoryButtonPanel;
    public Transform DebugPanel;
    public Transform DebugButtonPanel;

    public GameObject InventoryButtonPrefab;
    public Inventory InventoryReference;
    
    private Dictionary<InventorySlots.Slot, Transform> SlotButtons = new Dictionary<InventorySlots.Slot, Transform>();

    private void Awake()
    {
        InventorySlotsPanel.gameObject.SetActive(false);
        DebugPanel.gameObject.SetActive(false);
    }

    private void Start()
    {
        UpdateDebugPanel();
    }

    private void OnEnable()
    {
        InventoryReference.OnInventoryLoaded.AddListener(UpdateInventory);
        InventoryReference.InventorySlotsReference.OnSlotChange.AddListener(UpdateSlot);
        InventoryReference.InventorySlotsReference.OnAnySlotChange.AddListener(UpdateInventory);
    }

    private void OnDisable()
    {
        InventoryReference.OnInventoryLoaded.RemoveListener(UpdateInventory);
        InventoryReference.InventorySlotsReference.OnSlotChange.RemoveListener(UpdateSlot);
        InventoryReference.InventorySlotsReference.OnAnySlotChange.RemoveListener(UpdateInventory);
    }

    private void UpdateSlot(InventorySlots.Slot slot)
    {
        UpdateSlotDisplay(slot);
    }

    private void UpdateInventory()
    {
        foreach (InventorySlots.Slot slot in InventoryReference.InventorySlotsReference.GetInventory())
        {
            if (!SlotButtons.ContainsKey(slot))
            {
                var go = Instantiate(InventoryButtonPrefab);
                go.transform.SetParent(InventoryButtonPanel);
                var button = go.GetComponent<Button>();
                button.onClick.AddListener(() => InventoryReference.TryUseInventorySlot(slot));
                SlotButtons.Add(slot, go.transform);
            }

            UpdateSlotDisplay(slot);
        }
    }

    private void UpdateSlotDisplay(InventorySlots.Slot slot)
    {
        var slotButton = SlotButtons[slot];
        slotButton.GetComponentInChildren<Text>().text = slot.Item != null ? InventoryReference.GetLabelFromGUID(slot.GUID) : "Empty";
    }

    private void UpdateDebugPanel()
    {
        foreach (InventoryItem item in InventoryReference.ItemDatabase)
        {
            var go = Instantiate(InventoryButtonPrefab);
            go.transform.SetParent(DebugButtonPanel);
            go.GetComponentInChildren<Text>().text = item.name;
            var button = go.GetComponent<Button>();
            button.onClick.AddListener(() => InventoryReference.GiveItem(new ItemPickupData(item)));
        }
    }

    public void ToggleEnabled()
    {
        DebugPanel.gameObject.SetActive(false);
        InventorySlotsPanel.gameObject.SetActive(!InventorySlotsPanel.gameObject.activeSelf);
    }

    public void ToggleDebugEnabled()
    {
        InventorySlotsPanel.gameObject.SetActive(false);
        DebugPanel.gameObject.SetActive(!DebugPanel.gameObject.activeSelf);
    }
}
