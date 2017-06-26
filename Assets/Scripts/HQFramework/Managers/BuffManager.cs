using System;
using System.Collections;
using System.Collections.Generic;
using HQFrameWork;
using UnityEngine;

public class BuffManager : MBSingleton<BuffManager>
{
    List<BuffBase> buffList = new List<BuffBase>();//交换masterclient之后又变成了0,所以导致不同步,因此每次init的时候都要先findwithtag一下
    
    const int BUFF_COUNT = 50;//场景中buff数量,少于则添加

    int toCreateCount = 0;
    private bool init = false;
    private EnumDefine.BuffType[] buffTypes;
    // Use this for initialization
    public void Init () {
        if(init)return;
        buffTypes = Enum.GetValues(typeof(EnumDefine.BuffType)) as EnumDefine.BuffType[];
        Messenger.Instance.AddEventListener<BuffBase>(MessageEventType.OnBuffDestroy, OnBuffDestroyed);
        InvokeRepeating("UpdateSceneBuff",2,4);
	    var buffGos = GameObject.FindGameObjectsWithTag(CustomDefine.Tag_Buff);
        buffList.Clear();
	    foreach (var buffGo in buffGos)
	    {
	        buffList.Add(buffGo.GetComponent<BuffBase>());
	    }
        init = true;
	}
	
	void OnDestroy()
    {
        CancelInvoke("UpdateSceneBuff");
        StopCoroutine(CreateBuff());
        Messenger.Instance.RemoveEventListener<BuffBase>(MessageEventType.OnBuffDestroy, OnBuffDestroyed);
    }

    void UpdateSceneBuff()
    {
        if (buffList.Count < BUFF_COUNT)
        {
            toCreateCount = BUFF_COUNT - buffList.Count;
            StopCoroutine(CreateBuff());
            StartCoroutine(CreateBuff());
        }
       
    }

    IEnumerator CreateBuff()
    {
        int index = 0;
        while (toCreateCount > index)
        {
            //var buff = PhotonNetWorkManager.Instance.InstantiateSceneBuff(UnityEngine.Random.Range(0,buffTypes.Length)).GetComponent<BuffBase>();
            //buffList.Add(buff);
            index++;
            yield return new WaitForEndOfFrame();//下一帧再创建防止一次性创建过多
        }
        yield return null;
    }

    void OnBuffDestroyed(BuffBase buff)
    {
        buffList.Remove(buff);
    }
}
