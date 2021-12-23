namespace Utils {
	public static class ScaleUtil {
		// UnityScale:IngameScale is 1:scale
		public static readonly float scale = 50;

		/// <summary>
		/// Convert game scale to Unity Scale
		/// </summary>
		public static float GameToUnity (float input) => input/scale;
		/// <summary>
		/// Convert Unity scale to game scale
		/// </summary>
		public static float UnityToGame (float input) => input*scale;
	}
}
