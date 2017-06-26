using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 这个不需要挂在物体上,因为你只需要关注自己的属性就行了,通过plaeyrcontrol同步出去
/// </summary>
public class PlayerModel
{
    public int NetWorkId;

    public string Name;

    public float Speed;

    public int Score=0;


    public void InitProperty( float speed,int netWorkId)
    {
        this.NetWorkId = netWorkId;
        this.Speed = speed;
    }

   
}
