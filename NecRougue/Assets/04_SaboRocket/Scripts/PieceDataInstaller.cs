using System;
using System.Linq;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "PieceDataInstaller", menuName = "Installers/PieceDataInstaller")]
public class PieceDataInstaller : ScriptableObjectInstaller<PieceDataInstaller>
{
    public PieceDatas PieceDatas;
    public override void InstallBindings()
    {
        Container.Bind<PieceDatas>().FromInstance(PieceDatas).AsSingle().NonLazy();
    }
}

[Serializable]
public class PieceDataSet
{
    public int Id;
    public PhysicsPiece PhysicsPrefab;
    public Piece ViewPrefab;
}

[Serializable]
public class PieceDatas
{
    public PieceDataSet[] Pieces;

    public PieceDataSet GetPiece(int id)
    {
        return Pieces.FirstOrDefault(_ => _.Id == id);
    }
}