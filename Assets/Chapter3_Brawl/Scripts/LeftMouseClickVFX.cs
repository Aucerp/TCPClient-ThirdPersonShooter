using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftMouseClickVFX : MonoBehaviour
{
    private GameObject containerLMB;
    public GameObject prefab_LeftMouseClickVFX;
    public Vector3 vfxScale = Vector3.one; // Variable to control VFX scale

    private RaycastHit hit;
    private Ray ray;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // on LMB 
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition); //cast ray relative to mose position
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.tag == "Terrian")
                { // spawn mouse animation only on Ground
                    if (containerLMB != null) // check in container is empty, if not delete gameobject
                    {
                        Destroy(containerLMB);
                    }
                    containerLMB = Instantiate(prefab_LeftMouseClickVFX, new Vector3(hit.point.x, hit.point.y, hit.point.z), transform.rotation); // spawn gameobject on mouse position relative to ground, assign gameobject to container
                    containerLMB.transform.localScale = vfxScale; // Set the scale of the instantiated VFX
                }
            }
        }
    }
}
