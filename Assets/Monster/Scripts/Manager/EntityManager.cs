using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using HQFrameWork;
using PathologicalGames;
using UnityEngine;




public class EntityManager : MBSingleton<EntityManager> {

    EntityManager()
    {
        singletonType=SingletonType.PerSceneInstance;
    } 
    /// <summary>
    /// 测试时定为public类型
    /// </summary>
     
    public Dictionary<int,EntityBase> _entityManagerDictionary=new Dictionary<int, EntityBase>();
    public Dictionary<int,PlayerControl> _playerManagerDictionary=new Dictionary<int, PlayerControl>();
    private Transform _entityTransform;
    private Transform _toSpawnTransform;
    private EntityData _entityData;
    public EntityBase AddEntityByType(int type,float posX,float posZ,int id=-1)
    {
        if (id == -1)
        {
            id = _entityManagerDictionary.Count+1;
        }

        _entityData = GameDBConfig.Instance.m_entityDataDic[type];
        //池中找到相应的transform
        _toSpawnTransform = PoolManager.Pools[CustomDefine.Pool_Entity].prefabs[_entityData.Name];
        

        if (null != _toSpawnTransform)
        {
            _entityTransform = PoolManager.Pools[CustomDefine.Pool_Entity].Spawn(_toSpawnTransform);
            _entityTransform.position = new Vector3(posX, 0, posZ);
            EntityBase entityBase = _entityTransform.GetComponent<EntityBase>();
            if (null != entityBase)
            {
                entityBase.Init(_entityData);
                entityBase.SetId(id);
                //entityBase.Id = id;
                //entityBase.Type = type;
                //entityBase.Width = _entityData.Width;
                //entityBase.Height = _entityData.Height;
                //entityBase.Score = _entityData.Score;

                _entityManagerDictionary.Add(id, entityBase);
                
            }
            else
            {
                Debug.LogError("缺少enititybase组件");
                return null;
            }

            if (type > 10000)//todo:假定 10000以后开始为人物id 
            {
                PlayerControl player = (PlayerControl) entityBase;
                _playerManagerDictionary.Add(id, player);
                player.Init(id,type);
                
            }
            return entityBase;
        }
        else
        {
           Debug.LogError("池中不包含该type="+type);
            return null;
        }
    }

    public void RemoveEntityById(int id,bool bIsPlayer)
    {
        if (bIsPlayer)//player还是走原来那一套
        {
            if (_entityManagerDictionary.ContainsKey(id))
            {
                PoolManager.Pools[CustomDefine.Pool_Entity].Despawn(_entityManagerDictionary[id].transform);
                _entityManagerDictionary.Remove(id);
                _playerManagerDictionary.Remove(id);
            }
        }
        else 
        {
            //PoolManager.Pools[CustomDefine.Pool_Entity].Despawn(_entityManagerDictionary[id].transform);todo:暂时不用pool因为都是场景里的东西,直接setactive false就行
            GameObject toRemoveGO = _entityManagerDictionary[id].gameObject;
            toRemoveGO.SetActive(false);
            //DOVirtual.DelayedCall(3, (() => toRemoveGO.SetActive(true)));

            //_entityManagerDictionary.Remove(id);
            //if (_playerManagerDictionary.ContainsKey(id))
            //{
            //    _playerManagerDictionary.Remove(id);
            //}
        }
       
        
    }

    /// <summary>
    /// 重生
    /// </summary>
    /// <param name="id"></param>
    public void RebornEntity(int id)
    {
        GameObject toRemoveGO = _entityManagerDictionary[id].gameObject; 
        toRemoveGO.SetActive(true);
    }
    /// <summary>
    /// 添加场景中已存在的建筑入表,id暂时使用count+1来标注
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public int AddEntityExistInScene(EntityBase entity)
    {
        int id = _entityManagerDictionary.Count+1;
        _entityManagerDictionary.Add(id, entity);
        return id;
    }
    //private PlayerControl playerCtrl;
    //public void UpdatePlayerPos(List<CustomDefine.PlayerPosSyncMsg> posSyncMsgs )
    //{
    //    foreach (var syncMsg in posSyncMsgs)
    //    {
    //        if (_playerManagerDictionary.TryGetValue(syncMsg.id,out playerCtrl))
    //        {
    //            playerCtrl.UpdatePlayerProperty(syncMsg.posX,syncMsg.posZ);
    //        }
    //    }
    //}
}
