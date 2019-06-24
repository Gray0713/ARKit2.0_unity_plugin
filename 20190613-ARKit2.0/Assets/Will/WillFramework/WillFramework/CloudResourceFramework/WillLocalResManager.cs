using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;
using System.Net;
using LitJson;
using UnityEngine.SceneManagement;

namespace WillFramework
{
    public delegate void LoadCloudResCallBack(bool overOrFailed, string message, object[] parameters = null);
    public class WillLocalResManager : WillSingleton<WillLocalResManager>
    {
        private LoadCloudResCallBack m_CallBack = null;
        private MonoBehaviour m_CoroutinesMono = null;
        private Coroutine m_Coroutine = null;
        private object[] m_Parameters = null;
        private List<string> m_DownLoadFileName = new List<string>();
        private List<string> m_DownLoadFileNameItem = new List<string>();
        private List<byte[]> m_DownLoadFileList = new List<byte[]>();

        private string m_SavePath;
        private bool m_CanChack = true;
        private float m_DownloadProgress = 0;
        private string m_DownloadMessage = "";
        private int m_DownloadFileCount = 0;

        WorldMapData worldMapData = new WorldMapData();
        WorldMapData s_worldMapData = new WorldMapData();

        /// <summary>
        /// 下载进度
        /// </summary>
        public float DownLoadProgress { get { return m_DownloadProgress; } }
        /// <summary>
        /// 当前下载的资源名字
        /// </summary>
        public string NowDownLoadFileName{get {
                if (m_DownLoadFileNameItem.Count <= 0)
                    return "";
                return m_DownLoadFileNameItem[0]; } }
        /// <summary>
        /// 当前下载的消息
        /// </summary>
        public string NowDownLoadMessage { get { return m_DownloadMessage; } }
        /// <summary>
        /// 下载的总数量
        /// </summary>
        public int DownloadFileCount { get { return m_DownloadFileCount; } }
        /// <summary>
        /// 当前下载了多少
        /// </summary>
        public int DownloadFileNowCount { get { return m_DownloadFileCount - m_DownLoadFileNameItem.Count; } }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="mono"></param>
        public void InitCloudRes(MonoBehaviour mono, string serverFtp, string savePath)
        {
            ConstData.DownloadServerFtp = serverFtp;
            ConstData.DownloadSavePath = savePath;
            m_CoroutinesMono = mono;
            m_SavePath = savePath;
        }

        /// <summary>
        /// 检查资源更新
        /// </summary>
        /// <param name="mono"></param>
        /// <param name="callBack"></param>
        /// <param name="parameters"></param>
        public void UpdataRes(LoadCloudResCallBack callBack,object[] parameters)
        {
            //if (!ResourceManager.Instance.m_LoadFormAssetBundle)
            //{
            //    callBack(true, "Test", parameters);
            //    return;
            //}
            if (m_CanChack == true)
            {
                m_CanChack = false;
                m_CallBack = callBack;
                if (m_CoroutinesMono == null)
                {
                    m_CallBack(false, "", parameters);
                    m_CanChack = true;
                    WDebuger.LogError("未初始化管理器，请检查！");
                    return;
                }
                m_Parameters = parameters;
                m_DownLoadFileName.Clear();
                m_DownLoadFileNameItem.Clear();
                m_DownLoadFileList.Clear();
                string configPath = m_SavePath + ConstData.WLOCALCONFIG;
                if (!File.Exists(configPath))
                {
                    worldMapData.version = "1.0";
                    worldMapData.date = System.DateTime.Now.ToString();
                    if (!Directory.Exists(m_SavePath))
                        Directory.CreateDirectory(m_SavePath);
                    m_DownLoadFileName.Add("WorldMap.json");
                    m_DownLoadFileNameItem.Add("WorldMap.json");
                    worldMapData.SaveJson(configPath);
                }
                worldMapData = WillData.LoadJson<WorldMapData>(configPath);

                WillFTPDownloadManager.Instance.Init(ConstData.FTPname, ConstData.FTPPassword);
                WillFTPDownloadManager.Instance.DownLoadData(ConstData.DownloadServerFtp + "WorldMap.json", DowlLoadConfigCallBack);
                
            }
        }

        /// <summary>
        /// 下载配置文件回调
        /// 处理下载后文件对比
        /// </summary>
        /// <param name="ftpHost"></param>
        /// <param name="downloadSucceed"></param>
        /// <param name="callBackMessage"></param>
        /// <param name="downloadData"></param>
        /// <param name="e"></param>
        private void DowlLoadConfigCallBack(string ftpHost, bool downloadSucceed, string callBackMessage, byte[] downloadData, DownloadDataCompletedEventArgs e)
        {
            Debug.Log(Application.persistentDataPath);
            if (downloadSucceed == false)
            {
                WDebuger.LogError("下载文件失败，请检查网络！");
                m_CallBack(true, "下载文件失败，请检查网络！", m_Parameters);
                m_CanChack = true;
                return;
            }
            if (downloadData == null)
            {
                WDebuger.LogError("下载文件失败，下载内容为空，请检查网络！");
                m_CallBack(true, "下载文件失败，下载内容为空，请检查网络！", m_Parameters);
                m_CanChack = true;
                return;
            }

            Debug.Log("Data : " + Encoding.Default.GetString(downloadData));
            s_worldMapData = JsonMapper.ToObject<WorldMapData>(Encoding.Default.GetString(downloadData));

            ContrastConfig();
            Debug.Log("配置表下载成功！");
        }

        /// <summary>
        /// 对比配置文件
        /// </summary>
        private void ContrastConfig()
        {
            string configPath = m_SavePath + ConstData.WLOCALCONFIG;
            Debug.Log(configPath);
            //本地没有文件
            if (!File.Exists(configPath))
            {
                Debug.Log("No file!");
                worldMapData.version = "1.0";
                worldMapData.date = System.DateTime.Now.ToString();
                if (!Directory.Exists(m_SavePath))
                    Directory.CreateDirectory(m_SavePath);
                //执行下载文件
                worldMapData.SaveJson(configPath);

            }
            Debug.Log("worldMapData" + worldMapData.version);
            m_DownLoadFileName.Add("WorldMap.json");
            m_DownLoadFileNameItem.Add("WorldMap.json");

            Debug.Log("s_worldMapData" + s_worldMapData.version);
            //校对配置文件
            if (s_worldMapData.version != worldMapData.version)
            {
                m_DownLoadFileName.Clear();
                m_DownLoadFileNameItem.Clear();

                worldMapData.version = s_worldMapData.version;
                m_DownLoadFileName.Add("WorldMap.json");
                m_DownLoadFileNameItem.Add("WorldMap.json");
                m_DownLoadFileName.Add("myFirstWorldMap.worldmap");
                m_DownLoadFileNameItem.Add("myFirstWorldMap.worldmap");
            }

            DownLoadFile();
        }

        /// <summary>
        /// 查找更新的文件
        /// </summary>
        private void LocatingUpdata()
        {
            //List<string> addABName = new List<string>();
            //for (int i = 0; i < m_ServerConfig.m_AllFileDirABVersioons.Count; i++)
            //{
            //    addABName.Add(m_ServerConfig.m_AllFileDirABVersioons[i].ABName);
            //}
            //for (int i = 0; i < m_LocalConfig.m_AllFileDirABVersioons.Count; i++)
            //{
            //    for (int j = 0; j < m_ServerConfig.m_AllFileDirABVersioons.Count; j++)
            //    {

            //        if (m_LocalConfig.m_AllFileDirABVersioons[i].ABName == m_ServerConfig.m_AllFileDirABVersioons[j].ABName)
            //        {
            //            addABName.Remove(m_ServerConfig.m_AllFileDirABVersioons[j].ABName);
            //            if (m_LocalConfig.m_AllFileDirABVersioons[i].ABCrc != m_ServerConfig.m_AllFileDirABVersioons[j].ABCrc)
            //            {
            //                m_DownLoadFileName.Add(m_ServerConfig.m_AllFileDirABVersioons[j].ABName);
            //                m_DownLoadFileName.Add(m_ServerConfig.m_AllFileDirABVersioons[j].ABName.Split('.')[0]);
            //                m_DownLoadFileNameItem.Add(m_ServerConfig.m_AllFileDirABVersioons[j].ABName);
            //                m_DownLoadFileNameItem.Add(m_ServerConfig.m_AllFileDirABVersioons[j].ABName.Split('.')[0]);
            //                break;
            //            }
            //        }
            //    }
            //}
            //for (int i = 0; i < addABName.Count; i++)
            //{
            //    m_DownLoadFileName.Add(addABName[i]);
            //    m_DownLoadFileName.Add(addABName[i].Split('.')[0]);
            //    m_DownLoadFileNameItem.Add(addABName[i]);
            //    m_DownLoadFileNameItem.Add(addABName[i].Split('.')[0]);
            //}
        }

        /// <summary>
        /// 单个下载回调
        /// 会继续执行后面的下载
        /// 加载完成会调用回调
        /// </summary>
        /// <param name="ftpHost"></param>
        /// <param name="downloadSucceed"></param>
        /// <param name="callBackMessage"></param>
        /// <param name="downloadData"></param>
        /// <param name="e"></param>
        private void DownloadFileCallBack(string ftpHost, bool downloadSucceed, string callBackMessage, byte[] downloadData, DownloadDataCompletedEventArgs e)
        {
            if (downloadSucceed == false)
            {
                WDebuger.LogError("下载失败，请检查网络！");
                m_CallBack(false, "下载失败，请检查网络！", m_Parameters);
                m_CanChack = true;
                return;
            }
            WDebuger.Log("文件下载成功!:["+ftpHost+"]");
            m_Coroutine = m_CoroutinesMono.StartCoroutine(SaveFile(downloadData));
        }

        /// <summary>
        /// 下载进度回调
        /// </summary>
        /// <param name="progreess"></param>
        /// <param name="e"></param>
        private void DownLoadProgressChangerEvent(float progreess, DownloadProgressChangedEventArgs e)
        {
            m_DownloadProgress = progreess;
            m_DownloadMessage =  "进度：" + (m_DownLoadFileName.Count - m_DownLoadFileNameItem.Count) + "/" + m_DownLoadFileName.Count;
        }

        /// <summary>
        /// 下载资源
        /// </summary>
        private void DownLoadFile()
        {
            if (m_DownLoadFileNameItem.Count > 0)
                WillFTPDownloadManager.Instance.DownLoadData(ConstData.DownloadServerFtp + m_DownLoadFileNameItem[0], DownloadFileCallBack, DownLoadProgressChangerEvent);
            else
                UpdateConfig();
        }

        private IEnumerator SaveFile(byte[] file)
        {
            WDebuger.Log(m_DownLoadFileNameItem[0]);
            m_DownloadMessage = "保存文件中";
            if (!Directory.Exists(m_SavePath))
                Directory.CreateDirectory(m_SavePath);
            FileInfo fileInfo = new FileInfo(m_SavePath + m_DownLoadFileNameItem[0]);
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }

            Stream sw = fileInfo.Create();
            yield return sw.WriteAsync(file, 0, file.Length);
            yield return null;
            sw.Flush();
            sw.Close();
            sw.Dispose();
            m_DownLoadFileNameItem.RemoveAt(0);

            if (m_DownLoadFileNameItem.Count <= 0)//所有资源加载完成
            {
                UpdateConfig();
            }
            else
            {
                m_DownloadProgress = (m_DownLoadFileName.Count - m_DownLoadFileNameItem.Count) / m_DownLoadFileName.Count;
                DownLoadFile();
            }
        }

        /// <summary>
        /// 更新完毕
        /// </summary>
        private void UpdateConfig()
        {
            //m_LocalConfig.GameVersions = m_ServerConfig.GameVersions;
             #if UNITY_IOS
            //m_LocalConfig.SaveConfig(m_SavePath + ConstData.LOCALCONFIG);
            #else 
            //保存配置表
            //SaveConfig();
            #endif
            m_DownloadMessage = "资源下载完成！";
            m_CallBack(true, "资源下载完成！", m_Parameters);
            m_CanChack = true;
            SceneManager.LoadScene("01Start");
        }
    }
}
