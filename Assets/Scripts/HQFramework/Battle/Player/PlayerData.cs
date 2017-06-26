using System.Collections;
using System.Collections.Generic;
using HQFrameWork;
using UnityEngine;

public class PlayerData : BaseData {
    //CsvTitle内的符号必须和表里配的最上面的名字一样而不是随便取的
    [CsvTitle("Type")]
    public int Type;
    [CsvTitle("Name")]
    public string Name;
    [CsvTitle("Speed")]
    public int Speed;
    [CsvTitle("Gold")]
    public int Gold;
  
    
    public override DataType dataType
    {
        get { return DataType.PlayerData; }
    }

    
    public override void InitData()
    {
        
        base.InitData();
    }

  
}
