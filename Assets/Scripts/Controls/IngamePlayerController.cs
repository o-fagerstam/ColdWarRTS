using UnityEngine;
namespace Controls {
	[RequireComponent(typeof(UnitCommandGiver), typeof(UnitSelector))]
	public class IngamePlayerController : ARtsController {

		private UnitSelector unitSelector;
		private UnitCommandGiver unitCommandGiver;

		protected override void OnEnable () {
			base.OnEnable();
			unitSelector = GetComponent<UnitSelector>();
			unitCommandGiver = GetComponent<UnitCommandGiver>();
		}

		protected override void UpdateMouseControl () {
			unitSelector.UpdateSelection();
			unitCommandGiver.UpdateUnitCommands();
		}
	}
}
