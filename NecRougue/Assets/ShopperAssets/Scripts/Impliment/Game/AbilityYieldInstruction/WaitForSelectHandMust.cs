using System;
using System.Collections;
using System.Collections.Generic;
using ShopperAssets.Scripts.Game;
using UnityEngine;
using Zenject;

public class WaitForAbilityCondition
{
    //inject
    private GamePresenter _gamePresenter;
    private ITextDialogPresenter _textDialogPresenter;
    private GameInputController _inputController;
    private GameInputPresenter _gameInputPresenter;
    private AbilityConditionRequestModel _req;
    private AbilityConditionResponseModel _res = null;

    public AbilityConditionResponseModel Response
    {
        get
        {
            if (_res == null)
                return null;
            return new AbilityConditionResponseModel()
            {
                Condition = _res.Condition,
                Targets = _res.Targets,
                Canceled = _res.Canceled,
            };
        }
    }

    public WaitForAbilityCondition(
        GamePresenter gamePresenter,
        ITextDialogPresenter textDialogPresenter,
        GameInputController inputController,
        GameInputPresenter inputPresenter)
    {
        _gameInputPresenter = inputPresenter;
        _gamePresenter = gamePresenter;
        _textDialogPresenter = textDialogPresenter;
        _inputController = inputController;
    }

    public IEnumerator Wait(AbilityConditionRequestModel model)
    {
        _req = model;
        _res = null;
        _res = new AbilityConditionResponseModel()
        {
            Condition = model.Condition,
        };
        switch (_req.Condition)
        {
            case AbilityUseCase.AbilityCondition.None:
                break;
            case AbilityUseCase.AbilityCondition.ReduceCoin:
                break;
            case AbilityUseCase.AbilityCondition.ReduceHp:
                break;
            case AbilityUseCase.AbilityCondition.SelectHandMust:
                yield return SelectCardMust();
                break;
            case AbilityUseCase.AbilityCondition.SelectHandAny:
                yield return SelectCardAny();
                break;
            case AbilityUseCase.AbilityCondition.SelectHandLess:
               
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    IEnumerator SelectCardMust()
    {
        _gameInputPresenter.ChangeCommand(InputCommand.SelectAnyHand);
        var count = _req.ConditionParam;
        _gamePresenter.UpdateUI();
        if (_gamePresenter.HandCount() < count)
            yield break;
        if (count <= -1)
            yield break;
        _textDialogPresenter.Open(new Vector2(0, 104), new Vector2(128, 64), $"手札を{count}枚選択");
        var valid = false;
        while (!valid)
        {

            while (true)
            {
                switch (_gameInputPresenter.SelectInputState)
                {
                    case SelectInputState.EndSelect:
                        var selectCarcCache = _gameInputPresenter.SelectCardCache;
                        Debug.Log($"手札選択:{selectCarcCache.Count}枚");
                        //枚数チェック
                        if (selectCarcCache.Count < count)
                        {
                            _textDialogPresenter.Open(new Vector2(0, 104), new Vector2(128, 64),
                                $"手札を{count}枚選択\n 枚数が足りません");
                            break;
                        }
                        
                        _textDialogPresenter.Close();
                        _res.Targets = (selectCarcCache.ToArray(), GUIDType.Hand);
                        yield break;
                    case SelectInputState.Cancel:
                        Debug.Log($"手札選択:キャンセル");
                        _textDialogPresenter.Close();
                        _res.Canceled = true;
                        yield break;
                }

                yield return null;
            }

          
        }
       
    }
    
    IEnumerator SelectCardAny()
    {
        _gameInputPresenter.ChangeCommand(InputCommand.SelectAnyHand);
        var count = _req.ConditionParam;
        _gamePresenter.UpdateUI();
        _textDialogPresenter.Open(new Vector2(0, 104), new Vector2(128, 64), $"手札を好きな数選択");
        var valid = false;
        while (!valid)
        {

            while (true)
            {
                switch (_gameInputPresenter.SelectInputState)
                {
                    case SelectInputState.EndSelect:
                        var selectCarcCache = _gameInputPresenter.SelectCardCache;
                        Debug.Log($"手札選択:{selectCarcCache.Count}枚");
                        _textDialogPresenter.Close();
                        _res.Targets = (selectCarcCache.ToArray(), GUIDType.Hand);
                        yield break;
                    case SelectInputState.Cancel:
                        Debug.Log($"手札選択:キャンセル");
                        _textDialogPresenter.Close();
                        _res.Canceled = true;
                        yield break;
                }

                yield return null;
            }

          
        }
       
    }
}