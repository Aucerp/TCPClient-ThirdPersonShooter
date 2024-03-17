using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverChangeMaterial : MonoBehaviour {

	private Material currentMaterial;
	public Material hoverMaterial;

    void Start(){
        currentMaterial = this.GetComponent<MeshRenderer>().material;
    }

    void OnMouseOver()
    {
        this.GetComponent<MeshRenderer>().material = hoverMaterial;
    }

    void OnMouseExit()
    {
        this.GetComponent<MeshRenderer>().material = currentMaterial;
    }



}
