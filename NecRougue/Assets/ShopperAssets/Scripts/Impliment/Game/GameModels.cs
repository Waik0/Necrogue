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
        public List<ConditionModel> Conditions;
        public int Id;
        public int Rank;
        public int Price;
        public int Rarity;
        public string Name;
        public string Description;
        public string GUID;
        //view 
        public Vector2 screenPos;
        public void Convert(ShMstCardRecord record)
        {
            GUID = Guid.NewGuid().ToString();
            Id = record.id;
            Name = record.Name;
            Price = record.Price;
            Description = record.Description;
            Abilities = new List<AbilityModel>();
            for (var i = 0; i < record.Command.Length; i++)
            {
                if (record.AbilityParam1.Length > i && 
                    record.AbilityParam2.Length > i &&
                    record.AbilityParam3.Length > i &&
                    record.Timing.Length > i)
                {
                    var c = 
                            record.Timing[i] == AbilityUseCase.AbilityTiming.Use ? 
                                record.UseCondition : 
                                AbilityUseCase.AbilityCondition.None;
                    Abilities.Add(new AbilityModel()
                    {
                        AbilityParam1 = record.AbilityParam1[i],
                        AbilityParam2 = record.AbilityParam2[i],
                        AbilityParam3 = record.AbilityParam3[i],
                        Command = record.Command[i],
                        Condition = c,
                        Timing = record.Timing[i],
                        
                    });
                }
            }
            Conditions = new List<ConditionModel>();
            var d = new ConditionModel()
            {
                Condition = record.UseCondition,
                ConditionParam = record.ConditionParam,
                Timing = AbilityUseCase.AbilityTiming.Use
            };
            if (Conditions.Find(_=>_.Condition == d.Condition) == null)
            {
                Conditions.Add(d);
            }
            // Abilities = 
            //     .Where(_=>MasterdataManager.Get<ShMstAbilityRecord>(_)!=null)
            //     .ToList().ConvertAll<AbilityModel>(id =>
            //     new AbilityModel().Generate(MasterdataManager.Get<ShMstAbilityRecord>(id)));
            
            // foreach (var ab in record.AbilityId)
            // {
            //     var mst = MasterdataManager.Get<ShMstAbilityRecord>(ab);
            //     
            //     
            // }
            // for (var i = 0; i < Abilities.Count; i++)
            // {
            //     if(record.AbilityParam1.Length > i)
            //         Abilities[i].AbilityParam1 = record.AbilityParam1[i];
            //     if(record.AbilityParam2.Length > i)
            //         Abilities[i].AbilityParam2 = record.AbilityParam2[i];
            //     if(record.ConditionParam.)
            // }
        }

        public CardModel Generate(ShMstCardRecord record)
        {
            Convert(record);
            return this;
        }
    }


    public class EnemyModel: IConvertFromMasterRecord<ShMstEnemyRecord>,IGenerateFromMasterRecord<EnemyModel,ShMstEnemyRecord>

    {
        public int Id;
        public string Name;
        public string Description;
        public CharacterModel Model;
        public int Rank;
        public string GUID;
        public bool Stun;
        public List<AbilityModel> Abilities;
        public void Convert(ShMstEnemyRecord record)
        {
            GUID = Guid.NewGuid().ToString();
            Id = record.id;
            Name = record.Name;
            Description = record.Description;
            Model = new CharacterModel();
            Model.Hp = record.Hp;
            Model.Attack = record.Attack;
            Model.Defence = record.Defence;
            Abilities = new List<AbilityModel>();
            for (var i = 0; i < record.Command.Length; i++)
            {
                if (record.AbilityParam1.Length > i && 
                    record.AbilityParam2.Length > i &&
                    record.AbilityParam3.Length > i 
                    )
                {
                    var c =
                        AbilityUseCase.AbilityCondition.None;
                    Abilities.Add(new AbilityModel()
                    {
                        AbilityParam1 = record.AbilityParam1[i],
                        AbilityParam2 = record.AbilityParam2[i],
                        AbilityParam3 = record.AbilityParam3[i],
                        Command = record.Command[i],
                        Condition = c,
                        Timing = AbilityUseCase.AbilityTiming.EnemyTurn,
                    });
                }
            }
            // Abilities = record.AbilityId
            //     .Where(_=>MasterdataManager.Get<ShMstAbilityRecord>(_)!=null)
            //     .ToList().ConvertAll<AbilityModel>(id =>
            //         new AbilityModel().Generate(MasterdataManager.Get<ShMstAbilityRecord>(id)));
        }

        public EnemyModel Generate(ShMstEnemyRecord record)
        {
            Convert(record);
            return this;
        }
    }

    public class PlayerModel
    {
        public CharacterModel Chara;
        public int Exp;
    }
    public class CharacterModel
    {
        public int Hp;
        public int Attack;
        public int Defence;
        public int Shield;
        public bool Stun;
        public int Poison;
        public bool Barrier;
    }

    public class ConditionModel
    {
        public AbilityUseCase.AbilityTiming Timing;
        public int ConditionParam;
        public AbilityUseCase.AbilityCondition Condition;
    }
    public class AbilityModel //: IConvertFromMasterRecord<ShMstAbilityRecord>,IGenerateFromMasterRecord<AbilityModel,ShMstAbilityRecord>
    {
        public int Id;
        public string Name;
        public string Description;
        public AbilityUseCase.AbilityCommands Command;
        public AbilityUseCase.AbilityTiming Timing;
        public AbilityUseCase.AbilityCondition Condition;
        public int AbilityParam1;
        public int AbilityParam2;
        public int AbilityParam3;
        public int PlayerMotionId;
        public int EnemyMotionId;
        // public void Convert(ShMstAbilityRecord record)
        // {
        //     Id = record.id;
        //     Name = record.Name;
        //     Description = record.Description;
        //     Command = record.Command;
        //     Timing = record.Timing;
        //     AbilityParam1 = record.AbilityParam1;
        //     AbilityParam2 = record.AbilityParam2;
        //     AbilityParam3 = record.AbilityParam3;
        //     Condition = record.Condition;
        //     
        //     PlayerMotionId = record.PlayerMotionId;
        //     EnemyMotionId = record.EnemyMotionId;
        //
        // }
        //
        // public AbilityModel Generate(ShMstAbilityRecord record)
        // {
        //     Convert(record);
        //     return this;
        // }
        
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
