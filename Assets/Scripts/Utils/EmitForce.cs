using System.Collections.Generic;
using UnityEngine;

public class EmitForce : MonoBehaviour
{
    [SerializeField] private GameObject forceEffectPrefab; // Prefab for the force emission effect
    [SerializeField] private float forceMagnitude = 500f; // Magnitude of the force applied
    [SerializeField] private float forceRadius = 5f; // Radius of the force field
    [SerializeField] private float forceDuration = 0.5f; // Duration of the force effect
    [SerializeField] private KeyCode forceKey = KeyCode.F; // Key to activate the force emission

    private float lastEmissionTime = 0f;

    [SerializeField] private AudioClip hitSound; // Serialize a hit sound clip


    void Update()
    {
        if (Input.GetMouseButtonDown(1) && GameManager.canEmitForce)
        {
            GameManager.instance.ResetSliderValue();
            EmitForceField();
            lastEmissionTime = Time.time;
        }

    }
    private void DestroyObjectsInRadius()
    {
        // Apply force to nearby objects
        Collider[] colliders = Physics.OverlapSphere(transform.position, forceRadius);
        foreach (Collider collider in colliders)
        {
            MeshDestroy script = collider.GetComponent<MeshDestroy>();
            if (script != null)
            {                
                // Calculate the closest point on the collider to the center of the sphere
                Vector3 closestPoint = collider.ClosestPoint(transform.position);
                if(closestPoint != null)
                {
                    Debug.Log("__________");
                    Debug.Log(closestPoint);
                    Debug.Log(script);
                    Debug.Log("__________");
                    script.DestroyMesh(closestPoint, Quaternion.identity);
                }     

            }
        }
    }
    private void EmitForceField()
    {
        DestroyObjectsInRadius();

        // Spawn the force effect
        GameObject forceEffect = Instantiate(forceEffectPrefab, transform.position, transform.rotation);
        Destroy(forceEffect, forceDuration); // Destroy the effect after its duration

        // Apply force to nearby objects
        Collider[] colliders = Physics.OverlapSphere(transform.position, forceRadius);
        foreach (Collider collider in colliders)
        {
            Rigidbody rigidbody = collider.attachedRigidbody;
            if (rigidbody != null && rigidbody.isKinematic == false)
            {
                AudioSource.PlayClipAtPoint(hitSound, collider.transform.position);

                Vector3 forceDirection = (collider.transform.position - transform.position).normalized;
                rigidbody.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
            }
        }
    }
}
