#region Version Info
/*========================================================================
 * 【本类功能概述】
 *  场景管理的基础类，他进行进行场景的基础预制体的加载管理
 *
 * 作者：李程      时间：2019.2.18
 * 文件名：WSceneManager
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
public class WSceneManager : MonoBehaviour {

	/// <summary>
	/// 场景名称
	/// </summary>
	/// <value></value>
	public String SceneName {get;set;}

	/// <summary>
	/// 场景加载
	/// </summary>
	public static Dictionary<String,WSceneManager> sceneManagers=new Dictionary<string, WSceneManager>(); 

	private void Awake() {
		
		SetScenename();
		sceneManagers[SceneName] = this;
	}
	public virtual void SetScenename(){
		SceneName = "";
        WDebuger.LogError("SceneName need Set!");
	}
	
	/// <summary>
	/// 预制体的加载
	/// </summary>
	public virtual void OnStartScene () {
		//这里编写需要加载的场景
		WDebuger.LogError("scene manger please override OnStartScene method!");
	}

	private void OnDestroy() {
		sceneManagers.Remove(SceneName);
	}
}