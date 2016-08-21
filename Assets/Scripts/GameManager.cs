using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public enum GameEvent
    {
        INIT, IDLE, READY, BANG, SLOWMOTION, FINISH, GAMEOVER
    }

    public PlayerController m_PlayerController;
    public PlayerManager m_PlayerLeft;
    public PlayerManager m_PlayerRight;

    public CameraManager m_CameraManager;

    private GameEvent m_Event;
    public GameEvent m_CurrentEvent {
        get { return m_Event; }
        private set { m_Event = value; Debug.Log(m_Event); }
    }

    void Awake()
    {
        m_CurrentEvent = GameEvent.INIT;

        //setup
        SetupLevel();
    }

    void SetupLevel()
    {

        m_PlayerController.enabled = false;
        StartCoroutine(SetState(0, GameEvent.IDLE));
    }

    IEnumerator SetState(float delay, GameEvent ev)
    {
        yield return new WaitForSeconds(delay);
        m_CurrentEvent = ev;

        switch (m_CurrentEvent)
        {
            case GameEvent.INIT:
                StartCoroutine(SetState(0, GameEvent.IDLE));
                break;
            case GameEvent.IDLE:
                StartCoroutine(SetState(2, GameEvent.READY));
                break;
            case GameEvent.READY:
                m_PlayerLeft.nextState();
                m_PlayerRight.nextState();
                StartCoroutine(SetState(2, GameEvent.BANG));
                break;
            case GameEvent.BANG:
                //enable controllers
                m_PlayerController.enabled = true;
                StartCoroutine(SetState(0, GameEvent.SLOWMOTION));
                break;

            case GameEvent.SLOWMOTION:
                m_PlayerLeft.onBulletShooted += OnBulletShootedEvent;
                //StartCoroutine(SetState(2, GameEvent.FINISH));
                break;
            case GameEvent.FINISH:
                StartCoroutine(SetState(100000, GameEvent.GAMEOVER));
                break;
        }
    }

    private void OnBulletShootedEvent(Bullet bullet)
    {
        m_CameraManager.StartFollowBullet(bullet);
        //slowdown bullet
        bullet.m_Speed *= 0.05f;

        StartCoroutine(OnSlowDownFinished(bullet));

        m_PlayerLeft.onBulletShooted -= OnBulletShootedEvent;
    }

    IEnumerator OnSlowDownFinished(Bullet bullet)
    {
        yield return new WaitForSeconds(5f);
        bullet.m_Speed *= 20f;

        m_CameraManager.StopFollowBullet(bullet);
    }
}
