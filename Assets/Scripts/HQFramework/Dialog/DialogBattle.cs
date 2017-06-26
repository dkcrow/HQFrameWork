using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using HQFrameWork;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


///<summary>
///主战斗界面
///<summary>
public class  DialogBattle : UGUIDialogBase
{

	#region 所有控件
	public UIButton ButtonLeft;
    public UIButton ButtonRight;
    public UIButton ButtonAccelerate;
    public UIButton ButtonStart;
    public UIButton ButtonPause; 
    public UIButton ButtonTeamCar; 
    public UIButton ButtonTeamMonster;

    public UIText TextCountDown; 

    public UIPanel PanelWaitForStart;
    public UIPanel PanelMainBattle;
    public UIPanel PanelWaitForReborn;
    public UIPanel PanelSelectTeam;

    List<UIPanel> _panleList=new List<UIPanel>(); 
    #endregion


    public List<PlayerModel> playerModelList = new List<PlayerModel>();
    public Dictionary<Text, Text> playerTextInfoDic=new Dictionary<Text, Text>();
	// 重写基类方法
	public override void Init(GameObject obj)
	{
		base.Init(obj);
	
        ButtonLeft = this["ButtonLeft"] as UIButton;
        ButtonRight = this["ButtonRight"] as UIButton;
        ButtonAccelerate = this["ButtonAccelerate"] as UIButton;
        ButtonPause = this["ButtonPause"] as UIButton;
        ButtonStart = this["ButtonStart"] as UIButton;
        ButtonTeamCar = this["ButtonTeamCar"] as UIButton;
        ButtonTeamMonster = this["ButtonTeamMonster"] as UIButton;
        TextCountDown = this["TextCountDown"] as UIText;

        PanelWaitForStart = this["PanelWaitForStart"] as UIPanel;
        PanelMainBattle = this["PanelMainBattle"] as UIPanel;
        PanelWaitForReborn = this["PanelWaitForReborn"] as UIPanel;
        PanelSelectTeam = this["PanelSelectTeam"] as UIPanel;

        
        _panleList.Add(PanelMainBattle);
        _panleList.Add(PanelWaitForStart);
        _panleList.Add(PanelWaitForReborn);
        _panleList.Add(PanelSelectTeam);


        ButtonLeft.AddOnClickListener(() =>
        {
            Messenger.Instance.BroadCastEventMsg(MessageEventType.PlayerSwerve, -1);
        });

        ButtonRight.AddOnClickListener(() =>
        {
            Messenger.Instance.BroadCastEventMsg(MessageEventType.PlayerSwerve, 1);
        });

        ButtonTeamCar.AddOnClickListener(() =>
        {
            Messenger.Instance.BroadCastEventMsg<EnumDefine.EntityType>(MessageEventType.PlayerSelect, EnumDefine.EntityType.Car);
        });
        ButtonTeamMonster.AddOnClickListener(() =>
        {
            Messenger.Instance.BroadCastEventMsg<EnumDefine.EntityType>(MessageEventType.PlayerSelect, EnumDefine.EntityType.Monster);

        });
        EventTriggerListener.Get(ButtonAccelerate.gameObject).onDown+= (go) =>
        {
            Messenger.Instance.BroadCastEventMsg(MessageEventType.PlayerAccelerate);
            Debug.LogError("update select now");
        };
        EventTriggerListener.Get(ButtonAccelerate.gameObject).onUp+= (go) =>
        {
            Messenger.Instance.BroadCastEventMsg(MessageEventType.CancelAccelerate);
            Debug.LogError("exit now");
        };


        //ButtonPause.AddOnClickListener(() =>
        //{
        //    Time.timeScale = 0;
        //});

        ButtonStart.AddOnClickListener(() =>
        {
            PanelWaitForStart.gameObject.SetActive(false);
            Messenger.Instance.BroadCastEventMsg(MessageEventType.GAME_BEGIN);
            BattleManager.Instance.CurrentGameState = EnumDefine.BattleGameState.GameOn;
        });

        Messenger.Instance.AddEventListener(MessageEventType.BATTLE_SCENE_INIT_OK,(() => ShowNextPanel(2)));
        Messenger.Instance.AddEventListener(MessageEventType.GAME_BEGIN, (() => ShowNextPanel(1)));
    }

   
    /// <summary>
    /// todo:暂时这样写
    /// </summary>
    /// <param name="index"></param>
    void ShowNextPanel(int index)
    {
        for (int i = 0; i < _panleList.Count; i++)
        {
            if (i != index - 1)
            {
                _panleList[i].gameObject.SetActive(false);
            }
            else
            {
                _panleList[i].gameObject.SetActive(true);
            }
        }
    }
    public override void OnChangeScene()
    {
        Messenger.Instance.RemoveEventListener(MessageEventType.BATTLE_SCENE_INIT_OK, (() => ShowNextPanel(2)));
        Messenger.Instance.RemoveEventListener(MessageEventType.GAME_BEGIN, (() => ShowNextPanel(1)));
        base.OnChangeScene();
    }

    public void ShowDeathPanel()
    {
        //TextCountDown.Text = CustomDefine.RebornTime.ToString();
        PanelWaitForReborn.gameObject.SetActive(true);
        DOVirtual.Float(CustomDefine.RebornTime+1, 0, CustomDefine.RebornTime-1, (value =>
        {
            TextCountDown.Text = ((int)value).ToString();
        }))
        .OnComplete(()=>PanelWaitForReborn.gameObject.SetActive(false));
    }
}
