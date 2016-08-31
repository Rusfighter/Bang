using UnityEngine;

public class Bullet : MonoBehaviour {
    public float m_HighSpeed = 10; // m/s
    public float m_LowSpeed = 1;

    private float m_Speed;
    private Vector3 m_InitPosition;
    private GameObject m_GO = null;

    public Vector3 DeltaPosition
    {
        get; private set;
    }

    Vector3 m_InitDirection;
    Vector3 m_Direction;
    Vector3 m_Target;
    bool m_HaveHitted = false;
    bool isSlowMotion = false;
    bool m_IsMissing = false;

    public delegate void OnDestroy(Bullet bullet);
    public event OnDestroy onDestroy;

    public void Init(GameObject obj)
    {
        m_GO = obj;
        m_Target = obj.transform.position;
        m_InitDirection = (m_Target - transform.position).normalized;
        m_InitPosition = transform.position;
        m_Speed = m_HighSpeed;
    }

    public void isMissing(Vector3 add)
    {
        m_Target += add + m_InitDirection * 20;
        m_IsMissing = true;
    }

    void Update()
    {
        if (m_GO == null) return;

        Vector3 vec3 = (m_Target - transform.position);

        m_Direction = vec3.normalized;

        float speed = m_Speed;
        if (isSlowMotion)
        {
            float factor = (m_Target - m_InitPosition).magnitude;
            speed *= vec3.magnitude / (2 * factor);
        }
        DeltaPosition = m_Direction * speed * Time.deltaTime;
        transform.position += DeltaPosition;

        if (vec3.sqrMagnitude < 0.75f)
            onTargetReached();
    }

    public void ChangeSpeed(bool slow)
    {
        m_Speed = slow ? m_LowSpeed : m_HighSpeed;
        isSlowMotion = slow;
    }

    void onTargetReached()
    {
        if (m_HaveHitted)
            return;

        PlayerManager playerManager = m_GO.GetComponentInParent<PlayerManager>();
        Rigidbody rb = m_GO.GetComponent<Rigidbody>();

        if (playerManager != null && rb != null && !m_IsMissing)
        {
            playerManager.onHit(rb, m_InitDirection);
        }

        m_HaveHitted = true;

        if (onDestroy != null)
            onDestroy(this);

        Destroy(gameObject);
    }


}
