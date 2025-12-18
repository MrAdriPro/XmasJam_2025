using System;
using UnityEngine;

public class WoodController : MonoBehaviour
{
    public int woodAmount = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerCarry playerCarry = other.GetComponent<PlayerCarry>();
            
            playerCarry.carriedWood =  playerCarry.carriedWood + woodAmount;
            
            Destroy(gameObject);
        }
    }
}
