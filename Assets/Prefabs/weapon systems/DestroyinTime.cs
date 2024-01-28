using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyinTime : MonoBehaviour
{
    public float time = 5;
    void Update()
    {
        Destroy(gameObject, time);
    }
}
