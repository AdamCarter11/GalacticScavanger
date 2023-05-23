using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class ParticleTest : MonoBehaviour
{
    [SerializeField] GameObject particleSystemToTest;
    private void Start()
    {
        StartCoroutine(LoopParticle());
    }
    IEnumerator LoopParticle()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            Instantiate(particleSystemToTest, transform.position, Quaternion.identity);
        }
    }
}
