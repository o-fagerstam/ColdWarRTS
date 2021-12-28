using Utils;
namespace Constants {
	public static class GeographyConstants {

		#region Elevation
		/// <summary>
		/// Maximum permissible elevation
		/// </summary>
		public static readonly float MAP_ELEVATION_MAX = ScaleUtil.GameToUnity(500f);
		/// <summary>
		/// Minimum permissible elevation
		/// </summary>
		public static readonly float MAP_ELEVATION_MIN = ScaleUtil.GameToUnity(-500f);
		/// <summary>
		///  Water level of the map
		/// </summary>
		public static readonly float MAP_WATER_LEVEL = ScaleUtil.GameToUnity(-20f);
		#endregion


		#region Trees
		/// <summary>
		/// Maximum angle at which trees will appear on a slope
		/// </summary>
		public static readonly float TREE_SLOPE_ANGLE_MAX = 63f;
		#endregion
	}
}
