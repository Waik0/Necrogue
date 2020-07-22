using System;
using System.Linq;
using System.Collections.Generic;
using ShopperAssets.Scripts.Master;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

namespace ShopperAssets.Scripts.Game
{

    public class PlayerModel
    {
        public List<CardModel> Deck = new List<CardModel>();
        public List<CardModel> Hand = new List<CardModel>();
        public List<CardModel> Trash = new List<CardModel>();
        public List<CardModel> Removed = new List<CardModel>();
        public CharacterModel playerCharacter;
        public int Coin;
        public int HandMax;
       
        public void Create()
        {
            playerCharacter = new CharacterModel()
            {
                Hp = 0,
                Attack = 0,
                Defence = 0
            };
            Deck.Clear();
            Coin = 0;
        }
        
    }

    public class CardModel: IConvertFromMasterRecord<ShMstCardRecord>,IGenerateFromMasterRecord<CardModel,ShMstCardRecord>

    {
        public List<AbilityModel> Abilities;
        public int Id;
        public int Rank;
        public int Price;
        public int Rarity;
        public string Name;
        public string Description;
        public string GUID;
        public void Convert(ShMstCardRecord record)
        {
            GUID = Guid.NewGuid().ToString();
            Id = record.id;
            Name = record.Name;
            Description = record.Description;
            Abilities = new[]
            {
                record.AbilityId1,
                record.AbilityId2,
                record.AbilityId3,
                record.AbilityId4,
            }.ToList().ConvertAll<AbilityModel>(id =>
                new AbilityModel().Generate(MasterdataManager.Get<ShMstAbilityRecord>(id)));
            var param = new[]
            {
                record.AbilityParam1_1,
                record.AbilityParam1_2,
                record.AbilityParam2_1,
                record.AbilityParam2_2,
                record.AbilityParam3_1,
                record.AbilityParam3_2,
                record.AbilityParam4_1,
                record.AbilityParam4_2,

            };
            for (var i = 0; i < Abilities.Count; i++)
            {
                Abilities[i].AbilityParam1 = param[i * 2];
                Abilities[i].AbilityParam2 = param[i * 2 + 1];
            }
        }

        public CardModel Generate(ShMstCardRecord record)
        {
            Convert(record);
            return this;
        }
    }
    public class ShopModel
    {
        public int Level;
        public int GoodsNum = 2;
        public List<CardModel> Deck = new List<CardModel>();
        public List<CardModel> Goods = new List<CardModel>();
        public void Create()
        {
            var goods = MasterdataManager.Records<ShMstCardRecord>();
            List<ShMstEnemyRecord> _candidate;
            Deck.Clear();
            Goods.Clear();
            Level = 0;
            GoodsNum = 2;
        }

        public void UpdateGoods()
        {
            while (Goods.Count < Level + 2 && Deck.Count > 0)
            {
                var pop = Deck[0];
                Deck.RemoveAt(0);
                Goods.Add(pop);
            }
        }
        
    }

    public class EnemyDeckModel
    {
        public List<EnemyModel> Deck = new List<EnemyModel>();
        public List<EnemyModel> Field = new List<EnemyModel>();
        public int Level;
        public int EnemyCount = 0;
        private List<int> RankCounts = new List<int>()
        {
            5,
            8,
            8,
            4,
            1
        };
        public void Create()
        {
            var enemies = MasterdataManager.Records<ShMstEnemyRecord>();
            List<ShMstEnemyRecord> _candidate;
            var targetRank = 0;
            foreach (var c in RankCounts)
            {
                _candidate = enemies.Where(_ => _.Rank == targetRank).ToList();
                for (int i = 0; i < _candidate.Count && i < c; i++)
                {
                    if (_candidate.Count > 0)
                    {
                        var target = _candidate[UnityEngine.Random.Range(0, _candidate.Count)];
                        Deck.Add(new EnemyModel().Generate(target));
                    }
                }
               

                targetRank++;
            }

            for (int i = 0; i < EnemyCount; i++)
            {
                Field.Add(null);
            }
        }
        public void Shuffle()
        {
            
        }
    }
    public class EnemyModel: IConvertFromMasterRecord<ShMstEnemyRecord>,IGenerateFromMasterRecord<EnemyModel,ShMstEnemyRecord>

    {
        public int Id;
        public string Name;
        public string Description;
        public int Hp;
        public int Attack;
        public int Defence;
        public int Rank;
        public List<AbilityModel> Abilities;
        public void Convert(ShMstEnemyRecord record)
        {
            Id = record.id;
            Name = record.Name;
            Description = record.Description;
            Hp = record.Hp;
            Attack = record.Attack;
            Defence = record.Defence;
            Abilities = new[]
            {
                record.AbilityId1,
                record.AbilityId2,
                record.AbilityId3,
                record.AbilityId4,
            }.ToList().ConvertAll<AbilityModel>(id =>
                new AbilityModel().Generate(MasterdataManager.Get<ShMstAbilityRecord>(id)));
            var param = new[]
            {
                record.AbilityParam1_1,
                record.AbilityParam1_2,
                record.AbilityParam2_1,
                record.AbilityParam2_2,
                record.AbilityParam3_1,
                record.AbilityParam3_2,
                record.AbilityParam4_1,
                record.AbilityParam4_2,

            };
            for (var i = 0; i < Abilities.Count; i++)
            {
                Abilities[i].AbilityParam1 = param[i * 2];
                Abilities[i].AbilityParam2 = param[i * 2 + 1];
            }
        }

        public EnemyModel Generate(ShMstEnemyRecord record)
        {
            Convert(record);
            return this;
        }
    }
    public class CharacterModel
    {
        public int Hp;
        public int Attack;
        public int Defence;
    }

    public class AbilityModel : IConvertFromMasterRecord<ShMstAbilityRecord>,IGenerateFromMasterRecord<AbilityModel,ShMstAbilityRecord>
    {
        public int Id;
        public string Name;
        public string Description;
        public AbilityResolver.AbilityCommands Command;
        public AbilityResolver.AbilityTiming Timing;
        public int AbilityParam1;
        public int AbilityParam2;
        public void Convert(ShMstAbilityRecord record)
        {
            Id = record.id;
            Name = record.Name;
            Description = record.Description;
            Command = record.Command;
            Timing = record.Timing;
        }

        public AbilityModel Generate(ShMstAbilityRecord record)
        {
            Convert(record);
            return this;
        }
        
    }
    public class GameSequenceReturnModel
    {
        public enum EndState
        {
            Error,
            Over,
            Clear
        }

        public EndState State;
    }
}
