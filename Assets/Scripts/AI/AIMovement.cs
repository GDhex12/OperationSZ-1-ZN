using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class AIMovement : MonoBehaviour
{
    Vector2 movement_dir;
    bool run;
    bool jump;
    bool onGround;
    Rigidbody rb;
    [SerializeField]
    private float speed = 1;
    [SerializeField]
    private float runSpeed = 2;
    [SerializeField]
    private float drag = 2;
    [SerializeField]
    private float maxWalkSpeed = 5;
    [SerializeField]
    private float maxRunSpeed = 10;
    [SerializeField]
    private float jumpHeight = 2;
    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private float gravity;
    [SerializeField]
    private float lerp;

    [SerializeField]
    private float viewDistance;
    [SerializeField]
    private float viewConeDegrees;
    [SerializeField]
    private Transform playerTransform;


    int newpathindex = 0;
    int oldpathindex = 0;

    Vector3 movement_rel;

    NavMeshPath navigationPath; 
    bool recalculatePath;
    float timeToRecalculate = 0.5f;
    float time = 5;
    Vector3 Path;
    [SerializeField]
    GameObject[] Waypoints;
    List<Vector3> pathPoints = new List<Vector3>();
    int destpoint = 0;

    bool playerSeen = false;
    [SerializeField]
    float circlingRadius = 10;
    Vector3 circle;
    [SerializeField]
    float waypointDelayFixed=3;
    [SerializeField]
    bool randomisedDelay;
    [SerializeField]
    float waypointDelayMax = 5;
    [SerializeField]
    float waypointDelayMin = 0;
    float waypointDelay = 3;
    float timeDelay = 0;

    private void Awake()
    {
        navigationPath = new NavMeshPath();
    }

    // Start is called before the first frame update
    void Start()
    { 
        rb = GetComponent<Rigidbody>();
        Path = Waypoints[0].transform.position;
    }

    private void Update()
    {

        if (navigationPath != null && recalculatePath)
        {
            //NavMesh.AllAreas
            NavMesh.CalculatePath(transform.position, Path, NavMesh.AllAreas, navigationPath);
            recalculatePath = false;
            pathPoints.Clear();
            foreach (Vector3 point in navigationPath.corners)
            {
                pathPoints.Add(point);
            }
        }
        Debug.Log(timeDelay);
        if(randomisedDelay && waypointDelay < timeDelay)
        {
            UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
            timeDelay = UnityEngine.Random.Range(waypointDelayMin, waypointDelayMax);
        }
        if(!randomisedDelay)
        {
            timeDelay = waypointDelayFixed;
        }

        if((Path - transform.position).magnitude <5)
        {
            timeDelay += Time.deltaTime;
        }

        time += Time.deltaTime;
        if (time > timeToRecalculate)
        {
            time = 0;
            recalculatePath = true;
        }

        if(waypointDelay < timeDelay)
        findtarget();

    }

    void findtarget()
    {
        if (Path != Waypoints[destpoint].transform.position && !playerSeen)
        { Path = Waypoints[destpoint].transform.position; recalculatePath = true; }

        if ((Waypoints[destpoint].transform.position - transform.position).magnitude < 2f && !playerSeen)
        {
            recalculatePath = true;
            GotoNextPoint();
        }

        if(playerSeen)
        {
            CircleAround();
        }
    }

    void CircleAround()
    {
        
        UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
        if(circle == Vector3.zero)
        circle = new Vector3((UnityEngine.Random.value - 0.5f) * 2, 0, (UnityEngine.Random.value - 0.5f) * 2).normalized * circlingRadius;
        if ((Path - transform.position).magnitude < 5f)
        circle = new Vector3((UnityEngine.Random.value-0.5f)*2, 0 ,(UnityEngine.Random.value-0.5f)*2).normalized * circlingRadius;
        

        Path = playerTransform.position + circle;
        timeDelay = 0;
    }

    void GotoNextPoint()
    {
        // Returns if no points have been set up
        if (Waypoints.Length == 0)
        { return; }



        int deltaindex = (newpathindex - oldpathindex) / Mathf.Clamp(Mathf.Abs(newpathindex - oldpathindex), 1, Waypoints.Length);



        if (deltaindex >= 0)
        { destpoint = (destpoint + 1); }
        else
        { destpoint = (destpoint - 1); }


        newpathindex = destpoint;

        if (destpoint >= Waypoints.Length)
        { destpoint = 0; }
        if (destpoint < 0)
        { destpoint = 0; }
        // Set the agent to go to the currently selected destination.
        Path = Waypoints[destpoint].transform.position;
        timeDelay = 0;
        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (pathPoints.Count > 1)
        {
            movement_rel = new Vector3(pathPoints[1].x - transform.position.x, 0, pathPoints[1].z - transform.position.z).normalized;
            if((pathPoints[1]-transform.position).magnitude < 2 && pathPoints.Count > 2)
            {
                movement_rel = new Vector3(pathPoints[2].x - transform.position.x, 0, pathPoints[2].z - transform.position.z).normalized;
            }
        }
        if ((transform.position - Path).magnitude < 2f)
            movement_rel = Vector3.zero;

            if (movement_rel.magnitude != 0)
        {
            if (!run)
                rb.AddForce(movement_rel * (maxWalkSpeed - rb.velocity.magnitude) * speed);
            else if (run)
                rb.AddForce(movement_rel * (maxRunSpeed - rb.velocity.magnitude) * runSpeed);

            rb.velocity = new Vector3(movement_rel.x, rb.velocity.normalized.y, movement_rel.z) * rb.velocity.magnitude;
        }
        else if (movement_rel.magnitude == 0)
        {
            rb.AddForce(-rb.velocity * drag);
        }
        rb.AddForce(transform.up * -gravity);
        Vector3 vecInterpol = Vector3.Slerp(transform.forward, movement_rel, lerp);
        if((transform.position - Path).magnitude > 5f)
        transform.rotation = Quaternion.LookRotation(vecInterpol);

        float dotResult = (90 - viewConeDegrees) / 90;

        if (!playerSeen)
            if ((playerTransform.position - transform.position).magnitude < viewDistance)
                if (Vector3.Dot(transform.forward, (playerTransform.position - transform.position).normalized) > dotResult)
                {
                    RaycastHit hit;
                    Physics.Raycast(transform.position, (playerTransform.position - transform.position).normalized, out hit, viewDistance);
                if (hit.transform.gameObject.layer == 3) 
                    playerSeen = true;
                }

        if (playerSeen)
            if ((playerTransform.position - transform.position).magnitude > 50) playerSeen = false;

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (navigationPath != null)
        {
            for (int i = 0; i < pathPoints.Count; i++)
            {

                if (i < pathPoints.Count) { Gizmos.color = Color.red; Gizmos.DrawSphere(pathPoints[i], 1); }


                // if (i < pathPoints.Count - 1) { Debug.DrawLine(pathPoints[i], pathPoints[i + 1], UnityEngine.Color.red); }





            }
        }

    }

}
