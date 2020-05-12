using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class DeepCopyExtensions
{
    public static T DeepCopy<T>(this T src) where T : IDeepCopyable
    {
        using (var memoryStream = new MemoryStream())
        {
            var binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(memoryStream, src);
            memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
            return (T)binaryFormatter.Deserialize(memoryStream);
        }
    }
}
//----------------------------------------------------------------------------------------------------------------------
//Common
//----------------------------------------------------------------------------------------------------------------------
public class PlayerData : IEntity,
    IConvertFromMasterRecord<MstCharacterRecord>,
    IGenerateFromMasterRecord<PlayerData,MstCharacterRecord>
{
    public int CharaId;
    public int Gold;
    public int ShopLevel;
    public List<CardData> Deck;//デッキ
    public List<CardData> Stock;//デッキに入れていないカード
    public void Convert(MstCharacterRecord record)
    {
        CharaId = record.Id;
        Deck = new List<CardData>();
        Stock = new List<CardData>();
        Gold = 0;
        ShopLevel = 1;
        var cards = new[]
        {
            record.MonsterId1,
            record.MonsterId2,
            record.MonsterId3,
            record.MonsterId4,
            record.MonsterId5,
            record.MonsterId6,
            record.MonsterId7,
            record.MonsterId8,
        };
        foreach (var card in cards)
        {
            if (card <= 0)
            {
                continue;
            }
            var mst = MasterdataManager.Get<MstMonsterRecord>(card);
            if (mst == null)
            {
                continue;
                
            }
            Deck.Add(new CardData().Generate(mst));
        }
    }

    public PlayerData Generate(MstCharacterRecord record)
    {
        Convert(record);
        return this;
    }
}
//道中で最大値を上げたりしたときのために値を取っておく
public class CardData : IEntity,
    IConvertFromMasterRecord<MstMonsterRecord>,
    IConvertFromEntity<BattleCard>,
    IGenerateFromMasterRecord<CardData,MstMonsterRecord>,
    IGenerateFromEntity<CardData,BattleCard>
   
{
    public int Id;
    public string Name;
    public int Rarity;
    public int Attack;
    public int Hp;
    public int Defence;
    public List<int> Abilities;
    public int Level;

    public CardData Generate(MstMonsterRecord record)
    {
        Convert(record);
        return this;
    }
    public CardData Generate(BattleCard entity)
    {
        Convert(entity);
        return this;
    }
    public void Convert(MstMonsterRecord record)
    {
        Id = record.id;
        Name = record.name;
        Attack = record.attack;
        Hp = record.hp;
        Defence = record.defence;
        Level = record.level;
        Rarity = record.rarity;
        Abilities = new List<int>()
        {
            record.abilityId1,
            record.abilityId2,
            record.abilityId3,
            record.abilityId4
        };
    }
    public void Convert(BattleCard entity)
    {
        Id = entity.Id;
        Name = entity.Name;
        Attack = entity.Attack.Max;
        Hp = entity.Hp.Max;
        Defence = entity.Defence.Max;
        Level = entity.Level;
        Rarity = entity.Rarity;
        Abilities = entity.AbilityList.ConvertAll(a => a.Id);

    }
}
//選択されたマップの敵データ
public class EnemyData : IEntity
{
    public int Id;
}
//----------------------------------------------------------------------------------------------------------------------
//Map
//----------------------------------------------------------------------------------------------------------------------


public class MapData : IEntity
{
    public int Depth;
    public int CurrentSerialNumber;
    public List<MapNode> Nodes;
}
public class MapNode : IEntity,IConvertFromMasterRecord<MstMapNodeRecord>
{
    public int NodeId;//マスターから引っ張る
    public int SerialNumber;//固有の番号
    public int Depth;
    public MapType MapType;
    public MapOption MapOption;
    public int EnemyId;//マスターから引っ張る
    
    
    public List<int> LinkNodeSerial;//つながっているノード

    public void Convert(MstMapNodeRecord record)
    {
        NodeId = record.id;
        MapType = record.type;
        MapOption = record.option1 | record.option2 | record.option3;
        EnemyId = record.enemyId;
    }
}
//----------------------------------------------------------------------------------------------------------------------
//Battle
//----------------------------------------------------------------------------------------------------------------------
[Serializable]
public class BattleData : IEntity,
    IDeepCopyable
{

    public int Turn;
    public int CurrentAttacker;
    public int CurrentDefencer;
    public bool IsEnd;
    public PlayerType Winner;
    public List<BattlePlayerData> PlayerList;
    public BattleState State;

}
[Serializable]
public class BattlePlayerData : IEntity,
    IConvertFromEntity<PlayerData>,
    IConvertFromMasterRecord<MstEnemyRecord>,
    IGenerateFromEntity<BattlePlayerData,PlayerData>,
    IGenerateFromMasterRecord<BattlePlayerData,MstEnemyRecord>

{
    public PlayerType PlayerType;
    public string Name;
    public int CharaId;
    public int AttackerIndex;
    public List<BattleCard> Deck;// = new List<BattleCard>();//モンスターカード 順番も関係ある
    public List<BattleCard> Stock;//操作キャラ以外はnull
    public int Speed;
    //public bool IsAttackIndexOut => AttackerIndex >= Deck.Count;
    public void ResetAttackIndex()
    {
        AttackerIndex = 0;
    }

    public void Convert(PlayerData entity)
    {
        PlayerType = PlayerType.Player;
        CharaId = entity.CharaId;
        Deck = entity.Deck.ConvertAll(card => new BattleCard().Generate(card));
        Stock = entity.Stock.ConvertAll(card => new BattleCard().Generate(card));
        AttackerIndex = 0;
        
    }

    public void Convert(MstEnemyRecord record)
    {
        PlayerType = PlayerType.Enemy;
        CharaId = record.id;
        Deck = new List<int>
        {
            record.MonsterId1,
            record.MonsterId2,
            record.MonsterId3,
            record.MonsterId4,
            record.MonsterId5,
            record.MonsterId6,
            record.MonsterId7,
            record.MonsterId8,
        }.Where(id =>
            {
                var mst = MasterdataManager.Get<MstMonsterRecord>(id);
                return mst != null;
            })
            .ToList().ConvertAll(id =>
        {
            var mst = MasterdataManager.Get<MstMonsterRecord>(id);
            return new BattleCard().Generate(mst);
        });
    }

    public BattlePlayerData Generate(PlayerData entity)
    {
        Convert(entity);
        return this;
    }

    public BattlePlayerData Generate(MstEnemyRecord record)
    {
        Convert(record);
        return this;
    }
}
[Serializable]
public class BattleCard :IEntity,
    IConvertFromEntity<CardData>,
    IGenerateFromEntity<BattleCard,CardData>,
    IConvertFromMasterRecord<MstMonsterRecord>,
    IGenerateFromMasterRecord<BattleCard,MstMonsterRecord>
{
    public int Id;//マスターから素材引っ張る用
    public string Name;
    public int Rarity;
    public int Level;
    public ValueSet Attack;
    public ValueSet Hp;
    public ValueSet Defence;
    public ValueSet AttackPriolity;
    public List<Ability> AbilityList = new List<Ability>();//マスターから素材ひっぱったり AbilityEffectsから効果ひっぱったり
    public List<Disease> DiseaseList = new List<Disease>();//状態異常
    public bool UseAbilityBefore;
    //view用
    public BattleCardState State;
    public void Convert(CardData entity)
    {
        Id = entity.Id;
        Name = entity.Name;
        Level = entity.Level;
        Rarity = entity.Rarity;
        Hp = new ValueSet(entity.Hp);
        Attack = new ValueSet(entity.Attack);
        Defence = new ValueSet(entity.Defence);
        AttackPriolity = new ValueSet(0);
        AbilityList = entity.Abilities.Where(id =>
        {
            var mst = MasterdataManager.Get<MstAbilityRecord>(id);
            return mst != null;
        }).ToList().ConvertAll(id =>
        {
            var mst = MasterdataManager.Get<MstAbilityRecord>(id);
            return new Ability().Generate(mst);
        });
        DiseaseList = new List<Disease>();//todo
    }


    public void Convert(MstMonsterRecord record)
    {
        Id = record.id;
        Name = record.name;
        Level = record.level;
        Rarity = record.rarity;
        Hp = new ValueSet(record.hp);
        Attack = new ValueSet(record.attack);
        Defence = new ValueSet(record.defence);
        AttackPriolity = new ValueSet(0);
        AbilityList = new List<int>()
        {
            record.abilityId1,
            record.abilityId2,
            record.abilityId3,
            record.abilityId4
        }.Where(id =>
        {
            var mst = MasterdataManager.Get<MstAbilityRecord>(id);
            return mst != null;
        }).ToList().ConvertAll(id =>
        {
            var mst = MasterdataManager.Get<MstAbilityRecord>(id);
            return new Ability().Generate(mst);
        });
        DiseaseList = new List<Disease>();//todo
    }
    public BattleCard Generate(CardData entity)
    {
        Convert(entity);
        return this;
    }

    public BattleCard Generate(MstMonsterRecord record)
    {
        Convert(record);
        return this;
    }
}
[Serializable]
public class ValueSet
{
    public ValueSet(int num)
    {
        Max = num;
        Current = num;
    }
    public int Max;
    public int Current;

}
[Serializable]
public class Disease
{
    public int Id;//DiseaseEffectsから効果ひっぱったり
    public int TurnLeft;//残りターン
   // public int Damage;//ダメージ
    
}
[Serializable]

public class Ability : IConvertFromMasterRecord<MstAbilityRecord>,
    IGenerateFromMasterRecord<Ability,MstAbilityRecord>
{
    public int Id;//AbilityEffectsから効果ひっぱったり
    public string Name;
    public Func<AbilityEffectsArgument,bool> Effect;
    //public AbilityTimingType TimingType;
    public void Convert(MstAbilityRecord record)
    {
        Name = record.name;
        Id = record.id;
        Effect = AbilityEffects.GetEffect(Id);
    //    TimingType = record.timingType;
    }

    public Ability Generate(MstAbilityRecord record)
    {
        Convert(record);
        return this;
    }
}


public class BattleCommand  : IEntity,
    IGenerateFromEntity<BattleCommand,BattleData>
{
    //public string Text;
    public BattleData SnapShot;

    public BattleCommand Generate(BattleData entity)
    {
        SnapShot = entity;
        return this;
    }
}
