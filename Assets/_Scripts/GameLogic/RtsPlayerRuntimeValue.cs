using Architecture.ScriptableObjectArchitecture;
using UnityEngine;
namespace GameLogic {
	[CreateAssetMenu(fileName = "Rts Player Runtime Value", menuName = "Scriptable Objects/Player/Rts Player Runtime Value")]
	public class RtsPlayerRuntimeValue : RuntimeScriptableValue<RtsPlayer> {}
}
