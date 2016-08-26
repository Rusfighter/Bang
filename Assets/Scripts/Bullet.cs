using UnityEngine;

public class Bullet : MonoBehaviour {
    public float m_HighSpeed = 10; // m/s
    public float m_LowSpeed = 1;

    private float m_Speed;

    Vector3 m_InitDirection;
    Vector3 m_Direction;
    Vector3 m_Target;
    bool m_HaveHitted = false;

    public delegate void OnDestroy(Bullet bullet);
    public event OnDestroy onDestroy;

    public void Init(Vector3 target)
    {
        m_Target = target;
        m_InitDirection = (m_Target - transform.position).normalized;

        m_Speed = m_HighSpeed;
    }

    public void isMissing(Vector3 add)
    {
        m_Target += add + m_InitDirection * 20;
    }

    void Update()
    {
        m_Direction = (m_Target - transform.position).normalized;
        Vector3 pos = transform.position;
        pos += m_Direction * m_Speed * Time.deltaTime;
        transform.position = pos;
    }

    public void ChangeSpeed(bool slow)
    {
        m_Speed = slow ? m_LowSpeed : m_HighSpeed;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Bullet>() != null) return;
        if (m_HaveHitted)
            return;

        PlayerManager playerManager = other.gameObject.GetComponentInParent<PlayerManager>();
        Rigidbody rb = other.GetComponent<Rigidbody>();

        if (playerManager != null && rb != null)
        {
            playerManager.onHit(rb, m_InitDirection);
        }

        m_HaveHitted = true;

        if (onDestroy != null)
            onDestroy(this);

        Destroy(gameObject);
    }


}
