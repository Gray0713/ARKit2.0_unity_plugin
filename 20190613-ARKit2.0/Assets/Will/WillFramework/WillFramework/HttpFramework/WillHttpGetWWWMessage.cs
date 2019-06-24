#region Version Info
/*========================================================================
* 【本类功能概述】
* 
* 作者：wen      时间：#CreateTime#
* 文件名：WFHttpGetWWWMessage
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

namespace WillFramework
{

    public class WillHttpGetWWWMessage : WillSingleton<WillHttpGetWWWMessage>
    {

        private MonoBehaviour m_CoroutinesMono;
        private WWW m_WWW;
        private string m_HttpUrl;
        private HttpCallBack m_CallBack;
        private bool m_IsCanNext;
        public float Progress = 0;
        private float m_WaitTime = 10;
        private float m_WaitCount = 2;

        public string HttpMessage;
        public byte[] HttpData;

        public void Init(MonoBehaviour mono)
        {
            m_CoroutinesMono = mono;
            m_IsCanNext = true;
        }

        public void Get(string httpUrl, HttpCallBack callBack = null, bool Coerce = false, object[] parameters = null)
        {
            m_HttpUrl = httpUrl;
            m_CallBack = callBack;
            Progress = 0;
            m_CoroutinesMono.StartCoroutine(GetToSave());
        }

        private IEnumerator GetToSave()
        {
            float waitTime = 0;
            int waitCount = 0;
            bool isContinue = true;
            while (true)
            {
                try
                {
                    m_WWW = new WWW(m_HttpUrl);
                }
                catch (Exception e)
                {
                    WDebuger.LogError(e.Message);
                }
                if (m_IsCanNext == true)
                {
                    while (m_WWW.isDone ==false)
                    {
                        Progress = m_WWW.progress;
                        yield return new WaitForEndOfFrame();
                        waitTime += Time.deltaTime;
                        if (m_WaitTime <= waitTime)
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
                    if (isContinue == true && m_WWW.isDone == true)
                    {
                        SetMessage();
                        break;
                    }
                    else if (m_WWW.error != null)
                    {
                        ErrorMessage(m_WWW.error);
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

        protected void SetMessage()
        {
            Progress = 1;
            HttpMessage = m_WWW.text;
            HttpData = m_WWW.bytes;
            m_CallBack(m_HttpUrl, true, HttpMessage, HttpData, null, null);
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
                m_CallBack(m_HttpUrl, false, errorMsg, HttpData, null, null);
            WDebuger.LogError(HttpMessage);
        }
    }
}