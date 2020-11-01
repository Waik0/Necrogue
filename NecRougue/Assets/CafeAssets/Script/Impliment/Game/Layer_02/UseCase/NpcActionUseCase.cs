using CafeAssets.Script.Interface.UseCase;
using CafeAssets.Script.Model;
using CafeAssets.Script.System.GameTimeSystem;
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
            CurrentStatus = NpcActionStatus.Start;
            count = 5;
        }

        public void Tick()
        {
            CurrentStatus = NpcActionStatus.Doing;
            count--;
            if (count == 0)
            {
                CurrentStatus = NpcActionStatus.Complete;
            }
        }
    }
    //ランダムな場所に移動
    public class NpcMoveToPlace : INpcActionUseCase
    {
        public NpcActionPattern TargetPattern { get; private set; } = NpcActionPattern.MoveToPlace;
        public NpcActionStatus CurrentStatus { get; private set; }

        public void StartAction(NpcActionModel model)
        {
            CurrentStatus = NpcActionStatus.Start;
        }

        public void Tick()
        {
            CurrentStatus = NpcActionStatus.Doing;
        }
    }



}
