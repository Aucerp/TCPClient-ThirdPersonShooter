using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeDraw : MonoBehaviour {

	private Vector3 boundPoint1, boundPoint2, boundPoint3, boundPoint4, boundPoint5, boundPoint6, boundPoint7, boundPoint8;
	[HideInInspector]	
	public Vector3 xMin, xMax, yMin, yMax, zMin, zMax;

	public float nodeCubeSize = .2f;
	public float nodePlaneSize = 2f;
    private Vector3 nodeCube;
    public bool facePlane = false;
    public bool disableOnStart = false;

    void Start(){
        if(disableOnStart == true){
            this.gameObject.SetActive(false);
        }
    }

    void OnDrawGizmos()
    {

        boundPoint1 = this.GetComponent<Collider>().bounds.min;
        boundPoint2 = this.GetComponent<Collider>().bounds.max;
        boundPoint3 = new Vector3(boundPoint1.x, boundPoint1.y, boundPoint2.z);
        boundPoint4 = new Vector3(boundPoint1.x, boundPoint2.y, boundPoint1.z);
        boundPoint5 = new Vector3(boundPoint2.x, boundPoint1.y, boundPoint1.z);
        boundPoint6 = new Vector3(boundPoint1.x, boundPoint2.y, boundPoint2.z);
        boundPoint7 = new Vector3(boundPoint2.x, boundPoint1.y, boundPoint2.z);
        boundPoint8 = new Vector3(boundPoint2.x, boundPoint2.y, boundPoint1.z);

        nodeCube = new Vector3(nodeCubeSize, nodeCubeSize, nodeCubeSize);


        Gizmos.color = new Color(0, 1, 0, 0.5F);
    
        xMin = new Vector3(boundPoint1.x, boundPoint4.y + (boundPoint1.y - boundPoint4.y) / 2 , boundPoint3.z + (boundPoint1.z - boundPoint3.z) / 2 );
        xMax = new Vector3(boundPoint5.x, boundPoint4.y + (boundPoint1.y - boundPoint4.y) / 2, boundPoint7.z + (boundPoint5.z - boundPoint7.z) / 2 );

        yMin = new Vector3(boundPoint5.x + (boundPoint1.x - boundPoint5.x) / 2, boundPoint1.y , boundPoint3.z + (boundPoint5.z - boundPoint3.z) / 2 );
        yMax = new Vector3(boundPoint4.x + (boundPoint8.x - boundPoint4.x) / 2, boundPoint8.y , boundPoint8.z + (boundPoint6.z - boundPoint8.z) / 2 );

        zMin = new Vector3(boundPoint5.x + (boundPoint1.x - boundPoint5.x) / 2, boundPoint3.y + (boundPoint6.y - boundPoint3.y) / 2, boundPoint3.z);
        zMax = new Vector3(boundPoint3.x + (boundPoint7.x - boundPoint3.x) / 2, boundPoint1.y + (boundPoint4.y - boundPoint1.y) / 2, boundPoint8.z);

        Vector3 nodePlaneX = new Vector3(0, nodePlaneSize, nodePlaneSize);
        Vector3 nodePlaneY = new Vector3(nodePlaneSize, 0, nodePlaneSize);
        Vector3 nodePlaneZ = new Vector3(nodePlaneSize, nodePlaneSize, 0);


		if(facePlane == true){
            Gizmos.DrawCube(xMin, nodePlaneX);
            Gizmos.DrawCube(xMax, nodePlaneX);
            Gizmos.DrawCube(yMin, nodePlaneY);
            Gizmos.DrawCube(yMax, nodePlaneY);
            Gizmos.DrawCube(zMin, nodePlaneZ);
            Gizmos.DrawCube(zMax, nodePlaneZ);
		}


        Gizmos.color = new Color(1, 0, 0, 0.5F);

        Gizmos.DrawCube(boundPoint1, nodeCube);
        Gizmos.DrawCube(boundPoint2, nodeCube);
        Gizmos.DrawCube(boundPoint3, nodeCube);
        Gizmos.DrawCube(boundPoint4, nodeCube);
        Gizmos.DrawCube(boundPoint5, nodeCube);
        Gizmos.DrawCube(boundPoint6, nodeCube);
        Gizmos.DrawCube(boundPoint7, nodeCube);
        Gizmos.DrawCube(boundPoint8, nodeCube);

        Gizmos.DrawLine(boundPoint6, boundPoint2);
        Gizmos.DrawLine(boundPoint2, boundPoint8);
        Gizmos.DrawLine(boundPoint8, boundPoint4);
        Gizmos.DrawLine(boundPoint4, boundPoint6);

        Gizmos.DrawLine(boundPoint3, boundPoint7);
        Gizmos.DrawLine(boundPoint7, boundPoint5);
        Gizmos.DrawLine(boundPoint5, boundPoint1);
        Gizmos.DrawLine(boundPoint1, boundPoint3);

        Gizmos.DrawLine(boundPoint6, boundPoint3);
        Gizmos.DrawLine(boundPoint2, boundPoint7);
        Gizmos.DrawLine(boundPoint8, boundPoint5);
        Gizmos.DrawLine(boundPoint4, boundPoint1);

    }
}
