using Shapes;
using UnityEngine;
namespace Map {
	[RequireComponent(typeof(CapturePoint)), ExecuteAlways]
	public class CapturePointOutlineDrawer : ImmediateModeShapeDrawer {
		private CapturePoint capturePoint;

		private void Awake () {
			capturePoint = GetComponent<CapturePoint>();
		}

		public override void OnEnable () {
			base.OnEnable();
			capturePoint.OnShapeChanged += HandleShapeChanged;
			capturePoint.OnDestruction += HandleCapturePointDestruction;
		}
		
		public override void OnDisable () {
			base.OnDisable();
			capturePoint.OnShapeChanged -= HandleShapeChanged;
			capturePoint.OnDestruction -= HandleCapturePointDestruction;
		}
		
		private void HandleCapturePointDestruction (AStaticMapElement obj) {
			Destroy(this);
		}
		private void HandleShapeChanged (AStaticMapElement obj) {}

		public override void DrawShapes (Camera cam) {
			base.DrawShapes(cam);
			using (Draw.Command(cam)) {
				Draw.LineGeometry = LineGeometry.Volumetric3D;
				Draw.ThicknessSpace = ThicknessSpace.Noots;
				Draw.Thickness = 1f;
				Draw.Ring(capturePoint.transform.position + Vector3.up * 0.05f, Vector3.up, capturePoint.Radius, Color.white);
			}
		}
	}
}
