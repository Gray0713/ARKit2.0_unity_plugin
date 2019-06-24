#region Version Info
/*========================================================================
* 【本类功能概述】
* 游戏主体流程
* 作者：#AUTHORNAME#      时间：#CREATETIME#
* 文件名：MainControl
* 版本：V1.0.1
*
* 修改者：          时间：              
* 修改说明：
* ========================================================================
*/
#endregion
using UnityEngine;
using WillFramework;

public class MainControl : WillMonoSigleton<MainControl> {

	public GameState state{get;private set;}
	protected override void Awake() {
		base.Awake();
		state = GameState.init;
	}


    /// <summary>
    /// 游戏初始化
    /// </summary>
	public void Init(){
		state = GameState.playvideo;
		//开始第一个界面的游戏
		//UIManager.Instance.PopUpWindow(ConstData.PLAYVIDEO);

	}

	/// <summary>
	/// 开始选择游戏界面	
	/// </summary>
	public void ToSelectPeople(){
		//UIManager.Instance.CloseWindow(ConstData.PLAYVIDEO);
		state =GameState.selectpeople;
		
	}
	GameObject sceneObj;
	public void StartGame(){
		
		state = GameState.startgame;
		//加载场景
	}

	public void OverGame(){

		//加载场景
		state = GameState.overgame;
		
	}
}

public enum GameState
{
	init,
	playvideo,
	selectpeople,
	startgame,
	overgame,
	end,
	error

}
