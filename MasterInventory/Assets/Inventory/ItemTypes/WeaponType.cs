using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MasterInventory
{
    [CreateAssetMenu(menuName = "Inventory/Weapon Type")]
    public class WeaponType : EquippableType
    {

        [Header("WeaponType Config")]
        [NaughtyAttributes.OnValueChanged("SetAmmoUse")]
        public bool UsesAmmunition;
        [NaughtyAttributes.OnValueChanged("SetUsesLaserSight")]
        public bool UsesLaserSight;

        public void SetAmmoUse()
        {
            if (UsesAmmunition)
            {
                if (!RequiredAttributes.Any(t => t.Key == "CurrentAmmo"))
                    RequiredAttributes.Add(new RequiredAttribute() { Key = "CurrentAmmo", AttributeDataType = ItemAttribute.DataType.Int, Serialized = true });
                if (!RequiredAttributes.Any(t => t.Key == "MaxAmmo"))
                    RequiredAttributes.Add(new RequiredAttribute() { Key = "MaxAmmo", AttributeDataType = ItemAttribute.DataType.Int, Serialized = false });
                if (!RequiredAttributes.Any(t => t.Key == "AmmoType"))
                    RequiredAttributes.Add(new RequiredAttribute() { Key = "AmmoType", AttributeDataType = ItemAttribute.DataType.InventoryItem, Serialized = false });
            }
            else
            {
                RequiredAttributes.RemoveAll(t => t.Key == "CurrentAmmo");
                RequiredAttributes.RemoveAll(t => t.Key == "MaxAmmo");
                RequiredAttributes.RemoveAll(t => t.Key == "AmmoType");
            }

            UpdateMyItems();
        }

        public void SetUsesLaserSight()
        {
            if (UsesLaserSight)
            {
                if (!RequiredAttributes.Any(t => t.Key == "CanUseLaserSight"))
                    RequiredAttributes.Add(new RequiredAttribute() { Key = "CanUseLaserSight", AttributeDataType = ItemAttribute.DataType.Bool, Serialized = false });
                if (!RequiredAttributes.Any(t => t.Key == "HasLaserSight"))
                    RequiredAttributes.Add(new RequiredAttribute() { Key = "HasLaserSight", AttributeDataType = ItemAttribute.DataType.Bool, Serialized = true });
            }
            else
            {
                RequiredAttributes.RemoveAll(t => t.Key == "CanUseLaserSight");
                RequiredAttributes.RemoveAll(t => t.Key == "HasLaserSight");
            }

            UpdateMyItems();
        }
    }

}