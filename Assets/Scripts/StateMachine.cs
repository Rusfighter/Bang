using System.Collections.Generic;

public class StateMachine<T> {
    private List<T> m_States = new List<T>();
    private int m_CurrentIndex = 0;
    private float m_Delay = 0;
    private bool m_NextState = false;

    public delegate void OnStateChange(T newState);
    public event OnStateChange onStateChange;

    public void AddState(T state)
    {
        m_States.Add(state);
    }

    public void Update(float deltaTime)
    {
        if (!m_NextState) return;

        if (m_Delay > 0)
            m_Delay -= deltaTime;
        else Next();
    }

    public T State {
        get { return m_States[m_CurrentIndex]; }
    }

    private void Next()
    {
        m_CurrentIndex++;
        m_NextState = false;
        if (onStateChange != null) onStateChange(State);
    }

    public bool NextState(float delay)
    {
        if (m_CurrentIndex+1 >= m_States.Count)
            return false;

        m_Delay = delay;
        m_NextState = true;

        return true;
    }
    
}
