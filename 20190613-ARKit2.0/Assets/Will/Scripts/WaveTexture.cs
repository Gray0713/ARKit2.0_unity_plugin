using UnityEngine;
using System.Collections;
using System.Threading;
public class WaveTexture : MonoBehaviour {  
    public int waveWidth;
public int waveHeight;
float[,] waveA;
float[,] waveB;
Color[] colorBuffer;
Texture2D tex_uv;
bool isRun = true;
int sleepTime;
    // Use this for initialization  
    void Start()
    {
        waveA = new float[waveWidth, waveHeight];
        waveB = new float[waveWidth, waveHeight];
        colorBuffer = new Color[waveWidth * waveHeight];
        tex_uv = new Texture2D(waveWidth, waveHeight);
        GetComponent<Renderer>().material.SetTexture("_WaveTex", tex_uv);
        Thread th = new Thread(new ThreadStart(ComputeWave));
        th.Start();
        //Putpop ();  
        //PutDrop(64,64);  
    }  
    // Update is called once per frame  
    void Update() {  
        sleepTime = (int) (Time.deltaTime* 1000);  
        tex_uv.SetPixels(colorBuffer);  
        tex_uv.Apply();  
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  
            if (Physics.Raycast (ray, out hit)) {  
                    Vector3 pos = transform.worldToLocalMatrix.MultiplyPoint(hit.point);//把世界坐标转换成图片像素坐标  
                    Debug.Log(pos);//pos = -0.5 ~ 0.5  
                    int w = (int)((pos.x + 0.5) * waveWidth);
                    int h = (int)((pos.y + 0.5) * waveHeight);
                    PutDrop(w, h);  
                    PutDrop(w, h);  
            }  
        }  
        //ComputeWave ();  
    }  
    void Putpop()
            {
                //能量置1  
                waveA[waveWidth / 2, waveHeight / 2] = 1;//中间点  
        waveA[waveWidth / 2 - 1, waveHeight / 2] = 1;//左  
        waveA[waveWidth / 2 + 1, waveHeight / 2] = 1;//右  
        waveA[waveWidth / 2, waveHeight / 2 - 1] = 1;//下  
        waveA[waveWidth / 2, waveHeight / 2 + 1] = 1;//上  
        waveA[waveWidth / 2 - 1, waveHeight / 2 - 1] = 1;//左下  
        waveA[waveWidth / 2 - 1, waveHeight / 2 + 1] = 1;//左上  
        waveA[waveWidth / 2 + 1, waveHeight / 2 - 1] = 1;//右下  
        waveA[waveWidth / 2 + 1, waveHeight / 2 + 1] = 1;//右上  
    }  
    void PutDrop(int x, int y)
                {
                    int radius = 20;
float dist;  
        for (int i = -radius; i <= radius; i++) {  
            for (int j = -radius; j <= radius; j++) {  
                if (((x + i >= 0) && (x + i<waveWidth - 1)) && ((y + j >= 0) && (y + j<waveHeight - 1))) {  

                  dist = Mathf.Sqrt (i* i + j* j);  
                    if (dist<radius)  
                        waveA[x + i, y + j] = Mathf.Cos (dist* Mathf.PI / radius);  
                }  
            }  
        }  
    }  
    void ComputeWave()
    {  
        while (isRun) {  
            for (int w = 1; w<waveWidth - 1; w++) {  
                for (int h = 1; h<waveHeight - 1; h++) {  
                    //8个方向计算  
                    waveB[w, h] = (waveA[w - 1, h] +  
                        waveA[w + 1, h] +  
                        waveA[w, h - 1] +  
                        waveA[w, h + 1] +  
                        waveA[w - 1, h - 1] +  
                        waveA[w + 1, h - 1] +  
                        waveA[w - 1, h + 1] +  
                        waveA[w + 1, h + 1]) / 4 - waveB[w, h];  
                    //能量限制  
                    float value = waveB[w, h];  
                    if (value > 1)  
                        waveB[w, h] = 1;  
                    if (value< -1)  
                        waveB[w, h] = -1;  
                    float offset_u = (waveB[w - 1, h] - waveB[w + 1, h]) / 2;
float offset_v = (waveB[w, h - 1] - waveB[w, h + 1]) / 2;
float r = offset_u / 2 + 0.5f;
float g = offset_v / 2 + 0.5f;
//tex_uv.SetPixel (w, h, new Color (r, g, 0));  
colorBuffer[w + waveWidth * h] = new Color(r, g, 0);
waveB[w, h] -= waveB[w, h] * 0.0025f;//能量衰减计算  
                }  
            }  
            //tex_uv.Apply ();  
            float[,] temp = waveA;
waveA = waveB;  
            waveB = temp;  
            Thread.Sleep(sleepTime);  
        }  
    }  
    void OnDestroy()
    {  
        isRun = false;  
    }  
}