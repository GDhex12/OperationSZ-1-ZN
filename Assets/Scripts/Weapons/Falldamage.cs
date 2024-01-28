using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Falldamage : MonoBehaviour
{
    //AdvancedCarController advcc;
    Rigidbody rb;
    public GameObject[] parts;
    bool[] isKinematic = new bool[40];
    Rigidbody[] partRigids = new Rigidbody[40];
    float damage;
    float airtime=0;
    Vector3 memVelocity;
    float acceleration;
    bool firstTime = true;
    bool kinematicsEnabled = false;
    bool prevKinematicsEnabled = false;

    public float accelLimit = 300;

    // Start is called before the first frame update
    void Start()
    {
        //advcc = GetComponent<AdvancedCarController>();

        if (parts.Length > 0)
        {
            if (!TryGetComponent<Rigidbody>(out rb))
            {

                int k = 0;
                foreach (GameObject part in parts)
                {
                    TryGetComponent<Rigidbody>(out partRigids[k]);
                    k++;
                }
            }
        }
        else
        {
            if (!TryGetComponent<Rigidbody>(out rb))
                rb = null;
        }

        if(parts.Length > 0 && partRigids[0] == null)
        {
            
        }

    }

    // Update is called once per frame
    void Update()
    {

        if (rb != null)
        {
            if (parts.Length > 0)
            {
                foreach (GameObject part in parts)
                {
                    AccelDamage(rb, part, memVelocity);
                }
            }
            else
            {
                AccelDamage(rb, transform.gameObject, memVelocity);
            }
            memVelocity = rb.velocity;
        }
        else
        {
            if (parts.Length > 0)
            {
                int k = 0;
                float health = 0;
                foreach (GameObject part in parts)
                {
                    if (partRigids[k] != null)
                    {
                        health = AccelDamage(partRigids[k], part, memVelocity);
                        memVelocity = partRigids[k].velocity;
                    }
                        

                    if (health <= 0) kinematicsEnabled = false;

                    if (prevKinematicsEnabled && !kinematicsEnabled)
                    {
                        int j = 0;
                        foreach (GameObject partvel in parts)
                        {
                            partRigids[j].gameObject.layer = 0;
                            partRigids[j].useGravity = true;
                            if(k!=j)
                            partRigids[j].velocity += partRigids[k].velocity;
                            j++;
                        }
                    }

                    k++;
                    
                }


            }

            if (firstTime)
                if (parts.Length > 0)
                {
                    int k = 0;
                    foreach (GameObject part in parts)
                    {
                        part.transform.TryGetComponent<Rigidbody>(out partRigids[k]);
                        k++;
                    }
                    if (partRigids[0] != null)
                        CheckForKinematics();
                }
                else
                {
                    if (!TryGetComponent<Rigidbody>(out rb))
                        rb = null;
                }
            firstTime = false;

            prevKinematicsEnabled = kinematicsEnabled;
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        

        if (TryGetComponent<health>(out health health))
        {
            health.contactPoint = collision.contacts[0].point;
            health.cuttingNormal = collision.collider.transform.right;
            health.force = acceleration;
            health.forceVector = (rb.velocity - memVelocity).normalized;
          
        }
    }

    private float AccelDamage(Rigidbody rb, GameObject part, Vector3 memVelocity)
    {
        float leftHealth = 1;
        acceleration = Mathf.Abs(rb.velocity.magnitude - memVelocity.magnitude) / Time.deltaTime;

        //if(!advcc.axles[0].wheelDataL.isOnGround && !advcc.axles[0].wheelDataR.isOnGround && !advcc.axles[1].wheelDataL.isOnGround && !advcc.axles[1].wheelDataR.isOnGround && acceleration < 50)
        //{
        //    airtime += Time.deltaTime;
        //}
        //else
        //{
        //    if(airtime > 1 && acceleration>50)
        //    {
        //        damage = Mathf.Clamp((airtime / 2), 1, 10) * rb.velocity.magnitude * (1 - Vector3.Dot(rb.velocity.normalized, Vector3.forward));
        //        foreach(GameObject part in parts)
        //        {
        //            if(part != null)
        //            {
        //                health HP = part.GetComponent<health>();

        //                HP.HP = HP.HP - damage;
        //            }

        //        }
        //    }
        //    airtime = 0;
        //}

        if (acceleration > accelLimit)
        {
            damage = Mathf.Clamp((airtime / 2), 1, 10) * acceleration/200 * (1 - Vector3.Dot(rb.velocity.normalized, Vector3.forward));
            
                    if (part != null)
                    {
                        if (part.TryGetComponent<health>(out health health))
                        {
                            health.HP = health.HP - damage;
                    leftHealth = health.HP;
                        }
                    }

                
            

        }


        //if (acceleration > accelLimit)
        //{
        //    damage = Mathf.Clamp((airtime / 2), 1, 10) * rb.velocity.magnitude * (1 - Vector3.Dot(rb.velocity.normalized, Vector3.forward));
        //    if (parts.Length > 0)
        //    {
        //        foreach (GameObject part in parts)
        //        {
        //            if (part != null)
        //            {
        //                health HP = part.GetComponent<health>();

        //                HP.HP = HP.HP - damage;
        //            }

        //        }
        //    }
        //    else
        //    {
        //        if (TryGetComponent<health>(out health health))
        //        {
        //            health.HP = health.HP - damage;
        //        }
        //    }
        //}


        return leftHealth;
    }

    private void CheckForKinematics()
    {
        for (int k = 0; k<parts.Length; k++)
        {
            if(partRigids[k].isKinematic)
            {
                isKinematic[k] = true;
                partRigids[k].useGravity = false;
                partRigids[k].isKinematic = false;
                partRigids[k].velocity = Vector3.zero;

                kinematicsEnabled = true;
            }
            
        }
    }

}
