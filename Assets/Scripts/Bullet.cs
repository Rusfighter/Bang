using UnityEngine;

public class Bullet : MonoBehaviour {
    public float m_Speed = 10; // m/s

    Vector3 m_InitDirection;
    Vector3 m_Direction;
    Vector3 m_Target;
    bool m_HaveHitted = false;

    public void Init(Vector3 target)
    {
        m_Target = target;
        m_InitDirection = (m_Target - transform.position).normalized;
    }

    void Update()
    {
        m_Direction = (m_Target - transform.position).normalized;
        Vector3 pos = transform.position;
        pos += m_Direction * m_Speed * Time.deltaTime;
        transform.position = pos;
    }

    void OnTriggerEnter(Collider other)
    {
        if (m_HaveHitted)
            return;

        //Debug.Log("collision");
        PlayerManager playerManager = other.gameObject.GetComponentInParent<PlayerManager>();
        Rigidbody rb = other.GetComponent<Rigidbody>();

        if (playerManager != null && rb != null)
        {
            playerManager.onHit(rb, m_InitDirection);
        }

        m_HaveHitted = true;

        Destroy(gameObject);
    }


}
