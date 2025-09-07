using UnityEngine;
using CustomAssets.EditorTools;

namespace CustomAssets.EditorTools.Examples
{
    /// <summary>
    /// Example script demonstrating the Button attribute functionality.
    /// This script shows various ways to use the Button attribute with different configurations.
    /// </summary>
    public class ButtonExample : MonoBehaviour
    {
        [Header("Button Examples")]
        [SerializeField] private int counter = 0;
        [SerializeField] private string message = "Hello World!";
        [SerializeField] private Color randomColor = Color.white;

        [Header("Settings")]
        [SerializeField] private bool enableLogging = true;

        /// <summary>
        /// Simple button with default settings.
        /// </summary>
        [Button]
        public void SimpleButton()
        {
            if (enableLogging)
                Debug.Log("Simple button clicked!");
            
            counter++;
        }

        /// <summary>
        /// Button with custom text.
        /// </summary>
        [Button("Increment Counter")]
        public void IncrementCounter()
        {
            counter++;
            if (enableLogging)
                Debug.Log($"Counter incremented to: {counter}");
        }

        /// <summary>
        /// Button with custom text and height.
        /// </summary>
        [Button("Reset Counter", 30f)]
        public void ResetCounter()
        {
            counter = 0;
            if (enableLogging)
                Debug.Log("Counter reset to 0");
        }

        /// <summary>
        /// Button with custom dimensions and order.
        /// </summary>
        [Button("Random Color", 25f, 150f, 10)]
        public void SetRandomColor()
        {
            randomColor = new Color(
                Random.Range(0f, 1f),
                Random.Range(0f, 1f),
                Random.Range(0f, 1f),
                1f
            );
            
            if (enableLogging)
                Debug.Log($"Random color set: {randomColor}");
        }

        /// <summary>
        /// Button that only works in play mode.
        /// </summary>
        [Button("Play Mode Only", true)]
        public void PlayModeOnlyButton()
        {
            if (enableLogging)
                Debug.Log("This button only works in play mode!");
        }

        /// <summary>
        /// Button that only works in edit mode.
        /// </summary>
        [Button("Edit Mode Only", false, true)]
        public void EditModeOnlyButton()
        {
            if (enableLogging)
                Debug.Log("This button only works in edit mode!");
        }

        /// <summary>
        /// Button with custom colors.
        /// </summary>
        [Button("Colored Button", "#FFFF00", "#FF0000")]
        public void ColoredButton()
        {
            if (enableLogging)
                Debug.Log("Colored button clicked!");
        }

        /// <summary>
        /// Button with confirmation dialog.
        /// </summary>
        [Button("Dangerous Action", true, "Are you sure you want to execute this dangerous action?")]
        public void DangerousAction()
        {
            if (enableLogging)
                Debug.Log("Dangerous action executed!");
        }

        /// <summary>
        /// Button that modifies the GameObject.
        /// </summary>
        [Button("Toggle GameObject")]
        public void ToggleGameObject()
        {
            gameObject.SetActive(!gameObject.activeSelf);
            if (enableLogging)
                Debug.Log($"GameObject active: {gameObject.activeSelf}");
        }

        /// <summary>
        /// Button that adds a component.
        /// </summary>
        [Button("Add Rigidbody")]
        public void AddRigidbody()
        {
            if (GetComponent<Rigidbody>() == null)
            {
                gameObject.AddComponent<Rigidbody>();
                if (enableLogging)
                    Debug.Log("Rigidbody added to GameObject");
            }
            else
            {
                if (enableLogging)
                    Debug.Log("Rigidbody already exists on GameObject");
            }
        }

        /// <summary>
        /// Button that removes a component.
        /// </summary>
        [Button("Remove Rigidbody")]
        public void RemoveRigidbody()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                DestroyImmediate(rb);
                if (enableLogging)
                    Debug.Log("Rigidbody removed from GameObject");
            }
            else
            {
                if (enableLogging)
                    Debug.Log("No Rigidbody found on GameObject");
            }
        }

        /// <summary>
        /// Button that logs all current values.
        /// </summary>
        [Button("Log Current Values")]
        public void LogCurrentValues()
        {
            Debug.Log($"Counter: {counter}");
            Debug.Log($"Message: {message}");
            Debug.Log($"Random Color: {randomColor}");
            Debug.Log($"Enable Logging: {enableLogging}");
        }

        /// <summary>
        /// Button that demonstrates method chaining with the attribute.
        /// </summary>
        [Button("Chained Button", 25f, 0f, 5)]
        public void ChainedButton()
        {
            if (enableLogging)
                Debug.Log("Chained button clicked!");
        }

        /// <summary>
        /// Button that shows how to handle errors gracefully.
        /// </summary>
        [Button("Error Handling Test")]
        public void ErrorHandlingTest()
        {
            try
            {
                // Simulate an error
                throw new System.Exception("This is a test error");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error in ErrorHandlingTest: {e.Message}");
            }
        }

        /// <summary>
        /// Button that demonstrates working with multiple objects.
        /// </summary>
        [Button("Find All Buttons")]
        public void FindAllButtons()
        {
            ButtonExample[] allButtons = FindObjectsByType<ButtonExample>(FindObjectsSortMode.None);
            if (enableLogging)
                Debug.Log($"Found {allButtons.Length} ButtonExample components in the scene");
        }

        /// <summary>
        /// Button that demonstrates coroutine usage.
        /// </summary>
        [Button("Start Coroutine")]
        public void StartCoroutine()
        {
            StartCoroutine(ExampleCoroutine());
        }

        private System.Collections.IEnumerator ExampleCoroutine()
        {
            if (enableLogging)
                Debug.Log("Coroutine started");
            
            yield return new WaitForSeconds(1f);
            
            if (enableLogging)
                Debug.Log("Coroutine finished");
        }

        /// <summary>
        /// Button that demonstrates working with transforms.
        /// </summary>
        [Button("Randomize Position")]
        public void RandomizePosition()
        {
            transform.position = new Vector3(
                Random.Range(-5f, 5f),
                Random.Range(-5f, 5f),
                Random.Range(-5f, 5f)
            );
            
            if (enableLogging)
                Debug.Log($"New position: {transform.position}");
        }

        /// <summary>
        /// Button that demonstrates working with materials.
        /// </summary>
        [Button("Change Material Color")]
        public void ChangeMaterialColor()
        {
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null && renderer.material != null)
            {
                renderer.material.color = randomColor;
                if (enableLogging)
                    Debug.Log($"Material color changed to: {randomColor}");
            }
            else
            {
                if (enableLogging)
                    Debug.Log("No renderer or material found");
            }
        }
    }
}
