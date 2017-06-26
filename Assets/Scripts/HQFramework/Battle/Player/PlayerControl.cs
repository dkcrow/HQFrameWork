using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using HQFrameWork;
//using PVPSdk;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerControl : EntityBase
{
    /// <summary>
    /// 所有拐点的队列,用于被追踪
    /// </summary>
    private Queue<Vector3> _cornerQueue = new Queue<Vector3>();
    public PlayerModel SelfModel;//M
    public PlayerHud SelfPlayerHud;//V
    public PlayerAnimatorControl SelfAnimatorControl;
    public bool IsAlive = true;

    // Use this for initialization
    void Awake()
    {
        Messenger.Instance.AddEventListener<int>(MessageEventType.PlayerSwerve, PlayerSwerve);
        Messenger.Instance.AddEventListener(MessageEventType.PlayerAccelerate, PlayerAccelerate);
        Messenger.Instance.AddEventListener(MessageEventType.CancelAccelerate, CancelAccelerate);
        Messenger.Instance.AddEventListener(MessageEventType.GAME_BEGIN, OnGameOn);
        Messenger.Instance.AddEventListener(MessageEventType.GAME_OVER, OnGameOver);


        
    }

    void OnDestroy()
    {
        if (_init == false)
        {
            return;
        }
       
        Messenger.Instance.BroadCastEventMsg<PlayerControl>(MessageEventType.OnPlayerDestroyed, this);

        Messenger.Instance.RemoveEventListener<int>(MessageEventType.PlayerSwerve, PlayerSwerve); 
        Messenger.Instance.RemoveEventListener(MessageEventType.PlayerAccelerate, PlayerAccelerate);
        Messenger.Instance.RemoveEventListener(MessageEventType.CancelAccelerate, CancelAccelerate);
        Messenger.Instance.RemoveEventListener(MessageEventType.GAME_BEGIN, OnGameOn);
        Messenger.Instance.RemoveEventListener(MessageEventType.GAME_OVER, OnGameOver);
    }

   

   private bool _init = false;
   public void Init(int networkId,int typeId)
    {
        SelfModel=new PlayerModel();
        
        PlayerData selfOrignalProperty = GameDBConfig.Instance.m_playerDataDic[typeId];
        SelfModel.InitProperty(selfOrignalProperty.Speed,networkId);
        SelfModel.Name = "玩家" + networkId;
       _orignalSpeed = SelfModel.Speed;
        Messenger.Instance.BroadCastEventMsg<PlayerControl>(MessageEventType.OnPlayerCreated, this);
        CreatePlayerHud();
       _init = true;
    }


    void CreatePlayerHud()
    {
        SelfPlayerHud=PlayerHudManager.Instance.CreateOrGetPlayerHUD(this);
    }

    void RemovePlayerHud()
    {
        if(SelfPlayerHud&& PlayerHudManager.Instance)
        PlayerHudManager.Instance.DisposePlayerHUD(SelfPlayerHud);
    }

    private int _currentRotation = 0;//一开始是0
    /// <summary>
    /// 1右转 -1 左转
    /// </summary>
    /// <param name="direction"></param>
   public void PlayerSwerve(int direction)
    {
        if(!IsMine)return;
        _currentRotation+= direction == 1 ? 90 : -90;//必须整加减90 否则会由于还没转过去又按转弯导致斜方向运动
        _currentRotation %= 360;
        transform.DORotate(Vector3.up* _currentRotation, 0.5f);
        //transform.Rotate(Vector3.up,direction==1?90:-90);
        RecordCorner();
    }


    private float _orignalSpeed;
    void PlayerAccelerate()
    {
        if (!IsMine) return; 

        //var orignalSpeed = SelfModel.Speed;
        SelfModel.Speed =2*_orignalSpeed;
        //DOVirtual.DelayedCall(5, () => SelfModel.Speed = orignalSpeed);
    }

    void CancelAccelerate()
    {
        if (!IsMine) return;

        //var orignalSpeed = SelfModel.Speed;
        SelfModel.Speed = _orignalSpeed;
        //DOVirtual.DelayedCall(5, () => SelfModel.Speed = orignalSpeed);
    }

    void RecordCorner()
    {
        _cornerQueue.Enqueue(transform.position);
        if (_cornerQueue.Count > 10)//只保存前10个
        {
            _cornerQueue.Dequeue();
        }
    }


    public Vector3 GetCornerAt(int index)
    {
        if (_cornerQueue.Count == 0)
        {
            RecordCorner();
        }

        if (index >= _cornerQueue.Count)
            return transform.position;

        return _cornerQueue.ElementAt(index);
    }

    private bool IsGameOn = false;
    void OnGameOn()
    {
        IsGameOn = true;

        if (IsAlive&&SelfAnimatorControl)
        {
            SelfAnimatorControl.PlayJumpAnimation(true);
        }
        
    }

    void OnGameOver()
    {
        IsGameOn = false;
        
    }

    /// <summary>
    /// 可碰撞状态才能碰撞,防止短时间多次碰撞
    /// </summary>
    private bool bEnableCollide = true;
    /// <summary>
    /// 碰撞后把碰撞事件广播出去,让battlemanager处理
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter(Collision collision)
    {
        if (!bEnableCollide) return;

        if(collision.transform.tag.Equals(CustomDefine.Tag_Plane))return;

        Messenger.Instance.BroadCastEventMsg<Transform,Transform>(MessageEventType.EntityCollide, this.transform,collision.transform);
        bEnableCollide = false;

        DOVirtual.DelayedCall(.1f, () => bEnableCollide = true);
    }

    void Update()
    {
        if (IsAlive&&IsGameOn)
        {
           if(!_init)return;

           transform.Translate(Vector3.forward * SelfModel.Speed * Time.deltaTime);
        }
      
    }

    void OnEnable()
    {
        
        IsAlive = true;
        if (BattleManager.Instance.CurrentGameState.Equals(EnumDefine.BattleGameState.GameOn))
        {
            OnGameOn();
        }
    }

    void OnDisable()
    {
        //玩家离开时的逻辑放在这里面
        RemovePlayerHud();

        IsAlive = false;
        IsMine = false;
        IsAi = false;
        
        AIBase ab = gameObject.GetComponent<AIBase>();
        if (ab)
        {
            ab.enabled = false;
        }
    }
        //gameObject.GetOrAddComponent<AIBase>().enabled = false;

    }

   


     


