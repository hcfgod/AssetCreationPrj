using UnityEngine;

/** Summary:
 * The IInteractable interface defines the contract for interactable objects in the interaction system.
 * It includes methods for starting, progressing, and ending interactions.
 */
namespace CustomAssets.InteractionSystem
{
    public struct InteractionData
    {
        public GameObject Interactor;
        public GameObject InteractionTarget;
    }

    public interface IInteractable
    {
        // Method to be called when interaction starts
        void OnInteractStart(InteractionData interactionData);

        // Method to be called to report progress of an ongoing interaction
        void OnInteractProgress(InteractionData interactionData, float progress);

        // Method to be called when interaction ends
        void OnInteractEnd(InteractionData interactionData);
    }
}