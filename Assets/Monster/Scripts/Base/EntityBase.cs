using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class EntityBase : MonoBehaviour
{
    public bool IsMine = false;
    public bool IsAi = false;
    public int Id;
    public int Type;
    public int Width;
    public int Height;
    public int Score;

    public void Init(EntityData entityData)
    {
        
        this.Type = entityData.Type;
        this.Width = entityData.Width;
        this.Height = entityData.Height;
        this.Score = entityData.Score;
    }

    public void SetId(int id)
    {
        this.Id = id;
    }
}
