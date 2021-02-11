using System.Collections;
using System.Collections.Generic;
using Basic3DTileSystem.Source.Interface.Script;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public enum FloorType
{
    Green,
    Sand,
    Stone,
    Iron,
    Fruits,
    River,
}

public enum BuildType
{
    Castle,
    House,
    Farm,
}
public class GameMain : MonoBehaviour
{
    private BuildFieldModel<BuildType> _buildField = new BuildFieldModel<BuildType>(200,200);
    private FloorFieldModel<FloorType> _floorField = new FloorFieldModel<FloorType>(200,200);
    void Start()
    {
        
    }
    void Update()
    {
        
    }
}
