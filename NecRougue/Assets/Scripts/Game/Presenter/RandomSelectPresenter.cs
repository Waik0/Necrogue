using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class FirstCardEventPresenter
{
    private FirstCardEventUseCase _firstCardEventUseCase = new FirstCardEventUseCase();
    private BattleDataUseCase _battleDataUseCase;
    private PlayerDataUseCase _playerDataUseCase;
    private CardData[] _ids;
    private int _getNum = 3;
    private int _currentGetNum = 0;
    public void Inject(PlayerDataUseCase playerDataUseCase)
    {
        _playerDataUseCase = playerDataUseCase;
    }
  
    private int _selectIndex = -1;
    public CardData[] GetRandomId()
    {
        var ids = new List<CardData>();
        for (int i = 0; i < 3; i++)
        {
            ids.Add(_firstCardEventUseCase.GetRandomFirstCardId());
        }
        Debug.Log("TEst");
        _ids = ids.ToArray();
        return _ids;
    }

    public void AddStock(int index)
    {
        _playerDataUseCase.AddStock(_ids[index]);
        _currentGetNum++;
    }
    public bool IsAllSelected()
    {
        return _currentGetNum >= _getNum;
    }
    public void ResetPresenter()
    {
        
    }
#if DEBUG
    public void DebugUI()
    {
        
    }
#endif
}

public class FirstCardEventUseCase
{
    public CardData GetRandomFirstCardId(int rarity = -1)
    {
        var cards = MasterdataManager.Records<MstMonsterRecord>().Where(_ => _.grade == 0).ToList();
        if (rarity != -1)
        {
            cards = cards.Where(_ => _.rarity == rarity).ToList();
        }
        return new CardData().Generate(cards[Random.Range(0, cards.Count)]);
    }
}