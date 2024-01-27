using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class RemoveHoles : MonoBehaviour
{
    DecalProjector projector;
    Camera cameraMain;

    private void Start()
    {
        projector = GetComponentInChildren<DecalProjector>();
        cameraMain = Camera.main;
    }
    void Update()
    {
        if ((cameraMain.transform.position - transform.position).magnitude > projector.drawDistance) Destroy(gameObject);
    }
}
