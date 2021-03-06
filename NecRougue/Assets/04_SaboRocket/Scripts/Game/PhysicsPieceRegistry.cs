using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

public class PhysicsPieceRegistry : MonoBehaviour
{
    private List<PhysicsPiece> _physicsPieces = new List<PhysicsPiece>();
    public List<PhysicsPiece> PhysicsPieces => _physicsPieces;
    private PieceDatas _pieceDatas;
    [Inject]
    void Inject(
        PieceDatas pieceDatas
    )
    {
        _pieceDatas = pieceDatas;
    }
    public void Init()
    {
        _physicsPieces.RemoveAll(p =>
        {
            Destroy(p);
            return true;
        });

    }

    public void Spawn(int id,Vector2Int pos,int angle)
    {
        var prefab = _pieceDatas.GetPiece(id);
        var p = Instantiate(prefab.PhysicsPrefab);
        p.Place(pos, angle);
        _physicsPieces.Add(p);
    }
}
