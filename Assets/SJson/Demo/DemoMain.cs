using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        this.NumField.text = DemoJson.DB.num.ToString();
        this.NameField.text = DemoJson.DB.name;
    }

    public void OnSaveBtn()
    {
        DemoJson.DB.num = int.Parse(m_NumField.text);
        DemoJson.DB.name = m_NameField.text;
        DemoJson.DB.info.save_count = DemoJson.DB.info.save_count + 1;
        DemoJson.DB.Save();
    }
}
