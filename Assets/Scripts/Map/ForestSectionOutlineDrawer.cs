using System;
using System.Collections.Generic;
using Shapes;
using UnityEngine;
namespace Map {
    [ExecuteAlways]
    [RequireComponent(typeof(AStaticMapElement))]
    public class ForestSectionOutlineDrawer : ImmediateModeShapeDrawer {
        [SerializeField] private ForestSection forestSection;
        private PolylinePath path;
        private bool drawClosed;
        private void Start () {
            forestSection = GetComponent<ForestSection>();
            forestSection.OnPolygonChanged += HandleForestChanged;
            forestSection.OnDestruction += HandleForestDestruction;
        }
        
        private void OnDestroy () {
            forestSection.OnPolygonChanged -= HandleForestChanged;
            forestSection.OnDestruction -= HandleForestDestruction;
        }

        private void HandleForestChanged (ForestSection element, bool isClosed) {
            path = GeneratePathFromPoints(forestSection.Points);
            if (isClosed) {
                enabled=false;
            }
        }
        
        private void HandleForestDestruction (AStaticMapElement element) {
            Destroy(this);
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
