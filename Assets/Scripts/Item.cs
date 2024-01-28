using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public void ThrowItem() 
    {      
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<BoxCollider>().enabled = true;
        GetComponent<Rigidbody>().AddForce(-transform.right * 10, ForceMode.Impulse);
        transform.parent = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.layer == 6)
        //{
        //    GetComponent<Rigidbody>().isKinematic = true;
        //    GetComponent<Rigidbody>().useGravity = false;
        //}
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject);
        if (collision.gameObject.layer == 0)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Rigidbody>().useGravity = false;
        }
    }
}
