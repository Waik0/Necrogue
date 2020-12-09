using Zenject;

namespace CafeAssets.Script.GameComponents
{
    public class GameFacadeInstaller : MonoInstaller
    {
    
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<GameFacade>().FromNewComponentOnNewGameObject().AsCached().NonLazy();
        }
    }
}