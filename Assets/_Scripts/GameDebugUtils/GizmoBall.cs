using UnityEngine;
namespace GameDebugUtils {
	public class GizmoBall : MonoBehaviour {

		private float _radius = 1f;
		private Color _color = Color.black;
		private float _destroyTime = float.PositiveInfinity;

		public void SetAttributes (float radius, Color color, float lifeTime) {
			this._radius = radius;
			this._color = color;
			this._destroyTime = Time.time + lifeTime;
		}

		private void Update () {
			if (Time.time >= _destroyTime) {Destroy(gameObject);}
		}

		private void OnDrawGizmos () {
			Gizmos.color = _color;
			Gizmos.DrawSphere(transform.position, _radius);
		}
	}
}
