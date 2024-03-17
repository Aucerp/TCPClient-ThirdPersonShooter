using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWhenEnter : MonoBehaviour {

	void OnCollisionEnter (Collision col) {
		if (col.collider.tag == "Selectable") {
			Destroy (col.gameObject);
		}
	}
}