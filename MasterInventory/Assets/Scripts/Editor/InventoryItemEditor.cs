using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace MasterInventory
{
    [CustomEditor(typeof(InventoryItem), true)]
    public class InventoryItemEditor : Editor
    {

        InventoryItem item;

        public void OnEnable()
        {
            item = (InventoryItem)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty prefab = serializedObject.FindProperty("ItemInstancePrefab");
            SerializedProperty type = serializedObject.FindProperty("MyItemType");

            EditorGUILayout.PropertyField(prefab);

            EditorGUILayout.PropertyField(type);

            if (GUILayout.Button("Clear Attributes"))
                ClearAttributes();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Static Attributes");
            if (GUILayout.Button("Add Static Attribute"))
                item.AddItemAttribute();
            EditorGUILayout.EndHorizontal();
            ShowElements(item.StaticAttributes);

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Serialized Attributes");
            if (GUILayout.Button("Add Saved Attribute"))
                item.AddItemAttribute(isSerialized: true);
            EditorGUILayout.EndHorizontal();
            ShowElements(item.SerializedAttributes);

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(item);
        }

        private void ShowElements(List<ItemAttribute> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                ItemAttribute item = list[i];

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(item.Key.Length > 0 ? item.Key : "Attribute " + i);
                EditorGUI.BeginDisabledGroup(item.isRequired == true);
                item.MyDataType = (ItemAttribute.DataType)EditorGUILayout.EnumPopup(item.MyDataType);
                if (GUILayout.Button(item.isRequired ? "Required" : "Delete"))
                {
                    list.Remove(item);
                    continue;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Key", GUILayout.MaxWidth(28));
                var key = EditorGUILayout.DelayedTextField(item.Key, GUILayout.MaxWidth(90));
                if (key != item.Key)
                {
                    item.Key = key;
                }
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.LabelField("Value", GUILayout.MaxWidth(50));
                switch (item.MyDataType)
                {
                    case ItemAttribute.DataType.Float: item.FloatValue = EditorGUILayout.DelayedFloatField(item.FloatValue); break;
                    case ItemAttribute.DataType.Int: item.IntValue = EditorGUILayout.DelayedIntField(item.IntValue); break;
                    case ItemAttribute.DataType.Bool: item.BoolValue = EditorGUILayout.Toggle(item.BoolValue); break;
                    case ItemAttribute.DataType.String: item.StringValue = EditorGUILayout.DelayedTextField(item.StringValue); break;
                    case ItemAttribute.DataType.InventoryItem: item.InventoryItemValue = (InventoryItem)EditorGUILayout.ObjectField(item.InventoryItemValue, typeof(InventoryItem), false); break;
                    default: Debug.LogError("Missing InventoryItemEditor display case for DataType: " + item.MyDataType); break;
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private void ClearAttributes()
        {
            item.SerializedAttributes.RemoveAll(t => !t.isRequired);
            item.StaticAttributes.RemoveAll(t => !t.isRequired);
        }
    }

}
