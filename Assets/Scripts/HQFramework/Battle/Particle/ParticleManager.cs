using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using HQFrameWork;
using PathologicalGames;
using UnityEngine;

public class ParticleManager : MBSingleton<ParticleManager> {

    private const string Pool_Name = "ParticalPool";
    private const string crashWordEffect = "CrashWordEffect";
    
    private SpawnPool _particlePool;
    private Transform _crashWordTrans;
    ParticleManager()
    {
        singletonType=SingletonType.PerSceneInstance;
    }
	// Use this for initialization
	void Awake () {
        _particlePool = PoolManager.Pools[Pool_Name];//或者这里通过getcomponent来获取
        _crashWordTrans = PoolManager.Pools[Pool_Name].prefabs[crashWordEffect];
    }


    public void PlayCrashWordEffect(Vector3 position)
    {

        Transform toSpawnTransform= _particlePool.Spawn(_crashWordTrans,position+Vector3.up,Quaternion.identity);
         DOVirtual.DelayedCall(1.5f, (() => { _particlePool.Despawn(toSpawnTransform); }));

    }
}
