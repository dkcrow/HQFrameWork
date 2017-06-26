using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationTest : MonoBehaviour
{
    public Transform Target;
    private Quaternion angle;
    // Use this for initialization
    void Start ()
	{
	     
       
	}
	
	// Update is called once per frame
	void Update () {
        angle = Quaternion.LookRotation(Target.position - transform.position, Vector3.up);
        transform.rotation=Quaternion.Lerp(transform.rotation,angle,Time.deltaTime);
	}
}
