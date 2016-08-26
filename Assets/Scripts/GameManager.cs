using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public enum GameMode
    {
        AI, MULTIPLAYER
    }

    public enum GameState
    {
        INIT, IDLE, READY, STEADY, BANG, FINISH, GAMEOVER
    }

    public PlayerManager m_Player1, m_Player2;

    public CameraManager m_CameraManager;

    public float m_BangWaitTime = 3f;


    private GameMode m_Mode;

    private PlayerManager m_Player, m_Opponent, m_Winner;
    private PlayerController m_PlayerController;
    private StateMachine<GameState> m_StateMachine;

    void Start()
    {
        //setup
        SetupLevel();
    }

    void SetupLevel()
    {
        m_StateMachine = new StateMachine<GameState>();
        m_StateMachine.AddState(GameState.INIT);
        m_StateMachine.AddState(GameState.IDLE);
        m_StateMachine.AddState(GameState.READY);
        m_StateMachine.AddState(GameState.STEADY);
        m_StateMachine.AddState(GameState.BANG);
        m_StateMachine.AddState(GameState.FINISH);
        m_StateMachine.AddState(GameState.GAMEOVER);

        m_StateMachine.onStateChange += onStateChanged;

        //from menu
        m_Mode = GameMode.AI;

        switch (m_Mode)
        {
            case GameMode.AI:
            case GameMode.MULTIPLAYER:
                bool val = Random.value > 0.5;
                m_Player = val ? m_Player1 : m_Player2;
                m_Opponent = val ? m_Player2 : m_Player1;
                break;
        }

        //setup camera
        m_CameraManager.SetPosition(m_Player.m_CameraTransform);

        m_PlayerController = m_Player.gameObject.AddComponent<PlayerController>();

        m_PlayerController.enabled = false;
        m_StateMachine.NextState(0);
    }

    void Update()
    {
        m_StateMachine.Update(Time.deltaTime);
    }

    void onStateChanged(GameState newState)
    {
        Debug.Log(newState);
        switch (newState)
        {
            case GameState.IDLE:
                m_StateMachine.NextState(2);
                break;
            case GameState.READY:
                m_StateMachine.NextState(1.8f);
                m_Player.nextState();
                m_Opponent.nextState();

                break;
            case GameState.STEADY:
                m_StateMachine.NextState(m_BangWaitTime);
                m_PlayerController.enabled = true;

                m_Player.onBulletShooted += OnBulletShootedEvent;
                m_Opponent.onBulletShooted += OnBulletShootedEvent;

                break;
            case GameState.BANG:
                StartCoroutine(PlayersNextState(m_Opponent, 1f));
                //simulate
                m_StateMachine.NextState(7);
                break;
            case GameState.FINISH:
                DecideWinner();
                m_PlayerController.enabled = false;
                m_StateMachine.NextState(20);
                break;
            case GameState.GAMEOVER:
                SceneManager.LoadScene(0);
                break;
        }
    }

    IEnumerator PlayersNextState(PlayerManager mgr, float delay)
    {
        yield return new WaitForSeconds(delay);
        mgr.nextState();
    }

    private void DecideWinner()
    {
        bool arg = m_Player.ShootTime > m_Opponent.ShootTime;
        m_Winner = arg ? m_Opponent : m_Player ;
        PlayerManager looser = arg ? m_Player : m_Opponent;

        if (m_Winner.Bullet != null && m_Player == m_Winner)
            m_Winner.Bullet.ChangeSpeed(false);
        else looser.Bullet.onDestroy += (Bullet bullet) => {
            StartCoroutine(ShowWinner());
        };

        if (looser.Bullet != null)
        {
            looser.Bullet.ChangeSpeed(false);
            looser.Bullet.isMissing(
                new Vector3(
                    Random.value > 0.5 ? -1 : 1,
                    Random.Range(0, 1), 0));
        }

        //reset camera
        m_CameraManager.StopFollow();
    }

    private IEnumerator ShowWinner()
    {
        m_CameraManager.StartFollow(m_Winner.Bullet.gameObject);

        yield return new WaitForSeconds(5f);
        m_CameraManager.StopFollow();
        m_Winner.Bullet.ChangeSpeed(false);
    }

    private void OnBulletShootedEvent(PlayerManager owner, float shootingTime)
    {
        owner.onBulletShooted -= OnBulletShootedEvent;

        //get target
        PlayerManager target = owner == m_Player ? m_Opponent : m_Player;

        owner.Bullet.Init(target.getHitPoint());

        //store times, penaly if not in bang
        if (m_StateMachine.State == GameState.STEADY)
            owner.ShootTime = shootingTime + m_BangWaitTime * 1000;
        else
            owner.ShootTime = shootingTime;

        Debug.Log(owner.ShootTime);

        //slowdown bullet
        owner.Bullet.ChangeSpeed(true);

        //disable after shooting
        owner.enabled = false;

        //camera 
        if (owner == m_Player)
        {
            m_CameraManager.StartFollow(owner.Bullet.gameObject);
        }
    }
}
