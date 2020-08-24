using System.Collections;
using System.Collections.Generic;
using ShopperAssets.Scripts.Game;
using UnityEngine;

public class AwaitAbilityInput
{
    
    private GamePresenter _gamePresenter;
    private ITextDialogPresenter _textDialogPresenter;
    
    private List<string> _selectCardCache = new List<string>();
    private bool _endSelect;
    private bool _cancelSelect;
    
    public AwaitAbilityInput(GamePresenter gamePresenter, ITextDialogPresenter presenter)
    {
        _gamePresenter = gamePresenter;
        _textDialogPresenter = presenter;
    }
    
    IEnumerator AwaitSelectHandsMust(AbilityConditionResponseModel refModel, int count, bool isCancelable)
    {
        _gamePresenter.UpdateUI();
        if (_gamePresenter.HandCount() < count)
            yield break;
        if (count <= -1)
            yield break;
        _textDialogPresenter.Open(new Vector2(0, 104), new Vector2(128, 64), $"手札を{count}枚選択");
        var valid = false;
        while (!valid)
        {

            while (!_endSelect && !_cancelSelect)
            {
                yield return null;
            }

            if (!_cancelSelect)
            {
                if (count <= -1)
                {
                }
                else
                {
                    Debug.Log($"手札選択:{_selectCardCache.Count}枚");
                    //枚数チェック
                    if (_selectCardCache.Count < count)
                    {
                        _textDialogPresenter.Open(new Vector2(0, 104), new Vector2(128, 64),
                            $"手札を{count}枚選択\n 枚数が足りません");
                    }
                    else
                    {
                        valid = true;
                    }
                }

                _endSelect = false;
            }
            else
            {
                Debug.Log($"手札選択:キャンセル");
                _textDialogPresenter.Close();
                _endSelect = false;
                yield return (new List<string>(), true);
            }

            yield return null;
        }
        _endSelect = false;
        List<string> _selected = new List<string>();
        _selected.AddRange(_selectCardCache);
        _selectCardCache.Clear();
        _textDialogPresenter.Close();
    }
}
