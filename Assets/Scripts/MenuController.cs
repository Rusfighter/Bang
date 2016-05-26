using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/*public class MenuController : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        MultiplayerController.Instance.TrySilentSignIn();
        Debug.Log("Sign in tried");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SignIn()
    {
        if (!MultiplayerController.Instance.IsAuthenticated())
        {
            MultiplayerController.Instance.SignIn();
        } else
        {
            Debug.Log("Already signed in");
        }
        
    }

    public void SignOut()
    {
        if (MultiplayerController.Instance.IsAuthenticated())
        {
            MultiplayerController.Instance.SignOut();
        } else
        {
            Debug.Log("Not signed in");
        }
    }

	public void StartMPGame() {
		if (MultiplayerController.Instance.IsAuthenticated ()) {
			MultiplayerController.Instance.StartMatchMaking();
            SceneManager.LoadScene("LoadingScene");
		} else
        {
            Debug.LogError("Not signed in");
            Debug.Log("Not signed in, could not start mp game");
        }
	}
}*/
