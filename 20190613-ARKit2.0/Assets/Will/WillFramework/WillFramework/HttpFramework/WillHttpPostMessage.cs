#region Version Info
/*========================================================================
* 【本类功能概述】
* 
* 作者：wen      时间：2019/1/2 9:40:00
* 文件名：HttpPostMessage
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
using System.IO;
using UnityEngine.Networking;

namespace WillFramework
{
    /// <summary>
    /// 通过http post方法上传资源到服务器
    /// </summary>
    public class WillHttpPostMessage :WillSingleton<WillHttpPostMessage>
    {
        //延时时间
        protected float m_WaitTime = 1f;
        //延时次数
        protected int m_WaitCount = 4;
        //回调函数
        protected HttpCallBack m_CallBack = null;
        //异步协程
        protected Coroutine m_Coroutine = null;
        //访问地址
        protected string m_HttpUrl = null;
        //回调携带的参数
        protected object[] m_Parameters = null;

        private string m_DataName = null;

        protected UnityWebRequest m_WebRequest = new UnityWebRequest();


        /// <summary>
        /// 执行异步加载使用的挂载组件
        /// </summary>
        protected MonoBehaviour m_CoroutinesMono = null;

        /// <summary>
        /// 加载进度
        /// </summary>
        public float Progress { get; protected set; }
        /// <summary>
        /// 当前访问到的消息
        /// </summary>
        public string HttpMessage { get; protected set; }
        /// <summary>
        /// 当前访问到的数据
        /// </summary>
        public byte[] HttpData { get; protected set; }

        /// <summary>
        /// 初始化http服务
        /// </summary>
        /// <param name="mono">执行异步加载使用的挂载在场景的组件</param>
        /// <param name="waitTime">延时时间</param>
        /// <param name="waitCount">延时次数</param>
        public void Init(MonoBehaviour mono, float waitTime = 1f, int waitCount = 4)
        {
            m_CoroutinesMono = mono;
            m_WaitTime = waitTime;
            m_WaitCount = waitCount;
        }


        private bool m_IsCanNext = true;
        private byte[] m_PostData = null;

        /// <summary>
        /// Post方法将资源上传到服务器，资源类型文件
        /// </summary>
        /// <param name="httpUrl">服务器地址</param>
        /// <param name="filePath">资源文件路径</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="Corece">关闭之前的协程,强制执行新请求</param>
        /// <param name="parameters">回调携带的参数</param>
        public void Post(string httpUrl, string filePath, string fileName = "data",HttpCallBack callBack = null,bool Corece = false,object[] parameters = null)
        {
            if (!File.Exists(filePath))
            {
                ErrorMessage("Error: 不存在该文件，请检查！ 参数：" + filePath);
                return;
            }
            Stream st = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            byte[] itemByte = new byte[st.Length];
            st.Read(itemByte, 0, itemByte.Length);
            st.Close();
            Post(httpUrl, itemByte, fileName, callBack, Corece, parameters);
        }

        /// <summary>
        /// Post方法将资源上传到服务器，资源类型字节数组
        /// </summary>
        /// <param name="httpUrl">服务器地址</param>
        /// <param name="postData">资源字节数组</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="Corece">关闭之前的协程,强制执行新请求</param>
        /// <param name="parameters">回调携带的参数</param>
        public void Post(string httpUrl, byte[] postData, string fileName = "data", HttpCallBack callBack = null, bool Corece = false, object[] parameters = null)
        {
//#if UNITY_EDITOR
//            if (!InitBaseData(httpUrl, callBack, parameters)) return;
//            m_IsCanNext = true;
//            m_PostData = postData;
//            m_DataName = fileName;
//            EditorCoroutineLooper.StartLoop(m_CoroutinesMono, HttpCoroutines());
//            return;
//#endif
            if (Corece == true)
            {
                m_CoroutinesMono.StopCoroutine(m_Coroutine);
                m_Coroutine = null;
            }
            if (m_Coroutine == null)
            {
                if (!InitBaseData(httpUrl, callBack, parameters)) return;
                m_PostData = postData;
                m_DataName = fileName;
                m_Coroutine = m_CoroutinesMono.StartCoroutine(HttpCoroutines());
            }
            else
            {
                WDebuger.Log("上一个请求尚未完成，请稍等");
            }
        }

        protected IEnumerator HttpCoroutines()
        {
            float waitTime = 0;
            int waitCount = 0;
            bool isContinue = true;
            while (true)
            {
                CheckUrl();
                if (m_IsCanNext)
                {
                    while (!m_WebRequest.isDone)
                    {
                        Progress = m_WebRequest.uploadProgress;
                        yield return new WaitForEndOfFrame();
                        waitTime += Time.deltaTime;
                        if (waitTime >= m_WaitTime)
                        {
                            waitCount++;
                            if (waitCount >= m_WaitCount)
                            {
                                ErrorMessage("Error: 网络连接失败，请检查！ 参数：" + m_HttpUrl);
                                isContinue = false;
                            }
                            waitTime = 0;
                            break;
                        }
                    }
                    if (isContinue == true && m_WebRequest.isDone == true && m_WebRequest.error == null)
                    {
                        SetMessage();
                        break;
                    }
                    else if (m_WebRequest.error != null)
                    {
                        ErrorMessage(m_WebRequest.error);
                        break;
                    }
                    else if (isContinue == false)
                    {
                        Progress = 1;
                        break;
                    }
                }
            }
        }

        protected void CheckUrl()
        {
            WWWForm form = new WWWForm();
            form.AddBinaryData(m_DataName, m_PostData);
            try
            {
                m_WebRequest = UnityWebRequest.Post(m_HttpUrl, form);
                m_WebRequest.SendWebRequest();
            }
            catch
            {
                ErrorMessage("Error: 无法访问地址，请检查！ 参数：" + m_HttpUrl);
                m_IsCanNext = false;
            }
        }

        /// <summary>
        /// 下载/上传完成，处理回调信息
        /// </summary>
        protected void SetMessage()
        {
            Progress = 1;
            HttpMessage = m_WebRequest.downloadHandler.text;
            HttpData = m_WebRequest.downloadHandler.data;
            if (m_CallBack != null)
            {
                m_CallBack(m_HttpUrl, true, HttpMessage, HttpData, null, m_Parameters);
            }
        }
        /// <summary>
        /// 通用处理初始数据
        /// </summary>
        /// <param name="httpUrl">服务器地址</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="parameters">回调携带的参数</param>
        protected virtual bool InitBaseData(string httpUrl, HttpCallBack callBack, object[] parameters)
        {
            if (string.IsNullOrEmpty(httpUrl))
            {
                ErrorMessage("Error: 服务器地址为空，请检查！ 参数：" + httpUrl);
                return false;
            }
            if (m_CoroutinesMono == null)
            {
                ErrorMessage("Error: 未初始化方法，请检查！ 参数：" + httpUrl);
                return false;
            }
            Progress = 0;
            HttpMessage = null;
            HttpData = null;
            m_CallBack = callBack;
            m_HttpUrl = httpUrl;
            m_Parameters = parameters;
            return true;
        }

        /// <summary>
        /// 错误消息处理
        /// </summary>
        /// <param name="errorMsg">错误消息</param>
        protected virtual void ErrorMessage(string errorMsg)
        {
            Progress = 1;
            HttpMessage = errorMsg;
            HttpData = null;
            if (m_CallBack != null)
                m_CallBack(m_HttpUrl, false, errorMsg, HttpData, null, m_Parameters);
            WDebuger.LogError(HttpMessage);
        }
    }
}