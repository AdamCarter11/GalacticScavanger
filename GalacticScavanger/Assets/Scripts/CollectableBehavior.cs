using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableBehavior : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ship"))
        {
            GameManager.instance.ChangeScrapVal(10); //val in paranthesis will be whatever we want to increase scrap by
            Destroy(this.gameObject);
        }
    }

}
