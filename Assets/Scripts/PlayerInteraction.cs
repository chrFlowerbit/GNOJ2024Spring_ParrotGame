using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public GameObject heldObject = null;
    public float throwForce = 10f;
    private bool isHolding = false;

    void Update()
    {
        if (isHolding && Input.GetButtonDown("Fire1"))
        {
            ThrowObject();
        }
    }
  
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pickable"))
        {
            isHolding = true;
            heldObject = collision.gameObject;
            heldObject.transform.parent = this.transform;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == heldObject)
        {
            isHolding = false;
            heldObject.transform.parent = null;
            heldObject = null;
        }
    }

    void ThrowObject()
    {
        if (heldObject)
        {
            heldObject.GetComponent<Rigidbody>().isKinematic = false;
            heldObject.GetComponent<Rigidbody>().AddForce(transform.forward * throwForce, ForceMode.Impulse);
            heldObject = null;
        }
    }
}
