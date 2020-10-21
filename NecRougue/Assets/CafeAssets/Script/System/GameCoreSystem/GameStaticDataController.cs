using System.Linq;
using CafeAssets.Script.System.GameMapSystem.TileInheritance;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.System.GameCoreSystem
{
    public interface IGameStaticDataController
    {
        TileModelProvider TileModelProvider { get; }
        FloorTileModel GetFloorTileModel(string name);
        FurnitureTileModel GetFurnitureTileModel(string name);
        GoodsTileModel GetGoodsTileModel(string name);
    }

   
    public class GameStaticDataController : IGameStaticDataController
    {
        private TileModelProvider _tileModelProvider;
        public GameStaticDataController(
            TileModelProvider tileModelProvider,
            MenuModelProvider menuModelProvider
            )
        {
            _tileModelProvider = tileModelProvider;
        }

        public TileModelProvider TileModelProvider { get => _tileModelProvider; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">ファイル名</param>
        /// <returns></returns>
        public FloorTileModel GetFloorTileModel(string name)
        {
            return _tileModelProvider.FloorTileModels.FirstOrDefault(_=>_.name == name);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">ファイル名</param>
        /// <returns></returns>
        public FurnitureTileModel GetFurnitureTileModel(string name)
        {
            return _tileModelProvider.FurnitureTileModels.FirstOrDefault(_=>_.name == name);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">ファイル名</param>
        /// <returns></returns>
        public GoodsTileModel GetGoodsTileModel(string name)
        {
            return _tileModelProvider.GoodsTileModels.FirstOrDefault(_=>_.name == name);
        }
    }
}
