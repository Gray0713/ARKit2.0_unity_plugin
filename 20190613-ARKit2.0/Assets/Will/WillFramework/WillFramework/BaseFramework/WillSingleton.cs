#region Version Info
/*========================================================================
* 【本类功能概述】
* 
* 作者：wen      时间：2018/12/5 17:17:40
* 文件名：Singleton
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
namespace WillFramework
{
    public class WillSingleton<T> where T : new()
    {
        private static T m_Instance;
        public static T Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new T();

                return m_Instance;
            }
        }
    }
}
