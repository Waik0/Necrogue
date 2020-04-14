using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMapUI
{
    int UpdateUI();//戻り値が0以上なら選択された
}

public interface IButtonUI
{
    void Init();
    bool UpdateUI();//trueなら押された
}
public interface IDeckUI
{
    void Init(List<int> ids);
    List<int> GetOrder();
}

