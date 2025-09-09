using UnityEngine;

namespace CustomAssets.InteractionSystem.Examples
{
    public class TestInteractable : InteractableBase
    {
        protected override void OnPostInteractionCustom()
        {
            Debug.Log("Custom post interaction action executed.");
        }
    }
}