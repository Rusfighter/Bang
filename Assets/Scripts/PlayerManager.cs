using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class PlayerManager : MonoBehaviour
{
    public enum PlayerState
    {
        IDLE, READY, BANG
    }

    public float m_ShootDelay = 0.35f;

    public GameObject m_Bullet;
    public Transform m_BulletStart;

    public Transform m_Target;

    Animator m_Animator;

    [SerializeField]
    private PlayerState m_State;

    public delegate void StateChanged(PlayerState newState);
    public event StateChanged onStateChange;

    public delegate void BulletShooted(Bullet bullet);
    public event BulletShooted onBulletShooted;

    private Rigidbody[] m_RigidBodys;


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

    public PlayerState State {
        get { return m_State; }
        private set { m_State = value; if (onStateChange != null) onStateChange(value); }
    }

    public void nextState()
    {
        switch (m_State)
        {
            case PlayerState.IDLE:
                State = PlayerState.READY;
                m_Animator.SetTrigger(m_ReadyTrigger);
                break;
            case PlayerState.READY:
                State = PlayerState.BANG;
                m_Animator.SetBool(m_BangBool, true);
                StartCoroutine(Shoot(m_ShootDelay));
                break;
            case PlayerState.BANG:
                StartCoroutine(Shoot(m_ShootDelay));
                break;
        }
    }

    public IEnumerator Shoot(float delay)
    {
        yield return new WaitForSeconds(delay);   //Wait

        //create a bullet at transform position
        GameObject bullet = (GameObject)Instantiate(m_Bullet, m_BulletStart.position, m_BulletStart.rotation);
        bullet.SetActive(true);

        Debug.Log("Shooted");


        //ugly code, should be replaced to game manager
        Bullet bull = bullet.GetComponent<Bullet>();
        bull.Init(m_Target.position);

        if (onBulletShooted != null)
            onBulletShooted(bull);
    }

    public void onHit(Rigidbody rb, Vector3 direction)
    {
        m_Animator.enabled = false;
        int lenght = m_RigidBodys.Length;
        for (int i = 0; i < lenght; i++)
            m_RigidBodys[i].isKinematic = false;

        rb.AddForce(direction.normalized * Random.Range(200, 400), ForceMode.Impulse);
    }
}
