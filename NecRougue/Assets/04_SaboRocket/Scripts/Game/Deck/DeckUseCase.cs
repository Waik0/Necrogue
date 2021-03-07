using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

/// <summary>
/// 山札データ
/// </summary>
public class DeckUseCase 
{
    private List<int> _cardIds = new List<int>();
    private PieceDatas _pieceDatas;
    private int _index;
    public Action<List<int>, int> OnUpdateDeck = null;
    [Inject]
    void Inject(PieceDatas pieceDatas)
    {
        _pieceDatas = pieceDatas;
    }

    public void InitRandom()
    {
        _index = 0;
        _cardIds.Clear();
        for (int i = 0; i < 20; i++)
        {
            var r = Random.Range(0, _pieceDatas.Pieces.Length);
            _cardIds.Add(_pieceDatas.Pieces[r].Id);
        }
        Debug.Log("デッキ生成" + _cardIds.Count);
    }

    public void SetDeck(List<int> deck,int index)
    {
        _cardIds = deck;
        _index = index;
        OnUpdateDeck?.Invoke(_cardIds,_index);
    }
    public int? Draw()
    {
        if (_index < _cardIds.Count)
        {
            var i = _index;
            _index++;
            OnUpdateDeck?.Invoke(_cardIds,_index);
            return _cardIds[i];
        }
        return null;
    }

    public int Remain() => _cardIds.Count - _index;
    public List<int> Deck => _cardIds;
    public int Index => _index;
}

public class HandUseCase
{
    private Dictionary<string, List<int>> _hands = new Dictionary<string, List<int>>();
    private DeckUseCase _deck;
    public Action<Dictionary<string, List<int>>> OnUpdateHand = null;
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
        Debug.Log("手札破棄");
        if (!_hands.ContainsKey(pid))
        {
            return;
        }
        _hands[pid].Remove(cid);
        OnUpdateHand?.Invoke(_hands);
    }
    public void SetHand(string pid, List<int> cards)
    {
        if (!_hands.ContainsKey(pid))
        { 
            _hands.Add(pid,new List<int>()); 
        }
        _hands[pid] = cards;
        OnUpdateHand?.Invoke(_hands);
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
        Debug.Log(card.Value);
        _hands[playerId].Add(card.Value);
        OnUpdateHand?.Invoke(_hands);
        Debug.Log("あと" + _deck.Remain());
        
        return true;

    }
    
}