using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingModel : EntityBase
{
   
	// Use this for initialization
	void Start () {
        ///奖励分数为0 说明是场景中的建筑,需要初始化
	    if (Score == 0)
	    {
	        Init();
	    }
	}

    void Init()
    {
        EntityData selfData = GameDBConfig.Instance.m_entityDataDic[Type];
        this.Init(selfData);
        this.SetId(EntityManager.Instance.AddEntityExistInScene(this));
    }
	// Update is called once per frame
	void Update () {
		
	}
}
