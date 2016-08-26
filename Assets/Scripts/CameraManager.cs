using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour {
    public Animator m_Animator;

    public Vector3 m_OffsetBullet = Vector3.zero;
    public float m_HighSmoothness, m_LowSmoothness;

    private float m_Smoothness = 1;

    private bool m_FollowBullet = false;
    private GameObject m_Bullet;

    private readonly int SlowMotionBool = Animator.StringToHash("SlowMotion");

    public void SetPosition(Transform tf)
    {
        transform.position = tf.position;
        transform.rotation = tf.rotation;
    }

    public void StartFollow(GameObject obj)
    {
        m_FollowBullet = true;
        m_Bullet = obj;

        m_Animator.SetBool(SlowMotionBool, true);
        m_Smoothness = m_LowSmoothness;
    }

    public void StopFollow()
    {
        m_Animator.SetBool(SlowMotionBool, false);
        m_Smoothness = m_HighSmoothness;
    }

    void Update()
    {
        if (m_FollowBullet && m_Bullet != null)
        {
            Vector3 pos = transform.position;
            pos = Vector3.Lerp(pos, m_Bullet.transform.position + m_OffsetBullet, Time.deltaTime * m_Smoothness);
            transform.position = pos;

            //transform.LookAt(m_Bullet.gameObject.transform);
            Quaternion targetRotation = Quaternion.LookRotation(m_Bullet.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, m_Smoothness * Time.deltaTime);
        }
    }
}
