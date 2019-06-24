#region Version Info
/*========================================================================
* 【本类功能概述】
* 
* 作者：wen      时间：#CreateTime#
* 文件名：MonoSigleton
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
    /// <summary>
    /// MonoBehaviour单例模板
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WillMonoSigleton<T> : MonoBehaviour where T : WillMonoSigleton<T>
    {
        protected static T instance;

        public static T Instance
        {
            get { return instance; }
        }

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = (T)this;
            }
            else
            {
                WDebuger.LogError("Gat a second instance of this calss" + this.GetType());
            }
        }
    }
}