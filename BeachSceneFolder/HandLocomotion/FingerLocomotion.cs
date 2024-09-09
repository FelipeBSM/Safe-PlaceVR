using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerLocomotion : MonoBehaviour
{
    private GameObject player;
    private GameObject camera;
    private GameObject fingerTip;
    private SkinnedMeshRenderer handRenderR,handRenderL;
    private Material handMaterial,newHandMaterial;

    private AudioSource deniedSource;

    private int ammountOfRays;
    private float rayDistance;
    private float distanceBetweenRays = 1.5f;
    private Vector3 yOffsetRay;

    private Ray[] raycast;

    private bool initialized = false;

    private LayerMask ignoreLayer;

    private bool canMove = true;
    private bool canMoveTrigger = true;
    private bool canMoveDistance = true;

    bool allSpeed = true;

    private float incrementAmmount;
    private float distanceToWall;

    public void SetPlayer(GameObject _player, GameObject _cam, Vector3 _rayOffset, float _rayDistance, int _ammountOfRays, float _distanceBRays, LayerMask _ignoreLayer, SkinnedMeshRenderer _handR, SkinnedMeshRenderer _handL, Material newMaterial, AudioSource ad, GameObject _finger, float _incrementAmmount)
    {
        this.ignoreLayer = ~_ignoreLayer;
        this.player = _player;
        this.camera = _cam;
        this.fingerTip = _finger;
        this.yOffsetRay = _rayOffset;
        this.ammountOfRays = _ammountOfRays;
        this.rayDistance = _rayDistance;
        this.distanceBetweenRays = _distanceBRays;
        this.handRenderR = _handR;
        this.handRenderL = _handL;
        this.handMaterial = handRenderR.material;
        this.newHandMaterial = newMaterial;
        this.deniedSource = ad;
        this.incrementAmmount = _incrementAmmount;
        raycast = new Ray[ammountOfRays];
        initialized = true;
    }

    public void ChangePlayerPosition()
    {
       
        if (canMove && canMoveTrigger && canMoveDistance)
        {
            if(allSpeed == true)
            {
                //mover com um
                incrementAmmount = 0.8f;
            }
            else
            {
                incrementAmmount = distanceToWall-0.09f;
            }

            Vector3 pnew = fingerTip.transform.forward * incrementAmmount;
            pnew = new Vector3(pnew.x, 0, pnew.z);
            //pnew = new Vector3(Mathf.RoundToInt(pnew.x), Mathf.RoundToInt(pnew.y), Mathf.RoundToInt(pnew.z));
            Debug.Log(pnew);

            player.transform.position += pnew;
            canMove = false;
        }
        else
        {
            //play audio

            deniedSource.Play();
        }
      
    }
    public void CheckFingerTip()
    {
        Ray ray = new Ray(fingerTip.transform.position, (fingerTip.transform.rotation * Vector3.forward));
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red);
        RaycastHit hit;

        if (Physics.Raycast(ray.origin, ray.direction, out hit, rayDistance, ignoreLayer))
        {
            //d=?((x2 – x1)² + (y2 – y1)²)
            allSpeed = false;
            float operation = ((hit.point.x - ray.origin.x) * (hit.point.x - ray.origin.x) + (hit.point.y - ray.origin.y)* (hit.point.y - ray.origin.y));
            distanceToWall = Mathf.Sqrt(operation);
            
            

            //fingerTip.GetComponent<LineRenderer>().SetPosition(0, ray.origin);
            //fingerTip.GetComponent<LineRenderer>().SetPosition(1,hit.point);

            if (distanceToWall <= 0.5)
            {
                canMoveDistance = false;
            }
            else
            {
                canMoveDistance = true;

            }
            allSpeed = false;
            //Debug.Log("Distance To Wall: " + distanceToWall);

            
        }
        else
        {
            //Debug.Log("no hit");

            allSpeed = true;
            canMoveDistance = true;
        }


           
        
    }
    public static Vector3 Round(Vector3 vector3, int decimalPlaces = 2)
    {
        float multiplier = 1;
        for (int i = 0; i < decimalPlaces; i++)
        {
            multiplier *= 10f;
        }
        return new Vector3(
            Mathf.Round(vector3.x * multiplier) / multiplier,
            Mathf.Round(vector3.y * multiplier) / multiplier,
            Mathf.Round(vector3.z * multiplier) / multiplier);
    }

    public void GenerateDetectionRays()
    {
        for(int i=0;i < ammountOfRays; i++)
        {
            Vector3 rayOrigin = new Vector3(camera.transform.position.x, camera.transform.position.y -distanceBetweenRays * i, camera.transform.position.z);
            Ray ray = new Ray(rayOrigin, (camera.transform.rotation * Vector3.forward));
            raycast[i] = ray;

            RaycastHit hit;

            if(Physics.Raycast(raycast[i].origin, raycast[i].direction, out hit, rayDistance, ignoreLayer))
            {
                //Debug.Log("Hit something: RAY " + i);
                canMove = false;
                handRenderR.material = newHandMaterial;
                handRenderL.material = newHandMaterial;

            }
            else
            {
                handRenderR.material = handMaterial;
                handRenderL.material = handMaterial;
                canMove = true;
               
            }

            //Debug.DrawRay(raycast[i].origin, raycast[i].direction * rayDistance, Color.red);
        }
    }


    private void Update()
    {
        if(initialized)
            CheckFingerTip();

        if(canMove && canMoveTrigger && canMoveDistance)
        {
            handRenderR.material = handMaterial;
            handRenderL.material = handMaterial;
        }
        else
        {
            handRenderR.material = newHandMaterial;
            handRenderL.material = newHandMaterial;
        }
        /*if (initialized)
            GenerateDetectionRays();*/



    }

    private void OnTriggerEnter(Collider other)
    {     
        Debug.Log("Ta no trigger parceiro, volta ai");
        canMoveTrigger = false;
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Ta no trigger parceiro, volta ai");
        canMoveTrigger = true;
    }

    public void ResetMovement()
    {
        canMove = true;
    }
}

