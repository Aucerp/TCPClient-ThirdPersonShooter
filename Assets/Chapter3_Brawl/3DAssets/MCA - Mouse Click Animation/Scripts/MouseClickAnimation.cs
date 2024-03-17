using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseClickAnimation : MonoBehaviour {

    private GameObject animationLeftMouseButton;
    private GameObject containerLMB;
    public GameObject[] animationLeftMouseButtonArray;
    private int currentLeftMouseButton = 0;

    public GameObject moveRightMouseButton;
    private GameObject containerRMB;

    private RaycastHit hit;
    private Ray ray;

    void Start () {
        if (animationLeftMouseButtonArray == null)
            Debug.LogError ("Add Animation for LMB, currently there's none associated");
        if (moveRightMouseButton == null)
            Debug.LogError ("Add Animation for RMB, currently there's none associated");

        containerRMB = Instantiate (moveRightMouseButton, Vector3.zero, Quaternion.identity); // spawns gameobject for RMB on 
        containerRMB.SetActive (false);
        QualitySettings.vSyncCount = 0; // disable vSync
    }

    void Update () {
        if (Input.GetKeyUp ("space")) {
            if (currentLeftMouseButton < (animationLeftMouseButtonArray.Length - 1)) {
                currentLeftMouseButton++;
            } else {
                currentLeftMouseButton = 0;
            }
        }
        animationLeftMouseButton = animationLeftMouseButtonArray[currentLeftMouseButton];

        if (Input.GetButtonDown ("Fire1")) // on LMB 
        {
            ray = Camera.main.ScreenPointToRay (Input.mousePosition); //cast ray relative to mose position
            if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {
                if (hit.collider.tag == "Ground") { // spawn mouse animation only on Ground
                    if (containerLMB != null) // check in container is empty, if not delete gameobject
                    {
                        Destroy (containerLMB);
                    }
                    containerLMB = Instantiate (animationLeftMouseButton, new Vector3 (hit.point.x, hit.point.y, hit.point.z), transform.rotation); // spawn gameobject on mouse position relative to ground, assign gameobject to container
                }
            }
        }
        if (Input.GetAxis ("Fire2") != 0) { // continuous executes the statment
            ray = Camera.main.ScreenPointToRay (Input.mousePosition); //cast ray relative to mose position

            if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {
                if (hit.collider.tag == "Ground") { // spawn mouse animation only on Ground
                    containerRMB.transform.position = new Vector3 (hit.point.x, hit.point.y, hit.point.z); // set gameobject position to hit position
                    containerRMB.SetActive (true); // enable the gameobject
                }
            }
        } else {
            containerRMB.transform.position = Vector3.zero; // resets gameobject to default position
            containerRMB.SetActive (false); // disables the gameobject
        }

    }
}