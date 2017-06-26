using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollowTest : MonoBehaviour
{
    public PlayerTestControl Player;
    private int _currentCornerIndex;
    public CornerInfo TargetCorner;
    // Use this for initialization
    void Start ()
    {
         GetNextCorner();

    }
	
	// Update is called once per frame
	void Update () {

        
	    if (Vector3.Distance(transform.position, TargetCorner.CornerPos) > 0.05f)//每一秒都计算距离 待优化
	    {
	       //transform.rotation=Quaternion.Lerp(transform.rotation, TargetCorner.CornerRot, 2*Time.deltaTime);
           
           transform.Translate(Vector3.forward * Time.deltaTime*5);
            //transform.position=Vector3.Lerp(transform.position, TargetCorner.CornerPos,2* Time.deltaTime);
        }
	    else
	    {
	        GetNextCorner();
	    }
	}


     void  GetNextCorner()
    {
        TargetCorner = Player.GetCornerAt(_currentCornerIndex++);
        transform.LookAt(TargetCorner.CornerPos);
    }
}
