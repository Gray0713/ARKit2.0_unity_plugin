#region Version Info
/*========================================================================
 * 【本类功能概述】
 * 
 * 作者：wen      时间：#CreateTime#
 * 文件名：ConstData
 * 版本：V1.0.1
 *
 * 修改者：          时间：              
 * 修改说明：
 * ========================================================================
 */
#endregion

using UnityEngine;
using WillFramework;

public class ConstData
{

    /// <summary>
    /// 共享空间数据路径
    /// </summary>
    public const string WorldMapUrl = "WorldMap";

    #region 本地资源
    /// <summary>
    /// 上传资源服务器地址
    /// </summary>
    //public const string UPLOADSERVERURL = "ftp://192.168.0.198:8008/";
    public const string UPLOADSERVERURL = "ftp://192.168.0.189:21/";
    /// <summary>
    /// 下载本地资源服务器地址
    /// </summary>
    //public const string DOWNLOADSERVERURL = "ftp://192.168.0.198:8008/";
    public const string DOWNLOADSERVERURL = "http://haishi.oss-cn-shenzhen.aliyuncs.com/WorldMap/";

    public const string FTPname = "wilsonftp";
    public const string FTPPassword = "haishi";
    #endregion

    #region 云端资源
    public const string OSSpath = "oss-cn-shenzhen.aliyuncs.com";
    public const string OSSkey = "LdgBmdqaNZ7V3Qwk";
    public const string OSSpassword = "BIcfGyHDn3BWroirEWxeospMVvb8J3";
    public const string companyname = "haishi";

    public const string projectname = "WorldMap";
    #endregion
    /// <summary>
    /// 版本配置文件
    /// </summary>
    public const string LOCALCONFIG = "ABConfigFile.xml";
    /// <summary>
    /// 版本配置文件
    /// </summary>
    public const string WLOCALCONFIG = WorldMapUrl + ".json";
    /// <summary>
    /// 配置表本地文件路径
    /// </summary>
    public const string ABFILEPATH = "/AssetBundle";
    //配置文件路径
    /// <summary>
    /// ABConfig配置信息
    /// </summary>
    public const string ABDATAFILE = "Assets/GameData/GameDataConfig.asset";
    /// <summary>
    /// 需要校验的文件后缀
    /// </summary>
    public const string ABNAMEEXP = ".manifest";
    /// <summary>
    /// AB包路径表
    /// </summary>
    public const string ASSETBUNDLECONFIG = "Assets/GameData/Data/ABData/AssetBundleConfig.bytes";
    /// <summary>
    /// AB包所在目录
    /// </summary>
    public const string ABPATH = "/AssetBundle";
    /// <summary>
    /// 更新后的AB包存放位置
    /// </summary>
    public const string ABNEWPAT = "/AssetBundleNewPath";
    public const string ABCONFIGFILE = "ABConfigFile.xml";

    //界面名称
    /// <summary>
    /// 加载界面预制体
    /// </summary>
    public const string LOADINGPANEL = "LoadingPanel.prefab";
    public const string MENUPANEL = "MenuPanel.prefab";
    public const string TESTPANEL = "Image.prefab";

    //public const string PLAYVIDEO ="01PlayVideo.prefab";
    public const string SELECTPEOPLE = "Select.prefab";

    public const string GameStart = "MainGame.prefab";

    public const string OVERGAME = "OverGame.prefab";

    /// <summary>
    /// 注册UI脚本
    /// </summary>
    public static void RegisterUI()
    {

    }

    //场景名称
    public const string EMPTYSCENE = "Empty";
    public const string MENUSCENE = "01Start";

    //临时prefab路径
    public const string Sceneprefab = "Assets/GameData/Prefabs/GameScene.prefab";
    //临时音乐资源

    /// <summary>
    /// 服务器地址
    /// </summary>
    public static string DownloadServerFtp { get; set; }
    /// <summary>
    /// 下载到的保存地址
    /// </summary>
    public static string DownloadSavePath { get; set; }

    /// <summary>
    /// 是否开启websocket
    /// </summary>
    public const bool IsWebsocketnnect = false;
    /// <summary>
    /// websocket的地址
    /// </summary>
    public const string websocketpath = "";
}

