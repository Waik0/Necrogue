﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum NpcActionPattern
{
    Stop,
    MoveToRandomPlace,
    MoveToNpc,
    Order,
    Talk,
    Cook,
    Carry,
    Pay,
    Clean,
    Fortune,
}
public enum InputMode
{
    MoveCamera,
    PlaceTile,
    None,
}
public enum TileType
{
    Floor,
    Furniture,
    Goods,
    None,
}
public enum PlaceTileMode
{
    PlaceTileSingle,//一度だけ置く
    PlaceTileDraw,//ドラッグしたところにも置いていく
    PlaceTileRect,//四角形に置く
    
}

/// <summary>
/// ローカライズ対応
/// ここではない場所に置く
/// </summary>

public enum Region
{
    Ja = 0,
    En = 1,
}
public enum TileEffectType
{
    Sit,//座る
    Table,//テーブル(飲食物等設置可能)
    Order,//注文
}
public enum NpcActionStatus
{
    Sleep,
    Doing,
    Complete,
}

public enum NpcType
{
    
}

public enum GameInputState
{
    PointerDown,
    Drug,
    PointerUp
}


#region GameParams
public enum ParameterStyle
{
    Fixable,
    Fluid
    
}

public enum GameParameters
{
    //基礎
    Money,
    //研究度
    
    MenuCoffee,
    Menu
}

public enum GameParameterOperations
{
    Add = 1,
    Times = 2,
    Division = 3,
}


#endregion