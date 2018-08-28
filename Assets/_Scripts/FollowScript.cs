/*Alex Greff
19/01/2016
Follow Script
Used for camera following
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowScript : MonoBehaviour {
    public Transform target;

    public bool useLerp;
    public bool useRotation;

    public float lerpRate = 5f;
    public Vector3 offset;

	// Update is called once per frame
	void Update () {
		if (target != null) { //If there is a target
            if (useLerp)
                transform.position = Vector3.Lerp(transform.position, target.position + offset, lerpRate * Time.deltaTime);
            else
                transform.position = target.position + offset;

            if (useRotation) {
                if (useLerp)
                    transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, lerpRate * Time.deltaTime);
                else
                    transform.rotation = target.rotation;
            }
        }
	}
}
