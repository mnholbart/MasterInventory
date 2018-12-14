using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Reflection;
using System.Linq;

namespace MasterInventory
{
    [CustomEditor(typeof(InventoryItem), true)]
    public class InventoryItemEditor : Editor
    {
        List<AttributeType> AttributeTypes = new List<AttributeType>();
        string[] enumDisplay;

        InventoryItem item;

        public void OnEnable()
        {
            item = (InventoryItem)target;
            PopulateAttributeTypes();
            enumDisplay = AttributeTypes.Select(t => t.key).ToArray();
        }

        private void PopulateAttributeTypes()
        {
            foreach (Type t in ItemAttributes.RegisteredTypes.Select(t => t.type))
            {
                string typeName = t.Name;
                typeName = typeName.Replace("Attribute", "");
                AttributeTypes.Add(new AttributeType(typeName, t));
            }
        }

        private void CreateItemAttribute(Type t, bool isSerialized = false)
        {
            item.AddItemAttribute(t.FullName, isSerialized: isSerialized);
        }

        private void CreateAttributeMenu(GUIContent label, Rect horizontal, bool isSerialized = false)
        {
            if (GUILayout.Button(label, EditorStyles.toolbarDropDown))
            {
                GenericMenu typesMenu = new GenericMenu();
                foreach (AttributeType t in AttributeTypes)
                {
                    typesMenu.AddItem(new GUIContent(t.key), false, () => CreateItemAttribute(t.type, isSerialized: isSerialized));
                }

                typesMenu.DropDown(new Rect(Screen.width, horizontal.y, 0, 16));
                EditorGUIUtility.ExitGUI();
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            SerializedProperty prefab = serializedObject.FindProperty("ItemInstancePrefab");
            SerializedProperty type = serializedObject.FindProperty("MyItemType");

            var refValue = type.objectReferenceValue;

            EditorGUILayout.PropertyField(prefab);
            EditorGUILayout.PropertyField(type);

            if (GUILayout.Button("Clear Attributes"))
            {
                ClearAttributes();
                return;
            }

            Rect r = EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Static Attributes");
            CreateAttributeMenu(new GUIContent("Add Static Attribute"), r);
            EditorGUILayout.EndHorizontal();
            ShowElements(item.InventoryItemAttributes.GetAllAttributes().Where(t => t.isStatic).ToList());

            EditorGUILayout.Space();

            r = EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Serialized Attributes");
            CreateAttributeMenu(new GUIContent("Add Saved Attribute"), r, isSerialized: true);
            EditorGUILayout.EndHorizontal();
            ShowElements(item.InventoryItemAttributes.GetAllAttributes().Where(t => !t.isStatic).ToList());



            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(item);

            if (type.objectReferenceValue != refValue)
                item.Refresh();
        }

        private void ShowElements(List<ItemAttribute> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                ItemAttribute itemAttribute = list[i];

                Rect r = EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(itemAttribute.Key.Length > 0 ? itemAttribute.Key : "Attribute " + i, GUILayout.MaxWidth(118));
                EditorGUI.BeginDisabledGroup(itemAttribute.isRequired == true);
                EditorGUILayout.LabelField(itemAttribute.GetAttributeName(), GUILayout.MinWidth(r.width - 200));

                if (GUILayout.Button(itemAttribute.isRequired ? "Required" : "Delete", GUILayout.MaxWidth(80)))
                {
                    item.InventoryItemAttributes.RemoveAttribute(itemAttribute);
                    i--;
                    continue;
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Key", GUILayout.MaxWidth(28));
                var key = EditorGUILayout.DelayedTextField(itemAttribute.Key, GUILayout.MaxWidth(90));
                if (key != itemAttribute.Key)
                {
                    itemAttribute.Key = key;
                }
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.LabelField("Value", GUILayout.MaxWidth(50));

                foreach (ItemAttributes.RegisteredAttributeType registeredType in ItemAttributes.RegisteredTypes)
                {
                    if (registeredType.type == itemAttribute.GetType())
                    {
                        object ret = registeredType.EditorDelegate.Invoke(itemAttribute.GetObjectValue());
                        if (ret != itemAttribute.GetObjectValue())
                            itemAttribute.SetObjectValue(ret);
                        break;
                    }
                }


                EditorGUILayout.EndHorizontal();
            }
        }
        
        private void ClearAttributes()
        {
            item.InventoryItemAttributes.ClearAttributes();
            item.Refresh();
        }
    }

    public class AttributeType
    {
        public string key = "";
        public Type type = null;

        public AttributeType(string k, Type t)
        {
            key = k;
            type = t;
        }
    }
}
