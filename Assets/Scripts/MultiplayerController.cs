using UnityEngine;
using UnityEngine.SceneManagement;
using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;
using System;
using System.Collections.Generic;

public class MultiplayerController : RealTimeMultiplayerListener {


	private delegate void OnBangTime(float time);
	public static event OnBangTime onBangTimeReceived;


    private static MultiplayerController _instance = null;
	private uint minimumOpponents = 1;
    private uint maximumOpponents = 1;
    private uint gameVariation = 0;
    private byte _protocolVersion = 1;
    private List<byte> _updateMessage;
    private float bangTime = 0;
    private bool bangTimeChecked = false;

    private MultiplayerController()
    {
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
		_updateMessage = new List<byte>();
    }

    public static MultiplayerController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MultiplayerController();
            }
            return _instance;
        }
    }

    public void SignIn()
    {
        if (!PlayGamesPlatform.Instance.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    Debug.Log("Signing in success user: " + PlayGamesPlatform.Instance.localUser.userName);
                    // We could start our game now
                }
                else
                {
                    Debug.Log("Sign in failed");
                }
            });
        }
        else
        {
            Debug.Log("Already signed in.");
        }
    }

    public void TrySilentSignIn()
    {
        if (!PlayGamesPlatform.Instance.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.Authenticate((bool success) =>
            {
                if (success)
                {
                    Debug.Log("Silently signed in! Welcome " + PlayGamesPlatform.Instance.localUser.userName);
                }
                else
                {
                    Debug.Log("Oh... we're not signed in.");
                }
            }, true);
        }
        else
        {
            Debug.Log("We're already signed in");
        }
    }

    public void SignOut()
    {
        PlayGamesPlatform.Instance.SignOut();
    }

    public bool IsAuthenticated()
    {
        return PlayGamesPlatform.Instance.localUser.authenticated;
    }

    public void StartMatchMaking()
    {
        Debug.Log("Started matchmaking");
        PlayGamesPlatform.Instance.RealTime.CreateQuickGame(minimumOpponents, maximumOpponents, gameVariation, this);
    }

    public void ShowMPStatus(String message)
    {
        Debug.Log(message);
    }

    public void OnRoomSetupProgress(float percent)
    {
        ShowMPStatus("We are " + percent + "% done with setup");
    }

    public void OnRoomConnected(bool success)
    {
        if (success)
        {
            ShowMPStatus("We are connected to the room! I would probably start our game now.");
            SceneManager.LoadScene("Example_02");
            bangTime = UnityEngine.Random.Range(1f, 2f);
            SendBangTime(bangTime);
        }
        else
        {
            ShowMPStatus("Uh-oh. Encountered some error connecting to the room.");
            SceneManager.LoadScene("Menu");
        }
    }

    public void OnLeftRoom()
    {
        ShowMPStatus("We have left the room. We should probably perform some clean-up tasks.");
    }

    public void OnParticipantLeft(Participant participant)
    {
        ShowMPStatus("Player " + participant + " has joined.");
    }

    public void OnPeersConnected(string[] participantIds)
    {
        foreach (string participantID in participantIds)
        {
            ShowMPStatus("Player " + participantID + " has joined.");
        }
    }

    public void OnPeersDisconnected(string[] participantIds)
    {
        foreach (string participantID in participantIds)
        {
            ShowMPStatus("Player " + participantID + " has left.");
        }
    }

    public void OnRealTimeMessageReceived(bool isReliable, string senderId, byte[] data)
    {
        ShowMPStatus("We have received some gameplay messages from participant ID:" + senderId);
        // We'll be doing more with this later...
        byte messageVersion = (byte)data[0];
        // Let's figure out what type of message this is.
        char messageType = (char)data[1];
        if (messageType == 'S')
        {
            Int16 hatNumber = System.BitConverter.ToInt16(data, 2);
            Int16 gunNumber = System.BitConverter.ToInt16(data, 4);
            Int16 characterNumber = System.BitConverter.ToInt16(data, 6);
            Debug.Log("Player " + senderId + " has hat: " + hatNumber + ", gun number: " + gunNumber + " and character number: " + characterNumber);
            // We'd better tell our GameController about this.
        } else if (messageType == 'T')
        {
            float time = System.BitConverter.ToSingle(data, 2);
            Debug.Log("Player " + senderId + " has shot with time: " + time );
            // We'd better tell our GameController about this.
        }
        else if (messageType == 'B')
        {
            float time = System.BitConverter.ToSingle(data, 2);
            if (time > bangTime)
            {
                bangTime = time;
                bangTimeChecked = true;
                Debug.Log("bangTime: " + bangTime);
            } else
            {
                bangTimeChecked = true;
                Debug.Log("bangTime: " + bangTime);
            }
            Debug.Log("Player " + senderId + " has the generated time is: " + bangTime);
			onBangTimeReceived (bangTime);
            // We'd better tell our GameController about this.
        }
    }

    public void SendStartMessage(Int16 hatNumber, Int16 gunNumber, Int16 characterNumber)
    {
        _updateMessage.Clear();
        _updateMessage.Add(_protocolVersion);
        _updateMessage.Add((byte)'S');
        _updateMessage.AddRange(System.BitConverter.GetBytes(hatNumber));
        _updateMessage.AddRange(System.BitConverter.GetBytes(gunNumber));
        _updateMessage.AddRange(System.BitConverter.GetBytes(characterNumber));
        byte[] messageToSend = _updateMessage.ToArray();
        Debug.Log("Sending my update message  " + messageToSend + " to all players in the room");
        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, messageToSend); //Bool for reliabilty true = tcp, false = udp
    }

    public void SendShootTime(float time) 
    {
        _updateMessage.Clear();
        _updateMessage.Add(_protocolVersion);
        _updateMessage.Add((byte)'T');
        _updateMessage.AddRange(System.BitConverter.GetBytes(time));
        byte[] messageToSend = _updateMessage.ToArray();
        Debug.Log("Sending my update message  " + messageToSend + " to all players in the room");
        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, messageToSend); //Bool for reliabilty true = tcp, false = udp
    }

    public void SendBangTime(float time)
    {
        _updateMessage.Clear();
        _updateMessage.Add(_protocolVersion);
        _updateMessage.Add((byte)'B');
        _updateMessage.AddRange(System.BitConverter.GetBytes(time));
        byte[] messageToSend = _updateMessage.ToArray();
        Debug.Log("Sending my update message  " + messageToSend + " to all players in the room");
        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, messageToSend); //Bool for reliabilty true = tcp, false = udp
    }
}
