using Ammotypes;
using Gunlist;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weaponscontrol : MonoBehaviour
{

    [SerializeField]
    bool magazineUse = false;
    [SerializeField]
    bool chambering = false;
    [SerializeField]
    bool leverAction = false;

    [SerializeField]
    InputActionProperty lFireAction;
    [SerializeField]
    InputActionProperty rFireAction;

    [SerializeField]
    float firerates;
    [SerializeField]
    float rangeM;
    [SerializeField]
    float impactforces;
    [SerializeField]
    float timetoExplosion;

    public bool activate;

    [SerializeField]
    bool chambered = false;

    public Weapons weapon;
    //public ParticleSystem activationeffect;
    public GameObject ammo;
    public GameObject casing = null;
    public GameObject unusedammo = null;
    public Transform gunbarrelend;
    public Transform ejectPointCasing;
    public Rigidbody parentrb;
    Controls playerinputactions;
    public ammo_sys ammosys;

    [SerializeField]
    float impactforce;
    [SerializeField]
    public float range;
    [SerializeField]
    float firerate;
    [SerializeField]
    float timetoexplode;

    private float timetofire = 0f;
    private Animator gunAnim;
    [SerializeField]
    float AnimTime = 1;
    public ParticleSystem sparks;
    public ParticleSystem VFX_Explode;

    public Vector3 casingDir;
    [SerializeField]
    Vector3 CasingOffset = Vector3.zero;
    public bool casingInPos = false;
    public float forceMultiplier = 0;

    private void Awake()
    {
        parentrb = GetComponentInParent<Rigidbody>();
        weapon = new Weapons();
        //ThisIsGrabbed = GetComponent<XRGrabbed>();

        playerinputactions = new Controls();
        playerinputactions.OnFoot.Enable();

                impactforce = impactforces;
                range = rangeM;
        firerate = firerates;
                timetoexplode = timetoExplosion;
         
        
      
        if(!magazineUse)
        findammo();

    }

    void Start()
    {
        parentrb = GetComponentInParent<Rigidbody>();
        //gunAnim = GetComponent<Animator>();
        //gunAnim.SetFloat("Speed", AnimTime*firerate);

        if (!magazineUse)
            findammo();
    }

    
    void Update()
    {
        //activate = ThisIsGrabbed.grabbed;

        if (activate)
        {
            var mouse = Mouse.current;
            var keyboard = Keyboard.current;

            RaycastHit hit;
            bool raycast = Physics.Raycast(gunbarrelend.position, gunbarrelend.transform.forward, out hit);
            bool AIed = transform.root.GetComponent<AIMovement>();
            bool reload = (keyboard.rKey.ReadValue() == 1) || AIed;


            bool fireCondition = EvaluateFireCondition(AIed,raycast,mouse,hit);

            if (reload && ammosys != null && !magazineUse)
            { ammosys.changeclip(); }

            if (ammosys != null)
                if (ammosys.ammo_current > 0 && !chambering)
                {
                    shoot(fireCondition);
                }
                else if(chambering)
                {
                    //if (fireCondition && timetofire <= Time.time && chambered)
                        //gunAnim.SetBool("Fire", true);

                }
        }
    }

    public void CheckIfChambered()
    {
        if(!chambered && ammosys.ammo_current > 0)
        {
            Chamber();
        }
    }

    public void ResetAnimation()
    {
        //gunAnim.SetBool("Fire", false);
    }

    private bool EvaluateFireCondition(bool AIed, bool raycast, Mouse mouse, RaycastHit hit)
    {
        if(!AIed)
        {
            // if (ThisIsGrabbed.grabbed)
            // {
            //     if (!leverAction)
            //         return (mouse.leftButton.ReadValue() == 1 || lFireAction.action.ReadValue<float>() >= 0.2 && ThisIsGrabbed.lgrabbed || rFireAction.action.ReadValue<float>() >= 0.2 && ThisIsGrabbed.rgrabbed);
            //     else
            //         return (mouse.leftButton.ReadValue() == 1 && mouse.leftButton.wasPressedThisFrame || lFireAction.action.ReadValue<float>() >= 0.2 && ThisIsGrabbed.lgrabbed && lFireAction.action.WasPressedThisFrame() || rFireAction.action.ReadValue<float>() >= 0.2 && ThisIsGrabbed.rgrabbed && rFireAction.action.WasPressedThisFrame());
            // }
             return (mouse.leftButton.ReadValue() == 1);
        }
        else
        {
            return raycast && hit.distance < range && hit.transform.gameObject.layer == 3;
        }
        return false;

    }

    public void Chamber()
    {
        if(ammosys.ammo_current > 0 && !chambered)
        {
            chambered = true;
            ammosys.reduceammo();
        }
        else if(ammosys.ammo_current > 0 && chambered)
        {
            chambered = true;

            ammosys.reduceammo();
            Rigidbody casingRb;
            GameObject casingOb;
            
            casingOb = Instantiate(unusedammo, ejectPointCasing.position + (ejectPointCasing.transform.up + CasingOffset) * ejectPointCasing.localScale.magnitude, Quaternion.LookRotation(-gunbarrelend.forward.normalized));
            casingOb.TryGetComponent<Rigidbody>(out casingRb);
            casingRb.AddForce((ejectPointCasing.transform.up + transform.rotation * casingDir) * impactforce * forceMultiplier, ForceMode.Impulse);
        }
        else if(ammosys.ammo_current <=0 && chambered)
        {
            chambered = false;

            Rigidbody casingRb;
            GameObject casingOb;

            casingOb = Instantiate(unusedammo, ejectPointCasing.position + (ejectPointCasing.transform.up + CasingOffset) * ejectPointCasing.localScale.magnitude, Quaternion.LookRotation(-gunbarrelend.forward.normalized));
            casingOb.TryGetComponent<Rigidbody>(out casingRb);
            casingRb.AddForce((ejectPointCasing.transform.up + transform.rotation * casingDir) * impactforce * forceMultiplier, ForceMode.Impulse);
        }
    }

    void shoot( bool fireCondition)
    {
        if(fireCondition && timetofire <= Time.time)
        {
                //activationeffect.Play();
                timetofire = Time.time + 1f / firerate;
                //gunAnim.SetBool("Fire", true);

                parentrb.AddForce(this.transform.forward * impactforce / 2 * -1);
                parentrb.AddForce(this.transform.up * impactforce / 2 * 1);

                Rigidbody projectile;
                GameObject bullet;

                bullet = Instantiate(ammo, gunbarrelend.position, Quaternion.LookRotation(gunbarrelend.forward.normalized));
                bullet.GetComponent<Bullet1>().sparks = sparks;
                bullet.GetComponent<Bullet1>().VFX_Explode = VFX_Explode;
                projectile = bullet.GetComponent<Rigidbody>();
                projectile.AddForce(projectile.transform.forward * impactforce, ForceMode.Impulse);

                Rigidbody casingRb;
                GameObject casingOb;
                if (casingInPos)
                    casingOb = Instantiate(casing, ejectPointCasing.position - bullet.transform.forward * bullet.transform.localScale.magnitude, Quaternion.LookRotation(-gunbarrelend.up.normalized));
                else
                    casingOb = Instantiate(casing, ejectPointCasing.position + ejectPointCasing.transform.up * ejectPointCasing.localScale.magnitude, Quaternion.LookRotation(-gunbarrelend.up.normalized));
                casingOb.TryGetComponent<Rigidbody>(out casingRb);
                casingRb.AddForce((ejectPointCasing.transform.up + transform.rotation * casingDir) * impactforce * forceMultiplier, ForceMode.Impulse);
                ammosys.reduceammo();
            
        }
        else if (!fireCondition && timetofire <= Time.time)
        {
            //gunAnim.SetBool("Fire", false);
        }

    }

    void shootChambered()
    {
        //activationeffect.Play();
        chambered = false;
        timetofire = Time.time + 1f / firerate;

            parentrb.AddForce(this.transform.forward * impactforce / 2 * -1);
            parentrb.AddForce(this.transform.up * impactforce / 2 * 1);

            Rigidbody projectile;
            GameObject bullet;

            bullet = Instantiate(ammo, gunbarrelend.position, Quaternion.LookRotation(gunbarrelend.forward.normalized));
            bullet.GetComponent<Bullet1>().sparks = sparks;
            bullet.GetComponent<Bullet1>().VFX_Explode = VFX_Explode;
            projectile = bullet.GetComponent<Rigidbody>();
            projectile.AddForce(projectile.transform.forward * impactforce, ForceMode.Impulse);

        Rigidbody casingRb;
        GameObject casingOb;

        casingOb = Instantiate(casing, ejectPointCasing.position + (ejectPointCasing.transform.up + CasingOffset) * ejectPointCasing.localScale.magnitude, Quaternion.LookRotation(-gunbarrelend.up.normalized));
        casingOb.TryGetComponent<Rigidbody>(out casingRb);
        casingRb.AddForce((ejectPointCasing.transform.up + transform.rotation * casingDir) * impactforce * forceMultiplier, ForceMode.Impulse);
            //Chamber();

    }

    void findammo()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 5);

        foreach (Collider nearbyobject in colliders)
        {
            ammo_storage ammo = nearbyobject.GetComponent<ammo_storage>();
            if(ammo != null)
            {
                ammosys = ammo.GetComponentInParent<ammo_sys>();
            }

        }
    }
}
