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
        _battleData.GetCard = new List<BattleCard>();
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
    public void AddStock(BattleCard card)
    {
        GetOperationPlayer().Stock.Add(card);
    }
    public void AddGold(int gold)
    {
        _battleData.GetGold += gold;
    }
    //todo できればハッシュ使う方式
    public BattleCard Summon(int stockOrder, int order)
    {
        var card = GetOperationPlayer().Stock[stockOrder];
        GetOperationPlayer().Stock.RemoveAt(stockOrder);
        GetOperationPlayer().Deck.Insert(order, card);
        return card;
    }

    public bool SummonDirect(int pIndex ,int dIndex,int id)
    {
        var card =  new BattleCard().Generate(MasterdataManager.Get<MstMonsterRecord>(id));
        if (_battleData.PlayerList[pIndex].Deck.Count < 8)
        {
            _battleData.PlayerList[pIndex].Deck.Insert(dIndex, card);
            return true;
        }

        return false;
    }

    // public void RemoveDeckCard(int order)
    // {
    //     
    //     GetOperationPlayer().Deck.RemoveAt(order);
    //
    // }

    public bool CheckAndMakeTriple()
    {
        var triple = new Dictionary<int,int>();
        foreach(var card in GetOperationPlayer().Deck)
        {
            if (!triple.ContainsKey(card.Id))
            {
                triple.Add(card.Id, 0);
            }
            triple[card.Id]++;
        }
        var makeTriple = false;
        foreach(var set in triple)
        {
            if(set.Value >= 3)
            {
                makeTriple = true;
                var newId = set.Key + 1000;//IDに1000を足したIDから引く
                if (null == MasterdataManager.Get<MstMonsterRecord>(newId))
                {
                    continue;
                }
                var newCard = new BattleCard().Generate(MasterdataManager.Get<MstMonsterRecord>(newId));
                var currentCard = new BattleCard().Generate(MasterdataManager.Get<MstMonsterRecord>(set.Key));
                var extAtk = 0;
                var extHp = 0;
                var extDef = 0;
                var extPri = 0;
                foreach (var set2 in GetOperationPlayer().Deck.Where(c => c.Id == set.Key))
                {
                    extAtk += set2.Attack - currentCard.Attack;
                    extDef += set2.Defence - currentCard.Defence;
                    extHp += Math.Max(set2.Hp - currentCard.Hp,0);
                    extPri += set2.AttackPriolity - currentCard.AttackPriolity;

                }
                newCard.Attack += Math.Max(extAtk, 0);
                newCard.Hp += Math.Max(extHp,0);
                newCard.Defence = Math.Max(extDef, 0);
                newCard.AttackPriolity += Math.Max(extPri, 0);
                AddStock(newCard);
                GetOperationPlayer().Deck.RemoveAll(_ => _.Id == set.Key);
            }

        }
        return makeTriple;
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
    //public bool IsDeadCurrentDefender()
    //{
    //    return _battleData.CurrentDefencer().Hp <= 0;
    //}
    public bool ConfirmTarget()
    {
        var indexList = Defender()
            .Deck
            .Select((item, index) => new {Index = index, Value = item})
            .Where(_ => _.Value.Hp > 0)
            .Where(_ =>
                _.Value.AttackPriolity == Defender()
                    .Deck
                    .Where(_2=>_2.Hp > 0)
                    .Max(_2 => _2.AttackPriolity)
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
    //倒したのが敵でゴールド入手した場合のみtrue

    public EnemyDestroyState Attack()
    {
        var attacker = _battleData.PlayerList[_battleData.CurrentAttacker].AttackerIndex;
        var atk = _battleData.PlayerList[_battleData.CurrentAttacker].Deck[attacker].Attack;
        _battleData.PlayerList[_battleData.CurrentDefencer].Deck[_target].Hp -= atk;
        if (_battleData.PlayerList[_battleData.CurrentDefencer].PlayerType == PlayerType.Enemy)
        {
            if (_battleData.PlayerList[_battleData.CurrentDefencer].Deck[_target].Hp < 0)
            {
                return EnemyDestroyState.Gold;
            }
            if (_battleData.PlayerList[_battleData.CurrentDefencer].Deck[_target].Hp == 0)
            {
                return EnemyDestroyState.Capture;
            }
        }
        return EnemyDestroyState.None;
    }
    public EnemyDestroyState AttackEach()
    {
        var ret = EnemyDestroyState.None;
        var attacker = _battleData.PlayerList[_battleData.CurrentAttacker].AttackerIndex;
        var atk = _battleData.PlayerList[_battleData.CurrentAttacker].Deck[attacker].Attack;
        _battleData.PlayerList[_battleData.CurrentDefencer].Deck[_target].Hp -= atk;
        if (_battleData.PlayerList[_battleData.CurrentDefencer].PlayerType == PlayerType.Enemy)
        {
            if (_battleData.PlayerList[_battleData.CurrentDefencer].Deck[_target].Hp < 0)
            {
                ret = EnemyDestroyState.Gold;
            }
            if (_battleData.PlayerList[_battleData.CurrentDefencer].Deck[_target].Hp == 0)
            {
                ret = EnemyDestroyState.Capture;
            }
        }
        //反撃
        var attacker2 = _target;
        var defender2 = _battleData.PlayerList[_battleData.CurrentAttacker].AttackerIndex;
        var atk2 =  _battleData.PlayerList[_battleData.CurrentDefencer].Deck[_target].Attack;
        
        _battleData.PlayerList[_battleData.CurrentAttacker].Deck[defender2].Hp -= atk2;
        if (_battleData.PlayerList[_battleData.CurrentAttacker].PlayerType == PlayerType.Enemy)
        {
            if (_battleData.PlayerList[_battleData.CurrentAttacker].Deck[defender2].Hp < 0)
            {
                ret = EnemyDestroyState.Gold;
            }
            if (_battleData.PlayerList[_battleData.CurrentAttacker].Deck[defender2].Hp == 0)
            {
                ret = EnemyDestroyState.Capture;
            }
        }
        
        return ret;
    }
    //種族
    public List<RaceData> GetRaceData(long unique)
    {
        for (var i = 0; i < _battleData.PlayerList.Count; i++)
        {
            var card = _battleData.PlayerList[i].Deck.FirstOrDefault(_=>_.Unique == unique);
            if (card == null)
            {
                continue;
            }

            return card.Race;
        }
        return new List<RaceData>();
    }
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
    public void ResolveAbilityDead(List<BattleCard> cards, Action<BattleData> command)
    {
        foreach (var card in cards)
        {
            for (var k = 0; k < card.AbilityList.Count; k++)
            {

                                    //todo 実行速度に影響するようなら書き換え
                var isAbilityExecute = AbilityEffects.GetEffect(
                            MasterdataManager.Get<MstAbilityRecord>(card.AbilityList[k].Id))
                            (new AbilityEffectsArgument()
                            {
                                BattleDataUseCase = this,
                                TimingType = AbilityTimingType.Dead,
                                //Level = card.Level,
                                AbilityCardUnique = card.Unique,
                            });
                if (isAbilityExecute)
                {
                    // _currentAbilityUser = PlayerType.Player;
                    // _currentAbilityTimingType = timingType;
                    // _currentAbilityUserCard = j;
                    //todo スナップショットに死亡カードデータを残す
                    command(GetSnapShot());

                }
            }
        }
    }

    public void ResolveAbilityAll(AbilityTimingType timingType, Action<BattleData> command,long action = -1,long defender = -1)
    {
        var execAbilities = new List<(int, AbilityEffectsArgument)>();
        foreach (var battlePlayerData in _battleData.PlayerList)
        {
            foreach (var battleCard in battlePlayerData.Deck)
            {
                foreach (var ability in battleCard.AbilityList)
                {
                    execAbilities.Add((ability.Id,
                            new AbilityEffectsArgument()
                            {
                                BattleDataUseCase = this,
                                AbilityCardUnique = battleCard.Unique,
                                ActionCardUnique = action,
                                DefenderCardUnique = defender,
                                TimingType = timingType,
                                //Level = battleCard.Grade
                            }
                            ));
                }
            }
        }
        foreach (var execAbility in execAbilities)
        {
            var isAbilityExecute = AbilityEffects.GetEffect(
                MasterdataManager.Get<MstAbilityRecord>(execAbility.Item1))
            (execAbility.Item2);
        }
        // for (var i = 0; i < _battleData.PlayerList.Count; i++)
        // {
        //     for (var j = 0; j < _battleData.PlayerList[i].Deck.Count; j++)
        //     {
        //         for (var k = 0; k < _battleData.PlayerList[i].Deck[j].AbilityList.Count; k++)
        //         {
        //
        //             {
        //                 var actionPlayerIndex = p;
        //                 var actionDeckIndex = d;
        //                 var defenderPlayerIndex = -1;
        //                 var defenderDeckIndex = -1;
        //                 if (
        //                     timingType == AbilityTimingType.Attack ||
        //                     timingType == AbilityTimingType.Defence)
        //                 {
        //                     defenderPlayerIndex = DefenderPlayerIndex();
        //                     defenderDeckIndex = DefenderDeckIndex();
        //                 }
        //
        //             //todo 実行速度に影響するようなら書き換え
        //                 var isAbilityExecute = AbilityEffects.GetEffect(
        //                     MasterdataManager.Get<MstAbilityRecord>(_battleData.PlayerList[i].Deck[j].AbilityList[k].Id))
        //                     (new AbilityEffectsArgument()
        //                 {
        //                     BattleDataUseCase = this,
        //                     AbilityCardUnique = 
        //                     AbilityDeckIndex = j,
        //                     AbilityPlayerIndex = i,
        //                     ActionPlayerIndex = actionPlayerIndex,
        //                     ActionDeckIndex = actionDeckIndex,
        //                     DefenderPlayerIndex = defenderPlayerIndex,
        //                     DefenderDeckIndex = defenderDeckIndex,
        //                     TimingType = timingType,
        //                     Level = _battleData.PlayerList[i].Deck[j].Level
        //                 });
        //                 if (isAbilityExecute)
        //                 {
        //                     _battleData.PlayerList[i].Deck[j].UseAbilityBefore = true;
        //                     // _currentAbilityUser = PlayerType.Player;
        //                     // _currentAbilityTimingType = timingType;
        //                     // _currentAbilityUserCard = j;
        //                     command(GetSnapShot());
        //                     
        //                 }
        //                
        //             }
        //         }
        //     }
        // }
    }
    //----------------------------------------------------------------------------------------------------------------------
    // プレイヤー系
    //----------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------
    // PlayerTypeから取得
    public List<BattlePlayerData> GetPlayerList()
    {
        return _battleData.PlayerList;
    }
    public BattlePlayerData GetPlayerRef(PlayerType type)
    {
        return _battleData.PlayerList.FirstOrDefault(_ => _.PlayerType == type);
    }
    public BattlePlayerData Attacker() => _battleData.PlayerList[_battleData.CurrentAttacker];
    public BattlePlayerData Defender()=>_battleData.PlayerList[_battleData.CurrentDefencer];
    //----------------------------------------------------------------------------------------------------------------------
    // カード系
    //----------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------
    // Uniqueから取得
    //UniqueからIndex取得
    public int GetDeckIndex(int pIndex,long unique)
    {
        if (_battleData.PlayerList.Count <= pIndex || pIndex < 0)
        {
            return -1;
        }

        for (var i = 0; i < _battleData.PlayerList[pIndex].Deck.Count; i++)
        {
            if (_battleData.PlayerList[pIndex].Deck[i].Unique == unique)
            {
                return i;
            }
        }

        return -1;
    }

    public int GetPlayerIndex(long unique)
    {
        for (var i = 0; i < _battleData.PlayerList.Count; i++)
        {
            var found = _battleData.PlayerList[i].Deck.Any(_ => _.Unique == unique);
            if (found)
            {
                return i;
            }
        }

        return -1;
    }
    //死亡プールの中から死亡時のデッキの場所を取得
    public int GetDeadPlayerIndex(long unique)
    {
        for (var i = 0; i < _battleData.PlayerList.Count; i++)
        {
            var found = _battleData.PlayerList[i].Dead.Any(_ => _.Item3.Unique == unique);
            if (found)
            {
                return i;
            }
        }

        return -1;
    }
    public int GetDeadDeckIndex(int pIndex,long unique)
    {
        if (_battleData.PlayerList.Count <= pIndex || pIndex < 0)
        {
            return -1;
        }
        for (var i = 0; i < _battleData.PlayerList[pIndex].Deck.Count; i++)
        {
            if (_battleData.PlayerList[pIndex].Dead[i].Item3.Unique == unique)
            {
                return _battleData.PlayerList[pIndex].Dead[i].Item2;
            }
        }

        return -1;
    }
    //死亡プールまたはデッキからインデックスを取得
    public (bool isDead, int pIndex, int dIndex) GetIndex(long unique)
    {
        
        for (var i = 0; i < _battleData.PlayerList.Count; i++)
        {
            for (var j = 0; j < _battleData.PlayerList[i].Deck.Count; j++)
            {
                var found = _battleData.PlayerList[i].Deck[j].Unique == unique;
                if (found)
                {
                    return (false,i,j);
                }
            }

            for (var j = 0; j < _battleData.PlayerList[i].Dead.Count; j++)
            {
                var found = _battleData.PlayerList[i].Dead[j].Item3.Unique == unique;
                if (found)
                {
                    return (true,_battleData.PlayerList[i].Dead[j].Item1,_battleData.PlayerList[i].Dead[j].Item2);
                }
            }
        }

        return (false, -1, -1);
    }
    
    //カードをデッキ内に所持しているプレイヤーのタイプ
    public PlayerType GetPlayerType(long unique)
    {
        for (var i = 0; i < _battleData.PlayerList.Count; i++)
        {
            var found = _battleData.PlayerList[i].Deck.Any(_ => _.Unique == unique);
            if (found)
            {
                return _battleData.PlayerList[i].PlayerType;
            }
        }

        return PlayerType.None;
    }
    //uniqueでカード取得
    public BattleCard GetCardRef(long unique)
    {
        foreach (var battlePlayerData in _battleData.PlayerList)
        {
            var card = battlePlayerData.Deck.FirstOrDefault(_ => _.Unique == unique);
            if (card != null)
            {
                return card;
            }
        }

        return null;
    }public BattleCard GetCardByIndex(int pIndex,int dIndex)
    {
        if (IsOutOfDeckRange(pIndex, dIndex))
        {
            return null;
        }
        return _battleData.PlayerList[pIndex].Deck[dIndex];
    }
    //----------------------------------------------------------------------------------------------------------------------
    // 引数無し取得
    //ターゲットを決めていないと取得できない。
    //todo タイミング強制的に限定
    public BattleCard GetCurrentDefenderCard()
    {
        return _battleData.PlayerList[_battleData.CurrentDefencer].Deck[_target];
    }
    //Attackerが確定してからでないとIndexOutOfRangeになる可能性がある。
    //todo タイミング強制的に限定
    public BattleCard GetCurrentAttackerCard()
    {
        var index = _battleData.PlayerList[_battleData.CurrentAttacker].AttackerIndex;
        return _battleData.PlayerList[_battleData.CurrentAttacker].Deck[index];
    }
    
    //----------------------------------------------------------------------------------------------------------------------
    // キャプチャ
    public void AddCaptureCard(int id)
    {
        var card = new BattleCard().Generate(MasterdataManager.Get<MstMonsterRecord>(id));
        _battleData.GetCard.Add(card);
    }
    //----------------------------------------------------------------------------------------------------------------------
    // デッキから削除
    
    public List<BattleCard> RemoveDeadCard()
    {
        var removed = new List<BattleCard>();
        for (var i = 0; i < _battleData.PlayerList.Count; i++)
        {
            for (var j = _battleData.PlayerList[i].Deck.Count - 1; j >= 0; j--)
            {
                if (_battleData.PlayerList[i].Deck[j].Hp <= 0)
                {
                    var pIndex = i;
                    var dIndex = j;
                    var remove = _battleData.PlayerList[i].Deck[j];
                    removed.Add(remove);
                    _battleData.PlayerList[i].Dead.Add((pIndex,dIndex,remove));
                    _battleData.PlayerList[i].Deck.RemoveAt(j);
                    
                }
            }
        }
        return removed;
    }
    //ショップなどで売ったときなど、操作キャラの所持カードを削除
    public bool RemoveOperationPlayerDeckCard(long unique)
    {
        var card = GetOperationPlayer().Deck.FirstOrDefault(_ => _.Unique == unique);
        if (card == null)
        {
            return false;
        }
        GetOperationPlayer().Deck.Remove(card);
        return true;
        

    }
    //超電磁など、死亡以外での削除
    public bool ForceRemoveDeckCard(long unique)
    {
        for (var i = 0; i < _battleData.PlayerList.Count; i++)
        {
            for (var j = _battleData.PlayerList[i].Deck.Count - 1; j >= 0; j--)
            {
                if (_battleData.PlayerList[i].Deck[j].Unique == unique)
                {
                    _battleData.PlayerList[i].Dead.Add((i,j,_battleData.PlayerList[i].Deck[j]));
                    _battleData.PlayerList[i].Deck.RemoveAt(j);
                    return true;
                }
                

            }
            
        }
        
        return false;

    }
    //書き換え
    //判定

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
                _battleData.PlayerList[i].Deck[j].Attack = _battleData.PlayerList[i].Deck[j].Attack;
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
                var isDead = _battleData.PlayerList[i].Deck[j].Hp <= 0;
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
        return _battleData.PlayerList[playerIndex].Deck[cardIndex].Hp > 0;
    }
    public bool IsFirstTurn() => _battleData.Turn == 0;
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
    
    //全滅判定
    public PlayerType CheckWinner()
    {
        var winner = PlayerType.None;
        var loser = new List<PlayerType>();
        foreach (var battlePlayerData in _battleData.PlayerList)
        {
            if (battlePlayerData.Deck.Count <= 0)
            {
                loser.Add(battlePlayerData.PlayerType);
            }
        }

        if (loser.Count > 0)
        {
            winner = loser.Contains(PlayerType.Player) ? PlayerType.Enemy : PlayerType.Player;
        }
        return winner;
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

    public BattleCard RandomCard(int player = -1)
    {
        if (player == -1)
        {
            player = Random.Range(0, _battleData.PlayerList.Count);
        }

        var deck = Random.Range(0, _battleData.PlayerList[player].Deck.Count);
        return GetCardByIndex(player, deck);
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
        return GetOperationPlayer().Deck.Where(_ => _.Hp > 0).ToList();
    }
    public List<BattleCard> GetRemainStocks()
    {
        return GetOperationPlayer().Stock;
    }
    public List<BattleCard> GetAndRemoveCaptureCard()
    {
        var list = new List<BattleCard>();
        foreach (var c in _battleData.GetCard) {
            list.Add(c);
        }
        _battleData.GetCard.Clear();
        return list;
    }
    public int GetAndRemoveGold()
    {
        int g = _battleData.GetGold;
        _battleData.GetGold = 0;
        return g;
    }
    //----------------------------------------------------------------------------------------------------------------------
    // 値取得(参照)
    //----------------------------------------------------------------------------------------------------------------------


}
