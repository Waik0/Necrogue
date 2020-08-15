using System;

namespace ShopperAssets.Scripts.Master
{
    [MasterPath("ShopperAssets/Masterdata/Resources/mst_ability.json"),Serializable]
    public class ShMstAbilityRecord : IMasterRecord
    {
        public int Id { get => id; }
        public int id;
        public string Name;
        public string Description;
        public AbilityResolver.AbilityTiming Timing;
        public AbilityResolver.AbilityCommands Command;
    }
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
        public int[] AbilityId;
        public int[] AbilityParam1;
        public int[] AbilityParam2;
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
        public int[] AbilityId;
        public int[] AbilityParam1;
        public int[] AbilityParam2;
    }
}