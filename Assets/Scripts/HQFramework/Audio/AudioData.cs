using HQFrameWork;

[System.Serializable]
public class AudioData
{
    [CsvTitle("音效ID")]
    public uint Id;
    [CsvTitle("音效名")]
    public string Name;
    [CsvTitle("音效类型")]
    public uint Type;
}