using UnityEngine;

namespace CustomAssets.EditorTools.Examples
{
	public class TabGroupExample : MonoBehaviour
	{
		[Header("Stats")]
		[TabGroup("Stats", "Base", order: 0)] public int health = 100;
		[TabGroup("Stats", "Base", order: 0)] public int stamina = 50;
		[TabGroup("Stats", "Advanced", order: 0)] public AnimationCurve regenCurve = AnimationCurve.Linear(0, 0, 1, 1);

		[Header("Inventory")]
		[TabGroup("Inventory", "Weapons", order: 0)] public string primaryWeapon;
		[TabGroup("Inventory", "Armor", order: 0)] public string chestArmor;
		[TabGroup("Inventory", "Armor", order: 0)] public string helmet;
	}
}


