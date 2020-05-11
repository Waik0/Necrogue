using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DeckEditUI : IModalUI
{
    public class ChangeEvent : UnityEvent<int, int>{}
    public  class SummonEvent : UnityEvent<int, int> {}
    //[SerializeField] private RectTransform _root;
    //[SerializeField] private CardUI _cardUiPrefab;

    public ChangeEvent OnChangeCard = new ChangeEvent();
    public SummonEvent OnSummonEvent = new SummonEvent(); 

    private BattleDataUseCase _battleDataUseCase;
    private bool _isEnd = false;
    //----------------------------------------------------------------------------------------------------------------------
    // パブリックメソッド
    //----------------------------------------------------------------------------------------------------------------------

    public void Inject(BattleDataUseCase data)
    {
        _battleDataUseCase = data;
        //todo 生成
    }

    public void ResetUI()
    {
        DebugLog.Function(this, 3);
        _isEnd = false;
    }

    public bool UpdateUI()
    {
        return _isEnd;
    }
    //----------------------------------------------------------------------------------------------------------------------
    // プライベートメソッド
    //----------------------------------------------------------------------------------------------------------------------

#if DEBUG
    private int _select = -1;
    private int _selectStock = -1;
    public void DebugUI()
    {
        
        if (_battleDataUseCase == null)
        {
            return;
        }
        GUILayout.BeginHorizontal();
        
        var pdata = _battleDataUseCase.GetOperationPlayer();
        if (pdata == null)
        {
            return;
        }
        for (var i = 0; i < pdata.Deck.Count; i++)
        {
            GUILayout.BeginVertical("box", GUILayout.Width(Screen.width / 4));
            GUILayout.Label(_select == i ? "selected" : " ");
            //GUILayout.Label(pdata.Deck[i].Id.ToString());
            //GUILayout.Label("Hp : " + pdata.Deck[i].Hp.ToString());
            //GUILayout.Label("At : "+  pdata.Deck[i].Attack.ToString());
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
                        OnChangeCard.Invoke(_select,i);;
                        _select = -1;
                    }
                }
            }

            GUILayout.EndVertical();
        }

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(GUILayout.Height(50));
        if (_selectStock >= 0)
        {
            for (var i = 0; i < pdata.Deck.Count + 1; i++)
            {
                GUILayout.BeginVertical(GUILayout.Width(Screen.width / 4));
                if (GUILayout.Button("↑召喚"))
                {
                    OnSummonEvent.Invoke(_selectStock, i); ;
                    _selectStock = -1;
                }
                GUILayout.EndVertical();
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.Label("手札");
        GUILayout.BeginHorizontal();
        for (var i = 0; i < pdata.Stock.Count; i++)
        {
            GUILayout.BeginVertical("box", GUILayout.Width(Screen.width / 4));
            GUILayout.Label(_selectStock == i ? "selected" : " ");
            GUILayout.Label(pdata.Stock[i].Name.ToString());
            GUILayout.Label($"<color=green>H: { pdata.Stock[i].Hp.Current,-3}</color> <color=red>A: { pdata.Stock[i].Attack.Current,-3}</color>");

            if (GUILayout.Button("Select"))
            {
                _selectStock = i;

            }

            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("OK"))
        {
            _select = -1;
            _selectStock = -1;
            _isEnd = true;
    
        }
    
    }
    
#endif
}
