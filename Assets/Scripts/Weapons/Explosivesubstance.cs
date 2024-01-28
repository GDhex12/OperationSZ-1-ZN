using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ammotypes;

public class Explosivesubstance : MonoBehaviour
{
    health health;
    Rigidbody projectile;
    Vector3 distancetoobject;
    float HEradius = 0f;
    float splashdamage = 0f;
    ammo_storage ammo_exists;
    ammo_sys ammosys;
    ammo ammotype;

    public ParticleSystem VFX_Explode;

    float damagebullet;
    float HEradiusbullet;
    float splashdamagebullet;

    void Start()
    {
        health = GetComponentInChildren<health>();
        if (health == null) health = GetComponent<health>();
        TryGetComponent(out ammo_exists);
        ammotype = new ammo();

        if (ammo_exists != null)
        {
            ammo_sys ammosys = ammo_exists.GetComponentInParent<ammo_sys>();
            Bullet1 bullets = ammosys.ammotype.GetComponent<Bullet1>();

            if (bullets.bullets == Bullet1.ammotypes.rifledefault)
            {
                damagebullet = ammotype.RifleDefault.damage;
                HEradiusbullet = ammotype.RifleDefault.HEradius;
                splashdamagebullet = ammotype.RifleDefault.splashdamage;
            }
            if (bullets.bullets == Bullet1.ammotypes.LMGdefault)
            {
                damagebullet = ammotype.LMGDefault.damage;
                HEradiusbullet = ammotype.LMGDefault.HEradius;
                splashdamagebullet = ammotype.LMGDefault.splashdamage;
            }
            if (bullets.bullets == Bullet1.ammotypes.LMGHE)
            {
                damagebullet = ammotype.LMGHE.damage;
                HEradiusbullet = ammotype.LMGHE.HEradius;
                splashdamagebullet = ammotype.LMGHE.splashdamage;
            }
            if (bullets.bullets == Bullet1.ammotypes.GLS)
            {
                damagebullet = ammotype.GLS.damage;
                HEradiusbullet = ammotype.GLS.HEradius;
                splashdamagebullet = ammotype.GLS.splashdamage;
            }
        }
    }


    void Update()
    {
        if (health.HP <= 0)
        {      
            if (ammo_exists != null)
            {
                ammo_sys ammosys = ammo_exists.GetComponentInParent<ammo_sys>();
                splashdamage = ammosys.ammo_combined * (damagebullet + splashdamagebullet);
                    
                if (splashdamage > 0)
                {
                    HEradius = ammosys.ammo_combined * HEradiusbullet;
                    explode();
                }
            }
        }
        
    }

    void explode()
    {
        if (VFX_Explode.particleCount > VFX_Explode.maxParticles - HEradius * 10)
            VFX_Explode.Clear();
        VFX_Explode.transform.position = transform.position;
        VFX_Explode.transform.up = transform.up;
        VFX_Explode.startSize = HEradius;
        VFX_Explode.startSpeed = HEradius * 10;
        VFX_Explode.Emit((int)(HEradius * 30));

        Collider[] colliders = Physics.OverlapSphere(transform.position, HEradius);

        foreach (Collider nearbyobject in colliders)
        {

           

            if (nearbyobject is TerrainCollider)
            {
                distancetoobject = distancetoobject - transform.position;
            }
            else
            {
                distancetoobject = Physics.ClosestPoint(transform.position, nearbyobject, nearbyobject.transform.position, nearbyobject.transform.rotation);
                distancetoobject = distancetoobject - transform.position;
            }

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
                rb.AddExplosionForce(splashdamage*1000, transform.position, distancetoobject.magnitude);
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

        Destroy(health.transform.parent.gameObject);
    }
}
