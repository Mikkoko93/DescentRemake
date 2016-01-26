using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.Chat;

public class ChatManager : MonoBehaviour, IChatClientListener
{



    private const string GeneralChannel = "general";
    private const string AppId = "ecdfc811-a1df-4dab-9e5d-e54fdb0276e3";
    private const string AppVersion = "1.0";

    private ChatClient _chat;
    private string username = "";
    private string _chatText = "";
    private string _privateText = "";
    private string _input = "";

    private bool _connected;


    public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message)
    {

    }

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        Application.runInBackground = true;

        _chat = new ChatClient(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (_chat != null)
            _chat.Service();
    }

    void OnGUI()
    {
        if (!_connected)
        {
            username = GUI.TextField(new Rect(10, 10, 200, 20), username);
            if (GUI.Button(new Rect(10, 35, 80, 20), "Enter"))
            {
                if (!string.IsNullOrEmpty(username) && username.Length > 0)
                    Connect();
            }
        }
        else
        {
            GUI.TextArea(new Rect(10, 10, 200, 200), _chatText);
            _input = GUI.TextField(new Rect(10, 215, 100, 20), _input);
            if (GUI.Button(new Rect(115, 215, 80, 20), "Send"))
            {
                if (!string.IsNullOrEmpty(_input) && _input.Length > 0)
                {
                    SendMessage(_input);
                    _input = "";
                }
            }
        }

    }

    void OnApplicationQuit()
    {
        if (_chat != null)
            _chat.Disconnect();
    }

    private void Connect()
    {
        _chat.Connect(AppId, AppVersion, null);
    }

    private void SendMessage(string message)
    {
        if (message.StartsWith("/"))
        {
            ParseCommand(message);
            return;
        }

        var  mas = message.Split(new[] { ':' });
        if (mas.Length == 2)
        {
            _chat.SendPrivateMessage(mas[0], mas[1]);
            return;
        }

        _chat.PublishMessage(GeneralChannel, message);
    }

    private void ParseCommand(string command)
    {
        switch (command.Remove(0, 1))
        {
            case "clear":
                {
                    _chatText = "";
                    _chat.PublicChannels[GeneralChannel].ClearMessages();
                    break;
                }
        }
    }

    public void OnDisconnected()
    {
        _connected = false;
    }

    public void OnConnected()
    {
        _connected = true;

        _chat.Subscribe(new[] { GeneralChannel }, -1);

        //_chat.SetFriendList(new[] { "admin" });
        _chat.SetOnlineStatus(ChatUserStatus.Online);
    }

    public void OnChatStateChange(ChatState state)
    {

    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        if (channelName == GeneralChannel)
        {
            for (int i = 0; i < senders.Length; i++)
            {
                _chatText = senders[i] + ":" + messages[i] + "\r\n" + _chatText;
            }
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        Debug.Log("Private message! " + sender + ":" + message);
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        foreach (var channel in channels)
        {
            _chat.PublishMessage(channel, "Hello.");
        }
    }

    public void OnUnsubscribed(string[] channels)
    {

    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.Log(string.Format("Friend {0} set status to {1}", user, status));
    }
}


