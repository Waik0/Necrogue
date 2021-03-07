using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class PieceViewerUseCase : MonoBehaviour
{
    private Queue<TimelineData> _data = new Queue<TimelineData>();
    private PieceDatas _pieceDatas;
    private Dictionary<int, Piece> _pieces = new Dictionary<int, Piece>();
    [Inject]
    void Inject(
        PieceDatas pieceDatas
    )
    {
        _pieceDatas = pieceDatas;
    }

    public void Init()
    {
        foreach (var i in _pieces.Keys.ToArray())
        {
            Destroy(_pieces[i].gameObject);
        }
        _pieces.Clear();
    }
    public void SetTimelineData(TimelineData data)
    {
        _data.Enqueue(data);
    }

    public void Play(Action onEnd)
    {
        StartCoroutine(PlayInternal(onEnd));
    }
    public IEnumerator PlayAwait(Action onEnd)
    {
        yield return PlayInternal(onEnd);
        Debug.Log("End");
    }

    private IEnumerator PlayInternal(Action onEnd)
    {
        while (true)
        {
            if (_data.Count > 0)
            {
                var d = _data.Dequeue();
                foreach (var timelineDataObject in d.objects)
                {
                    if (!_pieces.ContainsKey(timelineDataObject.uniqueId))
                    {
                        _pieces.Add(timelineDataObject.uniqueId,Instantiate(_pieceDatas.GetPiece(timelineDataObject.pieceId).ViewPrefab));
                    }

                    _pieces[timelineDataObject.uniqueId].SetVertex(timelineDataObject.pos, timelineDataObject.angle);
                }
                if (d.isLast)
                {
                    break;
                }
            }
            yield return null;
        }
        onEnd?.Invoke();
    }
}
