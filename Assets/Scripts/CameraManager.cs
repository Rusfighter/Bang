using UnityEngine;

public class CameraManager : MonoBehaviour {
    public Animator m_Animator;

    public float m_FollowSmoothness = 2;
    public float m_RotationSmoothness = 3;
    public Vector3 m_BulletFollowOffset = Vector3.one;
   
    private Bullet m_Bullet;

    private readonly int NextPhaseTrigger = Animator.StringToHash("NextPhase");
    private readonly int GameOverState = Animator.StringToHash("GameOver");

    public void SetPosition(Transform tf)
    {
        transform.position = tf.position;
        transform.rotation = tf.rotation;
    }

    public void GameOverAnimation(bool condition)
    {
        m_Animator.SetBool(GameOverState, condition);
    }

    public void NextAnimationState()
    {
        m_Animator.SetTrigger(NextPhaseTrigger);
    }

    public void Focus(Bullet obj)
    {
        m_Bullet = obj;
    }

    void Update()
    {
        if (m_Bullet != null)
        {
            Vector3 pos = transform.position;
            Transform bulletTransform = m_Bullet.transform;
            Vector3 bulletPos = bulletTransform.position;

            pos += m_Bullet.DeltaPosition;
            pos = Vector3.Lerp(pos, 
                bulletPos - bulletTransform.forward * m_BulletFollowOffset.z - bulletTransform.right * m_BulletFollowOffset.y - bulletTransform.up * m_BulletFollowOffset.x
                , Time.deltaTime * m_FollowSmoothness);
            transform.position = pos;

            Quaternion targetRotation = Quaternion.LookRotation(bulletPos - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, m_RotationSmoothness * Time.deltaTime);
        }
    }
}
