using CafeAssets.Script.Model;

namespace CafeAssets.Script.Interface.Facade
{
    public interface INpcFacade
    {
        NpcFacadeModel FacadeModel { get; }
        NpcActionPattern CurrentAction();
        string[] GetParamKeys();
        int GetParam(string key);
    }
}