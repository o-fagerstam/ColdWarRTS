using System;
using System.Collections.Generic;
using UnityEngine;
namespace Math {
    public class BezierCurve {
        public Vector2 anchor1 { get; set; }
        public Vector2 anchor2 { get; set; }
        public Vector2 control1 { get; set; }
        public Vector2 control2 { get; set; }
        public float ApproximateLength { get; private set; }

        public BezierCurve (Vector2 anchor1, Vector2 anchor2, Vector2 control1, Vector2 control2) {
            this.anchor1 = anchor1;
            this.anchor2 = anchor2;
            this.control1 = control1;
            this.control2 = control2;
            RecalculateLength();
        }

        public Vector2 GetPointOnCurve (float t) => CubicLerp(t);

        public void MovePoint (BezierPoint point, Vector2 toPosition) {
            switch (point) {
                case BezierPoint.Anchor1: anchor1 = toPosition; break;
                case BezierPoint.Anchor2: anchor2 = toPosition; break;
                case BezierPoint.Control1: control1 = toPosition; break;
                case BezierPoint.Control2: control2 = toPosition; break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(point), point, null);
            }
            RecalculateLength();
        }

        public Vector2 GetPoint (BezierPoint point) {
            return point switch {
                BezierPoint.Anchor1 => anchor1,
                BezierPoint.Anchor2 => anchor2,
                BezierPoint.Control1 => control1,
                BezierPoint.Control2 => control2,
                _ => throw new ArgumentOutOfRangeException(nameof(point), point, null)
            };
        }

        private void RecalculateLength() {
            float sumLength = 0f;
            Vector2 a = anchor1;
            int numSubdivisions = 100;
            for (int i = 1; i <= numSubdivisions; i++) {
                Vector2 b = CubicLerp(i/(float)numSubdivisions);
                sumLength += (a - b).magnitude;
                a = b;
            }
            ApproximateLength = sumLength;
        }

        public IEnumerable<Vector2> GetEquidistantPoints (int numPoints) {
            const int subdivisionsPerPoint = 10;
            int numSubdivisions = subdivisionsPerPoint*numPoints;
            int pointIndex = 0;
            int subdivisionIndex = 1;
            float dstBetweenPoints = ApproximateLength/numPoints;
            float targetDst = 0f;
            Vector2 a = anchor1;
            float dstA = 0f;
            while (pointIndex < numPoints) {
                Vector2 b = CubicLerp(subdivisionIndex/(float)numSubdivisions);
                float dstB = dstA + (a - b).magnitude;
                while (targetDst - dstA < dstB - targetDst) {
                    yield return a;
                    pointIndex++;
                    targetDst = dstBetweenPoints*pointIndex;
                }
                a = b;
                dstA = dstB;
                subdivisionIndex++;
            }
        }

        private static Vector2 QuadraticLerp (Vector2 p1, Vector2 p2, Vector2 p3, float t) {
            Vector2 l1 = Vector2.Lerp(p1, p2, t);
            Vector2 l2 = Vector2.Lerp(p2, p3, t);
            return Vector2.Lerp(l1, l2, t);
        }

        public Vector2 CubicLerp (float t) {
            Vector2 q1 = QuadraticLerp(anchor1, control1, control2, t);
            Vector2 q2 = QuadraticLerp(control1, control2, anchor2, t);
            return Vector2.Lerp(q1, q2, t);
        }
    }
    
    public enum BezierPoint {
        Anchor1, Anchor2, Control1, Control2
    }
}
