using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MatchingClientView : MonoBehaviour
{
    [SerializeField] private Transform _viewRoot;
    [SerializeField] private Text _roomName;
    [SerializeField] private Text _peers;
    
    [SerializeField] private InputField _inputField;
    private MatchingClientSequence _matchingSequence;
    private ITortecClientUseCaseWithWebSocket _tortecClient;
    [Inject]
    void Inject(
        MatchingClientSequence matchingSequence,
        ITortecClientUseCaseWithWebSocket tortecClient
    )
    {
        _tortecClient = tortecClient;
        _matchingSequence = matchingSequence;
        _matchingSequence.OnActiveSequence.Subscribe(OnChangeActive).AddTo(this);
        //tortecClient.OnJoinRoom.Subscribe(OnJoinRoom).AddTo(this);
        _viewRoot.gameObject.SetActive(false);
    }

    public void Join()
    {
        _matchingSequence?.Join(_inputField.text);
    }
    void OnChangeActive(bool active)
    {
        _viewRoot.gameObject.SetActive(active);
    }

    void OnJoinRoom(string name)
    {
        _roomName.text = name;
    }
    
}
