using CafeAssets.Script.System.GameTimeSystem;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.System.GameNpcSystem
{
    public enum NpcActionStatus
    {
        Start,
        Doing,
        Complete,
    }

    public interface INpcActionUseCase
    {
        NpcActionPattern TargetPattern { get; }
        NpcActionStatus CurrentStatus { get; }
        void StartAction(NpcActionModel model);

        void Tick(IGameTimeManager gameTimeManager);
    }

    public class NpcMoveToPlace : INpcActionUseCase
    {
        public NpcActionPattern TargetPattern { get; private set; } = NpcActionPattern.MoveToPlace;
        public NpcActionStatus CurrentStatus { get; private set; }

        public void StartAction(NpcActionModel model)
        {
            CurrentStatus = NpcActionStatus.Start;
        }

        public void Tick(IGameTimeManager gameTimeManager)
        {
        }
    }

    public class NpcActionModel
    {
        public string Param;
    }

}
