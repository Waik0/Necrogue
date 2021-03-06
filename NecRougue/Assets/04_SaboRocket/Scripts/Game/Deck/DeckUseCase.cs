using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// 山札データ
/// </summary>
public class DeckUseCase 
{
    private List<int> _cardIds = new List<int>();
    private PieceDatas _pieceDatas;
    private int _index;
    [Inject]
    void Inject(PieceDatas pieceDatas)
    {
        _pieceDatas = pieceDatas;
    }

    public void InitRandom()
    {
        _index = 0;
        _cardIds.Clear();
        for (int i = 0; i < 30; i++)
        {
            var r = Random.Range(0, _pieceDatas.Pieces.Length);
            _cardIds.Add(_pieceDatas.Pieces[r].Id);
        }
        Debug.Log("デッキ生成" + _cardIds.Count);
    }

    public int? Draw()
    {
        if (_index < _cardIds.Count)
        {
            var i = _index;
            _index++;
            return _cardIds[i];
        }
        return null;
    }

    public int Remain() => _cardIds.Count - _index;
    public List<int> Deck => _cardIds;
}

public class HandUseCase
{
    private Dictionary<string, List<int>> _hands = new Dictionary<string, List<int>>();
    private DeckUseCase _deck;
    [Inject]
    void Inject(DeckUseCase deck)
    {
        _deck = deck;
    }

    public Dictionary<string, List<int>> Hands => _hands;
    public void Init()
    {
        _hands.Clear();
    }

    public void FirstDraw(List<string> playerId)
    {
        foreach (var s in playerId)
        {
            TryDraw(s);
        }
    }

    public List<int> GetHand(string pid)
    {
        if (!_hands.ContainsKey(pid))
        {
            _hands.Add(pid,new List<int>());
        }
        return _hands[pid];
    }

    public void DeleteHand(string pid, int cid)
    {
        
    }
    public void SetHand(string pid, List<int> cards)
    {
        if (!_hands.ContainsKey(pid))
        {
            _hands.Add(pid,new List<int>());
        }

        _hands[pid] = cards;
    }

    public bool TryDraw(string playerId)
    {
        Debug.Log("ドロー" + playerId);
        if (!_hands.ContainsKey(playerId))
        {
            _hands.Add(playerId,new List<int>());
        }

        var card = _deck.Draw();
        if(card == null)
            return false;
        Debug.Log("あと" + _deck.Remain());
        _hands[playerId].Add(card.Value);
        return true;

    }
    
}