using System.Collections;
using System.Collections.Generic;
using HQFrameWork;
using UnityEngine;

public class EntityData : BaseData
{

    [CsvTitle("Id")]
    public int Id;
    [CsvTitle("Name")]
    public string Name;
    [CsvTitle("Type")]
    public int Type;
    [CsvTitle("Width")]
    public int Width;
    [CsvTitle("Height")]
    public int Height;
    [CsvTitle("Score")]
    public int Score;

    public override DataType dataType
    {
        get { return DataType.EntityData; }
    }
}
