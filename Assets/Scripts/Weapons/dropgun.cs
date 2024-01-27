using System.Collections;
using Gunlist;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class dropgun : MonoBehaviour
{
    public enum droptypes {explosivecap }

    public droptypes guns;
    public Weapons weapon;
    public ParticleSystem VFX_Explode;
    //public ParticleSystem activationeffect;
    public Rigidbody parentrb;
    float HEradius;
    float splashdamage;
    float range;
    float timetoexplode;
    float explodealarm;
    float ejectionforce;
    Vector3 distancetoobject;
    public Transform targetingpoint;
    bool armed = false;
    float viewRadius;
    float viewAngle = 20;
    float timer = 0;

    private void Awake()
    {
        weapon = new Weapons();

        if (guns == droptypes.explosivecap)
        {
            HEradius = weapon.explosivecap.HEradius;
            splashdamage = weapon.explosivecap.splashdamage;
            timetoexplode = weapon.explosivecap.timetoexplode;
            ejectionforce = weapon.explosivecap.ejectforce;
        }
       

    }


    void Update()
    {

        var mouse = Mouse.current;
        range = parentrb.velocity.magnitude * timetoexplode;
        viewRadius = range;

        if (mouse.rightButton.ReadValue()==1 && !transform.root.GetComponent<AIMovement>())
        {
            shoot();
        }

        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - targetingpoint.transform.position).normalized;

            if (Vector3.Angle(-transform.right, dirToTarget) < viewAngle / 2)
            {
                RaycastHit hit;
                bool raycast = Physics.Raycast(targetingpoint.position, dirToTarget, out hit);

                if (transform.root.GetComponent<AIMovement>() && raycast && hit.collider.tag == "Player")
                {

                    float distanceCalc = Mathf.Abs(hit.rigidbody.velocity.magnitude * timetoexplode - (parentrb.velocity.magnitude / 2) * timetoexplode - hit.distance);
                    //-HEradius/3
                    if (distanceCalc <= HEradius && distanceCalc > -HEradius)
                    {
                        shoot();
                        explodealarm = timetoexplode;
                    }
                    
                }
            }
        }

        if(explodealarm != 0)
        {
            timer += Time.deltaTime;
        }
        

        if (explodealarm < timer && timetoexplode != 0 && armed)
        {
            explode();
        }
    }

    void shoot()
    {
        //activationeffect.Play();
        this.GetComponent<Rigidbody>().isKinematic = false;
        this.GetComponent<Rigidbody>().useGravity = true;
        this.transform.SetParent(null);
        this.GetComponent<Rigidbody>().velocity = parentrb.velocity/2;
        this.GetComponent<Rigidbody>().AddForce(transform.forward * ejectionforce, ForceMode.Impulse);
        //set rb velocity to parents velocity
        armed = true;

    }

    void explode()
    {
        VFX_Explode.transform.position = transform.position;
        VFX_Explode.transform.up = transform.up;
        VFX_Explode.startSize = HEradius;
        VFX_Explode.startSpeed = HEradius * 10;
        VFX_Explode.Emit((int)(HEradius * 10));

        Collider[] colliders = Physics.OverlapSphere(transform.position, HEradius);

        foreach (Collider nearbyobject in colliders)
        {

            Rigidbody rb = nearbyobject.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = nearbyobject.GetComponentInParent<Rigidbody>();
            }
            if (rb == null)
            {
                rb = nearbyobject.GetComponentInChildren<Rigidbody>();
            }
            if (rb != null)
            {
                rb.AddExplosionForce(HEradius * splashdamage * 1000, transform.position, HEradius);
            }

            if (nearbyobject is TerrainCollider)
            {
                distancetoobject = distancetoobject - transform.position;
            }
            else
            {
                distancetoobject = Physics.ClosestPoint(transform.position, nearbyobject, nearbyobject.transform.position, nearbyobject.transform.rotation);
                distancetoobject = distancetoobject - transform.position;
            }



            health targetstats = nearbyobject.GetComponent<health>();
            if (targetstats != null)
            {

                targetstats.HP = targetstats.HP - Mathf.Clamp(((splashdamage - Mathf.Clamp(((distancetoobject.magnitude / (HEradius)) * splashdamage), 0f, splashdamage)) - (targetstats.armor * 3)), 0f, targetstats.HP);

                if (targetstats.HEarmor > 0)
                { targetstats.HEarmor = targetstats.HEarmor - (Mathf.Clamp(((splashdamage - Mathf.Clamp(((distancetoobject.magnitude / (HEradius)) * splashdamage), 0, splashdamage))) / Mathf.Clamp((targetstats.startingHEarmor * targetstats.startingHEarmor), 1, targetstats.startingHEarmor * targetstats.startingHEarmor), 0f, targetstats.HEarmor)); }
                else
                { targetstats.armor = targetstats.armor - (Mathf.Clamp(((splashdamage - Mathf.Clamp(((distancetoobject.magnitude / (HEradius)) * splashdamage), 0, splashdamage))) / Mathf.Clamp(targetstats.startingarmor, 1, targetstats.startingarmor), 0f, targetstats.armor)); }

            }


        }

        Destroy(gameObject);
    }

    public void manualExplode()
    {
        explode();
    }

    private void OnCollisionEnter(Collision collision)
    {
        shoot();
        explodealarm = timetoexplode;
    }
}
