// using System.Collections;
// using System.Collections.Generic;
// using UniRx;
// using UnityEngine;
// using Zenject;
//
// public interface ICursorMessageSender
// {
//     void StartSendCoroutine(ITortecUseCaseBaseWithWebSocket peer);
//     void EndSendCoroutine();
// }
// public class CursorMessageSender : MonoBehaviour,ICursorMessageSender
// {
//
//     private Coroutine _coroutine;
//
//     //private ITortecUseCaseBaseWithWebSocket _peer;
//     public void StartSendCoroutine(ITortecUseCaseBaseWithWebSocket peer)
//     {
//         //_peer = peer;
//         if (_coroutine!= null)
//         {
//             StopCoroutine(_coroutine);
//         }
//         _coroutine = StartCoroutine(SendCursorData(peer));
//     }
//
//     public void EndSendCoroutine()
//     {
//         Debug.Log("カーソル送信を停止");
//         if (_coroutine!= null)
//         {
//             StopCoroutine(_coroutine);
//         }
//     }
//     IEnumerator SendCursorData(ITortecUseCaseBaseWithWebSocket useCaseBase )
//     {
//         int interval = 0;
//         Vector3 cache = new Vector3(-1000, -1000);
//         while (true)
//         {
//             if (Input.GetMouseButton(0))
//             {
//                 var mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//                 if ((mouse - cache).sqrMagnitude > 2)
//                 {
//                     if (interval <= 0)
//                     {
//                         interval = 2;
//
//                         useCaseBase.BroadcastAll(new CursorData()
//                         {
//                             down = true,
//                             id = useCaseBase.SelfId,
//                             worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition),
//                         });
//                         cache = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//                     }
//                 }
//             }
//             if (interval > 0)
//             {
//                 interval--;
//             }
//             if (Input.GetMouseButtonUp(0))
//             {
//                 useCaseBase.BroadcastAll(new CursorData()
//                 {
//                     down = false,
//                     id = useCaseBase.SelfId,
//                     worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition),
//                 });
//                 interval = 0;
//             }
//             yield return null;
//         }
//     }
//     
// }
