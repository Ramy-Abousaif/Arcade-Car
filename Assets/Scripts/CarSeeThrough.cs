using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class CarSeeThrough : MonoBehaviour
{
    public Transform cam;
    public Transform p;
    private float newSize = 3f;
    private int posID = Shader.PropertyToID("_PlayerPosition");
    private int sizeID = Shader.PropertyToID("_Size");
    private int opacityID = Shader.PropertyToID("_Opacity");

    private HashSet<GameObject> previouslyHitObjects = new HashSet<GameObject>();
    private Dictionary<GameObject, Material> materials = new Dictionary<GameObject, Material>();

    void Start()
    {
        // Cache materials and their properties for objects under the "buildings" layer
        GameObject[] buildingObjects = GameObject.FindGameObjectsWithTag("Building");
        foreach (GameObject buildingObject in buildingObjects)
        {
            MeshRenderer renderer = buildingObject.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                Material material = renderer.material;
                materials.Add(buildingObject, material);
            }
        }
    }

    void Update()
    {
        // Perform the sphere cast and store the hits information in an array
        RaycastHit[] hits = Physics.SphereCastAll(cam.position, newSize, cam.forward, Mathf.Infinity, LayerMask.GetMask("Buildings"));

        // Tween properties for currently hit objects
        foreach (RaycastHit hit in hits)
        {
            GameObject hitObject = hit.transform.gameObject;
            Material hitMaterial = materials[hitObject];

            // Check if the object is not already tweening
            if (!previouslyHitObjects.Contains(hitObject))
            {
                DOVirtual.Float(hitMaterial.GetFloat(sizeID), newSize, 0.5f, x => hitMaterial.SetFloat(sizeID, x));
                DOVirtual.Float(hitMaterial.GetFloat(opacityID), 0.9f, 0.5f, x => hitMaterial.SetFloat(opacityID, x));
            }

            // Add the hit object to the list of previously hit objects
            previouslyHitObjects.Add(hitObject);
        }

        // Tween properties for previously hit objects that are no longer hit
        HashSet<GameObject> objectsToRemove = new HashSet<GameObject>();
        foreach (GameObject previouslyHitObject in previouslyHitObjects)
        {
            // Check if the object is still being hit
            if (!Array.Exists(hits, hit => hit.transform.gameObject == previouslyHitObject))
            {
                Material previouslyHitMaterial = materials[previouslyHitObject];
                DOVirtual.Float(previouslyHitMaterial.GetFloat(sizeID), 0f, 0.5f, x => previouslyHitMaterial.SetFloat(sizeID, x));
                DOVirtual.Float(previouslyHitMaterial.GetFloat(opacityID), 0f, 0.5f, x => previouslyHitMaterial.SetFloat(opacityID, x));
                objectsToRemove.Add(previouslyHitObject);
            }
        }

        // Remove previously hit objects that are no longer hit
        foreach (GameObject obj in objectsToRemove)
        {
            previouslyHitObjects.Remove(obj);
        }
    }
}