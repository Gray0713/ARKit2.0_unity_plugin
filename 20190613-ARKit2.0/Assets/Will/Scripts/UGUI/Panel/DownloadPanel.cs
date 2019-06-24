using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WillFramework;


/// <summary>
/// 这个类游戏运行期间只会执行一次下载游戏资源
/// 后面会对这个对象清除
/// </summary>
public class DownloadPanel : MonoBehaviour
{
    public Slider DownloadSlider;
    public Text FileName;
    public Text Progress;

    private bool m_StartDownload;
    private string m_FtpServer = "";
    private string m_SavePath = "";
    private WillDownloadPanelCallBack m_CallBack = null;
    private object[] m_Parameters = null;

    private void Update()
    {
        if (m_StartDownload == true)
        {
            DownloadSlider.value = WillLocalResManager.Instance.DownLoadProgress;
            FileName.text = WillLocalResManager.Instance.NowDownLoadFileName;
            Progress.text = WillLocalResManager.Instance.NowDownLoadMessage;
        }
    }

    /// <summary>
    /// 启动下载面板
    /// </summary>
    /// <param name="callBack"></param>
    /// <param name="ftpServer"></param>
    /// <param name="savePath"></param>
    public void StartDownload(WillDownloadPanelCallBack callBack,string ftpServer, string savePath = "/"+ ConstData.WorldMapUrl +"/",object[] parameters = null)
    {
        m_CallBack = callBack;
        m_StartDownload = true;
        m_FtpServer = ftpServer;
        m_Parameters = parameters;
        m_SavePath = Application.persistentDataPath + savePath;
#if UNITY_ANDROID
        m_SavePath = Application.persistentDataPath + savePath;
#endif

#if UNITY_IPHONE
            m_SavePath = Application.persistentDataPath + savePath;
#endif
        GameObject.DontDestroyOnLoad(gameObject);
        WillLocalResManager.Instance.InitCloudRes(this, m_FtpServer, m_SavePath);
        WillLocalResManager.Instance.UpdataRes(UpdataResCallBack, null);
    }

    /// <summary>
    /// 检查更新回调
    /// </summary>
    /// <param name="overOrFailed"></param>
    /// <param name="message"></param>
    /// <param name="parameters"></param>
    private void UpdataResCallBack(bool overOrFailed, string message, object[] parameters = null)
    {
        WDebuger.Log(message);
        if (overOrFailed == true)
        {
            m_CallBack(true, "下载完成", m_Parameters);
            Destroy(gameObject);
        }
        else
        {
            m_CallBack(false, message, m_Parameters);
            WDebuger.LogError("错误");
        }
    }
}
