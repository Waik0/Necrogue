using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface INpcDebugUnitCollection
{
}
public class NpcDebugUnitCollectionFactory : PlaceholderFactory<NpcDebugModel, INpcDebugUnitCollection> { }
