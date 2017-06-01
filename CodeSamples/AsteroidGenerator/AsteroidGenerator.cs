/*
below code creates a randomely generated minecraft style asteroid by choosing random indexes in a 3d array as 'centers of mass' than builds up the area around the center of mass.
its resolution is halved to remove strange noise. 
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AsteroidGenerator : MonoBehaviour {
	public Shader style;
	public int massMultiplier;
	private class Rection
	{
		public int x, y, z;
		public Rection(int ex, int wy, int ze){
			x = ex;
			y = wy;
			z = ze;
		}
	}

	private int[ , ] centerPositions;


	private int quadThreshold = 5;
	private Rection[] quadCombos = new Rection[8];
	private Vector3 massCenter;
	private float threeDHypotenuse(int x, int y, int z, int xTwo, int yTwo, int zTwo){
		float twoDHyp = Mathf.Sqrt( ( ( x - xTwo) * ( x - xTwo) ) + ( y - yTwo) * ( y - yTwo) );// a2 + b2 == c2
		float threeDHyp = Mathf.Sqrt( ( ( z - zTwo) * ( z - zTwo) ) + (twoDHyp * twoDHyp) );
		return threeDHyp;
	}

	void Awake(){
		quadCombos [0] = new Rection (0, 0, 0);
		quadCombos[1] = new Rection (0, 0, 1);
		quadCombos[2] = new Rection (0, 1, 0);
		quadCombos[3] = new Rection (0, 1, 1);
		quadCombos[4] = new Rection (1, 0, 0);
		quadCombos[5] = new Rection (1, 0, 1);
		quadCombos[6] = new Rection (1, 1, 0);
		quadCombos[7] = new Rection (1, 1, 1);
	}
	public GameObject Generate (int width, int height, int length, int numberOfCenters, int distanceRadius, Vector3 scale, int rand) {


		bool[,,] positions = new bool[width, height, length];
		centerPositions = new int[numberOfCenters, 3];

		string seed = System.DateTime.Now.ToString();
		System.Random pseudoRandom = new System.Random(seed.GetHashCode() * rand);
		int xCenter = 0;
		int yCenter = 0;
		int zCenter = 0;
		for (int i = 0; i < numberOfCenters; i++)
        { //sets the centers of mass for the asteroid

            centerPositions [i, 0] = pseudoRandom.Next (distanceRadius, width - distanceRadius);
			centerPositions [i, 1] = pseudoRandom.Next (distanceRadius, height - distanceRadius);
			centerPositions [i, 2] = pseudoRandom.Next (distanceRadius, length - distanceRadius);
			xCenter += centerPositions [i, 0] / 2;
			yCenter += centerPositions [i, 1] / 2;
			zCenter += centerPositions [i, 2] / 2;
		}
		massCenter = new Vector3 (xCenter / numberOfCenters, yCenter / numberOfCenters, zCenter / numberOfCenters);//determines true center of mass to set transform of eventual gameobject
		List<Rection> tweenPos = new List<Rection>();
		for (int i = 0; i < numberOfCenters; i++) {//below adds 'mini' mass centers between mass centers in case the closest mass center is too far away, i.e is disconnected from asteroid
			for (int j = 0; j < numberOfCenters; j++) {
				if (i == j) {
					continue;
				}
				float centerDist = threeDHypotenuse (centerPositions [i, 0], centerPositions [i, 1], centerPositions [i, 2], centerPositions [j, 0], centerPositions [j, 1], centerPositions [j, 2]);
				if (centerDist > distanceRadius * 2 && centerDist <= distanceRadius * 3) {

					int x = (centerPositions [i, 0] - centerPositions [j, 0]) / 2;
					if (centerPositions [i, 0] >= centerPositions [j, 0]) {
						x = centerPositions [i, 0] - x;
					} else {
						x = centerPositions [j, 0] - x;
					}
					////////////////////////////////
					int y = (centerPositions [i, 1] - centerPositions [j, 1]) / 2;
					if (centerPositions [i, 1] >= centerPositions [j, 1]) {
						y = centerPositions [i, 1] - y;
					} else {
						y = centerPositions [j, 1] - y;
					}
					/////////////////////////
					int z = (centerPositions [i, 2] - centerPositions [j, 2]) / 2;
					if (centerPositions [i, 2] >= centerPositions [j, 2]) {
						z = centerPositions [i, 2] - z;
					} else {
						z = centerPositions [j, 2] - z;
					}


					tweenPos.Add (new Rection (x, y, z));
				}
			}
		}



		//checks all mass centers to see if current position is part of asteroid
		for (int x = 0; x < width; x++){
			for (int y = 0; y < height; y++){
				for (int z = 0; z < length; z++){
					float closestDistance = 100000000000;

					for (int i = 0; i < numberOfCenters; i++) {

						int xTwo = centerPositions [i, 0];
						int yTwo = centerPositions [i, 1];
						int zTwo = centerPositions [i, 2];
						float twoDHyp = Mathf.Sqrt( ( ( x - xTwo) * ( x - xTwo) ) + ( y - yTwo) * ( y - yTwo) );// a2 + b2 = c2
						float threeDHyp = Mathf.Sqrt( ( ( z - zTwo) * ( z - zTwo) ) + (twoDHyp * twoDHyp) );
						if (threeDHyp < closestDistance) {
							closestDistance = threeDHyp;
						}
					}
					if (closestDistance <= ( (pseudoRandom.Next(0, 100) - 50) * 0.01 + 1 ) * distanceRadius) {
						positions [x, y, z] = true;
					} else {
						positions [x, y, z] = false;
						float tweenDistance = 1000000000000;
						for (int i = 0; i < tweenPos.Count; i++) {
							float currentDist = threeDHypotenuse (x, y, z, tweenPos [i].x, tweenPos [i].y, tweenPos [i].z);
							if (currentDist < tweenDistance) {
								tweenDistance = currentDist;
							}
						}
						if (tweenDistance <= ((pseudoRandom.Next (0, 100) - 50) * 0.01 + 1) * distanceRadius / 2) {
							positions [x, y, z] = true;
						}
					}


				}
			}
		}
		bool[,,] compressedMesh = new bool[width / 2, height / 2, length / 2];
		int mass = 0;
		for (int x = 0; x < width; x+= 2) {//below creates a lower resolution asteroid, used as a smoothing process
			for (int y = 0; y < height; y+= 2) {
				for (int z = 0; z < length; z+= 2) {
					int boxCount = 0;
					for (int j = 0; j < quadCombos.Length; j++) {
						if (positions [quadCombos [j].x + x , quadCombos [j].y + y, quadCombos [j].z + z] == true) {
							boxCount++;
						}
					}
					if (boxCount >= quadThreshold){
						//
						compressedMesh[x/2,y/2,z/2] = true;
						mass++;
					} else {
						compressedMesh[x / 2, y / 2, z / 2] = false;
					}
				}
			}
		}



		Mesh neoMesh = new Mesh();
		List<Vector3> vertices= new List<Vector3> ();
        for (int x = 0; x < (width / 2) - 1; x++)
        {//below creates border from edge of array
            for (int y = 0; y < (height / 2) - 1; y++)
            {
                for (int z = 0; z < (length / 2) - 1; z++)
                {
                    if (x == 0 || x == (width / 2) - 1)
                    {
                        compressedMesh[x, y, z] = false;
                    }
                    if (y == 0 || y == (height / 2) - 1)
                    {
                        compressedMesh[x, y, z] = false;
                    }
                    if (z == 0 || z == (length / 2) - 1)
                    {
                        compressedMesh[x, y, z] = false;
                    }
                }
            }
        }


        for (int x = 0; x < (width / 2) - 1; x++)
        {//below sets values
            for (int y = 0; y < (height / 2) - 1; y++)
            {
                for (int z = 0; z < (length / 2) - 1; z++)
                {

                    if (compressedMesh [x, y, z] == true) {

						if (compressedMesh [x - 1, y, z] == false) {
							vertices.Add(Vector3.Scale( new Vector3(x, y, z) - massCenter,  scale) );
							vertices.Add (Vector3.Scale(new Vector3 (x, y, z + 1) - massCenter, scale ));
							vertices.Add(Vector3.Scale(new Vector3(x, y + 1, z + 1) - massCenter, scale) );
							vertices.Add(Vector3.Scale(new Vector3(x, y + 1, z) - massCenter, scale) );
						}


						if (compressedMesh [x, y - 1, z] == false) {
							vertices.Add(Vector3.Scale(new Vector3(x, y - 1, z) - massCenter, scale) );
							vertices.Add(Vector3.Scale(new Vector3(x + 1, y - 1, z) - massCenter, scale) );
							vertices.Add(Vector3.Scale(new Vector3(x + 1, y - 1, z + 1) - massCenter, scale) );
							vertices.Add (Vector3.Scale(new Vector3 (x, y - 1, z + 1) - massCenter, scale) );
						}


						if (compressedMesh [x, y, z - 1] == false) {
							vertices.Add(Vector3.Scale(new Vector3(x, y, z) - massCenter, scale) );
							vertices.Add(Vector3.Scale(new Vector3(x, y + 1, z) - massCenter, scale) );
							vertices.Add(Vector3.Scale(new Vector3(x + 1, y + 1, z) - massCenter, scale) );
							vertices.Add(Vector3.Scale(new Vector3(x + 1 , y, z) - massCenter, scale) );

						}


						if (compressedMesh [x + 1, y, z] == false) {
							vertices.Add(Vector3.Scale(new Vector3(x + 1, y, z) - massCenter, scale) );
							vertices.Add(Vector3.Scale(new Vector3(x + 1, y+1, z)  - massCenter, scale));
							vertices.Add(Vector3.Scale(new Vector3(x + 1, y+1, z + 1) - massCenter , scale));
							vertices.Add(Vector3.Scale(new Vector3(x + 1, y, z+1) - massCenter, scale));

						}


						if (compressedMesh [x, y + 1, z] == false) {
							vertices.Add(Vector3.Scale(new Vector3(x, y + 1, z)  - massCenter, scale));
							vertices.Add(Vector3.Scale(new Vector3(x, y + 1, z+ 1)  - massCenter, scale));
							vertices.Add(Vector3.Scale(new Vector3(x + 1, y + 1, z + 1) - massCenter , scale));
							vertices.Add(Vector3.Scale(new Vector3(x+ 1, y + 1, z)  - massCenter, scale));
						}


                        if (compressedMesh [x, y, z + 1] == false) {
							vertices.Add(Vector3.Scale(new Vector3(x, y, z + 1) - massCenter, scale));
							vertices.Add(Vector3.Scale(new Vector3(x + 1 , y, z + 1)  - massCenter, scale));
							vertices.Add(Vector3.Scale(new Vector3(x + 1, y + 1, z + 1)  - massCenter, scale));
							vertices.Add(Vector3.Scale(new Vector3(x, y + 1, z + 1)  - massCenter, scale));
						}

					}
				}
			}
		}

		neoMesh.vertices = vertices.ToArray ();
		List<int> tris = new List<int> ();
		for (int i = 0; i < neoMesh.vertices.Length; i += 4) {
			tris.Add (i);
			tris.Add (i + 1);
			tris.Add (i + 2);
			tris.Add (i);
			tris.Add (i + 2);
			tris.Add (i + 3);
		}
		neoMesh.triangles = tris.ToArray ();
		neoMesh.RecalculateNormals ();
		GameObject newRoid = new GameObject ();
		newRoid.AddComponent<MeshFilter> ();
		newRoid.AddComponent<MeshRenderer> ();
		newRoid.AddComponent<Rigidbody> ();
		newRoid.GetComponent<Rigidbody> ().mass = mass * massMultiplier;
		newRoid.AddComponent<BoxCollider> ().size = new Vector3(width * scale.x/2f, height * scale.y/2f, length * scale.z/2f);

		newRoid.GetComponent<MeshFilter> ().sharedMesh = neoMesh;
		newRoid.GetComponent<MeshRenderer> ().material = new Material (style);
		newRoid.GetComponent<MeshRenderer>().material.SetColor ("_OutlineColor", Color.HSVToRGB(Random.Range(0,100) * 0.01f, 1.0f, 1.0f ));
		return newRoid;
	}
}
