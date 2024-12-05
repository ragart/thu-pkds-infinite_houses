using UnityEngine;

namespace PKDS.Managers
{
    /// <summary>
    /// Class <c>GameManager</c> handles the game logic.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        /// <value>Property <c>Instance</c> represents the instance of the game manager.</value>
        public static GameManager Instance;

        /// <value>Property <c>areAllInteractionsPrevented</c> represents if the level is transitioning.</value>
        public bool areAllInteractionsPrevented;

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
