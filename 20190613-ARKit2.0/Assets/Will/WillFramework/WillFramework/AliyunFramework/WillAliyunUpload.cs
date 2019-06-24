#region Version Info
/*========================================================================
* 【本类功能概述】
* 
* 作者：wen      时间：#CreateTime#
* 文件名：WFAliyunUpload
* 版本：V1.0.1
*
* 修改者：          时间：              
* 修改说明：
* ========================================================================
*/
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using System.IO;
using System.Threading;
using System.Text;
using Aliyun.OSS.Util;
namespace WillFramework
{

/// <summary>
/// 上传资源到服务器
/// 回调返回地址和信息
/// </summary>
public class WillAliyunUpload : WillSingleton<WillAliyunUpload>
{
    private string m_Endpoint;
    private string m_AccessKeyId;
    private string m_AccessKeySecret;
    private string m_BucketName;
    private string m_ObjectName;
    private string m_LocalFilename;
    private byte[] m_LocalData;
    private WillAliyunUploadCallBack m_CallBack;
    private WillAliyunUploadProcess m_UploadProcess;
    private float m_Process;
    // 设置回调请求的服务器地址。
    const string callbackUrl = "http://oss-demo.aliyuncs.com:23450";
    // 设置发起回调时请求body的值。
    const string callbackBody = "bucket=${bucket}&object=${object}&etag=${etag}&size=${size}&mimeType=${mimeType}&" +
                                        "my_var1=${x:var1}&my_var2=${x:var2}";

    private Thread m_UploadThread;
    private OssClient m_Client;
    public string UploadReturnUrl { get; private set; }
    public string UploadReturnMsg { get; private set; }

    public bool UploadOver { get; private set; }

    /// <summary>
    /// 上传进度
    /// </summary>
    public float Process { get { return m_Process; } }

    /// <summary>
    /// 初始化Aliyun
    /// </summary>
    /// <param name="endpoint"></param>
    /// <param name="accessKetId"></param>
    /// <param name="accessKeySecret"></param>
    public void InitAliyun(string endpoint, string accessKetId, string accessKeySecret)
    {
        m_Endpoint = endpoint;
        m_AccessKeyId = accessKetId;
        m_AccessKeySecret = accessKeySecret;
        m_Client = new OssClient(m_Endpoint, m_AccessKeyId, m_AccessKeySecret);
        UploadOver = false;
    }

    /// <summary>
    /// 上传文件到Aliyun
    /// </summary>
    /// <param name="filePath">本地文件所在路径</param>
    /// <param name="bucketName">Aliyun空间名</param>
    /// <param name="objectName">存储到Aliyun路径</param>
    /// <param name="callBack">回调</param>
    public void UploadData(string filePath, string bucketName, string objectName, WillAliyunUploadCallBack callBack, WillAliyunUploadProcess process = null)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("无法找到需要上传的文件，请检查！ 参数：" + filePath);
            return;
        }
        byte[] data = File.ReadAllBytes(filePath);
        UploadData(data, bucketName, objectName, callBack);
    }
    /// <summary>
    /// 上传数据到Aliyun
    /// </summary>
    /// <param name="data">数据</param>
    /// <param name="bucketName">Aliyun空间名</param>
    /// <param name="objectName">存储到Aliyun路径</param>
    /// <param name="callBack">回调</param>
    public void UploadData(byte[] data, string bucketName, string objectName, WillAliyunUploadCallBack callBack, WillAliyunUploadProcess process = null)
    {
        if (string.IsNullOrEmpty(m_Endpoint))
        {
            Debug.LogError("Aliyun未初始化，请检查！");
            return;
        }
        m_BucketName = bucketName;
        m_ObjectName = objectName;
        m_CallBack = callBack;
        m_UploadProcess = process;
        m_LocalData = data;
        m_Process = 0;
        UploadOver = false;
        m_UploadThread = new Thread(UploadThread);
        m_UploadThread.Start();
    }

    /// <summary>
    /// 上传线程
    /// </summary>
    private void UploadThread()
    {
        try
        {
            //ObjectMetadata metadata = BuildCallbackMetadata("oss-cn-shenzhen.aliyuncs.com", callbackBody);
            //Stream stream = new MemoryStream(m_LocalData);
            MemoryStream requestContent = new MemoryStream(m_LocalData);
            //PutObjectRequest putObjectRequest = new PutObjectRequest(m_BucketName, m_ObjectName, stream);
            //putObjectRequest.StreamTransferProgress += UploadProcess;
            /*PutObjectResult request = */
            m_Client.PutObject(m_BucketName, m_ObjectName, requestContent);
            UploadCallBack();
           // stream.Close();
        }
        catch (OssException e)
        {
            Debug.LogError("上传错误：" + e);
        }
        catch (Exception e)
        {
            Debug.LogError("上传错误：" + e);
        }
        finally
        {
            m_UploadThread.Join();
        }
    }

    /// <summary>
    /// 上传进度回调
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void UploadProcess()
    {

    }

    /// <summary>
    /// 完成回调处理
    /// </summary>
    /// <param name="callbackUrl"></param>
    /// <param name="callbackBody"></param>
    /// <returns></returns>
    private ObjectMetadata BuildCallbackMetadata(string callbackUrl, string callbackBody)
    {
        string callbackHeaderBuilder = new CallbackHeaderBuilder(callbackUrl, callbackBody).Build();
        string CallbackVariableHeaderBuilder = new CallbackVariableHeaderBuilder().
            AddCallbackVariable("x:var1", "x:value1").AddCallbackVariable("x:var2", "x:value2").Build();
        var metadata = new ObjectMetadata();
        metadata.AddHeader(HttpHeaders.Callback, callbackHeaderBuilder);
        metadata.AddHeader(HttpHeaders.CallbackVar, CallbackVariableHeaderBuilder);
        return metadata;
    }

    /// <summary>
    /// 上传完成回调
    /// </summary>
    /// <param name="putObjectResult"></param>
    private void UploadCallBack()
    {
        m_CallBack(true, "");
    }
}
}