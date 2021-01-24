using System.Collections;
using System.Collections.Generic;
using Basic3DTileSystem.Source.Interface.Script;
using UnityEngine;
using Zenject;

public class Tilemap3DFacade : MonoBehaviour
{
    private ITilemap3DInterfaceFacade _tilemap3DInterfaceFacade;
    
    [Inject]
    void Inject(
        ITilemap3DInterfaceFacade tilemap3DInterfaceFacade)
    {
        _tilemap3DInterfaceFacade = tilemap3DInterfaceFacade;
    }
}
