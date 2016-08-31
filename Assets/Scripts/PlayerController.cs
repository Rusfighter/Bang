using UnityEngine;

public class PlayerController : MonoBehaviour {

    public PlayerManager m_PlayerManager;
    public delegate void Shoot();
    public event Shoot onShoot;

    void Awake()
    {
        //try to get from same object
        if (m_PlayerManager == null)
            m_PlayerManager = GetComponent<PlayerManager>();

        Debug.Assert(m_PlayerManager != null, "No player manager is set");
    }
	
	void Update () {
	    if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) 
            || Input.GetKeyDown(KeyCode.Space))
        {
            m_PlayerManager.nextState();
            if (onShoot != null) onShoot(); 
        }
	}
}
