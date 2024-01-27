using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class health : MonoBehaviour
{
    public float HP = 100;
    public float armor = 5;
    public float HEarmor = 5;
    public float Sarmor = 5;
    public float Barmor = 5;
    public bool destroywhenzero = true;
    public bool removeKinematic = false;
    public bool removeParent = false;
    public bool notshattering = false;
    public bool penned = false;
    public bool FXwhenzero = false;

    public Vector3 contactPoint;
    public Vector3 forceVector;
    public float force;
    public Vector3 cuttingNormal;
    private Rigidbody rb;
    public GameObject FX;

    public float startingarmor;
    public float startingHEarmor;
    public float startingSarmor;
    public float startingBarmor;
    private void Start()
    {
        startingarmor = armor;
        startingHEarmor = HEarmor;
        startingSarmor = Sarmor;
        startingBarmor = Barmor;
        


        if(removeKinematic)
        if (TryGetComponent<Rigidbody>(out Rigidbody _rb))
        {
            rb = _rb;
        }
        else
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
                rb.drag = 1;
        }

    }
    void Update()
    {
        if(armor <0)
        {
            armor = 0;
        }
        if (HEarmor < 0)
        {
            HEarmor = 0;
        }

        if ((HP <= 0 || float.IsNaN(HP)) && !destroywhenzero && !removeKinematic && !notshattering)
        {
            penned = false;
        }
        else if ((HP <= 0 || float.IsNaN(HP)) && destroywhenzero)
        {
            Destroy(gameObject);
        }
        else if((HP <= 0 || float.IsNaN(HP)) && !destroywhenzero && removeKinematic)
        {
            if(rb.isKinematic)
            rb.isKinematic = false;

            if (HP <= -100) Component.Destroy(gameObject.GetComponent<health>());
        }
        else if((HP <= 0 || float.IsNaN(HP)) && FXwhenzero)
        {
            FX.SetActive(FXwhenzero);
        }
        else if((HP <= 0 || float.IsNaN(HP)) && removeParent)
        {
            transform.parent = null;
        }

            if (!notshattering)
        if (penned && !destroywhenzero)
        {
            penned = false;
        }
    }

}
