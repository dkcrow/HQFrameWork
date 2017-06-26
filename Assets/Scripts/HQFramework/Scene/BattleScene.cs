using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HQFrameWork;
using PathologicalGames;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleScene : BaseScene
{

    /// <summary>
    /// 战斗场景才用到,离开场景后销毁
    /// </summary>
    public GameObject PlayerHudPool;
    /// <summary>
    /// 弓箭池 同上
    /// </summary>
    public GameObject ArrowPool;

   
//    /// <summary>
//    /// 玩家列表
//    /// </summary>
    public List<PlayerControl> PlayerList=new List<PlayerControl>();
    public override EnumSceneType SceneType
    {
        get
        {
            return  EnumSceneType.BATTLE;
        } 
    }


    public override void OnEnterScene()
    {
       
        Messenger.Instance.AddEventListener<EnumDefine.EntityType>(MessageEventType.PlayerSelect, OnSelectPlayer);

        UGUIDialogManager.Instance.GetInstance<DialogBattle>().ShowDlg(true);

        //EntityBase entity = EntityManager.Instance.AddEntityByType(10001, 1, 20,10);
        //EntityBase entity2 = EntityManager.Instance.AddEntityByType(10002, 2, 20, 5);
        //EntityBase entity2 = EntityManager.Instance.AddEntityByType(10001, 2, Random.Range(-CustomDefine.WorldHalfWidth, CustomDefine.WorldHalfWidth), Random.Range(-CustomDefine.WorldHalfHeight, CustomDefine.WorldHalfHeight));

        //BattleManager.Instance.SetAsMainPlayer((PlayerControl)entity);
        //BattleManager.Instance.SetAsAI((PlayerControl)entity2);
        //entity.transform.Rotate(0,90,0);

    }


    void OnSelectPlayer(EnumDefine.EntityType playerType)
    {
        EntityBase entity=null;
        switch (playerType)
        {
            case EnumDefine.EntityType.Car:
                entity = CreateCar();
                break;
            case EnumDefine.EntityType.Monster:
                entity = CreateMonster();
                break;
        }
        BattleManager.Instance.SetAsMainPlayer((PlayerControl)entity);

        OnCreatePlayerComplete();
    }

    void OnCreatePlayerComplete()
    {
        CreateAi();
        Messenger.Instance.BroadCastEventMsg(MessageEventType.BATTLE_SCENE_INIT_OK);
    }
    public void CreateAi()
    {
        for (int i = 0; i < CustomDefine.AiCount; i++)
        {
            if (Random.Range(1,100) > 40)
            {
                
                BattleManager.Instance.SetAsAI((PlayerControl)CreateCar());
            }
            else
            {
                BattleManager.Instance.SetAsAI((PlayerControl)CreateMonster());
            }
        }
    }

    EntityBase CreateMonster()
    {
        return EntityManager.Instance.AddEntityByType(10001,
                    Random.Range(-CustomDefine.WorldHalfWidth, CustomDefine.WorldHalfWidth),
                    Random.Range(-CustomDefine.WorldHalfHeight, CustomDefine.WorldHalfHeight));
    }

    EntityBase CreateCar()
    {
        return EntityManager.Instance.AddEntityByType(10002,
                    Random.Range(-CustomDefine.WorldHalfWidth, CustomDefine.WorldHalfWidth),
                    Random.Range(-CustomDefine.WorldHalfHeight, CustomDefine.WorldHalfHeight));
    }


    public override void OnLeaveScene()
    {
        if (null != PlayerHudPool)
        {
            GameObject.Destroy(PlayerHudPool);
        }

        if (null != ArrowPool)
        {
            GameObject.Destroy(ArrowPool);
        }

       
        Messenger.Instance.RemoveEventListener<EnumDefine.EntityType>(MessageEventType.PlayerSelect, OnSelectPlayer);

        foreach (var buff in GameObject.FindGameObjectsWithTag(CustomDefine.Tag_Buff))
        {
            GameObject.Destroy(buff);
        }
        
        if (BuffManager.Instance)
        {
            GameObject.Destroy(BuffManager.Instance);
        }

//        PoolManager.Pools.DestroyAll();
    }


    /// <summary>
    /// 创建角色 后面换肤时要改变Player的名字
    /// </summary>
    PlayerControl CreatePlayer()
    {
       // var player = PhotonNetWorkManager.Instance.InstantiatePlayer();
       // player.transform.position = Vector3.up;
       //// player.transform.localScale = Vector3.one*0.1f;
        //return player.GetComponent<PlayerControl>();
        return null;
    }

    /// <summary>
    /// 刷新玩家表,以提供分数
    /// </summary>
//    public void RefreshPlayerList()
//    {
//        PlayerList.Clear();
//        var players = GameObject.FindGameObjectsWithTag("Player");
//        foreach (var player in players)
//        {
//            PlayerList.Add(player.GetComponent<PlayerControl>());
//        }
//        Messenger.Instance.BroadCastEventMsg(MessageEventType.OnPlayerNumChange, PlayerList);
//    }

    public void AddPlayer(PlayerControl playerControl)
    {
        if (!PlayerList.Contains(playerControl))
        {
            PlayerList.Add(playerControl);
            Messenger.Instance.BroadCastEventMsg(MessageEventType.OnPlayerNumChange, PlayerList);
        }
        
    }

    public void RemovePlayer(PlayerControl playerControl)
    {
        if (PlayerList.Contains(playerControl))
        {
            PlayerList.Remove(playerControl);
            Messenger.Instance.BroadCastEventMsg(MessageEventType.OnPlayerNumChange, PlayerList);
        }
       
        
    }

    /// <summary>
    /// 如果是主客户端就要做一些该做的逻辑,比如场景物体的创建和销毁
    /// </summary>
    void DoMasterClientLogicIfNeed()
    {
        //if (PhotonNetwork.isMasterClient)
        //{
        //    BuffManager.Instance.Init();
        //}

        RefreshPlayerListOnSwitchedPlayer();

    }

    /// <summary>
    /// 易主时刷新列表
    /// </summary>
    void RefreshPlayerListOnSwitchedPlayer()
    {
        PlayerList.Clear();
        var playerGos = GameObject.FindGameObjectsWithTag(CustomDefine.Tag_Player);
        foreach (var playerGo in playerGos)
        {
            PlayerList.Add(playerGo.GetComponent<PlayerControl>());
        }
        Messenger.Instance.BroadCastEventMsg(MessageEventType.OnPlayerNumChange, PlayerList);
    }
}
