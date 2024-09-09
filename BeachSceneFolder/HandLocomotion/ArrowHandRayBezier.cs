using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(LineRenderer))]
public class ArrowHandRayBezier : MonoBehaviour
{

    [Header("Settings")]

    public GameObject teleportLocationFeedback; // point two - aka - hit location

    public GameObject caster,casterRay; // point1

    public GameObject midPointLocal; // agular point used to do the bezier angularaty
    public Vector3 angularPointOffset;

    public GameObject playerObject;

    public Vector3 rayAngle;

    [Tooltip("Max distance of the ray.")]
    public float rayCastDistance;

    [Tooltip("Offset of the ray starting point.")]
    public Vector3 originOffset;

    [Tooltip("Ray material. This is the displayed(rendered) material")]
    public Material rayMaterial;

    [Tooltip("Ray witdh. This is the tickness of the displayed ray.")]
    public float rayWidthStart = 1.5f;
    public float rayWidthEnd = 1f;

    public bool isAiming = false;
    public bool aimHitGround = false;

    public LayerMask IgnoreLayer;

    private Vector3 destination;
    private LineRenderer lineRender;

    private int numberOfPoints = 25;
    private Vector3[] positions = new Vector3[25];

    private bool hasTeleported = false;

    void Start()
    {

        if (rayMaterial == null)
        {
            rayMaterial = GetComponent<MeshRenderer>().material;
        }
        IgnoreLayer = ~IgnoreLayer;
        lineRender = GetComponent<LineRenderer>();
        lineRender.material = rayMaterial;
        lineRender.startWidth = rayWidthStart;
        lineRender.endWidth = rayWidthEnd;
       
        //lineRender.positionCount = numberOfPoints;


    }


    public void SwitchAiming()
    {
        isAiming = !isAiming;
    }

    private void CheckDestination()
    {



        Ray ray = new Ray(caster.transform.position, -(caster.transform.rotation * rayAngle));


       // Debug.DrawRay(ray.origin, ray.direction * rayCastDistance, Color.red);
     

        RaycastHit hit;

        

        if (Physics.Raycast(ray.origin, ray.direction, out hit, rayCastDistance, IgnoreLayer))
        {
            //casterRay.GetComponent<LineRenderer>().positionCount = 2;
            //casterRay.GetComponent<LineRenderer>().SetPosition(0, ray.origin);
            //casterRay.GetComponent<LineRenderer>().SetPosition(1, hit.point);

            if (hit.collider.tag == "Floor")
            {
                midPointLocal.transform.position = MidPointCalculation(caster.transform.position, hit.point) + angularPointOffset; // seting the mid point

                //Teleporting Area
                destination = hit.point;
                //set teleport ghost
                teleportLocationFeedback.SetActive(true);
                teleportLocationFeedback.transform.position = new Vector3(destination.x, teleportLocationFeedback.transform.position.y, destination.z);

                // BEZIER CURVE CALCULATION
                DrawQuadraticCurve(caster.transform.position,hit.point,midPointLocal.transform.position);
                lineRender.enabled = true;
                lineRender.positionCount = numberOfPoints;
                lineRender.SetPositions(positions);

                aimHitGround = true;
                //Debug.Log("Hit Location: " + destination);

            }
            else
            {
                aimHitGround = false;
                lineRender.enabled = false;
                lineRender.positionCount = 0;
                teleportLocationFeedback.SetActive(false);
                return;
            }

        }
    }
    private Vector3 MidPointCalculation(Vector3 p0, Vector3 p1)
    {
        return new Vector3((p0.x + p1.x) / 2, (p0.y + p1.y) / 2, (p0.z + p1.z) / 2);
    }
    private void DrawQuadraticCurve(Vector3 p0, Vector3 p1, Vector3 mid)
    {
        for(int i = 0; i < numberOfPoints; i++)
        {
            float t = i / (float)numberOfPoints;
            positions[i] = CalculateBezierCurve(t, p0, p1, mid);
        }
    }

    private Vector3 CalculateBezierCurve(float t, Vector3 p0, Vector3 p1, Vector3 anglePoint)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = uu * p0;
        p += 2 * u * t * anglePoint;
        p += tt * p1;

        return p;
    }
    private Vector3 MultiplieVectors(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }
    void FixedUpdate()
    {
        if (isAiming == true)
        {
            CheckDestination();
        }
        else
        {
            //not show teleport
            lineRender.enabled = false;
            lineRender.positionCount = 0;
            teleportLocationFeedback.SetActive(false);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("aaaaaaaaa");
        if(aimHitGround == true)
        {
            if (other.gameObject.tag == "LeftHandTrigger" && hasTeleported == false)
            {
                Debug.Log("collided");
                playerObject.transform.position = new Vector3(destination.x, playerObject.transform.position.y, destination.z);
                hasTeleported = true;
            }
        }
       
    }
    public void MoveToLocation()
    {
        Debug.Log("aaaaaaaaa");
        if (aimHitGround == true)
        {
            if(hasTeleported == false)
            {
                playerObject.transform.position = new Vector3(destination.x, playerObject.transform.position.y, destination.z);
                hasTeleported = true;
            }
          
            
        }
    }
    public void DesableMovement()
    {
        if (isAiming == true)
            hasTeleported = false;
    }
    private void OnTriggerExit(Collider other)
    {
        if (isAiming == true)
            hasTeleported = false;
    }
}
