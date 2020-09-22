using System;

namespace ShopperAssets.Scripts.Master
{
    // [MasterPath("ShopperAssets/Masterdata/Resources/mst_ability.json"),Serializable]
    // public class ShMstAbilityRecord : IMasterRecord
    // {
    //     public int Id { get => id; }
    //     public int id;
    //     public string Name;
    //     public string Description;
    //     public AbilityUseCase.AbilityCondition Condition;
    //     public AbilityUseCase.AbilityTiming Timing;
    //     public AbilityUseCase.AbilityCommands Command; //複合効果を考えて配列に
    //     public int AbilityParam1;
    //     public int AbilityParam2;
    //     public int AbilityParam3;
    //
    //     public int PlayerMotionId;
    //     public int EnemyMotionId;
    //     
    // }
    [MasterPath("ShopperAssets/Masterdata/Resources/mst_enemy.json"),Serializable]
    public class ShMstEnemyRecord : IMasterRecord
    {
        public int Id { get => id; }
        public int id;
        public int Rank;
        public string Name;
        public string Description;
        public int Attack;
        public int Defence;
        public int Hp;
        public int[] DropCardId;
        public AbilityUseCase.AbilityCondition Condition;
        public AbilityUseCase.AbilityCommands[] Command; 
        public int ConditionParam;
        public int[] AbilityParam1;
        public int[] AbilityParam2;
        public int[] AbilityParam3;

        public int PlayerMotionId;
        public int EnemyMotionId;

    }
    [MasterPath("ShopperAssets/Masterdata/Resources/mst_card.json"),Serializable]
    public class ShMstCardRecord : IMasterRecord
    {
        public int Id { get => id; }
        public int id;
        public int Rank;
        public int Price;
        public int Rarity;//100枚当たり何枚入るか
        public string Name;
        public string Description;
        //public int[] AbilityId;
        public AbilityUseCase.AbilityCondition UseCondition; //timing useの時のみ
        public AbilityUseCase.AbilityTiming[] Timing;
        public AbilityUseCase.AbilityCommands[] Command; 
        public int ConditionParam;
        public int[] AbilityParam1;
        public int[] AbilityParam2;
        public int[] AbilityParam3;

        public int PlayerMotionId;
        public int EnemyMotionId;
        
        //public int[] AbilityParam1;
        //public int[] AbilityParam2;
        //public int[] ConditionParam;
    }
}