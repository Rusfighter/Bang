using UnityEngine;
using System.Collections;

public class Resolution : MonoBehaviour {

    public int m_factor = 2;

    private int m_StartResolution_x;
    private int m_StartResolution_y;

    // Use this for initialization
    void Awake () {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}
