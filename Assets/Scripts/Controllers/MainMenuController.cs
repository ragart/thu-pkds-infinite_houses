using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PKDS.Entities;
using PKDS.Managers;
using PKDS.Properties;

namespace PKDS.Controllers
{
    /// <summary>
    /// Method <c>MainMenuController</c> handles the main menu logic.
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        #region Menu Container Properties

            /// <value>Property <c>gameModeLeftButton</c> represents the game mode left button.</value>
            [Header("Menu Container Properties")]
            [SerializeField]
            private Button gameModeLeftButton;

            /// <value>Property <c>gameModeText</c> represents the text containing the game mode.</value>
            [SerializeField]
            private TextMeshProUGUI gameModeText;

            /// <value>Property <c>gameModeRightButton</c> represents the game mode right button.</value>
            [SerializeField]
            private Button gameModeRightButton;

            /// <value>Property <c>startGameButton</c> represents the start game button.</value>
            [SerializeField]
            private Button startGameButton;

            /// <value>Property <c>creditsButton</c> represents the credits button.</value>
            [SerializeField]
            private Button creditsButton;

            /// <value>Property <c>quitGameButton</c> represents the quit game button.</value>
            [SerializeField]
            private Button quitGameButton;
        
        #endregion
        
        #region Custom Container Properties
        
            /// <value>Property <c>customGameMode</c> represents the custom game mode.</value>
            [Header("Custom Game Mode Properties")]
            [SerializeField]
            private GameMode customGameMode;
            
            /// <value>Property <c>_loopBehaviours</c> represents the loop behaviours.</value>
            private Loop.Behaviour[] _loopBehaviours;
            
            private List<string> _loopBehavioursList;
            
            /// <value>Property <c>_filteredLoopBehaviours</c> represents the filtered loop behaviours.</value>
            private Loop.Behaviour[] _filteredLoopBehaviours;
            
            private List<string> _filteredLoopBehavioursList;

            /// <value>Property <c>customContainer</c> represents the custom container.</value>
            [SerializeField]
            private GameObject customContainer;

            /// <value>Property <c>behaviourDropdown</c> represents the loop behaviour dropdown.</value>
            [SerializeField]
            private TMP_Dropdown behaviourDropdown;
            
            /// <value>Property <c>gameTimeInputField</c> represents the game time input field.</value>
            [SerializeField]
            private TMP_InputField gameTimeInputField;
            
            /// <value>Property <c>maxRoundTimeInputField</c> represents the maximum round time input field.</value>
            [SerializeField]
            private TMP_InputField maxRoundTimeInputField;
            
            /// <value>Property <c>minRoundTimeInputField</c> represents the minimum round time input field.</value>
            [SerializeField]
            private TMP_InputField minRoundTimeInputField;
            
            /// <value>Property <c>showScoreDropdown</c> represents the show score dropdown.</value>
            [SerializeField]
            private TMP_Dropdown showScoreDropdown;
            
            /// <value>Property <c>zoomDelayInputField</c> represents the zoom delay input field.</value>
            [SerializeField]
            private TMP_InputField transitionDelayInputField;

        #endregion
        
        #region Unity Event Methods
        
            /// <summary>
            /// Method <c>Awake</c> is called when the script instance is being loaded.
            /// </summary>
            private void Awake()
            {
                // Initialize the loop behaviours
                _loopBehaviours = (Loop.Behaviour[]) Enum.GetValues(typeof(Loop.Behaviour));
                _loopBehavioursList = new List<string>(_loopBehaviours.Select(behaviour => behaviour.ToString()));
                _filteredLoopBehaviours = _loopBehaviours
                    .Where(behaviour => behaviour != Loop.Behaviour.None && behaviour != Loop.Behaviour.Start)
                    .ToArray();
                _filteredLoopBehavioursList = new List<string>(_filteredLoopBehaviours.Select(behaviour => behaviour.ToString()));
            }

            /// <summary>
            /// Method <c>Start</c> is called before the first frame update.
            /// </summary>
            private void Start()
            {
                // Set the starting game mode
                SetStartingGameMode();
                
                // Populate the custom container
                PopulateCustomStartingValues();
                
                // Set the listeners
                gameModeLeftButton.onClick.AddListener(OnGameModeLeftButtonClick);
                gameModeRightButton.onClick.AddListener(OnGameModeRightButtonClick);
                startGameButton.onClick.AddListener(OnStartGameButtonClick);
                creditsButton.onClick.AddListener(OnCreditsButtonClick);
                quitGameButton.onClick.AddListener(OnQuitGameButtonClick);
                behaviourDropdown.onValueChanged.AddListener(OnBehaviourDropdownValueChanged);
                gameTimeInputField.onEndEdit.AddListener(OnGameTimeInputFieldEndEdit);
                maxRoundTimeInputField.onEndEdit.AddListener(OnMaxRoundTimeInputFieldEndEdit);
                minRoundTimeInputField.onEndEdit.AddListener(OnMinRoundTimeInputFieldEndEdit);
                showScoreDropdown.onValueChanged.AddListener(OnShowScoreDropdownValueChanged);
                transitionDelayInputField.onEndEdit.AddListener(OnTransitionDelayInputFieldEndEdit);
            }
        
        #endregion

        /// <summary>
        /// Method <c>SetStartingGameMode</c> sets the starting game mode.
        /// </summary>
        private void SetStartingGameMode()
        {
            GameManager.Instance.GameMode = GameManager.Instance.gameModes[0];
            gameModeLeftButton.gameObject.SetActive(false);
        }

        /// <summary>
        /// Method <c>PopulateCustomStartingValues</c> populates the custom starting values.
        /// </summary>
        private void PopulateCustomStartingValues()
        {
            // Populate the loop behaviour dropdown
            behaviourDropdown.ClearOptions();
            behaviourDropdown.AddOptions(_filteredLoopBehavioursList);
            
            // Set the default value of the loop behaviour dropdown
            behaviourDropdown.value = _loopBehavioursList.IndexOf(customGameMode.loopBehaviour.ToString());
            
            // Set the default value of the game time input field
            gameTimeInputField.text = customGameMode.gameTime.ToString(CultureInfo.InvariantCulture);
            
            // Set the default value of the maximum round time input field
            maxRoundTimeInputField.text = customGameMode.maxRoundTime.ToString(CultureInfo.InvariantCulture);
            
            // Set the default value of the minimum round time input field
            minRoundTimeInputField.text = customGameMode.minRoundTime.ToString(CultureInfo.InvariantCulture);
            
            // Set the default value of the show score dropdown
            showScoreDropdown.value = customGameMode.showScore ? 0 : 1;
            
            // Set the default value of the zoom delay input field
            transitionDelayInputField.text = customGameMode.zoomDelay.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Method <c>OnGameModeLeftButtonClick</c> handles the game mode left button click event.
        /// </summary>
        private void OnGameModeLeftButtonClick()
        {
            var gameModeIndex = Array.IndexOf(GameManager.Instance.gameModes, GameManager.Instance.GameMode);
            if (gameModeIndex == 0)
                return;
            gameModeIndex--;
            GameManager.Instance.GameMode = GameManager.Instance.gameModes[gameModeIndex];
            gameModeText.text = GameManager.Instance.GameMode.name;
            gameModeRightButton.gameObject.SetActive(true);
            if (gameModeIndex == 0)
                gameModeLeftButton.gameObject.SetActive(false);
        }
        
        /// <summary>
        /// Method <c>OnGameModeRightButtonClick</c> handles the game mode right button click event.
        /// </summary>
        private void OnGameModeRightButtonClick()
        {
            var gameModeIndex = Array.IndexOf(GameManager.Instance.gameModes, GameManager.Instance.GameMode);
            if (gameModeIndex == GameManager.Instance.gameModes.Length - 1)
                return;
            gameModeIndex++;
            HandleGameModeChange(GameManager.Instance.gameModes[gameModeIndex]);
            gameModeLeftButton.gameObject.SetActive(true);
            if (gameModeIndex == GameManager.Instance.gameModes.Length - 1)
                gameModeRightButton.gameObject.SetActive(false);
        }
        
        /// <summary>
        /// Method <c>HandleGameModeChange</c> handles the game mode change.
        /// </summary>
        /// <param name="gameMode">The game mode to change to.</param>
        private void HandleGameModeChange(GameMode gameMode)
        {
            gameModeText.text = gameMode.name;
            GameManager.Instance.GameMode = gameMode;
            customContainer.SetActive(gameMode.name == "Custom");
        }
        
        /// <summary>
        /// Method <c>OnStartGameButtonClick</c> handles the start game button click event.
        /// </summary>
        private void OnStartGameButtonClick()
        {
            gameObject.SetActive(false);
            UIManager.Instance.gameStatsPanel.SetActive(true);
            GameManager.Instance.GameStart();
        }
        
        /// <summary>
        /// Method <c>OnCreditsButtonClick</c> handles the credits button click event.
        /// </summary>
        private void OnCreditsButtonClick()
        {
            gameObject.SetActive(false);
            UIManager.Instance.creditsPanel.SetActive(true);
        }

        /// <summary>
        /// Method <c>OnQuitGameButtonClick</c> handles the quit game button click event.
        /// </summary>
        private static void OnQuitGameButtonClick()
        {
            GameManager.QuitGame();
        }
        
        /// <summary>
        /// Method <c>OnBehaviourDropdownValueChanged</c> handles the behaviour dropdown value changed event.
        /// </summary>
        /// <param name="value">The value of the dropdown.</param>
        private void OnBehaviourDropdownValueChanged(int value)
        {
            var behaviour = _filteredLoopBehaviours[value];
            customGameMode.loopBehaviour = behaviour;
        }
        
        /// <summary>
        /// Method <c>OnGameTimeInputFieldEndEdit</c> handles the game time input field end edit event.
        /// </summary>
        /// <param name="value">The value of the input field.</param>
        private void OnGameTimeInputFieldEndEdit(string value)
        {
            ProcessInputTextValue<int>(
                gameTimeInputField,
                value,
                customGameMode.gameTime.ToString(CultureInfo.InvariantCulture),
                typeof(int),
                newValue => customGameMode.gameTime = newValue,
                0.0f,
                300.0f);
        }
        
        /// <summary>
        /// Method <c>OnMaxRoundTimeInputFieldEndEdit</c> handles the maximum round time input field end edit event.
        /// </summary>
        /// <param name="value">The value of the input field.</param>
        private void OnMaxRoundTimeInputFieldEndEdit(string value)
        {
            ProcessInputTextValue<int>(
                maxRoundTimeInputField,
                value,
                customGameMode.maxRoundTime.ToString(CultureInfo.InvariantCulture),
                typeof(int),
                newValue => customGameMode.maxRoundTime = newValue,
                0.0f,
                10.0f);
        }
        
        /// <summary>
        /// Method <c>OnMinRoundTimeInputFieldEndEdit</c> handles the minimum round time input field end edit event.
        /// </summary>
        /// <param name="value">The value of the input field.</param>
        private void OnMinRoundTimeInputFieldEndEdit(string value)
        {
            ProcessInputTextValue<int>(
                minRoundTimeInputField,
                value,
                customGameMode.minRoundTime.ToString(CultureInfo.InvariantCulture),
                typeof(int),
                newValue => customGameMode.minRoundTime = newValue,
                0.0f,
                10.0f);
        }
        
        /// <summary>
        /// Method <c>OnShowScoreDropdownValueChanged</c> handles the show score dropdown value changed event.
        /// </summary>
        /// <param name="value">The value of the dropdown.</param>
        private void OnShowScoreDropdownValueChanged(int value)
        {
            customGameMode.showScore = value == 0;
        }

        /// <summary>
        /// Method <c>OnTransitionDelayInputFieldEndEdit</c> handles the transition delay input field end edit event.
        /// </summary>
        /// <param name="value">The value of the input field.</param>
        private void OnTransitionDelayInputFieldEndEdit(string value)
        {
            ProcessInputTextValue<float>(
                transitionDelayInputField,
                value,
                customGameMode.zoomDelay.ToString(CultureInfo.InvariantCulture),
                typeof(float),
                newValue => customGameMode.zoomDelay = newValue,
                0.1f,
                10.0f);

        }

        /// <summary>
        /// Method <c>ProcessInputTextValue</c> processes the input text value.
        /// </summary>
        /// <param name="inputField">The input field to be updated.</param>
        /// <param name="value">The updated value.</param>
        /// <param name="originalValue">The original value.</param>
        /// <param name="type">The type of the value.</param>
        /// <param name="updateProperty">The method to update the property.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <typeparam name="T">The type of the value.</typeparam>
        private static void ProcessInputTextValue<T>(TMP_InputField inputField, string value, string originalValue, Type type, Action<T> updateProperty, float? min = null, float? max = null) where T : struct
        {
            value = UIManager.CleanInputValue(value);

            if (type == typeof(int))
            {
                if (UIManager.Instance.ParseIntValue(value) is not { } intValue)
                {
                    inputField.text = originalValue;
                    return;
                }
                if (min.HasValue && max.HasValue)
                    intValue = Mathf.Clamp(intValue, Mathf.FloorToInt(min.Value), Mathf.FloorToInt(max.Value));
                inputField.text = intValue.ToString(CultureInfo.InvariantCulture);
                updateProperty((T)(object)intValue);
            }
            else if (type == typeof(float))
            {
                if (UIManager.Instance.ParseFloatValue(value) is not { } floatValue)
                {
                    inputField.text = originalValue;
                    return;
                }
                if (min.HasValue && max.HasValue)
                    floatValue = Mathf.Clamp(floatValue, min.Value, max.Value);
                inputField.text = floatValue.ToString(CultureInfo.InvariantCulture);
                updateProperty((T)(object)floatValue);
            }
            else if (type == typeof(string))
            {
                inputField.text = value;
                updateProperty((T)(object)value);
            }
        }
    }
}
