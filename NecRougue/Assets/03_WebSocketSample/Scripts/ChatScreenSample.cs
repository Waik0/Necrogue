using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ChatScreenSample : MonoBehaviour
{
    [SerializeField] private RectTransform _content;
    [SerializeField] private Text _text;
    [SerializeField] private InputField _input;
    [SerializeField] private MatchingSample _matchingSample;
    private WebSocketSample _ws;
    [Inject]
    void Inject(WebSocketSample ws)
    {
        Debug.Log("Inject Chat");
        _ws = ws;
        _input.onEndEdit.AddListener(CompleteText);
        _ws.OnMessage.Subscribe(Route).AddTo(this);
    }

    void Route(WebSocketSampleResponce res)
    {
        if (res.command == WebSocketSample.TestCommands.Chat)
        {
            var name = "???";
            if (_matchingSample.UserData.ContainsKey(res.msgfrom))
            {
                name = _matchingSample.UserData[res.msgfrom].name;
            }
            name = name.Substring(0, Math.Min(5, name.Length));
            AddText($"{name,-5} : {res.data}");
        }
    }

    void CompleteText(string str)
    {
        _ws?.Send(WebSocketSample.TestCommands.Chat,str);
    }
    public void ClearText()
    {
        _text.text = "";
    }

    public void AddText(string chatText)
    {
        _text.text = chatText + "\n" + _text.text;
        _content.sizeDelta = new Vector2(_content.sizeDelta.x, _text.preferredHeight);
    }
}
