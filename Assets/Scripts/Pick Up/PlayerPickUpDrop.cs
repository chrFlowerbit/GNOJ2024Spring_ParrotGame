using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUpDrop : MonoBehaviour {


    [SerializeField] private Transform raycastPoint;
    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private LayerMask pickUpLayerMask;

    public ObjectGrabbable objectGrabbable;

    float pickUpDistance = 100f;
    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (objectGrabbable == null) {
                // Not carrying an object, try to grab
                if (Physics.Raycast(raycastPoint.position, raycastPoint.forward, out RaycastHit raycastHit, pickUpDistance, pickUpLayerMask)) {

                    Debug.Log(raycastHit.transform);
                    
                    if (raycastHit.transform.GetComponent<ObjectGrabbable>() != null) {
                        objectGrabbable = raycastHit.transform.GetComponent<ObjectGrabbable>();
                        objectGrabbable.Grab(objectGrabPointTransform);
                    }
                }
                } else
                {
                    // Currently carrying something, drop
                    objectGrabbable.Drop();
                    objectGrabbable = null;
                }
        }
    }
}