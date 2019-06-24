using UnityEngine.SceneManagement;
using UnityEngine;
using System.IO;

public class WSceneControl : MonoBehaviour {

    public string m_BunleTargetPath = "new_folder";

    private void dAwake()
    {
        m_BunleTargetPath = Application.streamingAssetsPath + ConstData.ABPATH;
        if (!Directory.Exists(m_BunleTargetPath))
        {
            Directory.CreateDirectory(m_BunleTargetPath);
            print("文件不存在，创建！");
        }
        else
        {
            DirectoryInfo file = new DirectoryInfo(m_BunleTargetPath);
            DeleteFile(file);
            file = null;
            print("文件夹存在,删除");
        }
    }

    /// <summary>
    /// 递归删除文件
    /// </summary>
    /// <param name="dirs"></param>
    void DeleteFile(DirectoryInfo dirs)
    {
        if (dirs == null || (!dirs.Exists))
        {
            return;
        }

        DirectoryInfo[] subDir = dirs.GetDirectories();
        if (subDir != null)
        {
            for (int i = 0; i < subDir.Length; i++)
            {
                if (subDir[i] != null)
                {
                    DeleteFile(subDir[i]);
                }
            }
            subDir = null;
        }

        FileInfo[] files = dirs.GetFiles();
        if (files != null)
        {
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i] != null)
                {
                    Debug.Log("删除文件:" + files[i].FullName + "__over");
                    files[i].Delete();
                    files[i] = null;
                }
            }
            files = null;
        }

        Debug.Log("删除文件夹:" + dirs.FullName + "__over");
        dirs.Delete();
    }

    public void DirectToScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void DirectToScene(string index)
    {
        SceneManager.LoadScene(index);
    }
}
