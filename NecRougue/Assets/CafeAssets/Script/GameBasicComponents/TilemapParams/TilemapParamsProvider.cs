using System.Collections.Generic;
using CafeAssets.Script.GameComponents.TilemapParams;
using UniRx;
using UnityEngine;

// namespace CafeAssets.Script.GameBasicComponents.TilemapParams
// {
//     /// <summary>
//     /// パラメータが変化したときにPos(x,y)ごとのパラメータを再計算するクラス
//     /// </summary>
//     public class TilemapParamsCalculator
//     {
//         private ITilemapParamsUseCase _tilemapParameterUseCase;
//         /// <summary>
//         /// posにパラメータが追加または変更されたとき影響範囲全域にポインタを格納する
//         /// </summary>
//         /// <param name="pos"></param>
//         void Add(Vector3Int pos)
//         {
//             var paramList = _tilemapParameterUseCase.GetTileParams(pos);
//             foreach (var tileParamsModelBase in paramList)
//             {
//                 var s = Mathf.Abs(tileParamsModelBase.Size);
//                 for (int x = -s; x <= s; x++)
//                 {
//                     for (int y = -s; y <= s; y++)
//                     {
//                         var pos = 
//                     }
//                 }
//             }
//         }
//
//         void Remove()
//         {
//             
//         }
//     }
// }
