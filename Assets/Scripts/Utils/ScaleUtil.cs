using Constants;
namespace Utils {
	public static class ScaleUtil {
		/// <summary>
		/// Convert game scale to Unity Scale
		/// </summary>
		public static float GameToUnity (float meters) => meters/ScaleConstants.GAME_SCALE;
		/// <summary>
		/// Convert Unity scale to game scale
		/// </summary>
		public static float UnityToGame (float units) => units*ScaleConstants.GAME_SCALE;
	}
}
