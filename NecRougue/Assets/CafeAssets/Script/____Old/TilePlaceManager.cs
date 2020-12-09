// using System.Collections;
// using System.Collections.Generic;
// using CafeAssets.Script.Interface.Layer_01.Manager;
// using CafeAssets.Script.Model;
// using UnityEngine;
// using Zenject;
// using NotImplementedException = System.NotImplementedException;
//
// namespace CafeAssets.Script.System.GameMapSystem
// {
//
//     /// <summary>
//     /// タイルが置かれたことを通知する
//     /// </summary>
//     public class TilePlaceTileManager : IPlaceTileManager
//     {
//         private List<IPlaceTileReceiver> _mapPlaceReceivers = new List<IPlaceTileReceiver>();
//         public TilePlaceTileManager(
//             [InjectOptional]
//             List<IPlaceTileReceiver> mapPlaceReceivers
//         )
//         {
//             if(mapPlaceReceivers != null)
//                 _mapPlaceReceivers = mapPlaceReceivers;
//         }
//         public void Add(IPlaceTileReceiver element)
//         {
//             _mapPlaceReceivers.Add(element);
//         }
//
//         public void RemoveNull()
//         {
//             _mapPlaceReceivers.RemoveAll(_ => _ == null);
//         }
//
//         public void Dispose()
//         {
//             _mapPlaceReceivers.Clear();
//         }
//         
//         public void OnPlaceTile(TilePlaceModel model)
//         {
//             foreach (var mapPlaceReceiver in _mapPlaceReceivers)
//             {
//                 mapPlaceReceiver?.OnPlaceTile(model);
//             }
//         }
//
//         public void OnRemoveTile(TilePlaceModel model)
//         {
//             foreach (var mapPlaceReceiver in _mapPlaceReceivers)
//             {
//                 mapPlaceReceiver?.OnRemoveTile(model);
//             }
//         }
//     }
//
//
// }