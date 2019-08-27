using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Movemouse : MonoBehaviour
{


    NavMeshAgent nav;
    Transform cameraMain;
    float NeedMoveVectorCam = 120;
    Vector3 VectorCam, finishPosition;


    void Start()
    {
        cameraMain = Camera.main.transform;
        nav = GetComponent<NavMeshAgent>();
        Vector3.Lerp(cameraMain.position, (transform.position + Vector3.up * NeedMoveVectorCam), Vector3.Distance(cameraMain.position, transform.position + Vector3.up * NeedMoveVectorCam));
        cameraMain.position = Vector3.Lerp(cameraMain.position, (transform.position + Vector3.up * NeedMoveVectorCam), Vector3.Distance(cameraMain.position, transform.position + Vector3.up * NeedMoveVectorCam) / 250);
    }


    void Update()
    {




        cameraMain.position = new Vector3(cameraMain.position.x, NeedMoveVectorCam, cameraMain.position.z);


        Vector3.Lerp(cameraMain.position, (transform.position + Vector3.up * NeedMoveVectorCam), Vector3.Distance(cameraMain.position, transform.position + Vector3.up * NeedMoveVectorCam));
        cameraMain.position = Vector3.Lerp(cameraMain.position, (transform.position + Vector3.up * NeedMoveVectorCam), Vector3.Distance(cameraMain.position, transform.position + Vector3.up * NeedMoveVectorCam) / 1000);




        if (Input.GetAxis("Mouse ScrollWheel") != 0 && (cameraMain.position.y > 5 || Input.GetAxis("Mouse ScrollWheel") < 0)&& (cameraMain.position.y < 200 || Input.GetAxis("Mouse ScrollWheel") > 0))
            NeedMoveVectorCam = NeedMoveVectorCam + -15 * Input.GetAxis("Mouse ScrollWheel");





        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 50000) && Input.GetMouseButton(1))
        {
            if (hit.collider.gameObject.layer != 8)
            {
                finishPosition = hit.point;
                nav.SetDestination(finishPosition);

            }
        }



    }



}
