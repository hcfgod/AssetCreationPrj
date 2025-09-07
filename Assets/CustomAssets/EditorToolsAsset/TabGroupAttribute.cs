using UnityEngine;

namespace CustomAssets.EditorTools
{
	/// <summary>
	/// Attribute that groups multiple fields into a shared tab group in the Inspector.
	/// Similar to Odin Inspector's TabGroup. Fields that share the same group and tab
	/// name will be shown together when that tab is selected.
	/// </summary>
	/// <example>
	/// <code>
	/// public class TabGroupExample : MonoBehaviour
	/// {
	/// 	[TabGroup("Stats", "Base")] public int health = 100;
	/// 	[TabGroup("Stats", "Base")] public int stamina = 50;
	/// 	[TabGroup("Stats", "Advanced")] public AnimationCurve regenCurve = AnimationCurve.Linear(0,0,1,1);
	/// 
	/// 	[TabGroup("Inventory", "Weapons")] public string primaryWeapon;
	/// 	[TabGroup("Inventory", "Armor")] public string chestArmor;
	/// }
	/// </code>
	/// </example>
	public class TabGroupAttribute : PropertyAttribute
	{
		/// <summary>
		/// Logical group name that ties several fields together.
		/// </summary>
		public string GroupName { get; }

		/// <summary>
		/// The tab name within the group that this field belongs to.
		/// </summary>
		public string TabName { get; }

		/// <summary>
		/// Optional ordering hint used to choose which field draws the tab header.
		/// Lower values have higher priority. Defaults to 0.
		/// </summary>
		public int Order { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="TabGroupAttribute"/>.
		/// </summary>
		/// <param name="groupName">Shared group name that binds fields together.</param>
		/// <param name="tabName">Tab name that this field belongs to inside the group.</param>
		/// <param name="order">Optional ordering hint for which field should draw the tabs header.</param>
		public TabGroupAttribute(string groupName, string tabName, int order = 0)
		{
			GroupName = string.IsNullOrEmpty(groupName) ? "Tabs" : groupName;
			TabName = string.IsNullOrEmpty(tabName) ? "Default" : tabName;
			Order = order;
		}
	}
}


