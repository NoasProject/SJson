using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DemoMain : MonoBehaviour
{
    [SerializeField]
    private InputField m_NumField;
    public InputField NumField { get { return this.m_NumField ?? (this.m_NumField = GameObject.Find("m_NumField").GetComponent<InputField>()); } }

    [SerializeField]
    private InputField m_NameField;
    public InputField NameField { get { return this.m_NameField ?? (this.m_NameField = GameObject.Find("m_NameField").GetComponent<InputField>()); } }

    [SerializeField]
    private Button m_SaveBtn;
    public Button SaveBtn { get { return this.m_SaveBtn ?? (this.m_SaveBtn = GameObject.Find("m_SaveBtn").GetComponent<Button>()); } }

    void Awake()
    {
        this.SaveBtn.onClick.AddListener(OnSaveBtn);
    }

    void Start()
    {
        this.NumField.text = SampleJsonClass.DB.num.ToString();
        this.NameField.text = SampleJsonClass.DB.name;
    }

    public void OnSaveBtn()
    {
        if (SampleJsonClass.DB.save_info.time_stamp > 0)
            Debug.LogErrorFormat("After Save DateTime : {0}", new DateTime().AddSeconds(SampleJsonClass.DB.save_info.time_stamp).ToString("yyyy-MM-dd hh:mm:ss"));

        SampleJsonClass.DB.num = int.Parse(m_NumField.text);
        SampleJsonClass.DB.name = m_NameField.text;
        SampleJsonClass.DB.save_info.count = SampleJsonClass.DB.save_info.count + 1;
        SampleJsonClass.DB.save_info.time_stamp = (long)(DateTime.Now - new DateTime()).TotalSeconds;
        SampleJsonClass.DB.Save();
    }
}
