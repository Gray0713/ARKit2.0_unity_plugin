#region Version Info
/*========================================================================
* 【本类功能概述】
* 
* 作者：wen      时间：2019/1/9 10:16:48
* 文件名：FtpUploadFramework
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
using System.IO;

namespace WillFramework
{
    /// <summary>
    /// 通过ftp将资源上传到服务器
    /// </summary>
    public class WillFtpUploadManager : WillSingleton<WillFtpUploadManager>
    {
        private string m_FTPHost;
        private string m_FTPUserName;
        private string m_FTPPasswork;
        private string m_FilePath;
        private long m_DataByteCount = 0;
        private WillFtpUploadCallBack m_FTPUploadCallBack = null;
        private WillFtpUploadProgressChangedEvent m_UploadProgressEvent = null;
        private int m_UploadDataByteCount = 0;
        private WebClient m_WebClient = null;
        private float m_Progreess = 0;
        private int m_CurrentUploadBytpCount = 0;
        private bool m_UploadOver = false;
        private bool m_UploadSucceed = false;

        /// <summary>
        /// 当前上传进度
        /// </summary>
        public float Progress { get { return m_Progreess; } }
        /// <summary>
        /// 当前上传字节数量
        /// </summary>
        public int CurrentUploadBytpCount { get { return m_CurrentUploadBytpCount; } }
        /// <summary>
        /// 上传结束
        /// </summary>
        public bool UploadOver { get { return m_UploadOver; } }
        /// <summary>
        /// 上传成功
        /// </summary>
        public bool UploadSucceed { get { return m_UploadSucceed; } }

        /// <summary>
        /// 初始化ftp上传
        /// </summary>
        /// <param name="ftpHost">ftp地址</param>
        /// <param name="ftpUserName">ftp密码，默认为匿名登录：anonymous</param>
        /// <param name="ftpPasswork">ftp密码，默认为匿名登录的空密码</param>
        public void Init(string ftpHost, string ftpUserName , string ftpPasswork )
        {
            if (string.IsNullOrEmpty(ftpHost))
            {
                WDebuger.LogError("ftp地址为空，请检查！ 参数：" + ftpHost);
                return;
            }
            m_FTPHost = ftpHost;
            m_FTPUserName = ftpUserName;
            m_FTPPasswork = ftpPasswork;
        }

       


        /// <summary>
        /// 通过ftp上传文件到服务器
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="fileName">存储到服务器的文件名，带后缀</param>
        /// <param name="callBack">完成回调函数</param>
        /// <param name="progress">进度改变回调函数</param>
        public void UploadData(string filePath, string fileName, WillFtpUploadCallBack callBack ,WillFtpUploadProgressChangedEvent proEvent = null)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                WDebuger.LogError("文件地址为空，请检查！ 参数：" + filePath);
                return;
            }
            if (!File.Exists(filePath))
            {
                WDebuger.LogError("找不到文件，请检查！ 参数" + filePath);
                return;
            }
            byte[] data = File.ReadAllBytes(filePath);
            UploadData(data, fileName, callBack, proEvent);
        }

        /// <summary>
        /// 通过ftp上传字节数据到服务器
        /// </summary>
        /// <param name="data">字节数据</param>
        /// <param name="fileName">存储到服务器的文件名，带后缀</param>
        /// <param name="callBack">完成回调函数</param>
        /// <param name="progress">进度改变回调函数</param>
        public void UploadData(byte[] data, string fileName, WillFtpUploadCallBack callBack, WillFtpUploadProgressChangedEvent proEvent = null)
        {
            if (string.IsNullOrEmpty(m_FTPHost) || string.IsNullOrEmpty(m_FTPUserName))
            {
                WDebuger.LogError("未进行初始化，请检查！");
                return;
            }
            if (string.IsNullOrEmpty(fileName))
            {
                WDebuger.LogError("存储文件名不能为空，请检查！ 参数：" + fileName);
            }
            m_FTPUploadCallBack = callBack;
            m_UploadProgressEvent = proEvent;
            m_UploadDataByteCount = data.Length;
            m_Progreess = 0;
            m_WebClient = new WebClient();
            m_WebClient.Encoding = Encoding.UTF8;
            Uri uri = new Uri(m_FTPHost + fileName);

            m_WebClient.UploadDataCompleted += new UploadDataCompletedEventHandler(OnDataUploadCompleted);
            m_WebClient.UploadProgressChanged += new UploadProgressChangedEventHandler(OnDataUploadProgressChanged);
            m_WebClient.Credentials = new NetworkCredential(m_FTPUserName, m_FTPPasswork);

            m_WebClient.UploadDataAsync(uri, "STOR", data);
        }

        /// <summary>
        /// 取消上传
        /// </summary>
        public void CancelUpload()
        {
            m_WebClient.Dispose();
        }

        //进度变化调用
        private void OnDataUploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            m_Progreess = (float)e.BytesSent /(float)m_UploadDataByteCount;
            m_UploadProgressEvent?.Invoke(m_Progreess, e);
        }

        //上传完毕调用
        private void OnDataUploadCompleted(object sender, UploadDataCompletedEventArgs e)
        {
            m_UploadOver = true;
            if (e.Error != null)
            {
                m_UploadSucceed = false;
                m_FTPUploadCallBack?.Invoke(m_FTPHost, m_UploadSucceed, e.Error.ToString(), e);
            }
            else
            {
                m_UploadSucceed = true;
                m_FTPUploadCallBack?.Invoke(m_FTPHost, m_UploadSucceed, "Succeed", e);
            }
           
        }
    }
}
