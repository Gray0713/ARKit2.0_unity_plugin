#region Version Info
/*========================================================================
* 【本类功能概述】
* 做整体数据控制的代码
* 作者：wilson     时间：2019.06.14
* 文件名：DataManager
* 版本：V1.0.1
*
* 修改者：          时间：              
* 修改说明：
* ========================================================================
*/
#endregion
using System;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

public class WillDataManager : WillFramework.WillMonoSigleton<WillDataManager>
{
    private void Awake()
    {
        WillData.LogFileDir = Application.persistentDataPath + "/WillLog/";
    }
}


/// <summary>
/// 数据
/// </summary>
public static class WillData
{
    public static string LogFileDir = null;

    /// <summary>
    /// json文件名
    /// </summary>
    public static string file_name = "WorldMap.json";

    /// <summary>
    /// 玩家房间类型
    /// </summary>
    public static PlayerType playerType;

    #region Json写入

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="gameData"></param>
    public static void SaveInitJson<T>(this T gameData)
    {
        string filePath = Application.persistentDataPath + "/" + file_name;
        StreamWriter sw = new StreamWriter(filePath);
        string text = JsonMapper.ToJson(gameData);

        sw.Write(text);
        sw.Close();

        sw.Dispose();
        Debug.Log(file_name + "初始化成功");
    }

    /// <summary>
    /// 保存json数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="gameData"></param>
    /// <param name="filePath"></param>
    public static void SaveJson<T>(this T gameData, string filePath)
    {
        using (StreamWriter sw = new StreamWriter(filePath))
        {
            try
            {
                string text = JsonMapper.ToJson(gameData);
                sw.Write(text);
            }
            finally
            {
                sw.Close();
            }
        }

        Debug.Log("数据保存成功 : " + filePath);
    }

    /// <summary>
    /// 保存m_JsonData数据为默认文件路径/Data.dat
    /// </summary>
    /// <param name="gameData">保存的数据类</param>
    public static void SaveJson<T>(this T gameData)
    {
        string filePath = Application.persistentDataPath + "/" + file_name;
        StreamWriter sw = new StreamWriter(filePath);
        string text = JsonMapper.ToJson(gameData);

        sw.Write(text);
        sw.Close();

        sw.Dispose();
        Debug.Log("数据保存成功, 路径： " + filePath);
    }


    /// <summary>
    /// 保存Json数据为默认文件路径
    /// </summary>
    /// <param name="gameData">m_JsonData数组</param>
    /// <param name="text">保存的字符串</param>
    public static void SaveJson<T>(this T[] gameData)
    {
        string filePath = Application.persistentDataPath + "/" + file_name;
        StreamWriter sw = new StreamWriter(filePath);
        string text = JsonMapper.ToJson(gameData);

        sw.Write(text);
        sw.Close();

        sw.Dispose();
        Debug.Log(file_name + "数据保存成功");
    }
    #endregion

    #region Json读取
    /// <summary>
    /// 读取m_JsonData数据，返回值为m_JsonData类
    /// </summary>
    /// <param name="filePath">指定文件</param>
    public static T LoadJson<T>(string fileName)
    {
        T jsonData;
        StreamReader sr = new FileInfo(fileName).OpenText();
        //读取所有数据
        string json = sr.ReadToEnd();

        jsonData = JsonMapper.ToObject<T>(json);
        sr.Close();

        sr.Dispose();
        Debug.Log(fileName + " 数据读取完成");

        return jsonData;
    }

    /// <summary>
    /// T的数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T LoadJson<T>()
    {
        T jsonData;
        string fileName = Application.persistentDataPath + "/" + file_name;
        FileInfo fs = new FileInfo(fileName);
        if (fs.Exists)
        {
            StreamReader sr = fs.OpenText();
            //读取所有数据
            string json = sr.ReadToEnd();
            jsonData = JsonMapper.ToObject<T>(json);
            sr.Close();
            sr.Dispose();
            Debug.Log("数据读取完成 ： " + fileName);
        }
        else
        {
            Debug.Log("文件不存在");
            jsonData = default(T);
        }
        return jsonData;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <returns></returns>
    public static T LoadInitJson<T>(this T t)
    {
        T jsonData;
        string fileName = Application.persistentDataPath + "/" + file_name;
        FileInfo fs = new FileInfo(fileName);
        if (fs.Exists)
        {
            StreamReader sr = fs.OpenText();
            //读取所有数据
            string json = sr.ReadToEnd();

            jsonData = JsonMapper.ToObject<T>(json);
            sr.Close();
            sr.Dispose();

            Debug.Log("数据读取完成 ： " + fileName);
        }
        else
        {
            Debug.Log("文件不存在");
            //jsonData = new VideoData();
            jsonData = default(T);
        }
        return jsonData;
    }

    /// <summary>
    /// Json[index][key]获取，返回值为string
    /// </summary>
    /// <returns>JsonString</returns>
    public static string LoadJsons()
    {
        string fileName = Application.persistentDataPath + "/" + file_name;
        StreamReader sr = new FileInfo(fileName).OpenText();
        //读取所有数据
        string json = sr.ReadToEnd();
        sr.Close();

        sr.Dispose();
        Debug.Log(file_name + "数据读取完成 : " + json);
        return json;
    }

    #endregion

    #region Json和T的转换

    /// <summary>
    /// Get Json data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    /// <param name="JsonStr"></param>
    /// <returns></returns>
    public static T GetJsonData<T>(this T t, string JsonStr)
    {
        return JsonMapper.ToObject<T>(JsonStr);
    }

    /// <summary>
    /// Set JsonData
    /// </summary>
    /// <param name="JsonStr"></param>
    /// <returns></returns>
    public static string SetJsonData<T>(this T JsonStr)
    {
        return JsonMapper.ToJson(JsonStr);
    }

    #endregion

}

/// <summary>
/// 共享空间数据
/// </summary>
[Serializable]
public class WorldMapData
{
    /// <summary>
    /// 版本信息
    /// </summary>
    public string version;

    /// <summary>
    /// 修改日期
    /// </summary>
    public string date;
}

/// <summary>
/// 从服务器获取的Json包
/// </summary>
[Serializable]
public class GetJsonData
{
    /// <summary>
    /// 
    /// </summary>
    public int status;
    /// <summary>
    /// 
    /// </summary>
    public string msg;
    /// <summary>
    /// 
    /// </summary>
    public string data;
    /// <summary>
    /// 
    /// </summary>
    public string ok;
}

/// <summary>
/// 位置信息列表
/// </summary>
[Serializable]
public class WorldList
{
    public int count;
    public WVector3 localPosition = new WVector3();
    public List<WorldPoint> worldPoints;
}

/// <summary>
/// 位置信息同步
/// </summary>
[Serializable]
public class WorldPoint
{
    public string name;
    public WorldType worldType;
    public WVector3 position = new WVector3();

    public WQuaternion rotation = new WQuaternion();
}

/// <summary>
/// 物体类型
/// </summary>
public enum WorldType
{
    cube,
    sphere,
    capsule,
    cylinder,
    quad,
    plane
}

/// <summary>
/// Vector3 double value
/// </summary>
[Serializable]
public class WVector3
{
    public double x;
    public double y;
    public double z;
}


/// <summary>
/// Quaternion double value
/// </summary>
[Serializable]
public class WQuaternion
{
    public double x;
    public double y;
    public double z;
    public double w;
}

/// <summary>
/// 玩家房间类型
/// </summary>
public enum PlayerType
{
    normal,
    host,
    client
}
