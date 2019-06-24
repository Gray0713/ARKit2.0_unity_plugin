using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace WillFramework
{
    /// <summary>
    /// 通过http get方法获取服务器资源
    /// </summary>
    public class WillHttpGetMessage : WillSingleton<WillHttpGetMessage>
    {
        private HttpGetType m_HttpGetType = HttpGetType.normal;
        private AudioType m_AudioType;
        private bool m_IsCanNext = true;

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

        /// <summary>
        /// Get方法访问服务器
        /// </summary>
        /// <param name="httpUrl">服务器地址</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="Coerce">关闭之前的协程,强制执行新请求</param>
        /// <param name="parameters">回调携带的参数</param>
        public void Get(string httpUrl, HttpCallBack callBack = null, bool Coerce = false, object[] parameters = null)
        {
            Get(httpUrl, callBack, Coerce, HttpGetType.normal, parameters);
        }

        /// <summary>
        /// Get方法访问服务器
        /// </summary>
        /// <param name="httpUrl">服务器地址</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="Coerce">关闭之前的协程,强制执行新请求</param>
        /// <param name="HttpGetType">请求类型</param>
        /// <param name="parameters">回调携带的参数</param>
        public void Get(string httpUrl, HttpCallBack callBack = null, bool Coerce = false,HttpGetType type = HttpGetType.normal ,object[] parameters = null)
        {
//#if UNITY_EDITOR
//            if (!InitBaseData(httpUrl, callBack, parameters)) return;
//            m_HttpGetType = type;
//            m_IsCanNext = true;
//            EditorCoroutineLooper.StartLoop(m_CoroutinesMono, HttpCoroutines());
//            return;
//#endif
            if (Coerce == true)
            {
                m_CoroutinesMono.StopCoroutine(m_Coroutine);
                m_Coroutine = null;
            }
            if (m_Coroutine == null)
            {
                if (!InitBaseData(httpUrl, callBack, parameters)) return;
                m_HttpGetType = type;
                m_IsCanNext = true;
                m_Coroutine = m_CoroutinesMono.StartCoroutine(HttpCoroutines());
            }
            else
            {
                WDebuger.Log("上一个请求尚未完成，请稍等");
            }
        }

        /// <summary>
        /// Get方法访问服务器，获取图片资源
        /// </summary>
        /// <param name="httpUrl">服务器地址</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="Coerce">关闭之前的协程,强制执行新请求</param>
        /// <param name="parameters">回调携带的参数</param>
        public void GetTexture(string httpUrl, HttpCallBack callBack = null, bool Coerce = false, object[] parameters = null)
        {
            Get(httpUrl, callBack, Coerce, HttpGetType.texture, parameters);
        }

        /// <summary>
        /// Get方法访问服务器，获取音频
        /// </summary>
        /// <param name="httpUrl">服务器地址</param>
        /// <param name="audioType">音频类型</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="Coerce">关闭之前的协程,强制执行新请求</param>
        /// <param name="parameters">回调携带的参数</param>
        public void GetAudioClip(string httpUrl, AudioType audioType, HttpCallBack callBack = null, bool Coerce = false, object[] parameters = null)
        {
            m_AudioType = audioType;
            Get(httpUrl, callBack, Coerce, HttpGetType.audioClip, parameters);
        }

        /// <summary>
        /// Get方法访问服务器，获取AB包
        /// </summary>
        /// <param name="httpUrl">服务器地址</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="Coerce">关闭之前的协程,强制执行新请求</param>
        /// <param name="parameters">回调携带的参数</param>
        public void GetAssetBundle(string httpUrl, HttpCallBack callBack = null, bool Coerce = false, object[] parameters = null)
        {
            Get(httpUrl, callBack, Coerce, HttpGetType.assetBundle, parameters);
        }

        protected IEnumerator HttpCoroutines()
        {
            float waitTime = 0;
            int waitCount = 0;
            bool isContinue = true;
            while (true)
            {
                CheckUrl();
                if (m_IsCanNext == true)
                {
                    while (!m_WebRequest.isDone)
                    {
                        Progress = m_WebRequest.downloadProgress;
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
                    if (isContinue == true && m_WebRequest.isDone == true)
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

        //检查Url是否能使用
        protected void CheckUrl()
        {
            try
            {
                switch (m_HttpGetType)
                {
                    case HttpGetType.normal:
                        m_WebRequest = UnityWebRequest.Get(m_HttpUrl);
                        break;
                    case HttpGetType.texture:
                        m_WebRequest = UnityWebRequestTexture.GetTexture(m_HttpUrl);
                        break;
                    case HttpGetType.audioClip:
                        m_WebRequest = UnityWebRequestMultimedia.GetAudioClip(m_HttpUrl, m_AudioType);
                        break;
                    case HttpGetType.assetBundle:
                        //m_WebRequest = UnityWebRequestAssetBundle.GetAssetBundle(m_HttpUrl);
                        m_WebRequest = UnityWebRequestAssetBundle.GetAssetBundle(m_HttpUrl);
                        break;
                    default:
                        break;
                }
                m_WebRequest.SendWebRequest();
            }
            catch
            {
                ErrorMessage("Error: 无法访问地址，请检查！ 参数：" + m_HttpUrl);
                m_IsCanNext = false;
            }
        }

        protected void SetMessage()
        {
            Progress = 1;
            HttpMessage = m_WebRequest.downloadHandler.text;
            HttpData = m_WebRequest.downloadHandler.data;
            m_Coroutine = null;
            if (m_CallBack != null)
            {
                switch (m_HttpGetType)
                {
                    case HttpGetType.normal:
                        m_CallBack(m_HttpUrl, true, HttpMessage, HttpData, null, m_Parameters);
                        break;
                    case HttpGetType.texture:
                        m_CallBack(m_HttpUrl, true, HttpMessage, HttpData, ((DownloadHandlerTexture)m_WebRequest.downloadHandler).texture, m_Parameters);
                        break;
                    case HttpGetType.audioClip:
                        m_CallBack(m_HttpUrl, true, HttpMessage, HttpData, ((DownloadHandlerAudioClip)m_WebRequest.downloadHandler).audioClip, m_Parameters);
                        break;
                    case HttpGetType.assetBundle:
                        m_CallBack(m_HttpUrl, true, HttpMessage, HttpData, DownloadHandlerAssetBundle.GetContent(m_WebRequest), m_Parameters);
                        break;
                    default:
                        break;
                }
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
            m_Coroutine = null;
            Progress = 1;
            HttpMessage = errorMsg;
            HttpData = null;
            if (m_CallBack != null)
                m_CallBack(m_HttpUrl, false, errorMsg, HttpData, null, m_Parameters);
            WDebuger.LogError(HttpMessage);
        }

        public enum HttpGetType
        {
            normal,
            texture,
            audioClip,
            assetBundle,
        }
    }
}
