using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace SaboRocketSteam.Scripts.GameHost.Physics
{
    public class PiecePhysicsUseCase
    {
        private List<PhysicsPiece> _physicsPieces = new List<PhysicsPiece>();
        public List<PhysicsPiece> PhysicsPieces => _physicsPieces;
        [Inject] private PieceDatas _pieceDatas;
        public void Init()
        {
            _physicsPieces.RemoveAll(p =>
            {
                Object.Destroy(p.gameObject);
                return true;
            });
        }
        public void Spawn(int id, Vector2Int pos, int angle)
        {
            var prefab = _pieceDatas.GetPiece(id);
            var p = Object.Instantiate(prefab.PhysicsPrefab);
            p.Place(pos, angle, id);
            _physicsPieces.Add(p);
        }

        public IEnumerator StartCalcAwait(List<PhysicsPiece> pieces, Action<List<TimelineData>> response,
            int firstFrame)
        {
            List<TimelineData> timelineDatas = null;
            yield return CalcPhysics(pieces, data => { timelineDatas = data; }, firstFrame);
            while (timelineDatas == null)
            {
                yield return null;
            }

            response?.Invoke(timelineDatas);
        }

        IEnumerator CalcPhysics(List<PhysicsPiece> pieces, Action<List<TimelineData>> res, int firstFrame)
        {
            var maxTime = .08f;
            var start = Time.realtimeSinceStartup;
            var timeline = new List<TimelineData>();
            var count = firstFrame;
            var maxFrame = firstFrame + 200;
            Debug.Log("StartCalc");
            while (count <= maxFrame)
            {

                Physics2D.Simulate(.1f);
                Physics2D.Simulate(.1f);
                timeline.Add(AddTimeLine(pieces, count, maxFrame));
                if (Time.realtimeSinceStartup - start > maxTime)
                {
                    start = Time.realtimeSinceStartup;
                    yield return null;
                }

                count++;
            }

            res(timeline);
            yield return null;
        }

        TimelineData AddTimeLine(List<PhysicsPiece> pieces, int frame, int maxframe)
        {
            var timeLine = new TimelineData();
            timeLine.timeLineId = frame;
            //        Debug.Log( $" {frame} {maxframe} {frame >= maxframe}");
            timeLine.isLast = frame >= maxframe;
            timeLine.objects = new List<ObjectVertexData>();
            foreach (var physicsPiece in pieces)
            {
                timeLine.objects.Add(new ObjectVertexData()
                {
                    uniqueId = physicsPiece.Id,
                    pieceId = physicsPiece.PieceId,
                    angle = physicsPiece.Angle,
                    pos = physicsPiece.Pos,
                });
            }

            return timeLine;
        }
    }
}
