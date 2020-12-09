using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "MenuModelProvider",menuName = "ScriptableObject/MenuModelProvider")]
public class CafeMenuModelProvider : ScriptableObjectInstaller
{
    public CafeMenuModel[] Models;
    public override void InstallBindings()
    {
        Container.BindInstance(this);
    }
}
