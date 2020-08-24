using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ShopperAssets.Scripts.Game;
using ShopperAssets.Scripts.Interface.Game;
using UnityEngine;
using UnityEngine.Events;
using Zenject;
using Random = UnityEngine.Random;

public class AbilityPresenter
{
    private AbilityUseCase _abilityUseCase;
    private WaitForAbilityCondition _waitForAbilityCondition;
    private GamePresenter _gamePresenter;
    private GameView _gameView;
    public AbilityPresenter(AbilityUseCase abilityUseCase,
        WaitForAbilityCondition abilityCondition,
        GamePresenter gamePresenter,
        GameView gameView)
    {
        _abilityUseCase = abilityUseCase;
        _waitForAbilityCondition = abilityCondition;
        _gamePresenter = gamePresenter;
        _gameView = gameView;
    }

    public List<AbilityUseCase.AbilityCondition> GetConditions(AbilityUseCase.AbilityTiming now,List<ConditionModel> abilityModels)
    {
        return abilityModels.Where(_=>_.Timing == now).ToList().ConvertAll(_=>_.Condition);
    }
    
    public AbilityCommandResponseModel UseAbility(AbilityCommandRequestModel requestModelRef)
    {
        Debug.Log(requestModelRef.AbilityModel.Timing);
        if (requestModelRef.AbilityModel.Timing != requestModelRef.NowTiming)
        {
            return null;
        }
        _abilityUseCase.Commands.TryGetValue(requestModelRef.AbilityModel.Command, out var command);
        Debug.Log($" {requestModelRef.AbilityModel.Command}" );
        if (command == null)
        {//スルーして次いく
            Debug.Log("Command not found");
            return null;
        }
        return command.Invoke(requestModelRef);
    }
        //Await系
        public IEnumerator ResolveAbility(AbilityUseCase.AbilityTiming timing,CardModel cardModel,bool cancelable,Action OnCanceled)
        {
            if (cardModel.Abilities == null || cardModel.Abilities.Count == 0)
                yield break;
            
            yield return ResolveAbility(timing,(cardModel.GUID,GUIDType.Hand),cardModel.Conditions,cardModel.Abilities,true,OnCanceled);
        }

        public IEnumerator ResolveAbilityEnemy(AbilityUseCase.AbilityTiming timing, EnemyModel enemyModel,int index)
        {
            if (index >= enemyModel.Abilities.Count || index < 0)
                yield break;
            yield return ResolveAbility(timing, (enemyModel.GUID,GUIDType.Enemy),enemyModel.Abilities[index],false,null);
        }

        IEnumerator ResolveAbility(AbilityUseCase.AbilityTiming now, (string GUID, GUIDType Type) self,
            AbilityModel model, bool cancelable, Action OnCanceled)
        {
            var condition = model.Condition;
            var conditionRes = new AbilityConditionResponseModel();
            yield return _waitForAbilityCondition.Wait(new AbilityConditionRequestModel()
            {
                Condition = condition,
                Cancelable = cancelable
            });
            var res = _waitForAbilityCondition.Response;
            //緊急停止
            if (res == null || res.Canceled)
            {
                OnCanceled?.Invoke();
                yield break;
            }

            conditionRes = res;


            var nextModel = new AbilityCommandRequestModel()
                {
                    Self = self,
                    AbilityModel = model,
                    Response = conditionRes,
                    NowTiming = now,
                };
            yield return ResolveAbilityInternal(new List<AbilityCommandRequestModel>(){ nextModel },cancelable);
        }
        
        IEnumerator ResolveAbility(AbilityUseCase.AbilityTiming now,(string GUID,GUIDType Type) self,List<ConditionModel> conditionModels,List<AbilityModel> model,bool cancelable,Action OnCanceled)
        {
            var conditions = GetConditions(now,conditionModels);
            var conditionRes = new List<AbilityConditionResponseModel>();
            Debug.Log(conditions.Count);
            foreach (var abilityCondition in conditions)
            {
                yield return _waitForAbilityCondition.Wait(new AbilityConditionRequestModel()
                {
                    Condition = abilityCondition,
                    Cancelable =  cancelable
                });
                var res = _waitForAbilityCondition.Response;
                //緊急停止
                if (res == null || res.Canceled)
                {
                    OnCanceled?.Invoke();
                    yield break;
                }

                conditionRes.Add(res);
            }
            
            var nextModels = model.ConvertAll(c => new AbilityCommandRequestModel()
            {
                Self = self,
                AbilityModel = c,
                Response = conditionRes.Find(_=>_.Condition == c.Condition),
                NowTiming = now,
            });
            yield return ResolveAbilityInternal(nextModels.ToList(),cancelable);
        }

        IEnumerator ResolveAbilityInternal(List<AbilityCommandRequestModel> model,bool cancelable)
        {
            foreach (var abilityCommandRequestModel in model)
            {
                //処理
                var res = UseAbility(abilityCommandRequestModel);
                if (res == null)
                    continue;
                //演出
                _gamePresenter.UpdateUI();
                AbilityAnimation(res);
                //カードをゲットした場合、Getアビリティをチェック
                if (res.NextResolveAbility == null)
                    continue;
                foreach (var nextResolveAbility in res.NextResolveAbility)
                {
                    switch (nextResolveAbility.Next.GuidType)
                    {
                        case GUIDType.Hand:
                            var nextCardHand =
                                _gamePresenter.FindHand(nextResolveAbility.Next.GUID);
                            yield return ResolveAbility( nextResolveAbility.Timing,nextCardHand,false,null);
                            break;
                        case GUIDType.Enemy:
                            break;
                        case GUIDType.Trash:
                            var nextCardTrash =
                                _gamePresenter.FindTrash(nextResolveAbility.Next.GUID);
                            yield return ResolveAbility(nextResolveAbility.Timing, nextCardTrash, false, null);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
               
            }
        }
        //アニメーション系
        void AbilityAnimation(AbilityCommandResponseModel responseModel)
        {
            if (responseModel.AbilityPerformanceParams != null)
            {
                Debug.Log(
                    $"<color=orange>[Animation] Player {responseModel.AbilityPerformanceParams.PlayerAction}</color>");
                if (responseModel.AbilityPerformanceParams.EnemyAction != null)
                {
                    foreach (var valueTuple in responseModel.AbilityPerformanceParams.EnemyAction)
                    {
                        Debug.Log($"<color=orange>[Animation] Enemy {valueTuple.Item2}</color>");
                    }
                }
            }
        }
}
public class AbilityUseCase
{
    public enum AbilityTiming
    {
        Get = 1,//手札に入れた時
        Use = 2,//手札から使ったとき
        Trash= 3 ,//すてられたとき
        PlayerCounter = 4,//敵の行動に対するカウンター
        //敵
        EnemyCounter =101,//プレイヤーに対するカウンター
        EnemyTurn = 102,//敵の行動
        EndTurn  = 201,
    }
    public enum AbilityCondition
    {
        None = 0,
        ReduceCoin = 1,//コインを消費 コインがないときは無効化
        ReduceHp = 2,//HP消費 足りない時は無効
        SelectHandMust = 5,//所定枚数選ぶ
        SelectHandAny = 6,//好きな枚数選ぶ
        SelectHandLess = 7,//足りない場合はすべて
        
    }
    public enum AbilityCommands
    {
        //プラス効果
        //カードを得る
        GetCard = 1, //ショップから無償で得る
        //攻撃
        PlayerAttackEnemy = 100, // 攻撃力 param1 * max( 1 , condition) 範囲 param2 
        PlayerAttackAllEnemy = 101,//攻撃力 param1 * max( 1 , condition)  全体攻撃
        PlayerAttackCombo = 102,// 攻撃力 ( param1 ) 攻撃回数 param2 回 連続攻撃
        PlayerAttackComboFromCondition = 103,// 攻撃力 ( param1 ) 攻撃回数 condition 回 連続攻撃
        PlayerAttackDeckNum = 104, //山札の枚数だけダメージ増加
        PlayerAttackRangeEnemyFromCondition = 105,// 攻撃力 param1 範囲 condition
        
        //コイン入手
        Coin = 200, //コイン入手　param1　枚
        CoinFromCondition = 201, //コイン入手　condition 枚
        //引く
        Draw = 300,//手札引く param1 枚
        DrawFromCondition  = 301,//手札引く condition 枚
        //捨てる
        Drop = 400, //手札捨てる param1 枚
        DropFromCondition = 401, //手札捨てる condition 枚
        DropAllHand = 402,//手札をすべて捨てる
        //バフ
        BuffTurn = 500,//このターンの攻撃力を param1 * max( 1 , condition) 上げる 連続攻撃向き
        BuffNext = 501, //次の攻撃の攻撃力を param1 * max( 1 , condition) 上げる 一撃向き
        //シールド
        Wall = 600,//
        WallFromCondition = 601,//param1 * condition ダメージを一定量無効化
        //バリア
        Barrier = 700,//次の攻撃を無効化
        //気絶
        Stun = 800, //敵を気絶させる
        StunAll = 801, //敵を気絶させる
        //回復
        Heal = 900,//param1 * max( 1 , condition) 回復
        //射程
        AddRange = 1000,//射程を伸ばす
        AttackAllNext = 1001,//次の攻撃の射程をのばす
        //ふきとばし
        Warp = 1100,//ふきとばし
        //経験値
        AddExp=1200,//param1 * max( 1 , condition) 経験値上昇
        //除去
        RemoveHand = 1300,//手札があればparam1枚破棄する
        
        //アクションカード 手札にあるとき攻撃以外のアビリティを受けると、効果発動
        Revive = 7001, //生き返り
        
        //マイナス効果
       
      


        //敵側効果
        EnemyAttackPlayer = 10001, //プレイヤーを攻撃
        Curse = 10002, //呪いを得る
        //DropFromEnemy = 1003, //param1枚すてさせる
        BuffEnemy = 10004, //前方の敵を強化
        BarrierEnemy = 10005, //次の攻撃を無効化
        WallEnemy = 10006, //一定量無力化
        EnemyChargeAttack = 10007,//1ターン後に攻撃する プレイヤーに防御を促す
        EnemyAttackCombo = 10008,//連続攻撃 ブロック対策
        //その他
        RemoveThis = 20001, //このアビリティを使った段階で手札から除外
        TurnEnd = 20002,//使った瞬間ターン終了
        LuckySlot = 20003, //手札を好きな数除外して1/8で除外した枚数*7ダメージ
        Hatena = 20004//何が起きるかわからない 1.全体に999ダメージ 2.ラッキースロットゲット 3.全回復 4.レベルアップ 5.全体に10ダメージ 6.自分に10ダメージ 7.手札を全捨て
    }

    void InitCommands()
    {
        Commands =
            new Dictionary<AbilityCommands, Func<AbilityCommandRequestModel, AbilityCommandResponseModel>>()
            {
//プラス効果
//カードを得る
                {AbilityCommands.GetCard, _abilityImpliment.GetCard}, //ショップから無償で得る
//攻撃
                {
                    AbilityCommands.PlayerAttackEnemy, _abilityImpliment.PlayerAttackEnemy
                }, // 攻撃力 param1 * max( 1 , condition) 範囲 param2 
                {
                    AbilityCommands.PlayerAttackAllEnemy, _abilityImpliment.PlayerAttackAllEnemy
                }, //攻撃力 param1 * max( 1 , condition)  全体攻撃
                {
                    AbilityCommands.PlayerAttackCombo, _abilityImpliment.PlayerAttackCombo
                }, // 攻撃力 ( param1 ) 攻撃回数 param2 回 連続攻撃
                {
                    AbilityCommands.PlayerAttackComboFromCondition, _abilityImpliment.PlayerAttackComboFromCondition
                }, // 攻撃力 ( param1 ) 攻撃回数 condition 回 連続攻撃
                {AbilityCommands.PlayerAttackDeckNum, _abilityImpliment.PlayerAttackDeckNum}, //山札の枚数だけダメージ増加
                {
                    AbilityCommands.PlayerAttackRangeEnemyFromCondition,
                    _abilityImpliment.PlayerAttackRangeEnemyFromCondition
                }, // 攻撃力 param1 範囲 condition

//コイン入手
                {AbilityCommands.Coin, _abilityImpliment.Coin}, //コイン入手　param1　枚
                {AbilityCommands.CoinFromCondition, _abilityImpliment.CoinFromCondition}, //コイン入手　condition 枚
//引く
                {AbilityCommands.Draw, _abilityImpliment.Draw}, //手札引く param1 枚
                {AbilityCommands.DrawFromCondition, _abilityImpliment.DrawFromCondition}, //手札引く condition 枚
//捨てる
                {AbilityCommands.Drop, _abilityImpliment.Drop}, //手札捨てる param1 枚
                {AbilityCommands.DropFromCondition, _abilityImpliment.DropFromCondition}, //手札捨てる condition 枚
                {AbilityCommands.DropAllHand, _abilityImpliment.DropAllHand}, //手札をすべて捨てる
//バフ
                {
                    AbilityCommands.BuffTurn, _abilityImpliment.BuffTurn
                }, //このターンの攻撃力を param1 * max( 1 , condition) 上げる 連続攻撃向き
                {
                    AbilityCommands.BuffNext, _abilityImpliment.BuffNext
                }, //次の攻撃の攻撃力を param1 * max( 1 , condition) 上げる 一撃向き
//シールド
                {AbilityCommands.Wall, _abilityImpliment.Wall}, //
                {
                    AbilityCommands.WallFromCondition, _abilityImpliment.WallFromCondition
                }, //param1 * condition ダメージを一定量無効化
//バリア
                {AbilityCommands.Barrier, _abilityImpliment.Barrier}, //次の攻撃を無効化
//気絶
                {AbilityCommands.Stun, _abilityImpliment.Stun}, //敵を気絶させる
                {AbilityCommands.StunAll, _abilityImpliment.StunAll}, //敵を気絶させる
//回復
                {AbilityCommands.Heal, _abilityImpliment.Heal}, //param1 * max( 1 , condition) 回復
//射程
                {AbilityCommands.AddRange, _abilityImpliment.AddRange}, //射程を伸ばす
                {AbilityCommands.AttackAllNext, _abilityImpliment.AttackAllNext}, //次の攻撃の射程をのばす
//ふきとばし
                {AbilityCommands.Warp, _abilityImpliment.Warp}, //ふきとばし
//経験値
                {AbilityCommands.AddExp, _abilityImpliment.AddExp}, //param1 * max( 1 , condition) 経験値上昇
//除去
                {AbilityCommands.RemoveHand, _abilityImpliment.RemoveHand}, //手札があればparam1枚破棄する

//アクションカード 手札にあるとき攻撃以外のアビリティを受けると、効果発動
                {AbilityCommands.Revive, _abilityImpliment.Revive}, //生き返り

//マイナス効果


//敵側効果
                {AbilityCommands.EnemyAttackPlayer, _abilityImpliment.EnemyAttackPlayer}, //プレイヤーを攻撃
                {AbilityCommands.Curse, _abilityImpliment.Curse}, //呪いを得る
//DropFromEnemy = 1003, //param1枚すてさせる
                {AbilityCommands.BuffEnemy, _abilityImpliment.BuffEnemy}, //前方の敵を強化
                {AbilityCommands.BarrierEnemy, _abilityImpliment.BarrierEnemy}, //次の攻撃を無効化
                {AbilityCommands.WallEnemy, _abilityImpliment.WallEnemy}, //一定量無力化
                {AbilityCommands.EnemyChargeAttack, _abilityImpliment.EnemyChargeAttack}, //1ターン後に攻撃する プレイヤーに防御を促す
                {AbilityCommands.EnemyAttackCombo, _abilityImpliment.EnemyAttackCombo}, //連続攻撃 ブロック対策
//その他
                {AbilityCommands.RemoveThis, _abilityImpliment.RemoveThis}, //このアビリティを使った段階で手札から除外
                {AbilityCommands.TurnEnd, _abilityImpliment.TurnEnd}, //使った瞬間ターン終了
                {AbilityCommands.LuckySlot, _abilityImpliment.LuckySlot}, //手札を好きな数除外して1/8で除外した枚数*7ダメージ
                {
                    AbilityCommands.Hatena, _abilityImpliment.Hatena
                } //何が起きるかわからない 1.全体に999ダメージ 2.ラッキースロットゲット 3.全回復 4.レベルアップ 5.全体に10ダメージ 6.自分に10ダメージ 7.手札を全捨て


            };
    }

    private AbilityImpliment _abilityImpliment;
    private IShopUsecase _shopUseCase = default;
    private IEnemyUsecase _enemyUseCase = default;
    private IPlayerUsecase _playerUseCase = default;
    public Dictionary<AbilityCommands, Func<AbilityCommandRequestModel,AbilityCommandResponseModel>> Commands { get; private set; }


    public AbilityUseCase(
        IShopUsecase shopUseCase,
        IEnemyUsecase enemyUseCase,
        IPlayerUsecase playerUseCase)
    {
        Debug.Log("[AbilityResolver] Init");
        _shopUseCase = shopUseCase;
        _enemyUseCase = enemyUseCase;
        _playerUseCase = playerUseCase;
        _abilityImpliment = new AbilityImpliment(playerUseCase,enemyUseCase,shopUseCase);
        InitCommands();
    }

}
