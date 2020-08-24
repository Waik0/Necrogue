using System;
using System.Collections;
using System.Collections.Generic;
using ShopperAssets.Scripts.Game;
using UnityEngine;
using UnityEngine.Events;

public enum InputCommand
{
    None,
    Normal,
    SelectAnyHand,
    GameOver,
}

public class GameInputController
{
    private InputCommand _command;
    //情報出す
    public CardEvent OnViewHandCard = new CardEvent();
    //Abilityを使う
    public CardEvent OnUseHandCard = new CardEvent();
    //Abilityで選ぶ
    public CardEvent OnSelectHandCard = new CardEvent();
    //
    public CardEvent OnViewShopCard = new CardEvent();
    //買う
    public CardEvent OnBuyShopCard = new CardEvent();
    //Abilityで選ぶ
    public CardEvent OnSelectShopCard  = new CardEvent();
    //選択終了
    public UnityEvent OnSelectEnd = new UnityEvent();
    //選択キャンセル
    public UnityEvent OnCancel = new UnityEvent();
    //ターン終了
    public  UnityEvent OnEndTurn = new UnityEvent();
    public void ChangeCommand(InputCommand command)
    {
        _command = command;
    }
    public void ClickHandCard(string guid)
        {
            switch (_command)
            {
                case InputCommand.Normal:
                    //情報出す
                    OnViewHandCard?.Invoke(guid);
                    break;
                case InputCommand.SelectAnyHand:
                    //選択
                    OnSelectHandCard?.Invoke(guid);
                    break;
            }
        }

    public void ClickShopCard(string guid)
        {
            switch (_command)
            {
                case InputCommand.Normal:
                    OnViewShopCard?.Invoke(guid);
                    break;
            }
        }
    public void DoubleClickHandCard(string guid)
        {
            switch (_command)
            {
                case InputCommand.Normal:
                    //アビリティを使う
                    OnUseHandCard?.Invoke(guid);
                    break;
                case InputCommand.SelectAnyHand:
                    //選択
                    OnSelectHandCard?.Invoke(guid);
                    break;
            }
        }
    public void DoubleClickShopCard(string guid)
        {
            switch (_command)
            {
                case InputCommand.Normal:
                    //購入
                    OnBuyShopCard?.Invoke(guid);
                    break;
            }
        }

    public void ClickEndTurn()
    {
        switch (_command)
        {
            case InputCommand.Normal:
                OnEndTurn?.Invoke();
                break;

        }

        
    }

    public void ClickCancel()
    {
        switch (_command)
        {
            case InputCommand.SelectAnyHand:
                OnCancel?.Invoke();
                break;
        }

      
    }

    public void ClickSelectEnd()
    {
        switch (_command)
        {
            case InputCommand.SelectAnyHand:
                OnSelectEnd?.Invoke();
                break;
        }
       
    }

}

