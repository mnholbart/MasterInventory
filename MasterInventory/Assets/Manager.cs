using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {

    public MasterInventory.Inventory inventory;

    private void Start()
    {
        inventory.InitializeInventory(this);

        inventory.Load();
    }

}
