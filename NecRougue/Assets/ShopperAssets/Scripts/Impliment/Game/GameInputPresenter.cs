using System.Collections;
using System.Collections.Generic;
using ShopperAssets.Scripts.Game;
using UnityEngine;

//GameInputControllerとGamePresenter,GameViewの接続
public enum MenuInputState
{
    None,
    UseCard,
    BuyCard,
    EndTurn,
}

public enum SelectInputState
{
    None,
    EndSelect,
    Cancel
    
}
public class GameInputPresenter 
{
  
    private GameInputController _inputController;
    private GameView _gameView;
    private GamePresenter _presenter;
    private SelectInputState _selectInputState;
    private string _usingCard;
    private string _buyingCard;
    private MenuInputState _menuInputState;
    private List<string> _selectCardCache = new List<string>();

    public SelectInputState SelectInputState
    {
        get
        {
            var r = _selectInputState;
            _selectInputState = SelectInputState.None;
            return r;
        }   
    }

    public string UsingCard
    {
        get
        {
            var r = _usingCard;
            _usingCard = "";
            return r;

        }
    }
    public string BuyingCard
    {
        get
        {
            var r = _buyingCard;
            _buyingCard = "";
            return r;

        }
    }

    public List<string> SelectCardCache
    {
        get
        {
            var r = new List<string>(_selectCardCache);
            _selectCardCache =  new List<string>();
            return r;
        }
    }


    public MenuInputState MenuInputState
    {
        get
        {
            var r = _menuInputState;
            _menuInputState = MenuInputState.None;
            return r;
        }
    }

    public GameInputPresenter(
        GameView gameView,
        GameInputController inputController,
        GamePresenter presenter)
    {
        _gameView = gameView;
        _inputController = inputController;
        _presenter = presenter;
        Connect();
        Init();
    }

    public void ChangeCommand(InputCommand c)
    {
        _inputController.ChangeCommand(c);
    }
    void Connect()
    {
        _gameView.EndTurnButton.onClick.AddListener(_inputController.ClickEndTurn);
        _gameView.CancelButton.onClick.AddListener(_inputController.ClickCancel);
        _gameView.EndSelectButton.onClick.AddListener(_inputController.ClickSelectEnd);
        _gameView.DeckUI.OnClick = _inputController.ClickHandCard;
        _gameView.DeckUI.OnUse = _inputController.DoubleClickHandCard;
        _gameView.ShopUI.OnClick = _inputController.ClickShopCard;
        _gameView.ShopUI.OnBuy = _inputController.DoubleClickShopCard;
    }
    void Init()
    {
        
        _inputController.OnViewHandCard.AddListener(SelectHandOrShop);
        _inputController.OnViewShopCard.AddListener(SelectHandOrShop);
        _inputController.OnBuyShopCard.AddListener(BuyCard);
        _inputController.OnUseHandCard.AddListener(UseHand);
        _inputController.OnCancel.AddListener(Cancel);
        _inputController.OnEndTurn.AddListener(ClickEndTurn);
        _inputController.OnSelectHandCard.AddListener(SelectAnyHand);
        _inputController.OnSelectEnd.AddListener(EndSelect);
    }
    void SelectHandOrShop(string guid)
    {
        _presenter.DeselectCardAll();
        _presenter.SelectCard(guid); 
        _presenter.ViewCardInfo(guid);
    }

    void EndSelect()
    {
        _selectInputState = SelectInputState.EndSelect;
    }

    void ClickEndTurn()
    {
        _menuInputState = MenuInputState.EndTurn;
    }

    void UseHand(string guid)
    {
        _usingCard = guid;
        _menuInputState = MenuInputState.UseCard;
    }

    void BuyCard(string guid)
    {
        _buyingCard = guid;
        _menuInputState = MenuInputState.BuyCard;
    }

    void Cancel()
    {
        _selectInputState = SelectInputState.Cancel;
    }
    void SelectAnyHand(string guid)
    {
        if (ToggleSelectCache(guid))
        {
            _presenter.SelectCard(guid);
            _presenter.ViewCardInfo(guid);
        }
        else
        {
            _presenter.DeselectCard(guid);
        }
    }
    bool ToggleSelectCache(string guid)
    {
        if (_selectCardCache.Contains(guid))
        {
            _selectCardCache.Remove(guid);
            return false;
        }
        else
        {
            _selectCardCache.Add(guid);
            return true;
        }
    }
}
