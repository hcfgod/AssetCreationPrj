using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CustomAssets.EditorTools.Editor
{
	/// <summary>
	/// Property drawer that groups fields into toolbar-style tabs, similar to Odin Inspector.
	/// Fields that share the same TabGroup (group name) will render a single tab bar and
	/// only draw the properties contained in the selected tab.
	/// </summary>
	[CustomPropertyDrawer(typeof(TabGroupAttribute))]
	public class TabGroupPropertyDrawer : PropertyDrawer
	{
		private const float TAB_BAR_HEIGHT = 22f;
		private const float SPACING = 2f;

		// Tracks active tab per object+group key
		private static readonly Dictionary<string, string> activeTabByGroupKey = new Dictionary<string, string>();
		// Tracks discovered tabs per group to draw a unified toolbar
		private static readonly Dictionary<string, SortedSet<string>> tabsByGroupKey = new Dictionary<string, SortedSet<string>>();
		// Tracks which property path should draw the toolbar for a given group (first by order then propertyPath)
		private static readonly Dictionary<string, string> toolbarOwnerByGroupKey = new Dictionary<string, string>();

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			TabGroupAttribute attr = (TabGroupAttribute)attribute;
			string groupKey = BuildGroupKey(property, attr);

			bool ownsToolbar = IsToolbarOwner(property, attr, groupKey);
			bool isVisible = IsVisibleForActiveTab(property, attr, groupKey);

			float height = 0f;
			if (ownsToolbar)
			{
				height += TAB_BAR_HEIGHT + SPACING;
			}
			if (isVisible)
			{
				height += EditorGUI.GetPropertyHeight(property, label, true);
			}
			return Mathf.Max(0f, height);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			TabGroupAttribute attr = (TabGroupAttribute)attribute;
			string groupKey = BuildGroupKey(property, attr);

			// Register this tab name in the group
			EnsureTabRegistered(groupKey, attr.TabName);
			EnsureToolbarOwner(property, attr, groupKey);
			EnsureActiveTabInitialized(groupKey);

			float y = position.y;
			bool ownsToolbar = IsToolbarOwner(property, attr, groupKey);
			if (ownsToolbar)
			{
				DrawToolbar(new Rect(position.x, y, position.width, TAB_BAR_HEIGHT), groupKey);
				y += TAB_BAR_HEIGHT + SPACING;
			}

			if (IsVisibleForActiveTab(property, attr, groupKey))
			{
				Rect propRect = new Rect(position.x, y, position.width, EditorGUI.GetPropertyHeight(property, label, true));
				EditorGUI.PropertyField(propRect, property, label, true);
			}
		}

		private static string BuildGroupKey(SerializedProperty property, TabGroupAttribute attr)
		{
			// Unique per-target-object and group name to prevent cross-object bleed
			int objId = property.serializedObject.targetObject != null ? property.serializedObject.targetObject.GetInstanceID() : 0;
			return objId + "::" + attr.GroupName;
		}

		private static bool IsVisibleForActiveTab(SerializedProperty property, TabGroupAttribute attr, string groupKey)
		{
			return !activeTabByGroupKey.TryGetValue(groupKey, out string active) || string.Equals(active, attr.TabName);
		}

		private static void EnsureActiveTabInitialized(string groupKey)
		{
			if (!activeTabByGroupKey.ContainsKey(groupKey))
			{
				if (tabsByGroupKey.TryGetValue(groupKey, out SortedSet<string> set) && set.Count > 0)
				{
					// pick first tab alphabetically as default
					foreach (var t in set) { activeTabByGroupKey[groupKey] = t; break; }
				}
				else
				{
					activeTabByGroupKey[groupKey] = "Default";
				}
			}
		}

		private static void EnsureTabRegistered(string groupKey, string tabName)
		{
			if (!tabsByGroupKey.TryGetValue(groupKey, out SortedSet<string> set))
			{
				set = new SortedSet<string>();
				tabsByGroupKey[groupKey] = set;
			}
			set.Add(tabName);
		}

		private static void EnsureToolbarOwner(SerializedProperty property, TabGroupAttribute attr, string groupKey)
		{
			if (!toolbarOwnerByGroupKey.TryGetValue(groupKey, out string ownerKey))
			{
				// First encountered property in this group becomes the owner
				toolbarOwnerByGroupKey[groupKey] = BuildOwnerKey(attr.Order, property.propertyPath);
			}
			else
			{
				// Only replace if a strictly lower order is provided
				(string currentPath, int currentOrder) = ParseOwnerKey(ownerKey);
				if (attr.Order < currentOrder)
				{
					toolbarOwnerByGroupKey[groupKey] = BuildOwnerKey(attr.Order, property.propertyPath);
				}
			}
		}

		private static string BuildOwnerKey(int order, string propertyPath)
		{
			return order.ToString() + "|" + propertyPath;
		}

		private static (string path, int order) ParseOwnerKey(string key)
		{
			int sep = key.IndexOf('|');
			if (sep <= 0) return (key, int.MaxValue);
			int.TryParse(key.Substring(0, sep), out int order);
			string path = key.Substring(sep + 1);
			return (path, order);
		}

		private static bool IsToolbarOwner(SerializedProperty property, TabGroupAttribute attr, string groupKey)
		{
			if (!toolbarOwnerByGroupKey.TryGetValue(groupKey, out string ownerKey)) return false;
			(string ownerPath, int _) = ParseOwnerKey(ownerKey);
			return ownerPath == property.propertyPath;
		}

		private static void DrawToolbar(Rect rect, string groupKey)
		{
			if (!tabsByGroupKey.TryGetValue(groupKey, out SortedSet<string> set) || set.Count == 0)
			{
				return;
			}

			// Convert set to array for display order (alphabetical). Could be improved via custom order metadata later.
			string[] tabs = new string[set.Count];
			int i = 0; foreach (var t in set) { tabs[i++] = t; }

			int currentIndex = 0;
			if (activeTabByGroupKey.TryGetValue(groupKey, out string active))
			{
				for (int idx = 0; idx < tabs.Length; idx++)
				{
					if (tabs[idx] == active) { currentIndex = idx; break; }
				}
			}

			// Draw nice toolbar tabs
			int newIndex = GUI.Toolbar(rect, currentIndex, tabs, EditorStyles.toolbarButton);
			if (newIndex != currentIndex)
			{
				activeTabByGroupKey[groupKey] = tabs[newIndex];
			}
		}
	}
}


