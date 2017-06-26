using System.Collections;
using System.Collections.Generic;
using HQFrameWork;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    private void Awake()
    {
        Application.targetFrameRate = 30;               //限60帧
        Screen.sleepTimeout = SleepTimeout.NeverSleep;  //禁止休眠
        QualitySettings.antiAliasing = 2;
       
    }

   

    private void Start()
    {
        StartGame();
    }
    public void StartGame()
    {
        InitPlayerPreference();
        HQSceneManager.Instance.ChangeScene<InitScene>();

    }

    private void Update()
    {
        ///退出游戏
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    /// <summary>
    /// 初始化游戏设置
    /// </summary>
    private void InitPlayerPreference()
    {
        HQAudioPlayer.Instance.ToggleBGM(PlayerPreference.Instance.MusicOn);
        HQAudioPlayer.Instance.ToggleSFX(PlayerPreference.Instance.MusicEffectOnOff);
        HQAudioPlayer.Instance.SetVolumeBGM(PlayerPreference.Instance.MusicVolume);
        HQAudioPlayer.Instance.SetVolumeSFX(PlayerPreference.Instance.MusicEffectVolume);

        CustomDefine.PlayerName=PlayerPrefs.GetString("PlayerName", "Player" + Random.Range(1, 9999));
    }
}
