using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using ShopperAssets.Scripts.Game;
using UnityEngine;

public class EnemyUI : MonoBehaviour
{
    //prefab
    [SerializeField] private EnemyUINode _nodePrefab; 
    //root
    [SerializeField] private RectTransform _nodeRoot;
    Dictionary<string,EnemyUINode> _guidNodeSet = new Dictionary<string, EnemyUINode>();

    public void ResetUI()
    {
        foreach (var value in _guidNodeSet.Values)
        {
            Destroy(value.gameObject);
        }
        _guidNodeSet.Clear();
    }
//0が→
    private Vector2 CalcEnemyPos(int count)
    {
        return new Vector2(- 48 + 48 * count,16);
    }

    private EnemyUINode CreateEnemyNode(EnemyModel enemyModel,int count)
    {
        var obj = Instantiate(_nodePrefab,_nodeRoot);
        obj.SetEnemy(enemyModel);
        obj.GetComponent<RectTransform>().anchoredPosition = CalcEnemyPos(count);
        return obj;
    }
    public void SetEnemies(List<EnemyModel> enemyModels)
    {
        var delete = _guidNodeSet.Keys.ToList();
        var count = 0;
        foreach (var enemyModel in enemyModels)
        {
            if (enemyModel != null)
            {

                if (!_guidNodeSet.ContainsKey(enemyModel.GUID))
                {
                    _guidNodeSet.Add(enemyModel.GUID,CreateEnemyNode(enemyModel,count));
                }
                else
                {
                    _guidNodeSet[enemyModel.GUID].GetComponent<RectTransform>().anchoredPosition = CalcEnemyPos(count);
                }
                delete.Remove(enemyModel.GUID);
            }
            count++;
        }
        foreach (var s in delete)
        {
            Destroy(_guidNodeSet[s].gameObject);
            _guidNodeSet.Remove(s);
        }
        

    }
}
