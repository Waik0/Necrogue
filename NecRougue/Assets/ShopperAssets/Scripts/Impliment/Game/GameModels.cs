using System;
using System.Linq;
using System.Collections.Generic;
using ShopperAssets.Scripts.Master;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

namespace ShopperAssets.Scripts.Game
{
    
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
            Price = record.Price;
            Description = record.Description;
            Abilities = record.AbilityId
                .Where(_=>MasterdataManager.Get<ShMstAbilityRecord>(_)!=null)
                .ToList().ConvertAll<AbilityModel>(id =>
                new AbilityModel().Generate(MasterdataManager.Get<ShMstAbilityRecord>(id)));
            
            for (var i = 0; i < Abilities.Count; i++)
            {
                if(record.AbilityParam1.Length > i)
                    Abilities[i].AbilityParam1 = record.AbilityParam1[i];
                if(record.AbilityParam2.Length > i)
                    Abilities[i].AbilityParam2 = record.AbilityParam2[i];
            }
        }

        public CardModel Generate(ShMstCardRecord record)
        {
            Convert(record);
            return this;
        }
    }

    public class EnemyDeckModel
    {

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
            Abilities = record.AbilityId
                .Where(_=>MasterdataManager.Get<ShMstAbilityRecord>(_)!=null)
                .ToList().ConvertAll<AbilityModel>(id =>
                    new AbilityModel().Generate(MasterdataManager.Get<ShMstAbilityRecord>(id)));
            
            for (var i = 0; i < Abilities.Count; i++)
            {
                if(record.AbilityParam1.Length > i)
                    Abilities[i].AbilityParam1 = record.AbilityParam1[i];
                if(record.AbilityParam2.Length > i)
                    Abilities[i].AbilityParam2 = record.AbilityParam2[i * 2 + 1];
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
