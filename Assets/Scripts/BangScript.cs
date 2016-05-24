using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Ready_steady_bang : MonoBehaviour {

    public Text screenText;
    private float timer;
    private bool timerActive;
    private bool controlsActive;
    private bool done;

	// Use this for initialization
	void Start () {
        timer = 0;
        timerActive = false;
        controlsActive = false;
        done = false;
        StartCoroutine(Countdown(1f));

		MultiplayerController.onBangTimeReceived += OnMessage;
	}

	void OnMessage(float time){
		Debug.Log (time);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0) && controlsActive)
        {
            timerActive = false;
            timer *= 1000;
            timer = Mathf.RoundToInt(timer);
            screenText.text = timer.ToString() + " ms";
            done = true;
        } else if (Input.GetMouseButtonDown(0) && !controlsActive)
        {
            StopAllCoroutines();
            screenText.text = "Disqualified";
            done = true;
        }

        if (timerActive)
        {
            timer += Time.deltaTime;
        }
	
	}

    IEnumerator Countdown(float interval)
    {
        screenText.text = "Ready";
        yield return new WaitForSeconds(interval);
        screenText.text = "Steady";
        yield return new WaitForSeconds(interval);
        screenText.text = "Bang";
        timerActive = true;
        controlsActive = true;
        yield return null;
        
    }

    public void restart()
    {
        if (done)
        {
            timer = 0;
            timerActive = false;
            controlsActive = false;
            done = false;
            StartCoroutine(Countdown(1f));
        }   
    }

}
