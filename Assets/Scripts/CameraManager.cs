using UnityEngine;

public class CameraManager : MonoBehaviour {
    public Animator m_Animator;

    public Vector3 m_OffsetBullet = Vector3.zero;
    public float m_Smoothness = 1;

    private bool m_FollowBullet = false;
    private GameObject m_Bullet;


    private readonly int SlowMotionBool = Animator.StringToHash("SlowMotion");

    public void StartFollowBullet(Bullet bullet)
    {
        m_FollowBullet = true;
        m_Bullet = bullet.gameObject;

        m_Animator.SetBool(SlowMotionBool, true);
        m_Smoothness *= 0.25f;
    }

    public void StopFollowBullet(Bullet bullet)
    {
        m_Animator.SetBool(SlowMotionBool, false);
        m_Smoothness *= 5;
    }

    void Update()
    {
        if (m_FollowBullet && m_Bullet != null)
        {
            Vector3 pos = transform.position;
            pos = Vector3.Lerp(pos, m_Bullet.transform.position + m_OffsetBullet, Time.deltaTime * m_Smoothness);
            transform.position = pos;

            transform.LookAt(m_Bullet.gameObject.transform);
        }
    }
}
