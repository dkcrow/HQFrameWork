using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerTestControl : MonoBehaviour {

    /// <summary>
    /// 所有拐点的队列,用于被追踪
    /// </summary>
    private Queue<CornerInfo> _cornerQueue = new Queue<CornerInfo>();
	// Use this for initialization
	void Awake ()
	{
	    RecordCorner();
	    
	}

    private Vector3 rot;
	// Update is called once per frame
	void Update () {

        transform.Translate(Vector3.forward*Time.deltaTime*3);
        if (Input.GetKeyDown(KeyCode.A))
        {
            rot = Vector3.up*90;
            transform.Rotate(rot);
            RecordCorner();
        }

        if (Input.GetKeyDown(KeyCode.D))
        { 
            rot = -Vector3.up * 90;
            transform.Rotate(rot);
            RecordCorner();
        }
    }

    void RecordCorner()
    {
        _cornerQueue.Enqueue(new CornerInfo(transform.position, transform.rotation));
        if (_cornerQueue.Count > 10)//只保存前10个
        {
            _cornerQueue.Dequeue();
        }
    }

    public CornerInfo GetCornerAt(int index)
    {
        if (index >= _cornerQueue.Count)
            return new CornerInfo(transform.position, transform.rotation);

        return _cornerQueue.ElementAt(index);
    }
}
