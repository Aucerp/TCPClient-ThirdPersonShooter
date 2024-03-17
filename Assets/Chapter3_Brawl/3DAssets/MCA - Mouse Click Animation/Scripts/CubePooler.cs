using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubePooler : MonoBehaviour {

	[System.Serializable]
	public class Pool{
		public string tag;
		public GameObject prefab;
		public int size;
	}

	public static CubePooler Instance;
	public int poolSize;

    public float upForce = 25f;
    public float sideForce = 7f;
    public bool forceActive = true;
    private Vector3 force;
    public bool randomPosition = true;
    private Vector3 positionSpawn;

    public GameObject cubeDraw;
    private float xMin, xMax, yMin, yMax, zMin, zMax;
	private float startpoolSize;


	private void Awake(){
		Instance = this;
	}

	public List<Pool> pools;
	public Dictionary<string, Queue<GameObject>> poolDictionary;

	void Start () {
		poolDictionary = new Dictionary<string, Queue<GameObject>>();
		foreach(Pool pool in pools){
			Queue<GameObject> objectPool = new Queue<GameObject>();
			for(int i=0; i<pool.size; i++){
				GameObject obj = Instantiate(pool.prefab);
                obj.transform.parent = gameObject.transform;
				obj.SetActive(false);
				objectPool.Enqueue(obj);
			}
            startpoolSize = pool.size;
			poolDictionary.Add(pool.tag, objectPool);
		}
	}


    public GameObject SpawnFromPool( string tag, Vector3 position, Quaternion rotation){

		if(!poolDictionary.ContainsKey(tag)){
			Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
			return null;
		}

		GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        poolSize += 1;

		objectToSpawn.SetActive(true);
		objectToSpawn.transform.position = position;
		objectToSpawn.transform.rotation = rotation;
        objectToSpawn.transform.parent = gameObject.transform;

        DestroyOnInteraction destroyObject = objectToSpawn.GetComponent<DestroyOnInteraction>();

		// if(destroyObject != null){
        //     poolDictionary[tag].Enqueue(objectToSpawn);
		// 	poolSize -= 1;
		// }

		return objectToSpawn;
	}

    void FixedUpdate()
    {

        if (randomPosition == true)
        {

            xMin = cubeDraw.GetComponent<CubeDraw>().xMin.x;
            xMax = cubeDraw.GetComponent<CubeDraw>().xMax.x;
            yMin = cubeDraw.GetComponent<CubeDraw>().yMin.y;
            yMax = cubeDraw.GetComponent<CubeDraw>().yMax.y;
            zMin = cubeDraw.GetComponent<CubeDraw>().zMin.z;
            zMax = cubeDraw.GetComponent<CubeDraw>().zMax.z;

            positionSpawn = new Vector3(Random.Range(xMin, xMax), Random.Range(yMin, yMax), Random.Range(zMin, zMax));
        }
        else
        {
            positionSpawn = transform.position;
        }


        SpawnFromPool("Cube", positionSpawn, Quaternion.identity);

    }

}
