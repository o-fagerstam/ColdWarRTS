using Units.Commands;
using UnityEngine;
namespace Controls {
	[RequireComponent(typeof(UnitCommandGiver), typeof(UnitSelector))]
	public class IngamePlayerController : ARtsController {

		private UnitSelector _unitSelector;
		private UnitCommandGiver _unitCommandGiver;

		protected override void OnEnable () {
			base.OnEnable();
			_unitSelector = GetComponent<UnitSelector>();
			_unitCommandGiver = GetComponent<UnitCommandGiver>();
		}

		protected override void UpdateMouseControl () {
			_unitSelector.UpdateSelection();
			_unitCommandGiver.UpdateUnitCommands();
		}
	}
}
