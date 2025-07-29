using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    private float timeForDestruct = 5f;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (rb == null)
            return;

        if(collision.collider.CompareTag("Car"))
        {
            rb.isKinematic = false;
            StartCoroutine(DestroyAfterCollision());
        }
    }

    IEnumerator DestroyAfterCollision()
    {
        yield return new WaitForSeconds(timeForDestruct);
        Destroy(this.gameObject);
        yield return null;
    }
}
