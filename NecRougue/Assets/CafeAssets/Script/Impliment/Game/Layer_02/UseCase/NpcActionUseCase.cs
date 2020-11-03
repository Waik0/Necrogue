using CafeAssets.Script.Interface.Layer_02.UseCase;
using CafeAssets.Script.Model;
using CafeAssets.Script.System.GameTimeSystem;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.System.GameNpcSystem
{
    
    //停止
    public class NpcStop : INpcActionUseCase
    {
        public NpcActionPattern TargetPattern { get; private set; } = NpcActionPattern.Stop;
        public NpcActionStatus CurrentStatus { get; private set; }

        private int count = 0;
        public void StartAction(NpcActionModel model)
        {
            CurrentStatus = NpcActionStatus.Doing;
            count = 5;
        }

        public void EndAction()
        {
            CurrentStatus = NpcActionStatus.Sleep;
        }

        public void Tick()
        {
            count--;
            if (count == 0)
            {
                CurrentStatus = NpcActionStatus.Complete;
            }
        }
    }
    //ランダムな場所に移動
    public class NpcMoveToRandomPlace : INpcActionUseCase
    {
        public NpcActionPattern TargetPattern { get; private set; } = NpcActionPattern.MoveToRandomPlace;
        public NpcActionStatus CurrentStatus { get; private set; }
        private INpcMoveUseCase _moveUseCase;
        private INpcParamUseCase _paramUseCase;
        private Vector2 _aim;
        public NpcMoveToRandomPlace(
            INpcMoveUseCase moveUseCase,
            INpcParamUseCase paramUseCase)
        {
            _moveUseCase = moveUseCase;
            _paramUseCase = paramUseCase;
        }
        public void StartAction(NpcActionModel model)
        {
            CurrentStatus = NpcActionStatus.Doing;
            _aim = new Vector2(Random.Range(-2,2),Random.Range(-2,2));
        }

        public void EndAction()
        {
            CurrentStatus = NpcActionStatus.Sleep;
        }

        public void Tick()
        {
            float speed = .2f;
            Vector2 direction = (_aim - _moveUseCase.CurrentPos()).normalized;
            _moveUseCase.Move(direction * speed);
            if ((_aim - _moveUseCase.CurrentPos()).magnitude < .2f)
            {
                CurrentStatus = NpcActionStatus.Complete;
            }

        }
    }



}
