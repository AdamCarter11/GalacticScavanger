using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    GameObject[] enemiesInScene;
    [SerializeField] float enemyRespawnTime = 10f;
    private void Start()
    {
        enemiesInScene = GameObject.FindGameObjectsWithTag("Enemy");
        print("Enemies: " + enemiesInScene.Length);
    }
    private void Update()
    {
        for(int i = 0; i < enemiesInScene.Length; i++)
        {
            if (enemiesInScene[i].GetComponent<EnemyAI6>().enemyHealth <= 0)
            {
                enemiesInScene[i].SetActive(false);
                StopCoroutine(respawnEnemies());
                StartCoroutine(respawnEnemies());
            }
        }
    }
    IEnumerator respawnEnemies()
    {
        yield return new WaitForSeconds(enemyRespawnTime);
        for (int i = 0; i < enemiesInScene.Length; i++)
        {
            enemiesInScene[i].SetActive(true);
        }
    }
}
