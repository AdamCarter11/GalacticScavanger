using System.Collections;
using UnityEngine;

public class CollectibleSpawner : MonoBehaviour
{
    public GameObject collectiblePrefab;   // The prefab of the collectible object to spawn
    public int numberOfCollectibles = 10;  // The number of collectibles to spawn
    public float spawnRadius = 10f;        // The maximum distance from the spawner at which collectibles can be spawned
    bool canSpawn;
    bool spawnDelayed = false;
    [SerializeField] float respawnDelay = 20f;
    void Start()
    {
        SpawnLogic();
    }
    private void Update()
    {
        if (!spawnDelayed)
        {
            spawnDelayed = true;
            SpawnLogic();
            StartCoroutine(spawnDelay());
        }
        
    }
    IEnumerator spawnDelay()
    {
        yield return new WaitForSeconds(respawnDelay);
        spawnDelayed = false;
    }

    void SpawnLogic()
    {
        for(int i = 0; i < numberOfCollectibles; i++)
        {
            // Generate a random position within the spawn radius
            Vector3 spawnPosition = transform.position + Random.insideUnitSphere * spawnRadius;

            // Check if any colliders intersect the intended spawn position
            
            Collider[] colliders = Physics.OverlapSphere(spawnPosition, 0.5f);
            //print(colliders.Length);
            int totalChecks = 0;
            while (colliders.Length > 2 && totalChecks < 10)
            {
                // Generate a random position within the spawn radius
                spawnPosition = transform.position + Random.insideUnitSphere * spawnRadius;

                // Check if any colliders intersect the intended spawn position
                colliders = Physics.OverlapSphere(spawnPosition, 0.5f);
                totalChecks++;
                
            }
            
            //print("Colliders length: " + colliders.Length);
            if(totalChecks < 10)
                canSpawn = true;
            /*
            foreach (Collider c in colliders)
            {
                if (c.gameObject.tag == "Scrap")
                {
                    // Don't spawn if there's already a collectible at this position
                    canSpawn = false;
                    break;
                }
            }
            */

            // If there's no overlap, spawn the collectible
            if (canSpawn)
            {
                Instantiate(collectiblePrefab, spawnPosition, Quaternion.identity);
                //print(collectiblePrefab.transform.position);
            }
            //numberOfCollectibles++;
        }
    }
}
