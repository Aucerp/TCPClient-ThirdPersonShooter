using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{

    public GameObject cubePrefab;
    private GameObject instanecubePrefab;
    public List<GameObject> cubeArray = new List<GameObject>();

    public float numberOfObjects = 0;

    public float upForce = 25f;
    public float sideForce = 7f;
    public bool forceActive = true;
    private Vector3 force;
    public bool randomPosition = true;
    private Vector3 positionSpawn;

    public GameObject cubeDraw;

    private float xMin, xMax, yMin, yMax, zMin, zMax;

    void FixedUpdate()
    {

        for (int i = 0; cubeArray.Count < numberOfObjects; i++) // check array count to match the objects spawn count
        {

            if (randomPosition == true) // random position from CubeDraw values
            {

                xMin = cubeDraw.GetComponent<CubeDraw>().xMin.x;
                xMax = cubeDraw.GetComponent<CubeDraw>().xMax.x;
                yMin = cubeDraw.GetComponent<CubeDraw>().yMin.y;
                yMax = cubeDraw.GetComponent<CubeDraw>().yMax.y;
                zMin = cubeDraw.GetComponent<CubeDraw>().zMin.z;
                zMax = cubeDraw.GetComponent<CubeDraw>().zMax.z;

                positionSpawn = new Vector3(Random.Range(xMin, xMax), Random.Range(yMin, yMax), Random.Range(zMin, zMax)); // generate random position for each gameobject
            }
            else
            {
                positionSpawn = transform.position; // if not get position from parent
            }

            instanecubePrefab = Instantiate(cubePrefab, positionSpawn, Quaternion.identity); // instantiate the gameobject
            instanecubePrefab.transform.parent = gameObject.transform; // parent instance gameobject to container
            
            if (forceActive == true)
            {

                float xForce = Random.Range(-sideForce, sideForce);
                // float yForce = Random.Range(upForce / 2f, upForce);
                float yForce = Random.Range(upForce / 2f, upForce);
                float zForce = Random.Range(-sideForce, sideForce);

                force = new Vector3(xForce, yForce, zForce);

                instanecubePrefab.GetComponent<Rigidbody>().velocity = force;
            }

            cubeArray.Add(instanecubePrefab as GameObject);
        }

        for (int i = 0; i < cubeArray.Count; i++) // verify each gameobject from array, if one is destroyed then remove it from array too in order for new object to spawn
        {
            if (cubeArray[i] == null)
            {
                cubeArray.Remove(cubeArray[i]);
            }
        }


    }

}
