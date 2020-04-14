using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckOrderUI : MonoBehaviour,IDeckUI
{
    [SerializeField] private RectTransform _root;

    //[SerializeField] private CardUI _cardUiPrefab;
    private List<int> _idsCache;
    public void Init(List<int> ids)
    {
        _idsCache = ids;
        //todo 生成
    }
    
    public List<int> GetOrder()
    {
        return _idsCache;
    }
}
