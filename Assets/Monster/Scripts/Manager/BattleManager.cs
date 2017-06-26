using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using HQFrameWork;
using MobileBaseCommon.UnityBase;
using UnityEngine;

public class BattleManager : MBSingleton<BattleManager>
{
    /// <summary>
    /// 主玩家
    /// </summary>
    public PlayerControl MainPlayer;
    public EnumDefine.BattleGameState CurrentGameState=EnumDefine.BattleGameState.GamePrepare;

    public BattleManager()
    {
        singletonType=SingletonType.PerSceneInstance;
    }

    /// <summary>
    /// 设置为自己操控的玩家
    /// </summary>
    public void SetAsMainPlayer(PlayerControl mainPlayer)
    {
        mainPlayer.IsMine = true;
        MainPlayer = mainPlayer;
        this.GetOrAddComponent<BattleCameraFollow>().SetTarget(mainPlayer.transform);
    }

    /// <summary>
    /// 设置为自己操控的玩家
    /// </summary>
    public void SetAsAI(PlayerControl player)
    {
        player.IsAi = true;
        switch (player.tag)
            {
                case CustomDefine.Tag_Player_Car:
                    player.GetOrAddComponent<CarAI>().enabled = true;
                    break;

                case CustomDefine.Tag_Player_Monster:
                    player.GetOrAddComponent<MonsterAI>().enabled = true;
                    break;
            }
    }
    // Use this for initialization
    void Awake ()
	{
        Messenger.Instance.AddEventListener<Transform,Transform>(MessageEventType.EntityCollide, OnEntityCollide);


    }

    void OnDestroy()
    {
        Messenger.Instance.RemoveEventListener<Transform, Transform>(MessageEventType.EntityCollide, OnEntityCollide);
    }

    private string playerTag;
    private string entityTag;
    private PlayerControl playerCtrl;
    private EntityBase entityBase;

    private Transform collider1;
    private Transform collider2;
    //private Dictionary<Transform,Transform>bufferTransformDic=new Dictionary<Transform, Transform>(); //防止多次碰撞,把每次碰撞的两者缓存起来
    void OnEntityCollide(Transform player,Transform entity)
    {
        if (null == collider1&&null==collider2)
        {
            collider1 = player;
            collider2 = entity;
        }

        if(player.Equals(collider2))return;//两物体碰撞都会发消息过来,但只计算1次
        collider1 = player;
        collider2 = entity;
        //if((bufferTransformDic.ContainsKey(player)&&bufferTransformDic[player].Equals(entity)) || (bufferTransformDic.ContainsKey(entity) && bufferTransformDic[entity].Equals(player)))
        //    return;
        //bufferTransformDic.Add(player,entity);
        //DOVirtual.DelayedCall(.5f, () =>
        //{
        //    bufferTransformDic.Remove(player);
        //    bufferTransformDic.Remove(entity);
        //});//2s后可再次碰撞

         playerTag = player.tag;
         entityTag = entity.tag;
        if (playerTag.Equals(entityTag))//同类型不产生逻辑
        {
            return;
        }

        if (IsWall(entityTag))
        {
            player.forward = -player.forward;//转向
        }

         playerCtrl = player.GetComponent<PlayerControl>();
         entityBase = entity.GetComponent<EntityBase>();
        if (IsMonster(playerTag))
        {
            if (IsBuilding(entityTag))
            {
                DestroyEntity(entityBase);
                AddScore(playerCtrl, entityBase.Score);
            }
            else if (IsCar(entityTag))//说明也是玩家,可以把entitybase强转为playercontrol
            {
                ///怪物在车后面撞到 直接破坏车
                if (IsOtherForward(player.forward, player.position, entity.position))
                {
                    DestroyEntity(entityBase);
                    AddScore(playerCtrl, entityBase.Score);
                }
                else
                {
                    if (IsCarHitMonsterBehind(entity, player))
                    {
                        DestroyEntity(playerCtrl);
                        AddScore((PlayerControl) entityBase, playerCtrl.Score);
                    }
                    else
                    {
                        DestroyEntity(entityBase);
                        AddScore(playerCtrl, entityBase.Score);
                    }
                }
                //float angle = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(entityBase.transform.forward.normalized,
                //    playerCtrl.transform.forward.normalized));

                //if (angle > 45 && angle < 225)
                //{
                //    DestroyEntity(playerCtrl);
                //    AddScore((PlayerControl) entityBase, playerCtrl.Score);
                //}
                //else
                //{
                //    DestroyEntity(entityBase);
                //    AddScore(playerCtrl,entityBase.Score);
                //}
            }
            return;
        }

         if (IsCar(playerTag))
        {
            if (IsBuilding(entityTag))
            {
                DestroyEntity(entityBase);
                AddScore(playerCtrl, entityBase.Score);
            }

            else if (IsMonster(entityTag))
            {
                if (IsOtherForward(player.forward, player.position, entity.position))
                {
                    if (IsCarHitMonsterBehind(player, entity))
                    {
                        DestroyEntity(entityBase);
                        AddScore(playerCtrl, playerCtrl.Score);
                    }
                    else
                    {
                        DestroyEntity(playerCtrl);
                        AddScore((PlayerControl) entityBase, playerCtrl.Score);
                    }
                }
                else//怪物在后面则直接撞毁player
                {
                    DestroyEntity(playerCtrl);
                    AddScore((PlayerControl)entityBase, playerCtrl.Score);
                }
                //float angle = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(entityBase.transform.forward.normalized,
                //    playerCtrl.transform.forward.normalized));
                //print(angle);
               
                //if (angle >= -45 && angle <= 45)
                //{
                //    DestroyEntity(entityBase);
                //    AddScore(playerCtrl, playerCtrl.Score);
                //}
                //else
                //{
                //    DestroyEntity(playerCtrl);
                //    AddScore((PlayerControl)entityBase, playerCtrl.Score);
                //}
            }
            return;
        }
    }

    void AddScore(PlayerControl player, int score)
    {
        if (player.SelfModel != null)
        {
            player.SelfModel.Score += score;
        }
    }

    bool IsMonster(string tag)
    {
        return tag.Equals(CustomDefine.Tag_Player_Monster);
    }

    bool IsCar(string tag)
    {
        return tag.Equals(CustomDefine.Tag_Player_Car);
    }

    bool IsBuilding(string tag)
    {
        return tag.Equals(CustomDefine.Tag_Building);
    }
    bool IsWall(string tag)
    {
        return tag.Equals(CustomDefine.Tag_Wall);
    }

    /// <summary>
    /// 对方是否在自己前方
    /// </summary>
    /// <param name="selForward"></param>
    /// <param name="selfPosition"></param>
    /// <param name="otherPosition"></param>
    /// <returns></returns>
    bool IsOtherForward(Vector3 selForward,Vector3 selfPosition, Vector3 otherPosition)
    {
        return Vector3.Dot(selForward, otherPosition - selfPosition) > 0;
    }

    /// <summary>
    /// 是否车背面撞击怪物,前提是通过IsOtherForward验证了车在怪物后面
    /// </summary>
    /// <param name="carTransform"></param>
    /// <param name="monsterTransform"></param>
    /// <returns></returns>
    bool IsCarHitMonsterBehind(Transform carTransform,Transform monsterTransform)
    {
        float angle = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(carTransform.forward.normalized,
                    monsterTransform.forward.normalized));
        print(angle);

        return angle >= -45 && angle <= 45;

    }
    public void DestroyEntity(EntityBase toDestroyEntity)
    {
        
        //bool bIsMainPlayer=toDestroyEntity.transform.Equals(MainPlayer.transform);
        bool bIsPlayer = toDestroyEntity is PlayerControl;
        bool bIsMainPlayer = toDestroyEntity.IsMine;
        bool bIsAi = toDestroyEntity.IsAi;
        if (bIsPlayer)
        {
            //todo:播放死亡/摧毁动画
            
        }

        EntityManager.Instance.RemoveEntityById(toDestroyEntity.Id,bIsPlayer);
        ParticleManager.Instance.PlayCrashWordEffect(toDestroyEntity.transform.position);


        if (bIsMainPlayer)
        {
            UGUIDialogManager.Instance.GetInstance<DialogBattle>().ShowDeathPanel();
        }

        DOVirtual.DelayedCall(CustomDefine.RebornTime, (() =>
        {
            if (bIsPlayer) //是玩家还要随机位置
            {
                print(toDestroyEntity.Id+" "+ toDestroyEntity.IsMine);
                EntityBase enityBase=EntityManager.Instance.AddEntityByType(toDestroyEntity.Type,
                    Random.Range(-CustomDefine.WorldHalfWidth, CustomDefine.WorldHalfWidth),
                    Random.Range(-CustomDefine.WorldHalfHeight, CustomDefine.WorldHalfHeight), toDestroyEntity.Id);

                if (bIsMainPlayer)
                {
                    SetAsMainPlayer((PlayerControl)enityBase);
                }
                else if (bIsAi)
                {
                    SetAsAI((PlayerControl)enityBase);
                }
            }
            else//暂时建筑物都走 原地setactive那一套 而不是对象池
            {
                EntityManager.Instance.RebornEntity(toDestroyEntity.Id);
            }
            //EntityManager.Instance.AddEntityByType(toDestroyEntity.Type,toDestroyEntity.Id, Random.Range(-CustomDefine.WorldHalfWidth, CustomDefine.WorldHalfWidth), Random.Range(-CustomDefine.WorldHalfHeight, CustomDefine.WorldHalfHeight));
            //todo:主角摄像头跟随

        }));
    }
}
