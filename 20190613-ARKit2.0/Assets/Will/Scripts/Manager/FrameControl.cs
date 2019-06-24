using UnityEngine;

public class FrameControl : MonoBehaviour {

    public int m_fps = 60;

    private void Awake()
    {
        Application.targetFrameRate = m_fps;
    }
}
