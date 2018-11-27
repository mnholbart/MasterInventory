using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MasterInventory
{
    public abstract class WeaponBehaviour : ItemBehaviour
    {
        [Header("References")]
        public InventoryItem MyInventoryWeapon;
        public Transform LaserSightAnchor;
    }
}
