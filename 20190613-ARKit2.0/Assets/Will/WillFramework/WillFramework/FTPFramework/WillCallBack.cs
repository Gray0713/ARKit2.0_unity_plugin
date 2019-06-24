#region Version Info
/*========================================================================
* 【本类功能概述】
* 
* 作者：wen      时间：2019/1/9 11:31:58
* 文件名：FTPCallBack
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
    /// <summary>
    /// ftp上传进度变化事件
    /// </summary>
    /// <param name="progreess">进度</param>
    /// <param name="e">上传数据进度变化的事件参数</param>
    public delegate void WillFtpUploadProgressChangedEvent(float progreess, UploadProgressChangedEventArgs e);

    /// <summary>
    /// ftp上传完成回调
    /// </summary>
    /// <param name="ftpHost">ftp地址</param>
    /// <param name="uploadSucceed">上传是否完成</param>
    /// <param name="callBackMessage">返回消息</param>
    /// <param name="e">上传数据完成的事件参数</param>
    public delegate void WillFtpUploadCallBack(string ftpHost, bool uploadSucceed, string callBackMessage, UploadDataCompletedEventArgs e);

    /// <summary>
    /// ftp下载进度变化事件
    /// </summary>
    /// <param name="progreess">进度</param>
    /// <param name="e">上传数据进度变化的事件参数</param>
    public delegate void WillFtpDownloadProgressChangerEvent(float progreess, DownloadProgressChangedEventArgs e);

    /// <summary>
    /// ftp下载完成回调
    /// </summary>
    /// <param name="ftpHost">ftp地址</param>
    /// <param name="uploadSucceed">上传是否完成</param>
    /// <param name="callBackMessage">返回消息</param>
    /// <param name="e">上传数据完成的事件参数</param>
    public delegate void WillFtpDownloadCallBack(string ftpHost, bool downloadSucceed, string callBackMessage, byte[] downloadData, DownloadDataCompletedEventArgs e);

    public delegate void WillDownloadPanelCallBack(bool over, string msg, object[] parameters);

    public delegate void WillAliyunUploadCallBack(bool over, string returnUrl);
    public delegate void WillAliyunUploadProcess(bool over, float process);

    /// <summary>
    /// 通过Http访问获取的回调
    /// </summary>
    /// <param name="httpUrl">访问的http地址</param>
    /// <param name="overOrFaliuer">成功或失败</param>
    /// <param name="callBackMsg">返回消息</param>
    /// <param name="callBackData">返回数据</param>
    /// <param name="parameters">携带参数</param>
    public delegate void HttpCallBack(string httpUrl, bool overOrFaliuer, string callBackMsg, byte[] callBackData, UnityEngine.Object obj, object[] parameters);


}