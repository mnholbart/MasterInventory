using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MasterInventory
{
    public abstract class EquippableType : ItemType
    {
        [Header("References")]
        public EquipmentSlot DefaultSlot;

        [Header("EquippableType Config")]
        public float EquipTime = .125f;
    }

}
