using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    Dictionary<string, int> _inventory = new Dictionary<string, int>();

    private void Start()
    {
        _inventory.Add("perla", 2);

        _inventory["perla"] = 4;

        if(_inventory["perla"] == 4) 
        {
            print("hola perla");
        }
    }
}
