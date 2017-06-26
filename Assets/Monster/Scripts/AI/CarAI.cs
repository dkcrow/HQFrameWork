using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CarAI : AIBase {
    private AIState _currentAiState;
    private int _currentCornerIndex;
    private PlayerControl _targetPlayer;
    private PlayerControl _selfPlayerControl;
    public Vector3 TargetCorner;
    // Use this for initialization
    //void Start() {

    //    //_selfPlayerControl=GetComponent<PlayerControl>();
    //    _currentAiState = AIState.Look_For_Player;
    //}
    public void SetTarget(PlayerControl player)
    {
        _targetPlayer = player;
    }

    private float currentDis;
    private Vector3 currentVectorToTarget;//当前位置指向目标的向量
    private float currentAngle;
    private bool enableAiTrack = true;//是否能够ai跟踪(防止短时间多次转向)
    
    // Update is called once per frame
    void Update() {


        if (_currentAiState.Equals(AIState.Look_For_Player))//找到最近的玩家设定为追踪目标
        {
             float minPlayerDis = 100;
            foreach (var playerPair in EntityManager.Instance._playerManagerDictionary)
            {
                if (!playerPair.Value.tag.Equals(CustomDefine.Tag_Player_Monster))
                    continue;

                currentDis = Vector3.Distance(transform.position, playerPair.Value.transform.position);
                if (currentDis < minPlayerDis)
                {

                    minPlayerDis = currentDis;
                    SetTarget(playerPair.Value);
                }

            }

            GetNextCorner();
            _currentAiState = AIState.Run_After_Player;
        }


        else if (_currentAiState.Equals(AIState.Run_After_Player))
        {
            if(!enableAiTrack) return;
            if (Vector3.Distance(transform.position, TargetCorner) > 0.5f)//每0.1秒都计算距离 
            {
                currentVectorToTarget = (TargetCorner - transform.position).normalized;
                currentAngle = Mathf.Acos(Vector3.Dot(transform.forward, currentVectorToTarget))*Mathf.Rad2Deg;
                
                //transform.LookAt(new Vector3(TargetCorner.x, transform.position.y, TargetCorner.z));
                if (currentAngle >0&& currentAngle<75)
                {
                    //前
                    AiSwerve(0);
                }
                else if (currentAngle > 105 && currentAngle < 180)
                {
                    //后
                    AiSwerve(2); 
                }
                else //这里左右都在里面，需要区分开来
                {
                    if (Vector3.Dot(transform.right, currentVectorToTarget) > 0)
                    {
                        //和右方同向，也就是右
                        AiSwerve(1);
                    }
                    //左方
                    else
                    {
                        AiSwerve(-1);
                    }
                }
            }
            else
            {
                GetNextCorner();
            }
        }

        if (!IsTargetAlive())
        {
            _currentAiState = AIState.Look_For_Player;
        }
    }

    private int _currentRotation = 0;//一开始是0
    /// <summary>
    /// 1右转,-1左转,0继续前进,2,后退
    /// </summary>
    /// <param name="direction"></param>
    void AiSwerve(int direction)
    {
        enableAiTrack = false;
        switch (direction)
        {
            case -1:
                _currentRotation += -90;
                break;
            case 0:
                
                break;
            case 1:
                _currentRotation += 90;
                break;
            case 2:
                _currentRotation += 180;
                break;
        }
        _currentRotation %= 360;
        transform.DORotate(Vector3.up * _currentRotation, 0.1f).OnComplete((() => enableAiTrack = true));

       

    }
    void GetNextCorner()
    {
        if (!IsTargetAlive()) return;

        TargetCorner = _targetPlayer.GetCornerAt(_currentCornerIndex++);
        
    }


    bool IsTargetAlive()
    {
        return _targetPlayer && _targetPlayer.IsAlive;
    }

    void OnEnable()
    {
        _currentAiState = AIState.Look_For_Player;
        _selfPlayerControl = GetComponent<PlayerControl>();
    }
}
