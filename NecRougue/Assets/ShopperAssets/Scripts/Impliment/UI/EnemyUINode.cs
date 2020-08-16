using System.Collections;
using System.Collections.Generic;
using ShopperAssets.Scripts.Game;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUINode : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private Slider _hp;
    private EnemyModel _model;
    private float _maxHp;
    public void SetEnemy(EnemyModel enemyModel)
    {
        _model = enemyModel;
        _maxHp = enemyModel.Hp;
    }
    
    //ダメージ監視
    void Update()
    {
        if (_model == null) return;
        _hp.value = _model.Hp / _maxHp;
    }
    
}
