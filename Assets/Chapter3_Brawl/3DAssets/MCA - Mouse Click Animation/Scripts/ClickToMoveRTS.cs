using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToMoveRTS : MonoBehaviour {

	public int smooth;
	private Vector3 targetPosition;
	public Vector3 targetPoint;

	
	void Update () {
	
		if(Input.GetKeyDown(KeyCode.Mouse0)) {
	
			smooth=1;
			Plane playerPlane = new Plane(Vector3.up, transform.position);
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			float hitdist = 0f;
	
			if (playerPlane.Raycast(ray, out hitdist)) {
	
				targetPoint = ray.GetPoint(hitdist);
				targetPosition = ray.GetPoint(hitdist);
                Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
				transform.rotation = targetRotation;
			}
		}
		// transform.position = Vector3.Slerp(transform.position, targetPosition, Time.deltaTime * smooth);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * smooth * 6); 
	}

}
