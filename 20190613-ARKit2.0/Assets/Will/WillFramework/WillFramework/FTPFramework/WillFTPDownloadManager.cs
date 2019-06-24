#region Version Info
/*========================================================================
* 【本类功能概述】
* 
* 作者：wen      时间：2019/1/9 11:47:11
* 文件名：FTPDownloadManager
* 版本：V1.0.1
*
* 修改者：          时间：              
* 修改说明：
* ========================================================================
*/
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Net;

namespace WillFramework
{
    public class WillFTPDownloadManager : WillSingleton<WillFTPDownloadManager>
    {
        private string m_FTPHost;
        private string m_FTPUserName;
        private string m_FTPPasswork;
        private string m_FilePath;
        private WillFtpDownloadCallBack m_FtpDownloadCallBack = null;
        private WillFtpDownloadProgressChangerEvent m_DownloadProgressEvent = null;
        private WebClient m_WebClient = null;
        private float m_Progreess = 0;
        private int m_CurrentDownloadBytpCount = 0;
        private bool m_DownloadOver = false;
        private bool m_DownloadSucceed = false;
        private byte[] m_DownloadData = null;

        /// <summary>
        /// 当前下载进度
        /// </summary>
        public float Progress { get { return m_Progreess; } }
        /// <summary>
        /// 当前下载字节数量
        /// </summary>
        public int CurrentDownloadBytpCount { get { return m_CurrentDownloadBytpCount; } }
        /// <summary>
        /// 下载结束
        /// </summary>
        public bool DownloadOver { get { return m_DownloadOver; } }
        /// <summary>
        /// 下载成功
        /// </summary>
        public bool DownloadSucceed { get { return m_DownloadSucceed; } }
        /// <summary>
        /// 下载到的资源
        /// </summary>
        private byte[] DownloadData { get { return m_DownloadData; } }

        /// <summary>
        /// 初始化ftp下载
        /// </summary>
        /// <param name="ftpUserName">ftp密码，默认为匿名登录：anonymous</param>
        /// <param name="ftpPasswork">ftp密码，默认为匿名登录的空密码</param>
        public void Init(string ftpUserName , string ftpPasswork)
        {
            m_FTPUserName = ftpUserName;
            m_FTPPasswork = ftpPasswork;
        }

        /// <summary>
        /// 通过ftp从服务器下载资源
        /// </summary>
        /// <param name="downloadHost">ftp地址</param>
        /// <param name="callBack">完成回调函数</param>
        /// <param name="progress">进度改变回调函数</param>
        public void DownLoadData(string downloadHost,WillFtpDownloadCallBack callBack , WillFtpDownloadProgressChangerEvent progress = null)
        {
            if (string.IsNullOrEmpty(downloadHost))
            {
                WDebuger.Log("ftp地址为空，请检查！ 参数：" + downloadHost);
                return;
            }
            m_FTPHost = downloadHost;
            m_FtpDownloadCallBack = callBack;
            m_DownloadProgressEvent = progress;
            m_Progreess = 0;
            m_DownloadData = null;
            m_DownloadSucceed = false;
            m_DownloadOver = false;
            m_WebClient = new WebClient();
            Uri uri = new Uri(downloadHost);

            m_WebClient.DownloadDataCompleted += new DownloadDataCompletedEventHandler(OnDataUploadCompleted);
            m_WebClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(OnDownloadDataProgressChanged);
            m_WebClient.Credentials = new NetworkCredential(m_FTPUserName, m_FTPPasswork);

            m_WebClient.DownloadDataAsync(uri);
        }

        //下载进度变化调用
        private void OnDownloadDataProgressChanged(object sender,DownloadProgressChangedEventArgs e)
        {
            m_CurrentDownloadBytpCount = (int)e.TotalBytesToReceive;
            m_Progreess = (float)e.BytesReceived / (float)m_CurrentDownloadBytpCount;
            m_DownloadProgressEvent?.Invoke(m_Progreess, e);
        }

        //下载完成
        private void OnDataUploadCompleted(object sender,DownloadDataCompletedEventArgs e)
        {
            m_DownloadOver = true;
            WDebuger.Log(m_FTPHost);
            if (e.Error != null)
            {
                m_DownloadSucceed = false;
                m_FtpDownloadCallBack?.Invoke(m_FTPHost, m_DownloadSucceed, e.Error.Message, null, e);
            }
            else
            {
                m_DownloadSucceed = true;
                m_FtpDownloadCallBack?.Invoke(m_FTPHost, m_DownloadSucceed, "Succeed", e.Result, e);
            }
        }
    }
}