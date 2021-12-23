using UnityEngine;
namespace Debug {
	public class GizmoBall : MonoBehaviour {

		private float radius = 1f;
		private Color color = Color.black;
		private float destroyTime = float.PositiveInfinity;

		public void SetAttributes (float radius, Color color, float lifeTime) {
			this.radius = radius;
			this.color = color;
			this.destroyTime = Time.time + lifeTime;
		}

		private void Update () {
			if (Time.time >= destroyTime) {Destroy(gameObject);}
		}

		private void OnDrawGizmos () {
			Gizmos.color = color;
			Gizmos.DrawSphere(transform.position, radius);
		}
	}
}
