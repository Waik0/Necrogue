using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;


//----------------------------------------------------------------------------------------------------------------------
//Battle
//----------------------------------------------------------------------------------------------------------------------
//バトル進行データ
public class BattleDataUseCase : IEntityUseCase<BattleData>
{
    private BattleData _battleData;
    public BattleData Data => _battleData;
    public int _target;
    // public AbilityTimingType _currentAbilityTimingType = AbilityTimingType.None;
    // public PlayerType _currentAbilityUser;
    // public int _currentAbilityUserCard;
    public BattleDataUseCase()
    {
        ResetData();
    }
    //----------------------------------------------------------------------------------------------------------------------
    // トランザクション
    //----------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------
    //初期設定
    public void ResetData()
    {
        _battleData = new BattleData();
        _battleData.Turn = 0;
        _battleData.PlayerList = new List<BattlePlayerData>();
    }

    public bool SetPlayer(BattlePlayerData data)
    {
        if (_battleData.PlayerList.Any(p => p.PlayerType == data.PlayerType))
        {
            return false;
        }
        _battleData.PlayerList.Add(data);
        return true;
    }
    public bool SetEnemyFromMaster(int id)
    {
        var mst = MasterdataManager.Get<MstEnemyRecord>(id);
        return SetPlayer(new BattlePlayerData().Generate(mst));
    }
    //----------------------------------------------------------------------------------------------------------------------
    //デッキ編成処理
    public bool ChangeCard(int select, int change)
    {
        var index = _battleData.PlayerList.FindIndex(_ => _.PlayerType == PlayerType.Player);
        if (index == -1)
        {
            return false;
        }

        var cache = _battleData.PlayerList[index].Deck[select];
        _battleData.PlayerList[index].Deck[select] = _battleData.PlayerList[index].Deck[change];
        _battleData.PlayerList[index].Deck[change] = cache;
        return true;

    }
    public void AddStock(int id)
    {
        var mst = MasterdataManager.Get<MstMonsterRecord>(id);
        GetOperationPlayer().Stock.Add(new BattleCard().Generate(mst));
    }
    public void Summon(int stockOrder, int order)
    {
        var card = GetOperationPlayer().Stock[stockOrder];
        GetOperationPlayer().Stock.RemoveAt(stockOrder);
        GetOperationPlayer().Deck.Insert(order, card);
    }
    //----------------------------------------------------------------------------------------------------------------------
    //バトル処理
    public void IncrementAndChangeAttacker()
    {
        //攻撃終了時インクリメント
        _battleData.PlayerList[_battleData.CurrentAttacker]
            .AttackerIndex++; 
        //次のアタッカーが終了していないか判定
        if (!IsAttackerIndexOut(_battleData.CurrentDefencer))
        {
            //チェンジ
            var cache = _battleData.CurrentDefencer;
            _battleData.CurrentDefencer = _battleData.CurrentAttacker;
            _battleData.CurrentAttacker = cache;
        }

        while (true)
        {
            //モンスター死亡チェック
            while (!IsAttackerIndexOut(_battleData.CurrentAttacker) &&
                   !IsAlive(_battleData.CurrentAttacker,
                       _battleData.PlayerList[_battleData.CurrentAttacker].AttackerIndex))
            {
                _battleData.PlayerList[_battleData.CurrentAttacker].AttackerIndex++;
            }
            //攻撃できるやつがいなかったら
            if (IsAttackerIndexOut(_battleData.CurrentAttacker))
            {
                //どっちも終わってたらぬける
                if (IsAllAttackEnd())
                {
                    break;
                }
                //終わってなければチェンジ
                var cache = _battleData.CurrentDefencer;
                _battleData.CurrentDefencer = _battleData.CurrentAttacker;
                _battleData.CurrentAttacker = cache;
            }
            else
            {
                break;
            }
        }

    }

    public void IncrementOrChangeIfAttackerIsDead()
    {
        
        while (true)
        {
            //モンスター死亡チェック
            while (!IsAttackerIndexOut(_battleData.CurrentAttacker) &&
                   !IsAlive(_battleData.CurrentAttacker,
                       _battleData.PlayerList[_battleData.CurrentAttacker].AttackerIndex))
            {
                _battleData.PlayerList[_battleData.CurrentAttacker].AttackerIndex++;
            }
            //攻撃できるやつがいなかったら
            if (IsAttackerIndexOut(_battleData.CurrentAttacker))
            {
                //どっちも終わってたらぬける
                if (IsAllAttackEnd())
                {
                    break;
                }
                //終わってなければチェンジ
                var cache = _battleData.CurrentDefencer;
                _battleData.CurrentDefencer = _battleData.CurrentAttacker;
                _battleData.CurrentAttacker = cache;
            }
            else
            {
                break;
            }
        }
    }
    public bool ConfirmTarget()
    {
        var indexList = Defender()
            .Deck
            .Select((item, index) => new {Index = index, Value = item})
            .Where(_ => _.Value.Hp.Current > 0)
            .Where(_ =>
                _.Value.AttackPriolity.Current == Defender()
                    .Deck
                    .Where(_2=>_2.Hp.Current > 0)
                    .Max(_2 => _2.AttackPriolity.Current)
            )
            .Select(_ => _.Index)
            .ToList();
        if (indexList.Count <= 0)
        {
            _target = -1;
            return false;
        }
        _target = indexList[Random.Range(0, indexList.Count)];
        return true;
    }

    public void Attack(int target)
    {
        var attacker = _battleData.PlayerList[_battleData.CurrentAttacker].AttackerIndex;
        var atk = _battleData.PlayerList[_battleData.CurrentAttacker].Deck[attacker].Attack;
        _battleData.PlayerList[_battleData.CurrentDefencer].Deck[target].Hp.Current -= atk.Current;
    }
    public BattlePlayerData Attacker() => _battleData.PlayerList[_battleData.CurrentAttacker];
    public BattlePlayerData Defender()=>_battleData.PlayerList[_battleData.CurrentDefencer];
    public int AttackerPlayerIndex() =>_battleData.CurrentAttacker;
    public int AttackerDeckIndex() =>_battleData.PlayerList[_battleData.CurrentDefencer].AttackerIndex;
    public int DefenderPlayerIndex() => _battleData.CurrentDefencer;
    public int DefenderDeckIndex() => _target;
    public void SortPlayerOrderAndConfirmFirstAttacker()
    {
        _battleData.PlayerList = _battleData.PlayerList.OrderBy(_ => _.Speed).ThenBy(_=>_.PlayerType).ToList();
        if (_battleData.PlayerList.Count < 2)
        {
            throw new Exception();
        }
        _battleData.CurrentAttacker = 0;
        _battleData.CurrentDefencer = 1;
    }

    public void ResetAttackIndex()
    {
        for (var i = 0; i < _battleData.PlayerList.Count; i++)
        {
            _battleData.PlayerList[i].ResetAttackIndex();// = _battleData.PlayerList[i].AttackerIndex = 0;
        }
    }

    public void ChangeState(BattleState state)
    {
        _battleData.State = state;
    }
    //todo タイミング判定をEffects側に移す
    public void ResolveAbilityAll(AbilityTimingType timingType, Action<BattleData> command, int playerIndex = -1, int deckIndex = -1)
    {
        for (var i = 0; i < _battleData.PlayerList.Count; i++)
        {
            for (var j = 0; j < _battleData.PlayerList[i].Deck.Count; j++)
            {
                for (var k = 0; k < _battleData.PlayerList[i].Deck[j].AbilityList.Count; k++)
                {

                    {
                        var isMine = true;
                        //SummonとかAttackとかの時に自分の行動かどうか判定するための処理
                        if (playerIndex != -1 && deckIndex != -1)
                        {
                            isMine = i == playerIndex && j == deckIndex;
                        }

                    
                        var isAbilityExecute = _battleData.PlayerList[i].Deck[j].AbilityList[k].Effect(new AbilityEffectsArgument()
                        {
                            BattleDataUseCase = this,
                            DeckIndex = j,
                            PlayerIndex = i,
                            TimingType = timingType,
                            IsMine   = isMine,
                            Level = _battleData.PlayerList[i].Deck[j].Level
                        });
                        if (isAbilityExecute)
                        {
                            _battleData.PlayerList[i].Deck[j].UseAbilityBefore = true;
                            // _currentAbilityUser = PlayerType.Player;
                            // _currentAbilityTimingType = timingType;
                            // _currentAbilityUserCard = j;
                            command(GetSnapShot());
                            
                        }
                       
                    }
                }
            }
        }
    }

    public void ConfirmWinner(PlayerType type)
    {
        DebugLog.Function(this,2);
        _battleData.Winner = type;
        _battleData.IsEnd = true;
        
    }

    public void ResetStatus()
    {
        for (var i = 0; i < _battleData.PlayerList.Count; i++)
        {
            for (var j = 0; j < _battleData.PlayerList[i].Deck.Count; j++)
            {
                _battleData.PlayerList[i].Deck[j].Attack.Current = _battleData.PlayerList[i].Deck[j].Attack.Max;
            }
        }
    }
    //private
    void UpdateCardState()
    {
        for (var i = 0; i < _battleData.PlayerList.Count; i++)
        {
            var attacker = _battleData.CurrentAttacker == i;
            var defender = _battleData.CurrentDefencer == i;
            for (var j = 0; j < _battleData.PlayerList[i].Deck.Count; j++)
            {
                BattleCardState state = BattleCardState.None;
                var isAttack = attacker && _battleData.PlayerList[i].AttackerIndex == j;
                var isTarget = defender && _target == j;
                var isDead = _battleData.PlayerList[i].Deck[j].Hp.Current <= 0;
                if (isDead) state = BattleCardState.Dead;
                if (_battleData.State == BattleState.Attack && isAttack) state = BattleCardState.Attack;
                if (_battleData.State == BattleState.Attack && isTarget) state = BattleCardState.Damage;
                //todo ability               
                if (_battleData.PlayerList[i].Deck[j].UseAbilityBefore) state = BattleCardState.Ability;
                _battleData.PlayerList[i].Deck[j].UseAbilityBefore = false;
                _battleData.PlayerList[i].Deck[j].State = state;
            }
        }
        // _currentAbilityTimingType = AbilityTimingType.None;
        // _currentAbilityUser = PlayerType.None;
        // _currentAbilityUserCard = -1;
    }

    //----------------------------------------------------------------------------------------------------------------------
    // 判定系
    //----------------------------------------------------------------------------------------------------------------------
    public bool IsEndBattle()
    {
        return _battleData.IsEnd;
    }

    public bool IsAlive(int playerIndex,int cardIndex)
    {
        return _battleData.PlayerList[playerIndex].Deck[cardIndex].Hp.Current > 0;
    }
    public bool IsFirstTurn() => _battleData.Turn == 1;
    public bool IsAttackerIndexOut(int index) => _battleData.PlayerList[index].AttackerIndex >= _battleData.PlayerList[index].Deck.Count;

    public bool IsAllAttackEnd()
    {
        var end = true;
        for (var i = 0; i < _battleData.PlayerList.Count; i++)
        {
            end = end && IsAttackerIndexOut(i);
        }

        return end;
        //.ForEach(_ => );
    }
    
    //防御側全滅判定
    public bool IsDefenderAllDead()
    {
        return Defender().Deck.All(_ => _.Hp.Current <= 0);
    }

    public bool IsOutOfDeckRange(int playerIndex, int deckIndex)
    {
        var isPlayerIndexOut = playerIndex < 0 ||  _battleData.PlayerList.Count <= playerIndex;
        if (isPlayerIndexOut)
        {
            return true;
        }
        return (deckIndex < 0 || _battleData.PlayerList[playerIndex].Deck.Count <= deckIndex);
    }
    //----------------------------------------------------------------------------------------------------------------------
    // 値取得
    //----------------------------------------------------------------------------------------------------------------------


    public PlayerType Winner()
    {
        return _battleData.Winner;
    }


    public BattlePlayerData GetPlayer(int index)
    {
        return _battleData.PlayerList[index];
    }




    //view用のスナップショット
    public BattleData GetSnapShot()
    {
        UpdateCardState(); 
        return  Data.DeepCopy();
    }
    //操作キャラのデータ取得
    public BattlePlayerData GetOperationPlayer()
    {
        var index = _battleData.PlayerList.FindIndex(_ => _.PlayerType == PlayerType.Player);
        if (index == -1)
        {
            return null;
        }

        return _battleData.PlayerList[index];
    }
    public int GetOperationPlayerIndex()
    {
        var index = _battleData.PlayerList.FindIndex(_ => _.PlayerType == PlayerType.Player);
        if (index == -1)
        {
            return -1;
        }
        return index;

    }
    //生き残ったデッキ
    public List<BattleCard> GetRemainDecks()
    {
        return GetOperationPlayer().Deck.Where(_ => _.Hp.Current > 0).ToList();
    }
    public List<BattleCard> GetRemainStocks()
    {
        return GetOperationPlayer().Stock;
    }
    //----------------------------------------------------------------------------------------------------------------------
    // 値取得(参照)
    //----------------------------------------------------------------------------------------------------------------------
    public BattleCard GetCardRef(int playerIndex,int deckIndex)
    {
        if (IsOutOfDeckRange(playerIndex, deckIndex))
        {
            return null;
        }
        return _battleData.PlayerList[playerIndex].Deck[deckIndex];
    }

}
