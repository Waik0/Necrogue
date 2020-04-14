using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// バトルに必要なデータ
/// </summary>
public class BattleData
{
    public enum PlayerType
    {
        Player,
        Enemy
    }
    public int Turn;
    public bool IsEnd;
    public PlayerType Winner;
    public Dictionary<PlayerType,BattlePlayerData> PlayerList = new Dictionary<PlayerType, BattlePlayerData>();

    public void Reset()
    {
        Turn = 0;
        PlayerList = new Dictionary<PlayerType, BattlePlayerData>();
    }
}

public class BattlePlayerData
{
    public string Name;
    public int CharaId;
    public BattleDeck Deck;
    public int Speed;

}
public class BattleDeck
{
    public List<BattleCard> Cards = new List<BattleCard>();//モンスターカード 順番も関係ある
}

public class BattleCard
{
    public int Id;//マスターから素材引っ張る用
    public int Attack;
    public int Hp;
    public int Defence;
    public List<int> AbilityId = new List<int>();//マスターから素材ひっぱったり AbilityEffectsから効果ひっぱったり
    public List<Disease> DiseaseList = new List<Disease>();//状態異常
}

public class Disease
{
    public int Id;//DiseaseEffectsから効果ひっぱったり
    public int TurnLeft;//残りターン
   // public int Damage;//ダメージ
    
}