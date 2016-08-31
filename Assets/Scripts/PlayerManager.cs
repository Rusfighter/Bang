using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class PlayerManager : MonoBehaviour
{
    public enum PlayerState
    {
        EMPTY, IDLE, READY, SHOOTED
    }

    public float m_ShootDelay = 0.35f;

    public GameObject m_Bullet;
    public Transform m_BulletStart;

    public Transform m_CameraTransform;

    Animator m_Animator;
    private PlayerState m_State;

    public delegate void StateChanged(PlayerState newState);
    public event StateChanged onStateChange;

    public delegate void BulletShooted(PlayerManager playerMgr, float time);
    public event BulletShooted onBulletShooted;

    private Rigidbody[] m_RigidBodys;

    private Bullet m_BulletScript;
    public Bullet Bullet { get { return m_BulletScript; } }

    private float m_ShootTime = float.MaxValue;
    public float ShootTime
    {
        set { m_ShootTime = value; }
        get { return m_ShootTime; }
    }

    readonly int m_ReadyTrigger = Animator.StringToHash("Ready");
    readonly int m_BangBool = Animator.StringToHash("Bang");

    void Awake()
    {
        m_Animator = GetComponent<Animator>();
        //always start in idle
        m_State = PlayerState.IDLE;

        //set all rigidbodys to kinamatic
        m_RigidBodys = GetComponentsInChildren<Rigidbody>();

        int lenght = m_RigidBodys.Length;
        for (int i = 0; i < lenght; i++)
            m_RigidBodys[i].isKinematic = true;
    }

    public GameObject getHitPoint()
    {
        GameObject hitPoint =  m_RigidBodys[Random.Range(0, m_RigidBodys.Length - 1)].gameObject;
        return hitPoint;
    }

    public PlayerState State {
        get { return m_State; }
        private set { m_State = value; if (onStateChange != null) onStateChange(value); }
    }

    public void nextState()
    {
        m_State++;
        switch (m_State)
        {
            case PlayerState.IDLE:
                break;
            case PlayerState.READY:
                m_Animator.SetTrigger(m_ReadyTrigger);
                break;
            case PlayerState.SHOOTED:
                m_Animator.SetBool(m_BangBool, true);
                StartCoroutine(Shoot(m_ShootDelay));
                break;
        }
    }

    private IEnumerator Shoot(float delay)
    {
        Debug.Log("Ready to Shoot");
        yield return new WaitForSeconds(delay);   //Wait

        //create a bullet at transform position
        GameObject bullet = (GameObject)Instantiate(m_Bullet, m_BulletStart.position, m_BulletStart.rotation);
        bullet.SetActive(true);
        m_BulletScript = bullet.GetComponent<Bullet>();

        if (onBulletShooted != null)
            onBulletShooted(this, Time.timeSinceLevelLoad);

        Debug.Log("Shoot");
    }

    public void onHit(Rigidbody rb, Vector3 direction)
    {
        m_Animator.enabled = false;
        int lenght = m_RigidBodys.Length;
        for (int i = 0; i < lenght; i++)
            m_RigidBodys[i].isKinematic = false;

        rb.AddForce(direction.normalized * Random.Range(200, 300), ForceMode.Impulse);
    }
}
