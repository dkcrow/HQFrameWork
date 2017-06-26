using System.Collections;
using System.Collections.Generic;
using HQFrameWork;
using UnityEngine;


/// <summary>
/// 角色头顶上的HUD管理器
/// weitiancheng
/// </summary>
public class PlayerHud:MonoBehaviour {
    #region Public Properties

    public Vector3 ScreenOffset = new Vector3(0, 8.5f, 0);

    public UIText PlayerNameText;
    public UIText PlayerScoreText;


    #endregion

    #region Private Properties

   

    float _characterControllerHeight = 0f;

    Transform _targetTransform;

    Renderer _targetRenderer;

    Vector3 _targetPosition;

    public PlayerControl PlayerControl;
    public PlayerModel _selfModel;
    #endregion


    public void Init(PlayerControl playerControl)
    {
        this.PlayerControl = playerControl;
        _selfModel = PlayerControl.SelfModel;
        PlayerNameText.Text = _selfModel.Name;
        switch (playerControl.tag)
        {
            case CustomDefine.Tag_Player_Monster:
                ScreenOffset = new Vector3(0, 10.5f, 0);
                break;
            case CustomDefine.Tag_Player_Car:
                ScreenOffset = new Vector3(0, 8.5f, 0);
                break;
        }
        //PlayerNameText.Text = PlayerControl.photonView.owner.NickName;
        //        PlayerHealthSlider.slider.maxValue = PlayerHealthSlider.slider.value= PlayerControl.SelfModel.CurrentHp;
    }

    public void UpdateScore()
    {
        PlayerScoreText.Text = _selfModel.Score.ToString();
        
    }

    //private Vector3 screenPos;
    public void UpdatePos(Camera worldCamera,Camera uiCamera)
    {
        //screenPos= uiCamera.ScreenToWorldPoint(worldCamera.WorldToScreenPoint(PlayerControl.transform.position)) + ScreenOffset;


        transform.position = uiCamera.ScreenToWorldPoint(worldCamera.WorldToScreenPoint(PlayerControl.transform.position)) + ScreenOffset;

        //transform.rectTransform().anchoredPosition = pos2;
        //transform.position = Vector3.Lerp(transform.position, pos2 + ScreenOffset, Time.deltaTime * 5);
        //transform.forward = gameObject.transform.position - Camera.main.transform.position;//hud看向的应该是主摄像头的方向

    }
  
}
