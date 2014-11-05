using UnityEngine;
using System.Collections;
using System;

[RequireComponent (typeof(CharacterController))]

public class cameraControl : MonoBehaviour
{	
	private bool isOrthographic;

    public GameObject[] targets;
	//public GameObject captureBox;
	public bool dubugText = true;
	public float[] yClamp = {5, 25};
	public float[] zClamp = {25, 20};
	public float maxDistanceAway = 40f;
	public Vector3 avgDistance;

    // Starts the camera, gets the player targest, and gets the diagonal distance of the room
    public void Start ()
    {
        Console.WriteLine ("START");
        targets = GameObject.FindGameObjectsWithTag ("Player");
		//captureBox = GameObject.FindGameObjectWithTag ("CaptureBox");
        //avgBox = GameObject.FindGameObjectWithTag ("avgBox");
        // Square diagonal = Sqrt(2) * Length 
        //roomDiagonal = 1.414f * roomLength;
        if (Camera.main) {
            isOrthographic = Camera.main.orthographic;
        }
    }

    

    public void LateUpdate ()
    {
        // If we can't find a player, we return without altering the camera's position
        if (!GameObject.FindWithTag ("Player")) {
            return;
        }

		// We sum the positions of all of the players, and from there we find the mid point between all of them
		Vector3 sum = new Vector3 (0, 0, 0);
		for (int i = 0; i < targets.Length; i++) {
			sum += targets [i].transform.position;
		}
		avgDistance = sum / targets.Length;
		//captureBox.transform.position = avgDistance;
		
		// Next, we find what the biggest difference in distance between any two characters is
		float[] largestDifferenceInfo = returnLargestDifference ();
		float largestDifference = largestDifferenceInfo [0];
		float maxX = largestDifferenceInfo [1];
		float maxZ = largestDifferenceInfo [2];

		float distanceOffset = largestDifference / maxDistanceAway;
		float yOffset = Mathf.Lerp (yClamp [0], yClamp [1], distanceOffset);
		float zOffset = Mathf.Lerp (zClamp [0], zClamp [1], distanceOffset);

		// The camera is set to its new position, pointed to the point to look at. 
		Camera.main.transform.position = new Vector3 (avgDistance.x, yOffset, avgDistance.z + zOffset);
		Camera.main.transform.LookAt(avgDistance);

                
    }

    // Gets the largest distance between any two players, and returns it
    public float[] returnLargestDifference ()
    {
        float currentDistance = 0.0f;
        float largestDistance = 0.0f;
		float xMax = 0.0f;
		float zMax = 0.0f;
        for (int i = 0; i < targets.Length; i++) {
            for (int j = 0; j < targets.Length; j++) {
				if (i == j) 
					continue;
                currentDistance = Vector3.Distance (targets [i].transform.position, targets [j].transform.position);
                if (currentDistance > largestDistance) {
                    largestDistance = currentDistance;
					xMax = targets[i].transform.position.x - targets[j].transform.position.x;
					zMax = targets[i].transform.position.z - targets[j].transform.position.z;
                }
            }
        }
		
		float[] toReturn = {largestDistance, xMax, zMax};
		return toReturn;
    }
}

// This is the old code used in the previous version of the camera. Its kept in case we need to refer back on our
// previous methods for new ideas, or alterations to the current code. 

/* LESS OLD CODE
 * 
 * private bool isOrthographic;
    public GameObject[] targets;
    //public GameObject avgBox;
    public float currentDistance;
    public float largestDistance;
    public float height = 5.0f;
    public Vector3 avgDistance;
    public float distance = 5.0f;                    // Default Distance 
    public float speed = 1;
    public float offset;
    public float roomLength = 48f;
    private float roomDiagonal;
    public float xRatio = 0f;
    public float yzRatio = 0f;
    public float offZRatio = 0f;
    public float offXRatio = 0f;
    public float[] xClamp = {-8,8};
    public float[] yClamp = {10,30};
    public float[] zClamp = {7,27};
    public float[] offsetClamp = {10, -10, 10, -10};
	public bool dubugText = true;
 * 
        // We sum the positions of all of the players, and from there we find the mid point between all of them
        Vector3 sum = new Vector3 (0, 0, 0);
        for (int i = 0; i < targets.Length; i++) {
            sum += targets [i].transform.position;
        }
        avgDistance = sum / targets.Length;

        // Next, we find what the biggest difference in distance between any two characters is
        float largestDifference = returnLargestDifference ();
                
        // The camera is done via clamping. The positions of the player weights how big of an offset
        // is used. This allows us to zoom in and out, and shift the camera to the left and right
        // to keep the room mostly in room. 

        // The xRatio of the room is what decides how far to the left or right the camera will shift
        // It is the current position of the players center divided by the rooms length
        xRatio = (avgDistance.x / roomLength) + .5f;
        float tempX = avgDistance.x + Mathf.Lerp (xClamp [0], xClamp [1], xRatio);

        // The yzRatio is the largest difference between two characters divided by the length. This is
        // used to decide the position of the Y, and Z of the camera depending on exactly where the players are
        yzRatio = largestDistance / roomLength;
        float tempY = Mathf.Lerp (yClamp [0], yClamp [1], yzRatio);
        float tempZ = avgDistance.z + 5 + Mathf.Lerp (zClamp [0], zClamp [1], yzRatio);

        // The offZRatio decides the exact position where the camera looks at. This lets the camera not always look
        // dead center, which is sometimes needed to finetune the exacts of the camera
        offZRatio = (avgDistance.z / roomLength) + .5f;
        float lookZOffset = Mathf.Lerp (offsetClamp [0], offsetClamp [1], offZRatio);
        float lookXOffset = Mathf.Lerp (offsetClamp [2], offsetClamp [3], xRatio);

        // The camera is set to its new position, pointed to the point to look at. 
        Camera.main.transform.position = new Vector3 (tempX, tempY, tempZ);
        Camera.main.transform.LookAt(new Vector3 ((avgDistance.x + lookXOffset), avgDistance.y, (avgDistance.z + lookZOffset)));


        // 'AVG BOX' code is simply debug code that's useful when trying to figure out how the camera is working. It 
        // is set to be where ever the camera is looking at. 
        //avgBox.transform.position = new Vector3 ((avgDistance.x + lookXOffset), avgDistance.y, (avgDistance.z + lookZOffset));

// All the GUI debug info
    public void OnGUI ()
    {
        GUILayout.Label ("largest distance is = " + largestDistance.ToString ());
        GUILayout.Label ("height = " + height.ToString ());
        GUILayout.Label ("number of players = " + targets.Length.ToString ());
        GUILayout.Label ("xRatio = " + xRatio.ToString ());
        GUILayout.Label ("yzRatio = " + yzRatio.ToString ());
        GUILayout.Label ("offRatio = " + offZRatio.ToString ());
        GUILayout.Label ("offRatio = " + offXRatio.ToString ());
		GUILayout.Label ("Average Distance = " + avgDistance.x.ToString () + ", " + avgDistance.y.ToString () + ", " + avgDistance.z.ToString ());
	}

 */






/* OLD CODE
 * targets = GameObject.FindGameObjectsWithTag ("Player");
        if (!GameObject.FindWithTag ("Player")) {
            return;
        }
        Vector3 sum = new Vector3 (0, 0, 0);
        for (int i = 0; i < targets.Length; i++) {
            sum += targets [i].transform.position;
        }

        avgDistance = sum / targets.Length;

        float largestDifference = returnLargestDifference ();

        distance = Mathf.Lerp (height, largestDifference, 0.5f);

        if (largestDifference < 8) {
            distance = 8;
            Vector3 temp = new Vector3 (theCamera.transform.position.x, 10, theCamera.transform.position.z);
            theCamera.transform.position = temp;
        }

        if (largestDifference > 8) {
            distance = 8;
        }
        */