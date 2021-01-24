using Basic3DTileSystem.Source.Core.Script;
using TileParamSystem.Source.Core.Script;
using UnityEngine;
using Zenject;

namespace TileParamSystem.Example.Script.PraramProvider
{
    public class ParamExample : TileParamProvider//,ITileParamCollection<ParamExample>
    {
        [SerializeField] private int _param = 1;
        public int Param => _param;
        
    }
}
