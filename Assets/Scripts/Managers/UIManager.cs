using UnityEngine;

namespace PKDS.Managers
{
    /// <summary>
    /// Class <c>UIManager</c> handles the UI logic.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        /// <value>Property <c>Instance</c> represents the instance of the UI manager.</value>
        public static UIManager Instance;

        /// <summary>
        /// Method <c>Awake</c> initializes the game manager.
        /// </summary>
        private void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
    }
}
