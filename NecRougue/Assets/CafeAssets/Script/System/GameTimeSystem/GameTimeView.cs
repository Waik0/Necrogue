using UnityEngine;
using UnityEngine.UI;
using Zenject;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.System.GameTimeSystem
{
    public class GameTimeView : MonoBehaviour,IInitializable
    {
        [SerializeField] private Text _text;
        
        
        private IGameTimeUseCase _gameTimeUseCase;
        [Inject]
        public void Inject(
            IGameTimeUseCase gameTimeUseCase
                )
        {
            _gameTimeUseCase = gameTimeUseCase;
            
        }

        public void Initialize()
        {
            _gameTimeUseCase.OnTick.AddListener(UpdateGameTime);
        }

        void UpdateGameTime()
        {
            var time = _gameTimeUseCase.GetNow();
            _text.text = $"{time.Year:D4}年 {time.Month:D2}月  {time.Week:D1}週  {time.Hour:D2}時間 {time.Minutes:D2}分";
        }
    }
}
