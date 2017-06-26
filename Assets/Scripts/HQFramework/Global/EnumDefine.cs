using System.Collections;
using System.Collections.Generic;
using HQFrameWork;
using UnityEngine;

public class EnumDefine
{

    public enum EntityType
    {
       Monster,
       Car,
       Building
    }

    

    public enum BattleGameState
    {
        GamePrepare,
        GameOn,
        GameOver

    }

    public enum Identity
    {
        self,
        other
    }

    public enum ControlType
    {
        [StringValue("FirstControl")]
        FirstControl,
        [StringValue("ThirdControl")]
        ThirdControl,
        AI
    }

    public enum BuffType
    {
        [StringValue("HpRecover")]
        HpRecover,
        [StringValue("AddSpeed")]
        AddSpeed,
        [StringValue("AddJump")]
        AddJump,
        [StringValue("AddRange")]
        AddRange,
//        [StringValue("AddHpMax")]
//        AddHpMax,
       
    }
}

   



