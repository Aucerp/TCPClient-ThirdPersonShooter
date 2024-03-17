using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

	public float keyboardSpeed = 0.3f;
	public float mouseSpeed = 1f;
	public bool invertXAxis = false;
	public bool invertZAxis = false;
	public bool invertVAxis = false;
	public bool invertHAxis = false;

	void Update () {
		float xAxisValue = Input.GetAxis ("Horizontal");
		float zAxisValue = Input.GetAxis ("Vertical");

		float hAxisValue = Input.GetAxis ("Mouse Y");
		float vAxisValue = Input.GetAxis ("Mouse X");

		if (invertXAxis) // inverts X
			xAxisValue = -xAxisValue;

		if (invertZAxis) // inverts Z
			zAxisValue = -zAxisValue;

		if (invertVAxis) // inverts Horizontal Axis, for dragging the mouse
			vAxisValue = -vAxisValue;

		if (invertHAxis) // inverts Vertical Axis, for dragging the mouse
			hAxisValue = -hAxisValue;

		if (this != null) {
			this.transform.Translate (new Vector3 (xAxisValue * keyboardSpeed, 0.0f, zAxisValue * keyboardSpeed));
		}

		if (Input.GetAxis ("Fire2") != 0) {
			this.transform.Translate (vAxisValue * mouseSpeed, 0, hAxisValue * mouseSpeed);
		}
	}
}