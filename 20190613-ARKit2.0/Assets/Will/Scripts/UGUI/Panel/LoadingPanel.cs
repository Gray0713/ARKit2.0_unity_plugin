#region Version Info
/*========================================================================
* 【本类功能概述】
* 
* 作者：wen      时间：#CreateTime#
* 文件名：LoadPanel
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
using UnityEngine.UI;

public class LoadingPanel : MonoBehaviour
{
    public Slider m_Slider;
    public Text m_Text;

    public void OnUpdateLoading(int loadingprocess){
            m_Slider.value = loadingprocess / 100.0f;
            m_Text.text = string.Format("{0}%",loadingprocess);
    } 
    
}