using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class ammo_sys : MonoBehaviour
{
    public float ammo_combined = 0f;
    public float ammo_current = 0f;
    public GameObject ammotype;
    public List<ammo_storage> ammo_storages;
    Controls playerinputactions;
    public int storagenum = 0;

    void Start()
    {
        playerinputactions = new Controls();
        playerinputactions.OnFoot.Enable();

        Updateammo();

    }

    public void changeclip()
    {

            for (int storeamm = 0; storeamm < ammo_storages.Count; storeamm++)
            {
                if (ammo_storages[storeamm].AmmoCount > 0 && (ammo_combined - ammo_storages[storeamm].AmmoCount * ammo_storages.Count) < 0)
                {
                    storagenum = storeamm;
                    ammo_current = ammo_storages[storagenum].AmmoCount;
                }
            }
        
    }

    // Update is called once per frame
    public void reduceammo()
    {
        float ammouse = 1;
        if (ammo_storages != null && ammo_storages.Count > 0)
        {

                if(ammo_storages[storagenum].AmmoCount > 0 && ammouse > 0)
                {
                    float ammo_level = ammo_storages[storagenum].AmmoCount;
                    ammo_storages[storagenum].AmmoCount = ammo_storages[storagenum].AmmoCount - ammouse;
                    ammouse = ammouse - ammo_level;
                }

                
            
        }
        Updateammo();
    }

    void Updateammo()
    {
        ammo_combined = 0;

        if (ammo_storages == null || ammo_storages.Count != GetComponentsInChildren<ammo_storage>().Length)
        {
            ammo_storages = GetComponentsInChildren<ammo_storage>().ToList();
        }
        if (ammo_storages != null && ammo_storages.Count > 0)
        {
            for (int storagenum = 0; storagenum < ammo_storages.Count; storagenum++)
            {
                ammo_combined += ammo_storages[storagenum].AmmoCount;
            }
            ammo_current = ammo_storages[storagenum].AmmoCount;
        }
    }
}
