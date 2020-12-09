using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CafeAssets.Script.System.GameTimeSystem
{
    public class GameTimeView : MonoBehaviour
    {
        [SerializeField] private Text _text;

        private IGameTimeUseCase _gameTimeUseCase;
        [Inject]
        void Inject(
            IGameTimeUseCase gameTimeUseCase)
        {
            _gameTimeUseCase = gameTimeUseCase;
        }
        void Update()
        {
            var time = _gameTimeUseCase.GetNow();
            _text.text = $"{time.Year:D4}年 {time.Month:D2}月  {time.Week:D1}週  {time.Hour:D2}時間 {time.Minutes:D2}分";
        }
    }
}
    