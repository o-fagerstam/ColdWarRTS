using Utils;
namespace Constants {
	public static class GeographyConstants {

		#region Basic
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
		public static readonly float MAP_WATER_LEVEL = ScaleUtil.GameToUnity(-50f);
		#endregion


		#region Trees
		/// <summary>
		/// Maximum angle at which trees will appear on a slope
		/// </summary>
		public static readonly float TREE_SLOPE_ANGLE_MAX = 63f;
		/// <summary>
		/// Minimum height over water that trees will grow
		/// </summary>
		public static readonly float TREE_ELEVATION_OVER_WATER_MIN = MAP_WATER_LEVEL + ScaleUtil.GameToUnity(3f);
		#endregion
	}
}
