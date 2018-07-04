﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;

public class SJson<T> where T : class, new()
{
    #region var

    // Write And Read Root Path.
    private static string ROOT = "SJsonDB/";

    // this my class Name
    public static string ClassName { get { return typeof(T).Name; } }

    // access filters.
    public virtual bool IsAccessFilter { get { return false; } }

    #endregion

    ///<summary>
    /// ReLoad Json Files.
    /// <para>
    /// このクラスに紐づく、Jsonファイルを再読み込みをします
    /// </para>
    ///</summary>
    public void ReLoad()
    {
        m_DB = m_Load();
    }

    ///<summary>
    /// Saved Json Files. Path : Root + ClassName
    /// <para>
    /// このクラスをJsonファイルとして保存します。
    /// </para>
    ///</summary>
    public void Save()
    {
        string callClassName = new StackFrame(1).GetMethod().ReflectedType.FullName;
        if (!this.m_IsAccessCheck(callClassName, ESJsonAccessLevel.SAVE))
        {
            this.ReLoad();
            UnityEngine.Debug.LogErrorFormat("!!! Save Access Error !!!  Call Class Name : {0}", callClassName);
            return;
        }

        if (Directory.Exists(PATH))
        {
            try
            {
                this.m_Delete(PATH);
                if (IsEditor)
                    this.m_Delete(PATH + ".meta");
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogFormat("<color=red>File Delete Error.{0}</color>", e.Message);
            }
        }
        this.m_Write(PATH, UnityEngine.JsonUtility.ToJson(DB));
    }

    ///<summary>
    /// Delete Json Files. and Create This New Class.
    /// <para>
    /// 保存されているJsonファイルを削除し、このクラスを再生成します
    /// </para>
    ///</summary>
    public void Delete()
    {
        try
        {
            string callClassName = new StackFrame(1).GetMethod().ReflectedType.FullName;
            if (!this.m_IsAccessCheck(callClassName, ESJsonAccessLevel.DELETE))
            {
                UnityEngine.Debug.LogErrorFormat("!!! Delete Access Error !!!  Call Class Name : {0}", callClassName);
                this.ReLoad();
                return;
            }

            if (IsFilesSave)
            {
                this.m_Delete(PATH);
                if (IsEditor)
                    this.m_Delete(PATH + ".meta");
            }
            else
            {
                if (UnityEngine.PlayerPrefs.HasKey(PlayerPrefabsKey))
                {
                    UnityEngine.PlayerPrefs.DeleteKey(PlayerPrefabsKey);
                    UnityEngine.PlayerPrefs.Save();
                }
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("File Delete Error." + e.Message);
        }

        ReLoad();
    }

    #region Access Filter Method.
    // Class Name : Enum
    private Dictionary<string, ESJsonAccessLevel> m_AccessLels;

    protected virtual void SetWriteAccess() { }

    private void m_SetWriteAccess()
    {
        this.m_AccessLels = new Dictionary<string, ESJsonAccessLevel>();
        this.SetWriteAccess();
    }

    protected void AddWriteAccessClass<T>(ESJsonAccessLevel level) where T : new()
    {
        this.m_AccessLels[typeof(T).Name] = level;
    }

    // True -> Access ok. false -> dont Access.
    private bool m_IsAccessCheck(string name, ESJsonAccessLevel level)
    {
        if (this.m_AccessLels == null)
            this.m_SetWriteAccess();

        if (!this.IsAccessFilter)
            return true;

        if (!this.m_AccessLels.ContainsKey(name))
            return false;

        if (this.m_AccessLels[name] == ESJsonAccessLevel.FULL)
            return true;

        if (this.m_AccessLels[name] == level)
            return true;

        return false;
    }
    #endregion

    #region  Privete Methods.
    private static bool IsEditor
    {
        get
        {
#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_OSX
            return true;
#else
            return false;
#endif
        }
    }

    private static bool IsFilesSave
    {
        get
        {
            if (IsEditor)
                return true;

#if UNITY_WEBGL
            return false;
#else
            return true;
#endif
        }
    }

    private static string PlayerPrefabsKey
    {
        get { return Convert.ToBase64String(Encoding.UTF8.GetBytes(PATH)); }
    }

    private static string PATH
    {
        get
        {
            if (IsEditor)
            {
                return UnityEngine.Application.dataPath + "/" + ROOT + ClassName;
            }
            else
            {
                return UnityEngine.Application.persistentDataPath + ClassName;
            }
        }
    }

    private static T m_DB = null;
    public static T DB { get { return m_DB ?? (m_DB = m_Load()); } }

    private void m_Delete(string self)
    {
        var info = new FileInfo(self);

        if (info.Exists)
        {
            try
            {
                info.Delete();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("Delete Error : " + e.Message);
            }
        }
    }

    private static T m_Load()
    {
        string json = m_ReadString(PATH);
        if (string.IsNullOrEmpty(json))
            return new T();

        return UnityEngine.JsonUtility.FromJson<T>(json);
    }

    private static string m_ReadString(string self)
    {
        try
        {
            if (IsFilesSave)
            {
                StreamReader m_reader = new StreamReader(self);
                string m_contents = m_reader.ReadToEnd();
                m_reader.Close();
                m_reader.Dispose();

                return m_contents;
            }
            else
            {
                if (UnityEngine.PlayerPrefs.HasKey(PlayerPrefabsKey))
                {
                    return UnityEngine.PlayerPrefs.GetString(PlayerPrefabsKey);
                }
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Not Load To Json Files" + e.Message);
        }

        return string.Empty;
    }

    private void m_DeleteFolder(string self) { Directory.Delete(self, true); }

    public bool Exists()
    {
        return this.m_Exists(PATH);
    }

    private bool m_Exists(string self)
    {
        return File.Exists(self);
    }

    private void m_Write(string self, string contents)
    {
        UnityEngine.Debug.LogFormat("Write Path : {0}, Contents : {1}", self, contents);
        if (IsFilesSave)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(contents);
            this.m_WriteBytes(self, bytes);
#if UNITY_IOS
        UnityEngine.iOS.Device.SetNoBackupFlag(self);
#endif
        }
        else
        {
            UnityEngine.PlayerPrefs.SetString(PlayerPrefabsKey, contents);
            UnityEngine.PlayerPrefs.Save();
        }
    }

    private void m_WriteBytes(string self, byte[] bytes)
    {
        try
        {
            string m_Directory = Path.GetDirectoryName(self);

            if (!Directory.Exists(m_Directory))
            {
                Directory.CreateDirectory(m_Directory);
#if UNITY_IOS
                UnityEngine.iOS.Device.SetNoBackupFlag(m_Directory);
#endif
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Create Directory Error : " + e.Message);
        }

        try
        {
            File.WriteAllBytes(self, bytes);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Write My Clietn Paths : " + e.Message);
        }
    }
    #endregion
}

public enum ESJsonAccessLevel
{
    NONE = 0,
    SAVE = 30,
    DELETE = 30,
    FULL = 99
}