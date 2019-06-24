using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;
using System;
using UnityEngine.UI;

public class ARAnchorControl : MonoBehaviour {
    
    public PlayerType m_PlayerType;
    public int NetWrokTimeOut = 2;
    public Dropdown DropObjType;

    public static ARAnchorControl instance;

    public float rate = 0.5f;
    string m_set_url = "http://192.168.0.177:8080/shen/setJson?json=";
    string m_get_url = "http://192.168.0.177:8080/shen/getJson";
    public GetJsonData m_WorldMapData;

    public WorldList m_WorldPoint;
    public WorldList m_get_WorldPoint;

    public string returnMsg;

    [Header("Object list")]
    public List<GameObject> gameObjectPool = new List<GameObject>();
    public List<GameObject> objType;
    public GameObject thisObj;

    public bool normal = true;

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DropObjType.onValueChanged.AddListener((int value) => SetObjType());
    }

    private void Start()
    {
        switch (WillData.playerType)
        {
            case PlayerType.normal:
                if (normal)
                {
                    StartCoroutine(SetServer(m_set_url + JsonMapper.ToJson(m_WorldPoint)));
                }
                else
                {
                    StartCoroutine(GetServer(m_get_url));
                }
                break;
            case PlayerType.host:
                StartCoroutine(SetServer(m_set_url + JsonMapper.ToJson(m_WorldPoint)));
                break;
            case PlayerType.client:
                StartCoroutine(GetServer(m_get_url));
                break;
            default:
                break;
        }
        m_PlayerType = WillData.playerType;
    }


    void Update()
    {
        if (m_PlayerType == PlayerType.host)
        {
            m_WorldPoint.localPosition.x = cube.position.x;
            m_WorldPoint.localPosition.y = cube.position.y;
            m_WorldPoint.localPosition.z = cube.position.z;
        }
        else
        {
            cube.position = new Vector3((float)m_get_WorldPoint.localPosition.x, (float)m_get_WorldPoint.localPosition.y, (float)m_get_WorldPoint.localPosition.z);
        }
    }


    #region World point control

    public Transform cube;
    public GameObject[] TempHitObj;

    public void PlaneObj()
    {
        WorldPoint Temp_worldPoint = new WorldPoint();

        Temp_worldPoint.position.x = cube.position.x;
        Temp_worldPoint.position.y = cube.position.y;
        Temp_worldPoint.position.z = cube.position.z;
        Temp_worldPoint.worldType = m_worldType;
        m_WorldPoint.worldPoints.Add(Temp_worldPoint);
        m_WorldPoint.count = m_WorldPoint.worldPoints.Count;
        print("m_WorldPoint.worldPoints : " + m_WorldPoint.worldPoints);
        thisObj = objType[m_worldType.GetHashCode()];
        GameObject gameObject = Instantiate(thisObj, cube.position, cube.rotation);
        gameObject.transform.parent = WorldAnchor;
        gameObjectPool.Add(gameObject);

    }

    WorldType m_worldType;
    void SetObjType()
    {
        m_worldType = (WorldType)Enum.ToObject(typeof(WorldType), DropObjType.value);
        for (int i = 0; i < TempHitObj.Length; i++)
        {
            TempHitObj[i].SetActive(m_worldType.GetHashCode() == i);
        }
        print("DropObjType.value : " + DropObjType.value);
    }

    public Transform WorldAnchor;
    void ObjPool()
    {
        for (int i = 0; i < m_get_WorldPoint.count; i++)
        {
            if (gameObjectPool.Count < m_get_WorldPoint.worldPoints.Count)
            {
                //int index = m_get_WorldPoint.worldPoints.Count - 1;

                switch (m_get_WorldPoint.worldPoints[gameObjectPool.Count].worldType)
                {
                    case WorldType.cube:
                        thisObj = objType[0];
                        break;
                    case WorldType.sphere:
                        thisObj = objType[1];
                        break;
                    case WorldType.capsule:
                        thisObj = objType[2];
                        break;
                    case WorldType.cylinder:
                        thisObj = objType[3];
                        break;
                    case WorldType.quad:
                        thisObj = objType[4];
                        break;
                    case WorldType.plane:
                        thisObj = objType[5];
                        break;
                    default:
                        break;
                }

                GameObject gameObject = Instantiate(
                    
                    thisObj,

                    new Vector3((float)m_get_WorldPoint.worldPoints[gameObjectPool.Count].position.x,
                    (float)m_get_WorldPoint.worldPoints[gameObjectPool.Count].position.y, 
                    (float)m_get_WorldPoint.worldPoints[gameObjectPool.Count].position.z),

                    new Quaternion((float)m_get_WorldPoint.worldPoints[gameObjectPool.Count].rotation.x, 
                    (float)m_get_WorldPoint.worldPoints[gameObjectPool.Count].rotation.y,
                    (float)m_get_WorldPoint.worldPoints[gameObjectPool.Count].rotation.z, 
                    (float)m_get_WorldPoint.worldPoints[gameObjectPool.Count].rotation.w));

                gameObject.transform.parent = WorldAnchor;
                gameObjectPool.Add(gameObject);

            }
        }
    }

    #endregion

    #region Server

    IEnumerator SetServer(string url)
    {
        UnityWebRequest Request = UnityWebRequest.Get(url);
        Request.timeout = NetWrokTimeOut;
        yield return Request.SendWebRequest();

        if (Request.isHttpError || Request.isNetworkError)
        {
            print("Time out, check the network.");
        }
        else
        {
            returnMsg = Request.downloadHandler.text;
            StartCoroutine(GetServer(m_get_url));
            print("GetServer");
        }
        yield return new WaitForSeconds(rate);
        StartCoroutine(SetServer(m_set_url + JsonMapper.ToJson(m_WorldPoint)));
    }

    IEnumerator GetServer(string url)
    {
        UnityWebRequest Request = UnityWebRequest.Get(url);
        Request.timeout = NetWrokTimeOut;
        yield return Request.SendWebRequest();

        if (Request.isHttpError || Request.isNetworkError)
        {
            print("Time out, check the network.");
        }
        else
        {
            m_WorldMapData = JsonMapper.ToObject<GetJsonData>(Request.downloadHandler.text);
            m_get_WorldPoint = JsonMapper.ToObject<WorldList>(m_WorldMapData.data);
            if (m_PlayerType == PlayerType.client)
            {
                ObjPool();
            }

        }
        yield return new WaitForSeconds(rate);
        StartCoroutine(GetServer(m_get_url));
    }

    #endregion

}
