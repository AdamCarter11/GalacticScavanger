using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    [SerializeField] float deathZoneTime;
    [SerializeField] GameObject textPopUp;
    bool playerOut = false;

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ship"))
        {
            //player has gone out of bounds
            playerOut = true;
            if (playerOut)
            {
                StartCoroutine(OutOfBoundsPlayerDeath());
            }
        }
        if (other.gameObject.CompareTag("Enemy"))
        {
            //enemy has gone out of bounds
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ship"))
        {
            //player has gone out of bounds
            print("Player is back");
            playerOut = false;
            textPopUp.SetActive(false);
            if (!playerOut)
            {
                StopCoroutine(OutOfBoundsPlayerDeath());
            }
        }
    }

    IEnumerator OutOfBoundsPlayerDeath()
    {
        //what to do while the player is out of bounds but still in the mercy time
        print("Out of bounds");
        textPopUp.SetActive(true);
        yield return new WaitForSeconds(5.0f);
        textPopUp.SetActive(false);
        if (playerOut)
        {
            Ship.isDead = true;
            print("Player death");
        }
        //what to do to the player if they have stayed out too long

    }
}
