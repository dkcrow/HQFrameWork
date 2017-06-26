using DG.Tweening;
using UnityEngine;

public  class BuffBase : MonoBehaviour
{
    /// <summary>
    /// inpector 下设置类型 prefab还是要做那么多个,但是脚本不需要那么多了 
    /// </summary>
    public EnumDefine.BuffType SelfBuffType;


    void Start()
    {
        //if (PhotonNetwork.isMasterClient)
        //{
        //    DOVirtual.DelayedCall(new System.Random().Next(30, 50), (() =>   ///一段时间没人捡到则销毁
        //    {
        //        PhotonNetWorkManager.Instance.Destroy(this.gameObject);
        //    }));
        //}
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag != CustomDefine.Tag_Player) return;

        //if (PhotonNetwork.isMasterClient)
        //{
        //    PhotonNetWorkManager.Instance.Destroy(this.gameObject);
        //}
    }

    void OnDestroy()
    {
        Messenger.Instance.BroadCastEventMsg<BuffBase>(MessageEventType.OnBuffDestroy, this);
    }

    /// <summary>
    /// 人物吃到buff后的属性变换
    /// </summary>
    /// <param name="player"></param>
    public virtual void ExecuteBuffFunction(PlayerControl playerControl)
    {
        switch (SelfBuffType)
        {
                //case EnumDefine.BuffType.HpRecover:
                //if (playerControl.SelfModel.MaxHp - playerControl.SelfModel.CurrentHp <= 1)
                //{
                //    playerControl.SelfModel.CurrentHp = playerControl.SelfModel.MaxHp;
                //}
                //else
                //{
                //    playerControl.SelfModel.CurrentHp += 1;
                //}             
                //break;
                //case EnumDefine.BuffType.AddSpeed:
                //    playerControl.SelfModel.Speed += 0.1f;
                //    break;
                //case EnumDefine.BuffType.AddJump:
                //    playerControl.SelfModel.Jump += 0.1f;
                //    break;
                //case EnumDefine.BuffType.AddRange:
                //    playerControl.SelfModel.Range += 0.1f;
                //    break;
        }
       
    }

  
}
