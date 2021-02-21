using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ChatScreenSample : MonoBehaviour
{
    [SerializeField] private RectTransform _content;
    [SerializeField] private Text _text;
    [SerializeField] private InputField _input;
    private WebSocketSample _ws;
    [Inject]
    void Inject(WebSocketSample ws)
    {
        Debug.Log("Inject Chat");
        _ws = ws;
        _input.onEndEdit.AddListener(CompleteText);
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
