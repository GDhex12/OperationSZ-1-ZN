using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XZrotation : MonoBehaviour
{
    Canvas canv;
    GameObject canvg;
    public GameObject model;
    public GameObject target;
    Vector3 canvPosHeightNull;
    private void Start()
    {
        if (model == null)
        {
            canv = GetComponentInChildren<Canvas>();
            if (canv == null)
            {
                canvg = this.gameObject;
            }
        }
        else
        {
            canvg = this.gameObject;
        }
    }
    // Update is called once per frame
    void Update()
    {

        if (target != null)
        {
            Vector3 targetPosHeightNull = new Vector3(target.transform.position.x, 0, target.transform.position.z);
            if (canv != null)
            {
                canvPosHeightNull = new Vector3(canv.transform.position.x, 0, canv.transform.position.z);
                Quaternion newQuat = Quaternion.LookRotation(targetPosHeightNull - canvPosHeightNull, Vector3.up);
                canv.transform.rotation = Quaternion.Lerp(canv.transform.rotation, newQuat, Time.deltaTime);
            }
            else
            {
                canvPosHeightNull = new Vector3(canvg.transform.position.x, 0, canvg.transform.position.z);
                Quaternion newQuat = Quaternion.LookRotation(targetPosHeightNull - canvPosHeightNull, Vector3.up);
                canvg.transform.rotation = Quaternion.Lerp(canvg.transform.rotation, newQuat, Time.deltaTime);
            }
        }
    }
}
