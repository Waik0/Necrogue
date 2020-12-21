using CafeAssets.Script.System.GameCoreSystem;
using Zenject;

namespace CafeAssets.Script.GameComponents.NpcSpawnController
{
    /// <summary>
    /// タイルマップパラメーターをもとにNPCの出現を制御
    /// 依存コンポーネント：
    /// NPCCollection
    /// TilemapParams
    /// </summary>
    public class NpcSpawnControllerInstaller: MonoInstaller
    {
    
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<DebugNpcSpawnTimingController>().AsCached().NonLazy();
        }
    }
}
