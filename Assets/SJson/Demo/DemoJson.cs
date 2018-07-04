﻿using System;

[Serializable]
public class SampleJsonClass : SJson<SampleJsonClass>
{
    public override bool IsAccessFilter { get { return true; } }

    protected override void SetWriteAccess()
    {
        this.AddWriteAccessClass<SampleJsonClass>(ESJsonAccessLevel.DELETE);
    }

    public int num;
    public string name;
    public SampleJsonSaveClass m_save_info;
    public SampleJsonSaveClass save_info
    {
        get { return this.m_save_info ?? (this.m_save_info = new SampleJsonSaveClass()); }
    }

}

[Serializable]
public class SampleJsonSaveClass
{
    public int count;
    public long time_stamp;
}