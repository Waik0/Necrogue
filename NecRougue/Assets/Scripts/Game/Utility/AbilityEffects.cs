using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class AbilityEffectsArgument
{
    public BattleDataUseCase BattleDataUseCase;
    public BattleProcessSequence BattleProcess;
    //能力発動をチェックする者
    public int AbilityPlayerIndex;
    public int AbilityDeckIndex;
    //アクションを起こしたもの(攻撃、召喚など
    public int ActionPlayerIndex = -1;
    public int ActionDeckIndex = -1;
    //防御者 Attack Defenceの時のみ使用
    public int DefenderPlayerIndex = -1;
    public int DefenderDeckIndex = -1;
    //
    public int Level;//能力レベル
    public int Param1;
    public int Param2; 
    public AbilityTimingType TimingType;
    //public bool IsMine = false;
    //あとで入れるやつ
    public List<BattleCard> TargetCards;
}
public static class AbilityEffects
{

    private static Dictionary<AbilityEffectType, Func<AbilityEffectsArgument,bool>> EffectList = new Dictionary<AbilityEffectType,  Func<AbilityEffectsArgument,bool>>()
    {
        {//
            AbilityEffectType.None,
            abilityEffectsArgument =>
            {
                return false;
            }

        },
        {//モンスターは攻撃力+{param1*level}を得る
            AbilityEffectType.AtUp,
            abilityEffectsArgument =>
            {
                if (abilityEffectsArgument.TargetCards.Count <= 0)
                {
                    return false;
                }
                foreach(var card in abilityEffectsArgument.TargetCards)
                {
                    card.Attack += abilityEffectsArgument.Level * abilityEffectsArgument.Param1;
                }

                return true;
            }
            
        },
        {//モンスターはHp+{param1*level}を得る
            AbilityEffectType.HpUp,
            abilityEffectsArgument =>
            {
                if (abilityEffectsArgument.TargetCards.Count <= 0)
                {
                    return false;
                }
                foreach(var card in abilityEffectsArgument.TargetCards)
                {
                    card.Hp += abilityEffectsArgument.Level * abilityEffectsArgument.Param1;
                }

                return true;
            }
            
        },
        {//モンスターは攻撃力+{param1*level}Hp+{param1*level}を得る
            AbilityEffectType.AtHpUp,
            abilityEffectsArgument =>
            {
                if (abilityEffectsArgument.TargetCards.Count <= 0)
                {
                    return false;
                }
                foreach(var card in abilityEffectsArgument.TargetCards)
                {
                    card.Attack += abilityEffectsArgument.Level * abilityEffectsArgument.Param1;
                    card.Hp += abilityEffectsArgument.Level * abilityEffectsArgument.Param1;
                }

                return true;
            }
            
        },
        {//{param1:EnemyId}を{Param2}体召喚する。
            AbilityEffectType.Summon,
            abilityEffectsArgument =>
            {
                //if (abilityEffectsArgument.TargetCards)
                //{
                    
                //}
                for (int i = 0; i < abilityEffectsArgument.Param2; i++)
                {

                    var summoned = abilityEffectsArgument.BattleDataUseCase
                        .SummonDirect(
                            abilityEffectsArgument.AbilityPlayerIndex,
                            abilityEffectsArgument.AbilityDeckIndex,
                            abilityEffectsArgument.Param1);
                    if (!summoned)
                    {
                        continue;
                    }

                    abilityEffectsArgument.BattleDataUseCase.ChangeState(BattleState.Ability);
                    //召喚時効果発動
                    abilityEffectsArgument.BattleDataUseCase.ResolveAbilityAll(
                        AbilityTimingType.Summon,
                        ss =>
                            abilityEffectsArgument.BattleProcess?.OnCommand.Invoke(new BattleCommand().Generate(ss)),
                        abilityEffectsArgument.AbilityPlayerIndex,
                        abilityEffectsArgument.AbilityDeckIndex);
                    //死亡したモンスターがいるかチェック
                    var removed = abilityEffectsArgument.BattleDataUseCase.RemoveDeadCard();

                    abilityEffectsArgument.BattleDataUseCase.ResolveAbilityDead(removed, ss =>
                        abilityEffectsArgument.BattleProcess?.OnCommand.Invoke(new BattleCommand().Generate(ss)));
                }
                return true;
            }
        },
        {// 挑発を付与
            AbilityEffectType.Blocker,
            abilityEffectsArgument =>
            {
                if (abilityEffectsArgument.TargetCards.Count <= 0)
                {
                    return false;
                }
                foreach (var card in abilityEffectsArgument.TargetCards)
                {
                    if (card.AttackPriolity <= 0)
                    {
                        card.AttackPriolity = 1;
                    }
                }

                return true;
            }
        },
        {// 攻撃無効を付与
            AbilityEffectType.Shield,
            abilityEffectsArgument =>
            {
                if (abilityEffectsArgument.TargetCards.Count <= 0)
                {
                    return false;
                }
                foreach (var card in abilityEffectsArgument.TargetCards)
                {
                    if (card.Defence <= 0)
                    {
                        card.Defence = 1;
                    }
                }

                return true;
            }
        },
        {// スタンさせる
            AbilityEffectType.Stun,
            abilityEffectsArgument =>
            {
                if (abilityEffectsArgument.TargetCards.Count <= 0)
                {
                    return false;
                }
                foreach (var card in abilityEffectsArgument.TargetCards)
                {
                    if (card.Stun <= 0)
                    {
                        card.Stun = 1;
                    }
                }

                return true;
            }
        },
        {// 超電磁
            AbilityEffectType.Union,
            abilityEffectsArgument =>
            {
                var union = abilityEffectsArgument.BattleDataUseCase.GetCardRef(abilityEffectsArgument.AbilityPlayerIndex,
                    abilityEffectsArgument.AbilityDeckIndex);
                if (abilityEffectsArgument.TargetCards.Count <= 0)
                {
                    return false;
                }
                foreach (var card in abilityEffectsArgument.TargetCards)
                {
                    card.Attack += union.Attack;
                    card.Hp += union.Hp;
                    card.Defence = Mathf.Max(  card.Defence + union.Defence,1);
                    card.Stun = Mathf.Max(  card.Stun + union.Stun,1);
                    card.AttackPriolity = Mathf.Max(  card.AttackPriolity + union.AttackPriolity,1);
                    
                }
                //自分を削除
                abilityEffectsArgument.BattleDataUseCase.ForceRemoveDeckCard(abilityEffectsArgument.AbilityPlayerIndex,
                    abilityEffectsArgument.AbilityDeckIndex);

                return true;
            }
        },
        {// ランダム召喚
            AbilityEffectType.SummonRandom,
            abilityEffectsArgument =>
            {
                var records = MasterdataManager.Records<MstMonsterRecord>()
                    .Where(_=>_.rarity == abilityEffectsArgument.Param1).ToList();
                if (records.Count <= 0)
                {
                    return false;
                }
                for (int i = 0; i < abilityEffectsArgument.Param2; i++)
                {
                    var id = records[UnityEngine.Random.Range(0, records.Count)].id;
                    var summoned = abilityEffectsArgument.BattleDataUseCase
                        .SummonDirect(
                            abilityEffectsArgument.AbilityPlayerIndex,
                            abilityEffectsArgument.AbilityDeckIndex,
                            id);
                    if (!summoned)
                    {
                        continue;
                    }

                    abilityEffectsArgument.BattleDataUseCase.ChangeState(BattleState.Ability);
                    //召喚時効果発動
                    abilityEffectsArgument.BattleDataUseCase.ResolveAbilityAll(
                        AbilityTimingType.Summon,
                        ss =>
                            abilityEffectsArgument.BattleProcess?.OnCommand.Invoke(new BattleCommand().Generate(ss)),
                        abilityEffectsArgument.AbilityPlayerIndex,
                        abilityEffectsArgument.AbilityDeckIndex);
                    //死亡したモンスターがいるかチェック
                    var removed = abilityEffectsArgument.BattleDataUseCase.RemoveDeadCard();

                    abilityEffectsArgument.BattleDataUseCase.ResolveAbilityDead(removed, ss =>
                        abilityEffectsArgument.BattleProcess?.OnCommand.Invoke(new BattleCommand().Generate(ss)));
                }
                return true;
            }
        },
        {// よみがえり
            AbilityEffectType.Revive,
            abilityEffectsArgument =>
            {
                if (abilityEffectsArgument.TargetCards.Count <= 0)
                {
                    return false;
                }
                foreach (var card in abilityEffectsArgument.TargetCards)
                {
                    if (card.Revive <= 0)
                    {
                        card.Revive = 1;
                    }
                }

                return true;

            }
        },
        {// 追加攻撃
            AbilityEffectType.ExtAttack,
            abilityEffectsArgument =>
            {
                if (abilityEffectsArgument.TargetCards.Count <= 0)
                {
                    return false;
                }
                foreach (var card in abilityEffectsArgument.TargetCards)
                {
                    card.Hp -= abilityEffectsArgument.Param1;
                }
                return true;
            }
        },
        {// アビリティ付与
            AbilityEffectType.AddAbility,
            abilityEffectsArgument => {
                if (abilityEffectsArgument.TargetCards.Count <= 0)
                {
                    return false;
                }
                var ability = MasterdataManager.Get<MstAbilityRecord>(abilityEffectsArgument.Param1);

                var addability = false;
                foreach (var card in abilityEffectsArgument.TargetCards)
                {
                    if (card.AbilityList.Any(_ => _.Id == ability.id))
                    { 
                        continue;
                    }

                    addability = true;
                    card.AbilityList.Add(new Ability().Generate(ability));
                }
                return addability;
                
            }
        },
    };

    private static Dictionary<AbilityEffectConditionType, Func<AbilityEffectsArgument, bool>> AbilityConditions = 
        new Dictionary<AbilityEffectConditionType, Func<AbilityEffectsArgument, bool>>()
    {
            {
                AbilityEffectConditionType.None,
                arg =>
                {
                    return false;
                }


            },
            {
                AbilityEffectConditionType.Self,
                arg =>
                {
                    return 
                    arg.ActionDeckIndex == arg.AbilityDeckIndex &&
                    arg.ActionPlayerIndex == arg.AbilityPlayerIndex;
                    
                }


            },
            {
                AbilityEffectConditionType.Ally,
                 arg =>
                {
                    return
                    arg.ActionPlayerIndex == arg.AbilityPlayerIndex && //味方サイド
                    arg.ActionDeckIndex != arg.AbilityDeckIndex;//自分以外

                }

            },
            {
                AbilityEffectConditionType.Enemy,
                arg =>
                {
                    return
                        arg.ActionPlayerIndex != arg.AbilityPlayerIndex; //敵サイド

                }

            },
            {//ActionCardの種族がId = param1のとき
                AbilityEffectConditionType.AllyRace,
                 arg =>
                {
                    return arg.ActionPlayerIndex == arg.AbilityPlayerIndex &&
                    arg.BattleDataUseCase.GetRaceData(arg.ActionPlayerIndex,arg.ActionDeckIndex).Any(race =>race.Id == arg.Param1 );

                }

            },
            {
                AbilityEffectConditionType.Any,
                 arg =>
                {
                    return true;

                }

            }
    };

    private static Dictionary<AbilityTimingType, Func<AbilityEffectConditionType, AbilityEffectsArgument, bool>>
        AbilityTiming =
            new Dictionary<AbilityTimingType, Func<AbilityEffectConditionType, AbilityEffectsArgument, bool>>()
            {
                {AbilityTimingType.BattleStart, (c, arg) => true},
                {AbilityTimingType.TurnStart, (c, arg) => true},
                {AbilityTimingType.TurnEnd, (c, arg) => true},
                {AbilityTimingType.Summon, (c, arg) => AbilityConditions[c](arg)},
                {AbilityTimingType.ConfirmTargetAttack, (c, arg) => AbilityConditions[c](arg)},
                {AbilityTimingType.ConfirmTargetDefence, (c, arg) => AbilityConditions[c](arg)},
                {AbilityTimingType.Attack, (c, arg) => AbilityConditions[c](arg)},
                {AbilityTimingType.Defence, (c, arg) => AbilityConditions[c](arg)},
                {AbilityTimingType.Dead, (c, arg) => AbilityConditions[c](arg)},
                {AbilityTimingType.None, (c, arg) => false},

            };
    
    private static Dictionary<AbilityEffectTargetType, Func<AbilityEffectsArgument, List<BattleCard>>> AbilityTargets =
        new Dictionary<AbilityEffectTargetType, Func<AbilityEffectsArgument, List<BattleCard>>>()
    {
            {//発動者の両隣
                AbilityEffectTargetType.MySide,
                arg =>
                {
                    var cards = new List<BattleCard>();
                    if(arg.AbilityPlayerIndex == -1 || arg.AbilityDeckIndex == -1)
                    {
                        return cards;
                    }
                    var left =   arg.BattleDataUseCase
                    .GetCardRef(arg.AbilityPlayerIndex,arg.AbilityDeckIndex - 1);
                    var right =  arg.BattleDataUseCase
                    .GetCardRef(arg.AbilityPlayerIndex,arg.AbilityDeckIndex + 1);
                     if(left != null )cards.Add(left);
                     if(right != null )cards.Add(right);
                    return cards;
                }

            },
            {
                AbilityEffectTargetType.MySelf,
                arg =>
                {
                    var cards = new List<BattleCard>();
                    if(arg.AbilityPlayerIndex == -1 || arg.AbilityDeckIndex == -1)
                    {
                        return cards;
                    }
                    var self =   arg.BattleDataUseCase
                    .GetCardRef(arg.AbilityPlayerIndex,arg.AbilityDeckIndex);
                     if(self != null )cards.Add(self);
                    return cards;
                }

            },
            {
                AbilityEffectTargetType.MyLeft,
                arg =>
                {
                    var cards = new List<BattleCard>();
                    if(arg.AbilityPlayerIndex == -1 || arg.AbilityDeckIndex == -1)
                    {
                        return cards;
                    }
                    var left =   arg.BattleDataUseCase
                        .GetCardRef(arg.AbilityPlayerIndex,arg.AbilityDeckIndex - 1);
                    if(left != null )cards.Add(left);
                    return cards;
                }

            },
            {
                AbilityEffectTargetType.DefenderSide,
                arg =>
                {
                    var cards = new List<BattleCard>();
                    if(arg.DefenderPlayerIndex == -1 || arg.DefenderDeckIndex == -1)
                    {
                        Debug.Log("Error : DefenderSide");
                        return cards;
                    }
                    var left =   arg.BattleDataUseCase
                        .GetCardRef(arg.DefenderPlayerIndex,arg.DefenderDeckIndex - 1);
                    var right =  arg.BattleDataUseCase
                        .GetCardRef(arg.DefenderPlayerIndex,arg.DefenderDeckIndex + 1);
                    if(left != null )cards.Add(left);
                    if(right != null )cards.Add(right);
                    return cards;
                }

            },
            {
                AbilityEffectTargetType.Action,
                arg =>
                {
                    var cards = new List<BattleCard>();
                    if(arg.ActionPlayerIndex == -1 || arg.ActionDeckIndex == -1)
                    {
                        UnityEngine.Debug.Log("Error : Action");
                        return cards;
                    }
                    var self =   arg.BattleDataUseCase
                        .GetCardRef(arg.ActionPlayerIndex,arg.ActionDeckIndex);
                    if(self != null )cards.Add(self);
                    return cards;
                }

            },
            {
                AbilityEffectTargetType.AllyAll,
                arg =>
                {
                    var cards = new List<BattleCard>();
                    if(arg.AbilityPlayerIndex == -1 || arg.AbilityDeckIndex == -1)
                    {
                        UnityEngine.Debug.Log("Error : AllyAll");
                        return cards;
                    }

                    for (int i = 0; i < arg.BattleDataUseCase.GetPlayer(arg.AbilityPlayerIndex).Deck.Count; i++)
                    {
                        if (arg.AbilityDeckIndex == i)
                        {
                            continue;
                        }
                        var c = arg.BattleDataUseCase.GetCardRef(arg.AbilityPlayerIndex, i);
                        if(c != null)cards.Add(c);
                    }
                    return cards;
                }

            },
            {
                AbilityEffectTargetType.EnemyAll,
                arg =>
                {
                    var cards = new List<BattleCard>();
                    if(arg.AbilityPlayerIndex == -1)
                    {
                        return cards;
                    }

                    var ptype = arg.BattleDataUseCase.GetPlayer(arg.AbilityPlayerIndex).PlayerType;
                    for (int j = 0; j < arg.BattleDataUseCase.Data.PlayerList.Count; j++)
                    {
                        if (arg.BattleDataUseCase.Data.PlayerList[j].PlayerType == ptype)
                        {
                            continue;
                        }
                        for (int i = 0; i < arg.BattleDataUseCase.GetPlayer(j).Deck.Count; i++)
                        {
                            if (arg.AbilityDeckIndex == i)
                            {
                                continue;
                            }
                            var c = arg.BattleDataUseCase.GetCardRef(arg.AbilityPlayerIndex, i);
                            if(c != null)cards.Add(c);
                        }
                    }

                    return cards;
                }
            },
            {
                AbilityEffectTargetType.AllyLeftmost,
                arg =>
                {
                    var cards = new List<BattleCard>();
                    if(arg.AbilityPlayerIndex == -1)
                    {
                        return cards;
                    }

                    var card = arg.BattleDataUseCase.GetCardRef(arg.AbilityPlayerIndex, 0);
                    if(card != null)cards.Add(card);

                    return cards;
                }

            },
            {
                AbilityEffectTargetType.EnemyLeftmost,
                arg =>
                {
                    var cards = new List<BattleCard>();
                    if(arg.AbilityPlayerIndex == -1)
                    {
                        return cards;
                    }
                    var ptype = arg.BattleDataUseCase.GetPlayer(arg.AbilityPlayerIndex).PlayerType;
                    for (int j = 0; j < arg.BattleDataUseCase.Data.PlayerList.Count; j++)
                    {
                        if (arg.BattleDataUseCase.Data.PlayerList[j].PlayerType == ptype)
                        {
                            continue;
                        }
                        var card = arg.BattleDataUseCase.GetCardRef(j, 0);
                        if(card != null)cards.Add(card);

                    }

                    
                    return cards;
                }
 
            },
            {
                AbilityEffectTargetType.Random,
                arg =>
                {
                    var cards = new List<BattleCard>();
                    var card = arg.BattleDataUseCase.RandomCard();
                    if(card!=null)cards.Add(card);
                    return cards;
                }

            }
            ,
            {
                AbilityEffectTargetType.All,
                arg =>
                {
                    var cards = new List<BattleCard>();
                    for (int j = 0; j < arg.BattleDataUseCase.Data.PlayerList.Count; j++)
                    {
                        for (int i = 0; i < arg.BattleDataUseCase.GetPlayer(j).Deck.Count; i++)
                        {
                            if (arg.AbilityDeckIndex == i)
                            {
                                continue;
                            }
                            var c = arg.BattleDataUseCase.GetCardRef(arg.AbilityPlayerIndex, i);
                            if(c != null)cards.Add(c);
                        }
                    }
                    return cards;
                }

            }
            ,
            {
                AbilityEffectTargetType.Defender,
                arg =>
                {
                    var cards = new List<BattleCard>();
                    if(arg.DefenderPlayerIndex == -1 || arg.DefenderDeckIndex == -1)
                    {
                        Debug.Log("Error : Defender");
                        return cards;
                    }
                    var self =   arg.BattleDataUseCase
                        .GetCardRef(arg.DefenderPlayerIndex,arg.DefenderDeckIndex);
                    if(self != null )cards.Add(self);
                    return cards;
                }

            }
            ,
            {
                AbilityEffectTargetType.RandomAlly,
                arg =>
                {
                    var cards = new List<BattleCard>();
                    if (arg.AbilityPlayerIndex == -1)
                    {
                        return cards;
                    }
                    var card = arg.BattleDataUseCase.RandomCard(arg.AbilityPlayerIndex);
                    if(card!=null)cards.Add(card);
                    return cards;
                }

            }
            ,
            {
                AbilityEffectTargetType.RandomEnemy,
                arg =>
                {
                    var cards = new List<BattleCard>();
                    var ptype = arg.BattleDataUseCase.GetPlayer(arg.AbilityPlayerIndex).PlayerType;
                    for (int j = 0; j < arg.BattleDataUseCase.Data.PlayerList.Count; j++)
                    {
                        if (arg.BattleDataUseCase.Data.PlayerList[j].PlayerType == ptype)
                        {
                            continue;
                        }
                        var card = arg.BattleDataUseCase.RandomCard(j);
                        if (card != null) cards.Add(card);
                    }

                    return cards;
                }

            }
            
    };

    public static Func<AbilityEffectsArgument, bool> GetEffect(MstAbilityRecord ability)
    {

        var targetType = ability.targetType;
        var conditionType = ability.conditionType;
        var effectType = ability.effectType;
        var timingType = ability.timingType;
       
        var chooseTaregt = AbilityTargets[targetType];
        var condition = AbilityTiming[timingType];
        var effect = EffectList[effectType];

        bool Effect(AbilityEffectsArgument arg)
        {
            if (arg.TimingType != timingType)
            {
                return false;
            }
            
            if (!condition(conditionType,arg))
            {
                return false;
            }
            var cards = chooseTaregt(arg);
            if (cards.Count == 0)
            {
                return false;
            }
            arg.TargetCards = cards;
            var result = effect(arg);

            return result;
        }
        return Effect;
       
    }
}