using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HQFrameWork;
public class BattleCameraFollow : MonoBehaviour
{

    private Transform _target;
    public Transform Camera3dTrans;
    public float OrthographicSize= 3.87f;
    public Vector3 _cameraOffset=new Vector3(-5.6f, 5.58f, 4.85f);
    private Vector3 _cameraRotation = new Vector3(35, 130, 0);
    public Vector3 TargetPos;
	// Use this for initialization
	void Start ()
	{
        //Target = GameObject.FindGameObjectWithTag(CustomDefine.Tag_Player).transform;
        Camera3dTrans=Camera.main.transform;
        Camera3dTrans.localRotation = Quaternion.Euler(_cameraRotation);
	    Camera.main.orthographic = true;
	    Camera.main.orthographicSize = OrthographicSize;


	}

    public void SetTarget(Transform target)
    {
        _target = target;
    }
	// Update is called once per frame
	void Update () {
	    if (null == _target) return;
	    TargetPos = _target.transform.position + _cameraOffset;
        Camera3dTrans.position=Vector3.Lerp(Camera3dTrans.position,TargetPos,5*Time.deltaTime);
	}

    void OnDestroy()
    {
        if(Camera.main)
        Camera.main.orthographic = false;
    }
}
