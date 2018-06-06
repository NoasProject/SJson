
[System.Serializable]
public class DemoJson : SJSon<DemoJson>
{
    public int num;
    public string name;
    public DemoJson2 m_info;
    public DemoJson2 info { get { return this.m_info ?? (this.m_info = new DemoJson2()); } }
}

[System.Serializable]
public class DemoJson2
{
    public int save_count;
}
