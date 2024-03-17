using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnInteraction : MonoBehaviour {
    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name == "Sphere" || other.gameObject.name == "Tank"){
            Destroy(this.gameObject);
        }
    }
}
