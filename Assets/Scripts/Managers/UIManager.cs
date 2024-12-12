using System.Globalization;
using UnityEngine;
using TMPro;

namespace PKDS.Managers
{
    /// <summary>
    /// Class <c>UIManager</c> handles the UI logic.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        /// <value>Property <c>Instance</c> represents the instance of the UI manager.</value>
        public static UIManager Instance;

        #region Panel Properties
        
            /// <value>Property <c>mainMenuPanel</c> represents the main menu panel.</value>
            [Header("Panel Properties")]
            public GameObject mainMenuPanel;

            /// <value>Property <c>creditsPanel</c> represents the credits panel.</value>
            public GameObject creditsPanel;

            /// <value>Property <c>gameStatsPanel</c> represents the game stats panel.</value>
            public GameObject gameStatsPanel;

            /// <value>Property <c>menuPanel</c> represents the pause panel.</value>
            public GameObject menuPanel;
        
        #endregion
        
        #region Unity Event Methodse

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
        
        #endregion
        
        #region Input Sanitizing Methods

            /// <summary>
            /// Method <c>CleanInputValue</c> cleans the input value.
            /// </summary>
            /// <param name="input">The input value.</param>
            /// <returns>The cleaned input value.</returns>
            public static string CleanInputValue(string input)
            {
                return input.Trim().Replace(" ", string.Empty);
            }
            
            /// <summary>
            /// Method <c>ParseFloatValue</c> parses the float value.
            /// </summary>
            /// <param name="input">The input value.</param>
            /// <returns>The parsed float value.</returns>
            public float? ParseFloatValue(string input)
            {
                input = CleanInputValue(input).Replace(',', '.');
                return float.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var value) ? value : null;
            }
            
            /// <summary>
            /// Method <c>ParseIntValue</c> parses the int value.
            /// </summary>
            /// <param name="value">The input value.</param>
            /// <returns>The parsed int value.</returns>
            public int? ParseIntValue(string value)
            {
                var floatValue = ParseFloatValue(value);
                return floatValue.HasValue ? Mathf.FloorToInt(floatValue.Value) : null;
            }

        #endregion
    }
}
