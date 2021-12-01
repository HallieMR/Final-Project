using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{

    Animator animator;
    public GameObject explosionParticlesPrefab;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
            controller.ChangeHealth(-1);
            GameObject explosionObject = Instantiate(explosionParticlesPrefab, transform.position, Quaternion.identity);
        }
    }

}
