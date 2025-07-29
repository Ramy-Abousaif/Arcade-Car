using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePaintTest : MonoBehaviour
{
    public float strength = 1;
    public float hardness = 1;

    [Space]
    ParticleSystem part;
    List<ParticleCollisionEvent> collisionEvents;

    private void Start()
    {
        part = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);

        for (int i = 0; i < numCollisionEvents; i++)
        {
            Vector3 pos = collisionEvents[i].intersection;
            Paintable paintable = other.GetComponent<Paintable>();
            PaintManager.instance.paint(paintable, pos, 1, 0.5f, 0.5f, Color.red);
        }
    }
}