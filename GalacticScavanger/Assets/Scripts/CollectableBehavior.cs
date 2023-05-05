using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableBehavior : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        //will have to change this to make the AI actually be able to increas their own scrap instead of the players
        if (other.gameObject.CompareTag("Ship") || other.gameObject.CompareTag("Enemy"))
        {
            GameManager.instance.ChangeScrapVal(10); //val in paranthesis will be whatever we want to increase scrap by
            Destroy(this.gameObject);
        }
    }

}
