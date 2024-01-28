using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Ammotypes;

public class Bullet1 : MonoBehaviour
{
    public enum ammotypes { rifledefault,LMGdefault,LMGHE,GLS }

    [SerializeField]
    bool useAmmoTypes;
    [SerializeField]
    bool handgun;

    public ammotypes bullets;
    Rigidbody projectile;
    Collider projCollider;
    ammo ammotype;

    Vector3 penvelocity;
    Vector3 distancetoobject;

    [SerializeField]
    float damage;
    [SerializeField]
    float armorpen;
    [SerializeField]
    bool explosive;
    [SerializeField]
    float HEradius;
    [SerializeField]
    float splashdamage;
    [SerializeField]
    float timetoexplode;
    float explodealarm = 0;

    float startingarmor;
    float startingHEarmor;

    Vector3 impactpoint;

    public ParticleSystem sparks;
    public ParticleSystem VFX_Explode;
    public GameObject bulletHole;
    Vector3 holePos;
    Transform holeParent;
    Quaternion holeRotation;
    float instantiationTime = 1000;

    [SerializeField]
    bool grenade;
    [SerializeField]
    bool arm;
    public bool activate;

    float grenadeExplodeTime = 0;


    void Awake()
    {
        projectile = GetComponent<Rigidbody>();
        ammotype = new ammo();
        projCollider = GetComponent<Collider>();

        if(useAmmoTypes)
        {
            if (bullets == ammotypes.rifledefault)
            {
                damage = ammotype.RifleDefault.damage;
                armorpen = ammotype.RifleDefault.armorpen;
                explosive = ammotype.RifleDefault.explosive;
                HEradius = ammotype.RifleDefault.HEradius;
                splashdamage = ammotype.RifleDefault.splashdamage;
                timetoexplode = ammotype.RifleDefault.timetoexplode;

            }
            if (bullets == ammotypes.LMGdefault)
            {
                damage = ammotype.LMGDefault.damage;
                armorpen = ammotype.LMGDefault.armorpen;
                explosive = ammotype.LMGDefault.explosive;
                HEradius = ammotype.LMGDefault.HEradius;
                splashdamage = ammotype.LMGDefault.splashdamage;
                timetoexplode = ammotype.LMGDefault.timetoexplode;
            }
            if (bullets == ammotypes.LMGHE)
            {
                damage = ammotype.LMGHE.damage;
                armorpen = ammotype.LMGHE.armorpen;
                explosive = ammotype.LMGHE.explosive;
                HEradius = ammotype.LMGHE.HEradius;
                splashdamage = ammotype.LMGHE.splashdamage;
                timetoexplode = ammotype.LMGHE.timetoexplode;
            }
            if (bullets == ammotypes.GLS)
            {
                damage = ammotype.GLS.damage;
                armorpen = ammotype.GLS.armorpen;
                explosive = ammotype.GLS.explosive;
                HEradius = ammotype.GLS.HEradius;
                splashdamage = ammotype.GLS.splashdamage;
                timetoexplode = ammotype.GLS.timetoexplode;
            }
        }

        if(!grenade)
        explodealarm = Time.fixedTime + timetoexplode;
        
    }

    // Update is called once per frame
    void Update()
    {

        if(activate && !arm)
        {
            var mouse = Mouse.current;
            Debug.Log(mouse.leftButton.ReadValue() + " " + mouse.leftButton.wasPressedThisFrame);
           arm = (mouse.leftButton.ReadValue() == 1 && mouse.leftButton.wasPressedThisFrame);
        }

        if (grenade && arm && explodealarm == 0)
        {
            explodealarm = timetoexplode;
        }

        if(explodealarm != 0 && arm && grenade)
        {
            grenadeExplodeTime += Time.deltaTime;

            if(grenadeExplodeTime > explodealarm) explode();
        }

        if (explodealarm < Time.fixedTime && timetoexplode != 0 && !grenade)
        {
            explode();
        }
        
        if (explosive == false)
        {
            Destroy(gameObject, 2f);
        }
        if (explosive == true && timetoexplode == 0)
        {
            Destroy(gameObject, 5f);
        }
        instantiationTime += Time.deltaTime;
        
    }

    private void FixedUpdate()
    {
        if (projCollider.isTrigger)
        {
            Ray ray = new Ray();
            ray.origin = transform.position; ray.direction = -transform.forward;
            RaycastHit rh;
            projCollider.Raycast(ray, out rh, 1f);

            if (rh.collider != null)
                projCollider.isTrigger = false;
        }

        if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, projectile.velocity.magnitude))
        {
            if(hitInfo.transform.TryGetComponent<health>(out health hp))
            {
                holePos = hitInfo.point + hitInfo.normal * 0.08f;
                holeRotation = Quaternion.FromToRotation(Vector3.forward, -hitInfo.normal);
                holeParent = hitInfo.transform;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!grenade)
        {
            float armorratiopen;
            health targetstats = collision.collider.GetComponent<health>();
            penvelocity = projectile.velocity;

            if (targetstats != null && timetoexplode == 0)
            {
                Vector3 projectilepos = transform.forward;
                impactpoint = collision.contacts[0].point;

                targetstats.forceVector = projectile.velocity.normalized;
                targetstats.force = projectile.velocity.magnitude * projectile.mass;
                targetstats.contactPoint = collision.contacts[0].point;
                targetstats.cuttingNormal = gameObject.transform.right;

                Vector3 objectpos = new Vector3(impactpoint.x - transform.position.x, impactpoint.y - transform.position.y, impactpoint.z - transform.position.z);
                float angle = Vector3.Angle(projectilepos.normalized, objectpos.normalized);
                if (angle < (70 - (targetstats.armor * 5)))
                {
                    if (armorpen != 0)
                    { armorratiopen = targetstats.armor * Mathf.Clamp((angle / 1.5f), 0, armorpen) / armorpen; }
                    else
                    { armorratiopen = targetstats.armor; }

                    if (armorratiopen <= 0.2f)
                    {
                        targetstats.HP = targetstats.HP - Mathf.Clamp(damage - targetstats.armor, 0, damage);
                        CreateBulletHole(holeParent, holePos, holeRotation);
                    }

                    if (armorpen > 0)
                    {

                        targetstats.armor = targetstats.armor - Mathf.Clamp(Mathf.Log((armorpen / (0.37f * targetstats.startingarmor * (angle + targetstats.startingarmor / 17))), 10) * 0.2f, 0, targetstats.armor);
                        CreateBulletHole(holeParent, holePos, holeRotation);
                    }


                    if (armorratiopen <= 1)
                    {
                        armorpen = Mathf.Clamp((armorpen - targetstats.armor), 0f, armorpen);
                        projectile.velocity = transform.forward * Mathf.Clamp((penvelocity.magnitude - (targetstats.armor * 25)), 0, penvelocity.magnitude);
                        projCollider.isTrigger = true;
                        targetstats.penned = true;


                    }
                    else
                    {
                        Destroy(gameObject);
                    }

                }
                else
                {
                    if (explosive)
                        explode();
                    else
                        spark(penvelocity);
                }

            }
            if (explosive && explodealarm < Time.fixedTime)
            {
                explode();
            }
            if (targetstats == null && !explosive)
            {
                spark(penvelocity);
            }
        }
       

    }

    private void OnTriggerExit(Collider other)
    {
        projCollider.isTrigger = false;
    }

    void explode()
    {
        if(VFX_Explode.particleCount > VFX_Explode.maxParticles- HEradius * 10)
        VFX_Explode.Clear();
        VFX_Explode.transform.position = transform.position - projectile.velocity * 0.0001f;
        VFX_Explode.transform.up = transform.up;
        VFX_Explode.startSize = HEradius;
        VFX_Explode.startSpeed = HEradius*10;
        VFX_Explode.Emit((int)(HEradius * 10));



        Collider[] colliders = Physics.OverlapSphere(transform.position, HEradius);

        foreach(Collider nearbyobject in colliders)
        {

            Rigidbody rb = nearbyobject.GetComponent<Rigidbody>();
            if(rb == null)
            {
                rb = nearbyobject.GetComponentInParent<Rigidbody>();
            }
            if (rb == null)
            {
                rb = nearbyobject.GetComponentInChildren<Rigidbody>();
            }
            if (rb != null)
            {
                rb.AddExplosionForce(HEradius * splashdamage*1000, transform.position, HEradius);
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
            if(targetstats != null)
            {
                
                targetstats.HP = targetstats.HP - Mathf.Clamp(((splashdamage - Mathf.Clamp(((distancetoobject.magnitude / (HEradius))*splashdamage), 0f, splashdamage)) - (targetstats.armor * 3)),0f,targetstats.HP);

                if(targetstats.HEarmor >0)
                { targetstats.HEarmor = targetstats.HEarmor - (Mathf.Clamp(((splashdamage - Mathf.Clamp(((distancetoobject.magnitude / (HEradius)) * splashdamage), 0, splashdamage))) / Mathf.Clamp((targetstats.startingHEarmor * targetstats.startingHEarmor),1, targetstats.startingHEarmor * targetstats.startingHEarmor), 0f, targetstats.HEarmor)); }
                else
                { targetstats.armor = targetstats.armor - (Mathf.Clamp(((splashdamage - Mathf.Clamp(((distancetoobject.magnitude / (HEradius))*splashdamage),0,splashdamage)))/Mathf.Clamp(targetstats.startingarmor,1,targetstats.startingarmor), 0f, targetstats.armor)); }

            }
            
            
        }

        Destroy(gameObject);
    }


    void spark(Vector3 velocity)
    {
        if(handgun)
        {
            sparks.transform.position = transform.position - projectile.velocity * 0.0025f;
            sparks.transform.forward = velocity;
            sparks.startSpeed = velocity.magnitude/10;
            sparks.Emit(3);
        }
        else
        {
            sparks.transform.position = transform.position - projectile.velocity * 0.005f;
            sparks.transform.forward = velocity;
            sparks.startSpeed = velocity.magnitude;
            sparks.Emit(3);
        }
        
    }

    void CreateBulletHole(Transform parent, Vector3 position, Quaternion rotation)
    {
        if (instantiationTime > 1)
        { 
            GameObject hole = Instantiate(bulletHole, position, rotation, parent); instantiationTime = 0;
            hole.transform.rotation *= Quaternion.AngleAxis(Random.Range(-180f, 180f), hole.transform.forward);
        }
    }

}
