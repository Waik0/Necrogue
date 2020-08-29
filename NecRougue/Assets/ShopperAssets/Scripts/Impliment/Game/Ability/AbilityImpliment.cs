using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ShopperAssets.Scripts.Interface.Game;
using UnityEngine;

public class AbilityImpliment
{
    private IShopUsecase _shopUseCase = default;
    private IEnemyUsecase _enemyUseCase = default;
    private IPlayerUsecase _playerUseCase = default;

    public AbilityImpliment(
        IPlayerUsecase playerUseCase,
        IEnemyUsecase enemyUsecase,
        IShopUsecase shopUsecase)
    {
        _shopUseCase = shopUsecase;
        _enemyUseCase = enemyUsecase;
        _playerUseCase = playerUseCase;
    }
    
    //プラス効果
//カードを得る
    public AbilityCommandResponseModel GetCard(AbilityCommandRequestModel req)
    {
        //ランダム購入
        if (_shopUseCase.Goods.Count <= 0)
        {
            return null;
        }

        var id = _shopUseCase.Goods[Random.Range(0, _shopUseCase.Goods.Count - 1)].GUID;
        var goods = _shopUseCase.Buy(id);
        if (goods != null)
        {
            _playerUseCase.AddHand(goods);
            //todo ↑のGetAbilityを発動させる
            return new AbilityCommandResponseModel()
            {
                NextResolveAbility = new List<NextResolveAbility>(){
                    new NextResolveAbility()
                    {
                        Next = (goods.GUID, GUIDType.Hand),
                        Timing = AbilityUseCase.AbilityTiming.Get,
                    }
                }
            };
        }

        return null;
    }

//攻撃
// 攻撃力 param1 * max( 1 , condition) 範囲 param2 攻撃回数 param3 回
    public AbilityCommandResponseModel PlayerAttackEnemy(AbilityCommandRequestModel req)
    {
        var animation = new List<AbilityPerformanceParams>();
        var length = req.Response?.Targets.GUIDs?.Length ?? 0;
        var num = Mathf.Max(1, req.AbilityModel.AbilityParam3);
        for (int i = 0; i < num; i++)
        {
            var actions = new List<(string, int)>();
            var damages = new List<(string, int)>();
            for (int j = 0; j < req.AbilityModel.AbilityParam2; j++)
            {
                var dmg = _enemyUseCase.Damage(j,
                    req.AbilityModel.AbilityParam1 * Mathf.Max(1, length));
                actions.Add((_enemyUseCase.GetGUIDFromIndex(j),req.AbilityModel.EnemyMotionId));
                damages.Add((_enemyUseCase.GetGUIDFromIndex(j),dmg));
            }

            animation.Add(new AbilityPerformanceParams()
            {
                PlayerAction = req.AbilityModel.PlayerMotionId,
                EnemyAction = actions,
                EnemyDamage = damages
            });
        }
       
        return new AbilityCommandResponseModel()
        {
            AbilityPerformanceParams = animation
        };
    }


// 攻撃力 ( param1 ) 攻撃回数 condition 回 連続攻撃   
    public AbilityCommandResponseModel PlayerAttackComboFromCondition(AbilityCommandRequestModel req)
    {
        return null;
    }

//山札の枚数だけダメージ増加   
    public AbilityCommandResponseModel PlayerAttackDeckNum(AbilityCommandRequestModel req)
    {
        return null;
    }

// 攻撃力 param1 範囲 condition   
    public AbilityCommandResponseModel PlayerAttackRangeEnemyFromCondition(AbilityCommandRequestModel req)
    {
        return null;
    }

//コイン入手
//コイン入手　param1　枚   
    public AbilityCommandResponseModel Coin(AbilityCommandRequestModel req)
    {
        _playerUseCase.AddCoin(req.AbilityModel.AbilityParam1);
        return new AbilityCommandResponseModel()
        {
        };
    }

//コイン入手　condition 枚   
    public AbilityCommandResponseModel CoinFromCondition(AbilityCommandRequestModel req)
    {
        Debug.Log(req.Response?.Targets.GUIDs.Length);
        if(req.Response?.Targets.GUIDs != null) 
            _playerUseCase.AddCoin(req.Response.Targets.GUIDs.Length);
        return null;
    }

//引く
//手札引く param1 枚   
    public AbilityCommandResponseModel Draw(AbilityCommandRequestModel req)
    {
        for (var i = 0; i < req.AbilityModel.AbilityParam1; i++)
        {
            if (_playerUseCase.Draw() == null)
                if (!_playerUseCase.TrashToDeckAll())
                    break;
        }

        return null;
    }

//手札引く condition 枚   
    public AbilityCommandResponseModel DrawFromCondition(AbilityCommandRequestModel req)
    {
        if(req.Response?.Targets.GUIDs != null) 
            for (var i = 0; i < req.Response.Targets.GUIDs.Length; i++)
            {
                if (_playerUseCase.Draw() == null)
                    if (!_playerUseCase.TrashToDeckAll())
                        break;
            }
        return null;
    }

//捨てる
//手札捨てる param1 枚   
    public AbilityCommandResponseModel Drop(AbilityCommandRequestModel req)
    {
        var trashes = new List<string>();
        for (var i = 0; i < req.AbilityModel.AbilityParam1; i++)
        {
            var c = _playerUseCase.DropHandRandom();
            if (c != null)
            {
                trashes.Add(c.Name);
            }
        }

        return new AbilityCommandResponseModel()
        {
            NextResolveAbility = trashes.ConvertAll(_ => new NextResolveAbility()
            {
                Next = (_, GUIDType.Trash),
                Timing = AbilityUseCase.AbilityTiming.Trash
            })
        };
        return null;
    }

//手札捨てる condition 枚   
    public AbilityCommandResponseModel DropFromCondition(AbilityCommandRequestModel req)
    {
        if (req.Response?.Targets.GUIDs == null)
            return null;
        foreach (var targetsGuiD in req.Response.Targets.GUIDs)
        {
            _playerUseCase.DropHand(targetsGuiD);
        }

        return new AbilityCommandResponseModel()
        {
            NextResolveAbility = req.Response?.Targets.GUIDs.ToList().ConvertAll(_ => new NextResolveAbility()
            {
                Next = (_, GUIDType.Trash),
                Timing = AbilityUseCase.AbilityTiming.Trash
            })
        };
    }

//手札をすべて捨てる   
    public AbilityCommandResponseModel DropAllHand(AbilityCommandRequestModel req)
    {
        return null;
    }

//バフ
//このターンの攻撃力を param1 * max( 1 , condition) 上げる 連続攻撃向き   
    public AbilityCommandResponseModel BuffTurn(AbilityCommandRequestModel req)
    {
        var length = req.Response?.Targets.GUIDs?.Length ?? 0;
        _playerUseCase.AddAttackByTurn(req.AbilityModel.AbilityParam1 * Mathf.Max(1,length));
        return null;
    }

//次の攻撃の攻撃力を param1 * max( 1 , condition) 上げる 一撃向き   
    public AbilityCommandResponseModel BuffNext(AbilityCommandRequestModel req)
    {
        return null;
    }

//シールド
//   
    public AbilityCommandResponseModel Wall(AbilityCommandRequestModel req)
    {
        //param1 上昇
        _playerUseCase.AddWall(req.AbilityModel.AbilityParam1);
        //選択数上昇
        if (req.Response != null && req.Response.Targets.GUIDs != null)
        {
            _playerUseCase.AddWall(req.Response.Targets.GUIDs.Length);
        }

        return new AbilityCommandResponseModel()
        {
            AbilityPerformanceParams = new List<AbilityPerformanceParams>(){new AbilityPerformanceParams()
            {
                PlayerAction = req.AbilityModel.PlayerMotionId
            }}
        };
    }

//param1 * condition ダメージを一定量無効化   
    public AbilityCommandResponseModel WallFromCondition(AbilityCommandRequestModel req)
    {
        var length = req.Response?.Targets.GUIDs?.Length ?? 0;
        var wall = Mathf.Max(1, req.AbilityModel.AbilityParam1) * length;
        _playerUseCase.AddWall(wall);
        return null;
    }

//バリア
//次の攻撃を無効化   
    public AbilityCommandResponseModel Barrier(AbilityCommandRequestModel req)
    {
        _playerUseCase.AddBarrier();

        return new AbilityCommandResponseModel()
        {
            AbilityPerformanceParams = new List<AbilityPerformanceParams>(){new AbilityPerformanceParams()
            {
                PlayerAction = req.AbilityModel.PlayerMotionId
            }}
        };
    }

//気絶
//敵を気絶させる   
    public AbilityCommandResponseModel Stun(AbilityCommandRequestModel req)
    {
        var stunEnemies = new List<string>();
        if (req.AbilityModel.AbilityParam2 >= 0)
        {
            var canStun = _enemyUseCase.Stun(req.AbilityModel.AbilityParam2);
            if (canStun)
            {
                stunEnemies.Add(_enemyUseCase.GetGUIDFromIndex(req.AbilityModel.AbilityParam2));
            }
        }

        if (req.Response != null && req.Response.Targets.GUIDs != null)
        {
            foreach (var targetsGuiD in req.Response.Targets.GUIDs)
            {
                var canStun = _enemyUseCase.Stun(targetsGuiD);
                if (canStun)
                {
                    stunEnemies.Add(targetsGuiD);
                }
            }
        }

        if (stunEnemies.Count < 1)
        {
            return null;
        }

        return new AbilityCommandResponseModel()
        {
            AbilityPerformanceParams = new List<AbilityPerformanceParams>(){new AbilityPerformanceParams()
            {
                EnemyAction = stunEnemies.ConvertAll(_ => (_, req.AbilityModel.EnemyMotionId)).ToList(),
                PlayerAction = req.AbilityModel.PlayerMotionId
            }}
        };
    }

//敵を気絶させる   
    public AbilityCommandResponseModel StunAll(AbilityCommandRequestModel req)
    {
        return null;
    }

//回復
//param1 * max( 1 , condition) 回復   
    public AbilityCommandResponseModel Heal(AbilityCommandRequestModel req)
    {
        var length = req.Response?.Targets.GUIDs?.Length ?? 0;
        _playerUseCase.Heal(req.AbilityModel.AbilityParam1 * Mathf.Max(1,length));
        return null;
    }

//射程
//射程を伸ばす param1 * max( 1 , condition) 
    public AbilityCommandResponseModel AddRange(AbilityCommandRequestModel req)
    {
        var length = req.Response?.Targets.GUIDs?.Length ?? 0;
        _playerUseCase.AddRange(req.AbilityModel.AbilityParam1* Mathf.Max(1,length));
        return null;
    }

//次の攻撃の射程をのばす   
    public AbilityCommandResponseModel AttackAllNext(AbilityCommandRequestModel req)
    {
        return null;
    }

//ふきとばし
//ふきとばし   
    public AbilityCommandResponseModel Warp(AbilityCommandRequestModel req)
    {
        return null;
    }

//経験値
//param1 * max( 1 , condition) 経験値上昇   
    public AbilityCommandResponseModel AddExp(AbilityCommandRequestModel req)
    {
        return null;
    }

//除去
//手札があればparam1枚破棄する   
    public AbilityCommandResponseModel RemoveHand(AbilityCommandRequestModel req)
    {
        return null;
    }

//アクションカード 手札にあるとき攻撃以外のアビリティを受けると、効果発動
//生き返り   
    public AbilityCommandResponseModel Revive(AbilityCommandRequestModel req)
    {
        return null;
    }

//マイナス効果


//敵側効果
//プレイヤーを攻撃   
    public AbilityCommandResponseModel EnemyAttackPlayer(AbilityCommandRequestModel req)
    {
        if (_enemyUseCase.GetFieldOwnIndex(req.Self.GUID) == req.AbilityModel.AbilityParam2)
            _playerUseCase.Damage(req.AbilityModel.AbilityParam1);
        return null;
    }

//呪いを得る   
    public AbilityCommandResponseModel Curse(AbilityCommandRequestModel req)
    {
        return null;
    }

//DropFromEnemy = 1003, //param1枚すてさせる
    public AbilityCommandResponseModel DropFromEnemy(AbilityCommandRequestModel req)
    {
        var trashes = new List<string>();
        for (var i = 0; i < req.AbilityModel.AbilityParam1; i++)
        {
            var c = _playerUseCase.DropHandRandom();
            if (c != null)
            {
                trashes.Add(c.Name);
            }
        }

        return new AbilityCommandResponseModel()
        {
            NextResolveAbility = trashes.ConvertAll(_ => new NextResolveAbility()
            {
                Next = (_, GUIDType.Trash),
                Timing = AbilityUseCase.AbilityTiming.Trash
            })
        };
    }
//前方の敵を強化   
    public AbilityCommandResponseModel BuffEnemy(AbilityCommandRequestModel req)
    {
        //req.AbilityModel.AbilityParam1
        return null;
    }

//次の攻撃を無効化   
    public AbilityCommandResponseModel BarrierEnemy(AbilityCommandRequestModel req)
    {
        return null;
    }

//一定量無力化   
    public AbilityCommandResponseModel WallEnemy(AbilityCommandRequestModel req)
    {
        return null;
    }

//1ターン後に攻撃する プレイヤーに防御を促す   
    public AbilityCommandResponseModel EnemyChargeAttack(AbilityCommandRequestModel req)
    {
        return null;
    }

//連続攻撃 ブロック対策   
    public AbilityCommandResponseModel EnemyAttackCombo(AbilityCommandRequestModel req)
    {
        return null;
    }

//その他
//このアビリティを使った段階で手札から除外   
    public AbilityCommandResponseModel RemoveThis(AbilityCommandRequestModel req)
    {
        return null;
    }

//使った瞬間ターン終了   
    public AbilityCommandResponseModel TurnEnd(AbilityCommandRequestModel req)
    {
        return null;
    }

//手札を好きな数除外して1/8で除外した枚数*7ダメージ   
    public AbilityCommandResponseModel LuckySlot(AbilityCommandRequestModel req)
    {
        return null;
    }

//何が起きるかわからない 1.全体に999ダメージ 2.ラッキースロットゲット 3.全回復 4.レベルアップ 5.全体に10ダメージ 6.自分に10ダメージ 7.手札を全捨て   
    public AbilityCommandResponseModel Hatena(AbilityCommandRequestModel req)
    {
        return null;
    }
}
