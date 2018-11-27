using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace MasterInventory
{
    [CustomEditor(typeof(Inventory))]
    public class InventoryEditor : Editor
    {

        Inventory inventory;

        SerializedProperty serializer;
        SerializedProperty slotsReference;
        SerializedProperty invenSlots;
        SerializedProperty equipSlots;
        SerializedProperty itemDatabase;

        List<EquipmentSlotLookup> SlotLookup;
        bool defaultSlotFoldout = false;

        public void OnEnable()
        {
            inventory = (Inventory)target;

            serializer = serializedObject.FindProperty("serializer");
            slotsReference = serializedObject.FindProperty("InventorySlotsReference");
            invenSlots = serializedObject.FindProperty("NumInventorySlots");
            equipSlots = serializedObject.FindProperty("EquipmentSlots");

            SlotLookup = new List<EquipmentSlotLookup>();
            SlotLookup.Add(new EquipmentSlotLookup() { name = "None", slot = null });
            foreach (EquipmentSlot slot in inventory.EquipmentSlots)
            {
                SlotLookup.Add(new EquipmentSlotLookup() { name = slot.name, slot = slot });
            }
        }

        public override void OnInspectorGUI()
        {

            if (!Application.isPlaying && GUILayout.Button("Revalidate Inventory"))
                inventory.RecheckInventory();
            if (!Application.isPlaying && GUILayout.Button("Reset"))
                inventory.ResetSave();
            if (Application.isPlaying && GUILayout.Button("Save"))
                inventory.Save();
            if (Application.isPlaying && GUILayout.Button("Load"))
                inventory.Load();

            EditorGUILayout.PropertyField(serializer);
            EditorGUILayout.PropertyField(slotsReference);

            EditorGUILayout.PropertyField(invenSlots);
            defaultSlotFoldout = EditorGUILayout.Foldout(defaultSlotFoldout, "Default Equip Slots");
            if (defaultSlotFoldout)
            {
                EditorGUI.indentLevel++;
                foreach (ItemType t in inventory.ItemTypes)
                {
                    if (!(t is EquippableType))
                        continue;

                    EquippableType et = t as EquippableType;

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Type: " + t.name);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("Default Equip Slot");

                    List<EquipmentSlotLookup> validSlots = new List<EquipmentSlotLookup>();
                    validSlots.AddRange(SlotLookup.Where(q => q.slot == null || q.slot.ValidEquipmentTypes.Contains(t)));
                    List<string> displayStrings = new List<string>(validSlots.Select(q => q.name));

                    int selected = validSlots.FindIndex(q => q.slot == et.DefaultSlot);
                    int changed = EditorGUILayout.Popup(selected, displayStrings.ToArray());
                    if (selected != changed)
                        et.DefaultSlot = validSlots[changed].slot;

                    EditorGUI.indentLevel--;
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;
            }


            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);
            EquipmentSlotsArray();

            serializedObject.ApplyModifiedProperties();
        }

        bool equipFoldout = false;
        private void EquipmentSlotsArray()
        {
            equipFoldout = EditorGUILayout.Foldout(equipFoldout, "Equipment Slots");
            if (equipFoldout)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < equipSlots.arraySize; i++)
                {
                    var prop = equipSlots.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(prop);

                }
                EditorGUI.indentLevel--;
            }
        }

        public class EquipmentSlotLookup
        {
            public string name;
            public EquipmentSlot slot;
        }
    }

}
