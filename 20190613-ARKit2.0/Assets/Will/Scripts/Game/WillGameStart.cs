#region Version Info
/*========================================================================
* 【本类功能概述】
* 
* 作者：wen      时间：#CreateTime#
* 文件名：GameStart
* 版本：V1.0.1
*
* 修改者：          时间：              
* 修改说明：
* ========================================================================
*/
#endregion

using UnityEngine;

namespace WillFramework
{
    /// <summary>
    /// 框架初始启动程序
    /// 目前框架功能有：AB包打包更新BundleEditor，资源加载ResourceManager，小型UI框架UIManager，Log工具WDebuger
    /// </summary>
    public class WillGameStart : WillMonoSigleton<WillGameStart>
    {
        private GameObject m_Obj;

        protected override void Awake()
        {
            base.Awake();
            Debug.Log(Application.persistentDataPath);
            //DontDestroyOnLoad(gameObject);
            GameObject.Find("DownloadPanel").GetComponent<DownloadPanel>().StartDownload(DownloadCallBack, ConstData.DOWNLOADSERVERURL);
        }

        private void Start()
        {
            
        }
        
        private void DownloadCallBack(bool downloadOver, string downloadMsg, object[] parameters)
        {
            if (downloadOver == true)
            {
                InitGameStart();
            }
            else
            {
                WDebuger.LogError("资源下载失败！");
            }
        }

        /// <summary>
        /// 游戏开始初始化
        /// </summary>
        private void InitGameStart()
        {
            //AssetBundleManager.Instance.LoadAssetBundleConfig();
            //ResourceManager.Instance.Init(this);
            WillHttpGetMessage.Instance.Init(this, 5, 2);

            //对象池检查
            //Transform RecyclePoolTrs = transform.Find("RecyclePoolTrs");
            //Transform SceneTrs = transform.Find("SceneTrs");
            //if (RecyclePoolTrs == false)
            //{
            //    RecyclePoolTrs = new GameObject("RecyclePoolTrs").transform;
            //    RecyclePoolTrs.gameObject.SetActive(false);
            //}
            //if (SceneTrs == false) {
            //    SceneTrs = new GameObject("SceneTrs").transform; ;
            //}
            //RecyclePoolTrs.transform.parent = transform;
            //SceneTrs.transform.parent = transform;
            //ObjectManager.Instance.Init(RecyclePoolTrs.transform, SceneTrs.transform);

            //Log工具检查
            //WFLogMessageGUI WDebuger = gameObject.GetComponent<WFLogMessageGUI>();
            //if (WDebuger == false)
            //    gameObject.AddComponent<WFLogMessageGUI>();
            //WFLogMessageGUI.instance.InitWFLogMessage(10);
            //WDebuger.InitWFLog();

            //ui工具检查
            //Transform UIRoot = transform.Find("UIRoot");
            //if (UIRoot == null)
            //    UIRoot = ObjectManager.Instance.InstantiateObject("Assets/GameData/Prefabs/UGUI/UIRoot.prefab").transform;
            //UIRoot.parent = transform;
            //UIManager.Instance.Init(UIRoot as RectTransform, UIRoot.Find("WindowRoot") as RectTransform,
            //        UIRoot.Find("UICamera").GetComponent<Camera>(), UIRoot.Find("EventSystem").GetComponent<EventSystem>());
            

            //UI注册
            //ConstData.RegisterUI();

            //lua工具
            //LuaHotFix lua = gameObject.GetComponent<LuaHotFix>();
            //if (lua == null)
            //    gameObject.AddComponent<LuaHotFix>();
            //LuaHotFix.instance.InitLuaHotFix();

            //场景管理工具
            //GameMapManager.Instance.Init(this);
            //GameMapManager.Instance.LoadScene("01Start");



            //SetAction();
        }

       

        #region 框架参考

        //显示或隐藏log
        bool isShowLog = false;
        private void ShowLog()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                isShowLog = !isShowLog;
            }
        }



        #endregion

        private void Update()
        {
            //UIManager.Instance.OnUpdate();
            ShowLog();
        }
    }
}