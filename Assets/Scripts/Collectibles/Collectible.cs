using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Collectible : MonoBehaviour
{

    private void OnEnable()
    {
        
    }
    private void Collected()
    {
       

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Collected();
           
        }
    }

}
