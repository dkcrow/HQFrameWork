using System.Collections;
using System.Collections.Generic;
using HQFrameWork;
using PathologicalGames;
using UnityEngine;

public class PlayerHudManager : MBSingleton<PlayerHudManager> {

    private Vector3 m_TargetPoint;
    private List<PlayerHud> playerHudList = new List<PlayerHud>();//当前正投入使用的hud,需要不断更新它们的位置
    private const string Pool_Name = "PlayerHudPool";
    private SpawnPool playerHudPool;
    private Transform playerHudIns;
    private Camera camera2d;
    private Camera camera3d;

    PlayerHudManager()
    {
        singletonType=SingletonType.PerSceneInstance;
    }
    void Awake()
    {
        playerHudPool = PoolManager.Pools[Pool_Name];//或者这里通过getcomponent来获取
        playerHudIns = playerHudPool._perPrefabPoolOptions[0].prefab;
        camera2d = UGUIDialogManager.Instance.GetInstance<UIRoot>().Camera2d;
        camera3d = UGUIDialogManager.Instance.GetInstance<UIRoot>().Camera3d;
    }

    private void Update()
    {
        for (int i = 0; i < playerHudList.Count; i++)
        {
            PlayerHud _hud = playerHudList[i];
            if (_hud.isActiveAndEnabled&&_hud.PlayerControl != null )//主相机作为镜头
            {
                _hud.UpdateScore();
                _hud.UpdatePos(camera3d,camera2d);
            }
        }
    }
    /// <summary>
    /// 每次有玩家加入游戏都要创建血条
    /// </summary>
    public PlayerHud CreateOrGetPlayerHUD(PlayerControl playerCtrl)
    {
        
        var playerHud = playerHudPool.Spawn(playerHudIns).GetComponent<PlayerHud>();
        playerHud.transform.SetParent(UGUIDialogManager.Instance.GetInstance<UIRoot>().gmobjCanvas2D.transform);
        playerHud.transform.localScale=Vector3.one;
        playerHud.transform.rectTransform().anchoredPosition3D =Vector3.zero;
        playerHud.transform.rectTransform().sizeDelta =Vector3.zero;
        //playerHud.transform.rotation=Quaternion.identity;
        playerHud.Init(playerCtrl);
        playerHudList.Add(playerHud);
        return playerHud;
    }

    /// <summary>
    /// 每次离开/掉线 删除一个
    /// </summary>
    /// <param name="hud"></param>
    public void DisposePlayerHUD(PlayerHud hud)
    {
        if (hud == null|| playerHudPool==null)
        {
            return;
        }

        if (playerHudList.Contains(hud) )
        {
            playerHudList.Remove(hud);
        }

        playerHudPool.Despawn(hud.transform);
    }

    public void DestroyPlayerHudPool()
    {
        
    }
}
