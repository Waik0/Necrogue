using CafeAssets.Script.Interface.Layer_01.Manager;
using CafeAssets.Script.System.GameCoreSystem;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.System.GameTimeSystem
{
    public class GameTimeView : MonoBehaviour,IGameTickable
    {
        [SerializeField] private Text _text;
        
        public void TickOnGame(IGameTimeManager gameTimeManager)
        {
            var time = gameTimeManager.GetNow();
            _text.text = $"{time.Year:D4}年 {time.Month:D2}月  {time.Week:D1}週  {time.Hour:D2}時間 {time.Minutes:D2}分";
        }
    }
}
