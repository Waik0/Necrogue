using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;


public class InGameUseCase : MonoBehaviour
{
    [SerializeField] private Cursor _cursorPrefab;
    [SerializeField] private Piece _piecePrefab;
    [SerializeField] private PiecePreview _preview;
    [SerializeField] private Transform _root;
    
    public Dictionary<string, PlayerData> PlayerDatas { get; } = new Dictionary<string, PlayerData>();
    public Dictionary<string, Cursor> CursorDatas { get; } = new Dictionary<string, Cursor>();
    public int CurrentSprite { get; private set; }
    private int _currentTurn = 0;
    public void SetPlayerList(List<string> playerIDs)
    {
        foreach (var keyValuePair in CursorDatas)
        {
            Destroy(keyValuePair.Value.gameObject);
        }
        PlayerDatas.Clear();
        CursorDatas.Clear();
    }

    public void SetPlayerData(PlayerData data)
    {
        if (PlayerDatas.ContainsKey(data.id))
        {
            PlayerDatas[data.id] = data;
        }
        else
        {
            PlayerDatas.Add(data.id, data);
        }

        if (CursorDatas.ContainsKey(data.id))
        {
            CursorDatas[data.id]?.SetName(data.Name);
        }
        else
        {
            CursorDatas.Add(data.id, Instantiate(_cursorPrefab));
            CursorDatas[data.id].SetName(data.Name);
        }
    }
    
    public void SetCursorPos( CursorData cursorData)
    {
        if (CursorDatas.ContainsKey(cursorData.id))
        {
            CursorDatas[cursorData.id]?.SetPos(cursorData.worldPos);
        }
        else
        {
            CursorDatas.Add(cursorData.id, Instantiate(_cursorPrefab));
            CursorDatas[cursorData.id].SetName(cursorData.id);
            CursorDatas[cursorData.id].SetPos(cursorData.worldPos);
        }
    }
    public void NextTurn()
    {
        _currentTurn++;
        CurrentSprite = Random.Range(0, _preview.CandidateNum);
        _preview.SetSprite(CurrentSprite);
    }

    public void Fall()
    {
        
    }
}
