using System;
using PKDS.Controllers;
using PKDS.Entities;
using UnityEngine;
using PKDS.Properties;
using UnityEngine.SceneManagement;

namespace PKDS.Managers
{
    /// <summary>
    /// Class <c>GameManager</c> handles the game logic.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        /// <value>Property <c>Instance</c> represents the instance of the game manager.</value>
        public static GameManager Instance;
        
        /// <value>Property <c>OnGameStart</c> represents the event when the game starts.</value>
        public static event Action OnGameStart;
        
        #region State Properties

            /// <value>Property <c>IsGameStarted</c> represents if the game is started.</value>
            public bool IsGameStarted { get; private set; }

            /// <value>Property <c>IsInteractionEnabled</c> represents if interactions are enabled.</value>
            public bool IsInteractionEnabled { get; private set; }
            
            /// <value>Property <c>IsGamePaused</c> represents if the game is paused.</value>
            public bool IsGamePaused { get; private set; }
        
        #endregion
        
        #region Game Mode Properties
        
            /// <value>Property <c>gameModes</c> represents a list of game modes.</value>
            [Header("Game Mode Properties")]
            public GameMode[] gameModes;
            
            /// <value>Property <c>gameMode</c> represents the game mode.</value>
            private GameMode _gameMode;
            
            /// <value>Property <c>GameMode</c> represents the current game mode.</value>
            public GameMode GameMode
            {
                get => _gameMode;
                set => _gameMode = value;
            }
        
        #endregion
        
        #region Game Properties
            
            /// <value>Property <c>LoopBehaviour</c> represents the loop behaviour.</value>
            public Loop.Behaviour LoopBehaviour => _gameMode.loopBehaviour;
            
            /// <value>Property <c>GameTime</c> represents the time limit of the game.</value>
            public float GameTime => _gameMode.gameTime;

            /// <value>Property <c>GameTimeLeft</c> represents the time left of the game.</value>
            public float GameTimeLeft { get; private set; }
            
            /// <value>Property <c>MaxRoundTime</c> represents the maximum time of a round.</value>
            public float MaxRoundTime => _gameMode.maxRoundTime;
            
            /// <value>Property <c>MinRoundTime</c> represents the minimum time of a round.</value>
            public float MinRoundTime => _gameMode.minRoundTime;
            
            /// <value>Property <c>ShowScore</c> represents if the score is shown.</value>
            private bool ShowScore => _gameMode.showScore;

            /// <value>Property <c>round</c> represents the current round.</value>
            public int Round { get; set; }
            
            /// <value>Property <c>ZoomDelay</c> represents the delay of the zoom effect.</value>
            public float ZoomDelay => _gameMode.zoomDelay;

        #endregion
        
        #region Score Properties

            /// <value>Property <c>wins</c> represents the number of wins.</value>
            private int _wins;
            
            /// <value>Property <c>loses</c> represents the number of loses.</value>
            private int _loses;
        
        #endregion
        
        #region Controller Properties
        
            /// <value>Property <c>gameStatsController</c> represents the game stats' controller.</value>
            [Header("Controller Properties")]
            [SerializeField]
            private GameStatsController gameStatsController;
            
            /// <value>Property <c>menuController</c> represents the menu controller.</value>
            [SerializeField]
            private MenuController menuController;
            
        #endregion
        
        #region Unity Event Methods

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
                Time.timeScale = 1f;
            }
            
            /// <summary>
            /// Method <c>Update</c> is called every frame, if the MonoBehaviour is enabled.
            /// </summary>
            private void Update()
            {
                GameUpdate();
            }
        
        #endregion
        
        #region State Methods
        
            /// <summary>
            /// Method <c>PreventInteraction</c> prevents all interactions.
            /// </summary>
            public void PreventInteraction(bool prevent = true)
            {
                IsInteractionEnabled = !prevent;
            }
        
        #endregion
        
        #region Game Methods

            /// <summary>
            /// Method <c>StartGame</c> starts the game.
            /// </summary>
            public void GameStart()
            {
                // Reset the game time
                GameTimeLeft = GameTime;
                gameStatsController.UpdateTimeText(GameTimeLeft);
                gameStatsController.ShowTimer(GameTime > 0f);
                
                // Reset the score
                _wins = 0;
                _loses = 0;
                gameStatsController.UpdateWinsText(_wins);
                gameStatsController.UpdateLosesText(_loses);
                gameStatsController.ShowScore(ShowScore);
                
                // Play the music
                AudioManager.Instance.PlayShortenedBackgroundMusic(GameTime);
                
                // Start the game
                IsGameStarted = true;
                OnGameStart?.Invoke();
            }

            /// <summary>
            /// Method <c>GameUpdate</c> updates the game.
            /// </summary>
            private void GameUpdate()
            {
                if (!IsGameStarted || GameTime <= 0f)
                    return;
                // Update the game timer and the corresponding UI
                GameTimeLeft -= Time.deltaTime;
                GameTimeLeft = Mathf.Clamp(GameTimeLeft, 0f, GameTime);
                gameStatsController.UpdateTimeText(GameTimeLeft);
                // If the time is up, end the game
                if (GameTimeLeft > 0f)
                    return;
                GameOver();
            }
            
            /// <summary>
            /// Method <c>GameOver</c> ends the game.
            /// </summary>
            private void GameOver()
            {
                PreventInteraction();
                IsGameStarted = false;
                UIManager.Instance.menuPanel.SetActive(true);
                menuController.TitleText = "Game Over";
                AudioManager.Instance.StopBackgroundMusic();
            }
        
        #endregion
        
        #region Score Methods
        
            /// <summary>
            /// Method <c>IncreaseWinCount</c> increases the win count.
            /// </summary>
            public void IncreaseWinCount(int wins = 1)
            {
                _wins += wins;
                gameStatsController.UpdateWinsText(_wins);
            }
                
            /// <summary>
            /// Method <c>IncreaseLossCount</c> increases the loss count.
            /// </summary>
            public void IncreaseLossCount(int loses = 1)
            {
                _loses += loses;
                gameStatsController.UpdateLosesText(_loses);
            }
        
        #endregion
        
        #region Input Methods
        
            /// <summary>
            /// Method <c>OnEscapeKeyPress</c> handles the escape key press event.
            /// </summary>
            public void OnCancel()
            {
                if (!IsGameStarted)
                    return;
                TogglePauseGame();
            }
            
        #endregion
        
        #region Game Management Methods
            
            /// <summary>
            /// Method <c>TogglePauseGame</c> pauses or resumes the game.
            /// </summary> 
            private void TogglePauseGame()
            {
                Time.timeScale = IsGamePaused ? 1f : 0f;
                AudioManager.Instance.TogglePauseBackgroundMusic();
                UIManager.Instance.menuPanel.SetActive(!IsGamePaused);
                menuController.TitleText = "Pause";
                IsGamePaused = !IsGamePaused;
            }
            
            /// <summary>
            /// Method <c>MainMenu</c> goes to the main menu.
            /// </summary>
            public void MainMenu()
            {
                StopAllCoroutines();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }

            /// <summary>
            /// Method <c>QuitGame</c> quits the game.
            /// </summary>
            public static void QuitGame()
            {
                Application.Quit();
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                #endif
            }
        
        #endregion
    }
}
