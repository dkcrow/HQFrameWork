using System.Collections;
using System.Collections.Generic;
using HQFrameWork;
using UnityEngine;

public class TestBattleScene : BaseScene {

	
    public override EnumSceneType SceneType
    {
        get
        {
            return EnumSceneType.TEST_BATTLE_SCENE;
        }
    }

    public override void OnEnterScene()
    {
        UGUIDialogManager.Instance.GetInstance<DialogBattleTest>();
        GameObject localLogicManager = new GameObject();
        
        //Debug.Log("EnterSceneOk");
        // for (int i = 0; i < 100; i++)
        // {
        //     int random = Random.Range(2, 6);
        // }
         EntityBase entity=EntityManager.Instance.AddEntityByType(10001,1, Random.Range(-CustomDefine.WorldHalfWidth, CustomDefine.WorldHalfWidth), Random.Range(-CustomDefine.WorldHalfHeight, CustomDefine.WorldHalfHeight));
        localLogicManager.name = "localLogicManager";
        BattleCameraFollow battleCameraFollow = localLogicManager.AddComponent<BattleCameraFollow>();
        battleCameraFollow.SetTarget(entity.transform);

    }

    public override void OnLeaveScene()
    {
        Debug.Log("LeaveSceneOk");
    }
}
