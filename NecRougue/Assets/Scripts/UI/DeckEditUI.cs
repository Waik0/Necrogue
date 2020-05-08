using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckEditUI : IModalUI
{
    //[SerializeField] private RectTransform _root;

    //[SerializeField] private CardUI _cardUiPrefab;
    private BattleDataUseCase _battleDataUseCase;
    private bool _isEnd = false;
    public void Inject(BattleDataUseCase data)
    {
        _battleDataUseCase = data;
        //todo 生成
    }

    public void ResetUI()
    {
        _isEnd = false;
    }

    public bool UpdateUI()
    {
        return _isEnd;
    }

#if DEBUG
    private int _select = -1;
    public void DebugUI()
    {
        
        if (_battleDataUseCase == null)
        {
            return;
        }
        GUILayout.BeginHorizontal();
        
        var pdata = _battleDataUseCase.GetCurrentPlayer();
        if (pdata == null)
        {
            return;
        }

        for (var i = 0; i < pdata.Deck.Count; i++)
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label(_select == i ? "selected" : " ");
            GUILayout.Label(pdata.Deck[i].Id.ToString());
            GUILayout.Label("Hp : " + pdata.Deck[i].Hp.ToString());
            GUILayout.Label("At : "+  pdata.Deck[i].Attack.ToString());
            if (_select < 0)
            {
                if (GUILayout.Button("Select"))
                {
                    _select = i;
                    
                }
            }
            else
            {
                if (_select == i)
                {
                    if (GUILayout.Button("Deselect"))
                    {
                        _select = -1;

                    }
                }
                else
                {
                    if (GUILayout.Button("Change"))
                    {
                        _battleDataUseCase.ChangeCard(_select,i);
                        _select = -1;
                    }
                }
            }

            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("OK"))
        {
            _isEnd = true;
        }
    }
    
#endif
}
