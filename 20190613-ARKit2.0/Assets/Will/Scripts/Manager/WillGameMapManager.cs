#region Version Info
/*========================================================================
* 【本类功能概述】
* 
* 作者：wen      时间：#CreateTime#
* 文件名：GameMapManager
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
using UnityEngine.SceneManagement;
namespace WillFramework
{
    public class WillGameMapManager : WillSingleton<WillGameMapManager>
    {
        //加载场景完成回调
        public Action LoadSceneOverCallBack;

        //加载场景开始回调
        public Action LoadSceneEnterCallBack;

        // 当前场景名
        public string CurrentMapName { get; set; }

        //加载场景是否完成
        public bool AlreadyLoadScene { get; set; }

        //切换场景进度条
        public static int LoadingProgress = 0;

        private MonoBehaviour m_Mono;

        /// <summary>
        /// 场景管理初始化
        /// </summary>
        /// <param name="mono"></param>
        public void Init(MonoBehaviour mono)
        {
            m_Mono = mono;
        }

        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="name">场景名</param>
        public void LoadScene(string name)
        {
            LoadingProgress = 0;
            m_Mono.StartCoroutine(LoadSceneAsync(name));
        }

        /// <summary>
        /// 设置场景环境
        /// </summary>
        /// <param name="name">场景名</param>
        private void SetSceneSetting(string name)
        {
            //设置各种场景环境，可以根据配置表 TODO
        }
        /// <summary>
        /// 异步加载场景名称
        /// </summary>
        /// <param name="name">levelname</param>
        /// <returns></returns>
        IEnumerator LoadSceneAsync(string name)
        {
            if (LoadSceneEnterCallBack != null)
            {
                LoadSceneEnterCallBack();
            }
            ClearCache();
            AlreadyLoadScene = false;
            AsyncOperation unLoadScene = SceneManager.LoadSceneAsync(ConstData.EMPTYSCENE, LoadSceneMode.Single);
            while (unLoadScene != null && !unLoadScene.isDone)
            {
                yield return new WaitForEndOfFrame();
            }

            LoadingProgress = 0;
            int targetProgress = 0;
            AsyncOperation asyncScene = SceneManager.LoadSceneAsync(name);
            if (asyncScene != null && !asyncScene.isDone)
            {
                asyncScene.allowSceneActivation = false;
                while (asyncScene.progress < 0.9f)
                {
                    targetProgress = (int)asyncScene.progress * 100;
                    yield return new WaitForEndOfFrame();
                    while (LoadingProgress < targetProgress)
                    {
                        ++LoadingProgress;
                        yield return new WaitForEndOfFrame();
                    }
                }

                CurrentMapName = name;
                SetSceneSetting(name);
                //自行加载到剩余的10%
                targetProgress = 100;
                while (LoadingProgress < targetProgress)
                {
                    ++LoadingProgress;
                    yield return new WaitForEndOfFrame();
                }
                
               
                asyncScene.allowSceneActivation = true;
                AlreadyLoadScene = true;
                // 等待场景运行
                while(!WSceneManager.sceneManagers.ContainsKey(name)){
                    yield return new WaitForEndOfFrame();
                }
                // 加载场景
                WSceneManager.sceneManagers[name].OnStartScene();
                if (LoadSceneOverCallBack != null)
                {
                    LoadSceneOverCallBack();
                }
            }
        }

        /// <summary>
        /// 跳场景需要清除的东西
        /// </summary>
        private void ClearCache()
        {

        }
    }
}