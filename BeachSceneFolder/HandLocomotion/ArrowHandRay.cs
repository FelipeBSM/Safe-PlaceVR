using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(LineRenderer))]
public class ArrowHandRay : MonoBehaviour
{

    [Header("Settings")]

    public GameObject teleportLocationFeedback;

    public GameObject caster;

    public GameObject playerObject;

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

    private bool hasTeleported = false;

    void Start()
    {

        if (rayMaterial == null)
        {
            rayMaterial = GetComponent<MeshRenderer>().material;
        }

        lineRender = GetComponent<LineRenderer>();
        lineRender.material = rayMaterial;
        lineRender.startWidth = rayWidthStart;
        lineRender.endWidth = rayWidthEnd;

    }


    public void SwitchAiming()
    {
        isAiming = !isAiming;
    }

    private void CheckDestination()
    {



            Ray ray = new Ray(caster.transform.position, -(caster.transform.rotation * Vector3.forward));
           

            //Debug.DrawRay(ray.origin, ray.direction * rayCastDistance, Color.red);


            RaycastHit hit;

            IgnoreLayer = ~IgnoreLayer;

            if (Physics.Raycast(ray.origin, ray.direction, out hit, rayCastDistance, IgnoreLayer))
            {

                if (hit.collider.tag == "Floor")
                {
                    //Teleporting Area
                    destination = hit.point;
                    //set teleport ghost
                    teleportLocationFeedback.SetActive(true);
                    teleportLocationFeedback.transform.position = new Vector3(destination.x, teleportLocationFeedback.transform.position.y, destination.z);

                    

                    lineRender.enabled = true;
                    lineRender.positionCount = 2;
                    lineRender.SetPosition(0, ray.origin);
                    lineRender.SetPosition(1, teleportLocationFeedback.transform.position);

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
    private void OnTriggerExit(Collider other)
    {
        if (isAiming == true)
            hasTeleported = false;
    }
}
