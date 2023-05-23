using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    GameObject[] enemiesInScene;
    [SerializeField] float enemyRespawnTime = 10f;
    [SerializeField] float changeTimeVal = 2f;

    private void Start()
    {
        enemiesInScene = GameObject.FindGameObjectsWithTag("Enemy");
        /*
        for (int i = 0; i < enemiesInScene.Length; i++)
        {
            enemiesInScene[i].GetComponent<EnemyAI6>().enemyHealth = 10;
        }
        */
        print("Enemies: " + enemiesInScene.Length);
    }
    private void Update()
    {
        for(int i = 0; i < enemiesInScene.Length; i++)
        {
            if (enemiesInScene[i] != null)
            {
                if (enemiesInScene[i].GetComponent<EnemyHealth>().health <= 0 && enemiesInScene[i].activeSelf)
                {
                    print("enemy destruction: " + enemiesInScene[i].name);
                    GameManager.instance.ChangeTIme(changeTimeVal);
                    enemiesInScene[i].SetActive(false);
                    StopCoroutine(respawnEnemies());
                    StartCoroutine(respawnEnemies());
                }
            }
            else
            {
                print("enemy doesn't exist anymore: " + enemiesInScene[i]);
            }
            
        }
    }
    IEnumerator respawnEnemies()
    {
        yield return new WaitForSeconds(enemyRespawnTime);
        print("respawn enemies: " + enemiesInScene.Length);
        for (int i = 0; i < enemiesInScene.Length; i++)
        {
            if (!enemiesInScene[i].activeSelf)
                enemiesInScene[i].SetActive(true);
                
        }
    }
}
