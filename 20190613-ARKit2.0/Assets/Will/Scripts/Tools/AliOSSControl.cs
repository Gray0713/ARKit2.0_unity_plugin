using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using WillFramework;
using UnityEngine.UI;

public class AliOSSControl : MonoBehaviour {

    public Text Text;

    private static string m_ResPath;
    private static string m_ConfigPath;
    private static string m_ConfigFileName;
    //private static ABConfigUpdata m_Config;
    private static List<string> m_ResFilePath = new List<string>();
    private static List<string> m_ItemResFilePath = new List<string>();
    private static Dictionary<string, string> m_ResFileName = new Dictionary<string, string>();
    private static Dictionary<string, string> m_ResServerPath = new Dictionary<string, string>();
    private static int m_NowUpCount = 0;
    private static bool m_UploadOver = false;
    private static bool m_UploadFailure = false;
    private static string m_ReturnoMsg;
    private static bool m_UploadConfig = false;
    private static bool m_AllUploadOver = false;

    public static string ReturnMsg { get { return m_ReturnoMsg; } }


    private void Update()
    {
        Text.text = string.IsNullOrEmpty(ReturnMsg) ? "save" : ReturnMsg;
        if (m_UploadConfig == true)
        {
            //OpenConfigFile();
        }
        if (m_AllUploadOver == true)
        {
            UpOver();
        }
    }

    public void OpenWindow()
    {
        UpToServer();
    }

    public static void UpToServer()
    {
        m_ResFilePath.Clear();
        m_ItemResFilePath.Clear();
        m_ResFileName.Clear();
        m_ResServerPath.Clear();
        m_UploadConfig = false;
        m_AllUploadOver = false;
        m_NowUpCount = 0;
        //m_ResPath = Application.dataPath + ConstData.ABNEWPAT;
        m_ResPath = Application.persistentDataPath + "/" + ConstData.WorldMapUrl;
        GetWorldMap();
        DirectoryInfo dir = new DirectoryInfo(m_ResPath);
        FileInfo[] info = dir.GetFiles();
        print(info.Length);
        for (int i = 0; i < info.Length; i++)
        {
            print("This file : " + i);
            if (info[i].Name.Contains(".json"))
            {
                m_ConfigPath = info[i].FullName;
                m_ConfigFileName = info[i].Name;
            }
            m_ResFilePath.Add(info[i].FullName);
            m_ItemResFilePath.Add(info[i].FullName);
            m_ResFileName.Add(info[i].FullName, info[i].Name);
        }
        WillAliyunUpload.Instance.InitAliyun(ConstData.OSSpath, ConstData.OSSkey, ConstData.OSSpassword);
        try
        {
            print("m_ItemResFilePath : " + m_ItemResFilePath.Count);
            print("m_ResFileName : " + m_ResFileName.Count);
            string itemResPath = m_ItemResFilePath[0];
            string itemResCloudName = m_ResFileName[m_ItemResFilePath[0]];
            WillAliyunUpload.Instance.UploadData(itemResPath, ConstData.companyname, ConstData.projectname + "/" + itemResCloudName, UpVideoCallBack, UploadProgressChangedEvent);
        }
        catch (Exception e)
        {
            WDebuger.LogError(e.Message);
            throw;
        }
    }

    static void GetWorldMap()
    {
        WorldMapData worldMapData = new WorldMapData();
        if (!Directory.Exists(m_ResPath))
        {
            worldMapData.version = "1.0";
            worldMapData.date = System.DateTime.Now.ToString();
            Directory.CreateDirectory(m_ResPath);
            worldMapData.SaveJson(m_ResPath + "/" + ConstData.WLOCALCONFIG);
        }
        else
        {
            worldMapData = WillData.LoadJson<WorldMapData>(m_ResPath + "/WorldMap.json");
            float current_version = float.Parse(worldMapData.version);
            //DirectoryInfo dir = new DirectoryInfo(m_ResPath);
            //dir.Delete(true);
            worldMapData.version = (current_version + 0.1f).ToString();
            worldMapData.date = System.DateTime.Now.ToString();
            Directory.CreateDirectory(m_ResPath);
            worldMapData.SaveJson(m_ResPath + "/" + ConstData.WLOCALCONFIG);
        }
    }

    /// <summary>
    /// 上传进度回调
    /// </summary>
    /// <param name="over"></param>
    /// <param name="process"></param>
    private static void UploadProgressChangedEvent(bool over, float process)
    {
        if (over == true)
            return;
        try
        {
            Debug.Log("上传文件：" + m_ResFileName[m_ItemResFilePath[0]] + "  当前进度：" + (m_ResFilePath.Count - m_ItemResFilePath.Count) + "/" + m_ResFilePath.Count + " process: " + process);
        }
        catch (Exception ex)
        {
            Debug.Log("文件上传失败，请检查！ 消息：" + ex.Message);
                UpToServer();
        }
    }

    /// <summary>
    /// 单个文件上传完成回调
    /// </summary>
    /// <param name="over"></param>
    /// <param name="msg"></param>
    private static void UpVideoCallBack(bool over, string msg)
    {
        if (over == false)
        {
            m_ReturnoMsg = "failure";
            WDebuger.LogError("文件上传失败，请检查！ 消息：" + msg);
            //EditorUtility.ClearProgressBar();
            //if (ChinarMessage.ShowRetryCancel("文件上传失败，请检查！ 消息：" + msg, "警告"))
                UpToServer();
            return;
        }
        WDebuger.Log("文件上传成功！文件名：" + m_ResFileName[m_ItemResFilePath[0]]);
        m_ReturnoMsg = "over";
        m_ResServerPath.Add(m_ResFileName[m_ItemResFilePath[0]], msg);
        m_ItemResFilePath.RemoveAt(0);
        if (m_ItemResFilePath.Count <= 0)
        {
            m_UploadConfig = true;
            return;
        }
        try
        {
            string itemResPath = m_ItemResFilePath[0];
            string itemResCloudName = m_ResFileName[m_ItemResFilePath[0]];
            WillAliyunUpload.Instance.UploadData(itemResPath, ConstData.companyname, ConstData.projectname + "/" + itemResCloudName, UpVideoCallBack, UploadProgressChangedEvent);
        }
        catch (Exception e)
        {
            WDebuger.LogError("文件上传失败，请检查！ 消息：" + e.Message);
            //EditorUtility.ClearProgressBar();
            //if (ChinarMessage.ShowRetryCancel("文件上传失败，请检查！ 消息：" + msg, "警告"))
                UpToServer();
            throw;
        }
    }

    private static void UpOver()
    {
        m_AllUploadOver = false;
        WDebuger.Log("文件上传成功！文件名：" + m_ConfigFileName);
        //ChinarMessage.ShowOK("上传完成！上传文件数量：" + (m_ResFilePath.Count + 1), "完成消息");
        WDebuger.Log("上传完成！上传文件数量：" + (m_ResFilePath.Count + 1));
        //EditorUtility.ClearProgressBar();
        //m_Window.Close();
    }

}
