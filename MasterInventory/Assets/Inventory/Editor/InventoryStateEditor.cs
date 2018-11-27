using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace MasterInventory
{
    [CustomEditor(typeof(InventoryState), true)]
    public class InventoryStateEditor : Editor
    {

        InventoryState state;

        public void OnEnable()
        {
            state = (InventoryState)target;
        }

        public override void OnInspectorGUI()
        {
            var itemData = serializedObject.FindProperty("OwnedItemData");
            
            EditorGUILayout.PropertyField(itemData, true);

            EditorUtility.SetDirty(state);
            serializedObject.ApplyModifiedProperties();
        }
    }

}
