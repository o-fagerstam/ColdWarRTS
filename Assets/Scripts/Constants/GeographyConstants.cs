using Utils;
namespace Constants {
	public static class GeographyConstants {
		public static readonly float MAX_ELEVATION = ScaleUtil.GameToUnity(500f);
		public static readonly float MIN_ELEVATION = ScaleUtil.GameToUnity(-500f);
		public static readonly float WATER_LEVEL = ScaleUtil.GameToUnity(-20f);
	}
}
