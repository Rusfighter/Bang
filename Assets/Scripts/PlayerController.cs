using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour {

    enum AnimatorState
    {
        IDLE, READY, STEADY, SHOOT
    }

    Animator m_Animator;
    AnimatorState m_State = AnimatorState.IDLE;

    readonly int m_ReadyTrigger = Animator.StringToHash("Ready");
    readonly int m_SteadyTrigger = Animator.StringToHash("Steady");
    readonly int m_BangBool = Animator.StringToHash("Bang");

    void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
	    if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) 
            || Input.GetKeyDown(KeyCode.Space))
        {
            switch (m_State)
            {
                case AnimatorState.IDLE:
                    m_Animator.SetTrigger(m_ReadyTrigger);
                    m_State = AnimatorState.STEADY;
                    break;
                case AnimatorState.READY:
                    m_Animator.SetTrigger(m_SteadyTrigger);
                    m_State = AnimatorState.STEADY;
                    break;
                case AnimatorState.STEADY:
                    m_Animator.SetBool(m_BangBool, true);
                    m_State = AnimatorState.SHOOT;
                    break;
            }
        }
	}
}
