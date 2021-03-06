using UnityEngine;

/// <summary>
/// 使用するカード情報と設置するピース情報
/// </summary>
public class InputSender
{
    public bool TrySend(int pieceId,int angle,ITortecUseCaseBaseWithWebSocket peer)
    {
        if (Input.GetMouseButton(0))
        {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var input = new InputData()
            {
                pos = new Vector2Int((int) pos.x, (int) pos.y),
                angle = angle,
                pieceId = pieceId
            };
            peer.BroadcastAll(input);
            return true;
        }
        return false;
    }

}
