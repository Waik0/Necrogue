using System.Collections;
using System.Collections.Generic;
using ShopperAssets.Scripts.Game;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Image _icon = default;
    [SerializeField] private Text _hp = default;
    [SerializeField] private Text _at = default;

    public void SetStatus(CharacterModel player)
    {
        _hp.text = $"HP : {player.Hp}";
        _at.text = $"HP : {player.Attack}";
    }
}
