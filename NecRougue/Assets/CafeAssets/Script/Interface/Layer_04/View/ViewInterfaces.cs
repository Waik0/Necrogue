using CafeAssets.Script.Model;
using UnityEngine;

namespace CafeAssets.Script.Interface.View
{
    /// <summary>
    /// View扱いだが、主に入力用のCanvas制御(Image1つだけあるようなやつ)
    /// </summary>
    public interface IGameInputView
    {
        GameInputModel Model { get; }
    }
    /// <summary>
    /// ゲーム内メインカメラ制御
    /// </summary>
    public interface ICameraView
    {
        void MoveCamera(Vector3 pos);
        Vector3 ScreenToWorldPoint(Vector2 screenPoint);
    }
    /// <summary>
    /// タイルマップ制御
    /// </summary>
    public interface ITilemapView
    {
        void SetTile(TilePlaceModel model);
        void RemoveTile(Vector3Int pos);
    }
    /// <summary>
    /// タイルマップ通過制御
    /// </summary>
    public interface ITilemapPassableView
    {
        void SetTile(TilePlaceModel model);
        void RemoveTile(Vector3Int pos);
    }
}