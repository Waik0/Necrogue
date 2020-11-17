using CafeAssets.Script.Model;

namespace CafeAssets.Script.Interface.Layer_01.Manager
{
    /// <summary>
    /// メニュー > ゲーム開始時に一度だけ呼ばれる
    /// </summary>
    public interface IGameResettable
    {
        //resetだとmonobehaviourのと被る
        void ResetOnGame();
    }
    /// <summary>
    /// ボタン以外の箇所をタッチ入力されたときに呼ばれる
    /// </summary>
    public interface IGameScreenInputReceiver
    {
        void GameInput(GameInputModel model);
    }
    /// <summary>
    /// タイルが配置された/はがされたときに呼ばれる
    /// </summary>
    public interface IPlaceTileReceiver
    {
        void OnPlaceTile(TilePlaceModel model);
        void OnRemoveTile(TilePlaceModel model);
    }
    /// <summary>
    /// タイルマップに変更が加えられたときに呼ばれる
    /// </summary>
    public interface ITilemapReceiver
    {
        void OnUpdateTile(TilemapModel model);

    }
    public interface IGameTickable
    {
        void TickOnGame(IGameTimeManager gameTimeManager);
    }

    public interface INpcSpawnReceiver
    {
        void OnSpawnNpc(NpcModel model);
    }
}