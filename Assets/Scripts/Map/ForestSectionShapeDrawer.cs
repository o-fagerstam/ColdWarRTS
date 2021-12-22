using System;
using System.Collections.Generic;
using Shapes;
using UnityEngine;
namespace Map {
    [ExecuteAlways]
    public class ForestSectionShapeDrawer : ImmediateModeShapeDrawer {
        [SerializeField] private ForestSection forestSection;
        private PolylinePath path;
        private bool drawClosed;
        private void Start () {
            forestSection = GetComponent<ForestSection>();
            forestSection.OnPolygonChanged += HandleForestSectionChanged;
        }

        private void OnDestroy () {
            forestSection.OnPolygonChanged -= HandleForestSectionChanged;
        }

        private void HandleForestSectionChanged () {
            path = GeneratePathFromPoints(forestSection.Points);
            drawClosed = forestSection.IsClosed;
        }

        private PolylinePath GeneratePathFromPoints (IEnumerable<Vector3> points) {
            PolylinePath p = new PolylinePath();
            foreach (Vector3 point in points) {
                p.AddPoint(point + Vector3.up * 0.2f);
            }
            return p;
        }
        public override void DrawShapes (Camera cam) {
            if (path == null) {return;}
            using (Draw.Command(cam)) {
                Draw.LineGeometry = LineGeometry.Volumetric3D;
                Draw.ThicknessSpace = ThicknessSpace.Noots;
                Draw.Thickness = 1f;
                
                Draw.Polyline(path, drawClosed, Color.white);
            }
        }
    }
}
