using System;
using PKDS.Entities;
using UnityEngine;
using PKDS.Properties;

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
        
        #endregion
        
        #region Game Properties
        
            /// <value>Property <c>gameMode</c> represents the game mode.</value>
            [Header("Game Properties")]
            [SerializeField]
            private GameMode gameMode;
            
            /// <value>Property <c>GameMode</c> represents the game mode.</value>
            public GameMode GameMode => gameMode;
            
            /// <value>Property <c>LoopBehaviour</c> represents the loop behaviour.</value>
            public Loop.Behaviour LoopBehaviour => gameMode.loopBehaviour;
            
            /// <value>Property <c>GameTime</c> represents the time limit of the game.</value>
            public float GameTime => gameMode.gameTime;

            /// <value>Property <c>GameTimeLeft</c> represents the time left of the game.</value>
            public float GameTimeLeft { get; private set; }
            
            /// <value>Property <c>MaxRoundTime</c> represents the maximum time of a round.</value>
            public float MaxRoundTime => gameMode.maxRoundTime;
            
            /// <value>Property <c>MinRoundTime</c> represents the minimum time of a round.</value>
            public float MinRoundTime => gameMode.minRoundTime;
            
            /// <value>Property <c>ShowScore</c> represents if the score is shown.</value>
            public bool ShowScore => gameMode.showScore;

            /// <value>Property <c>round</c> represents the current round.</value>
            public int Round { get; set; }
            
            /// <value>Property <c>ZoomDelay</c> represents the delay of the zoom effect.</value>
            public float ZoomDelay => gameMode.zoomDelay;

            #endregion
        
        #region Score Properties

            /// <value>Property <c>wins</c> represents the number of wins.</value>
            private int _wins;
            
            /// <value>Property <c>loses</c> represents the number of loses.</value>
            private int _loses;
        
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
                LoadGameMode(gameMode);
            }
            
            /// <summary>
            /// Method <c>Start</c> is called before the first frame update.
            /// </summary>
            private void Start()
            {
                GameStart();
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
            /// Method <c>LoadGameMode</c> loads the game mode.
            /// </summary>
            /// <param name="setGameMode">The game mode to load.</param>
            private void LoadGameMode(GameMode setGameMode)
            {
                gameMode = setGameMode;
                gameMode.minRoundTime = Mathf.Min(gameMode.minRoundTime, gameMode.maxRoundTime);
            }
        
            /// <summary>
            /// Method <c>StartGame</c> starts the game.
            /// </summary>
            private void GameStart()
            {
                // Reset the game time
                GameTimeLeft = GameTime;
                UIManager.Instance.UpdateTimeText(GameTimeLeft);
                UIManager.Instance.ShowTimer(GameTime > 0f);
                
                // Reset the score
                _wins = 0;
                _loses = 0;
                UIManager.Instance.UpdateWinsText(_wins);
                UIManager.Instance.UpdateLosesText(_loses);
                UIManager.Instance.ShowScore(ShowScore);
                
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
                UIManager.Instance.UpdateTimeText(GameTimeLeft);
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
            }
        
        #endregion
        
        #region Score Methods
        
            /// <summary>
            /// Method <c>IncreaseWinCount</c> increases the win count.
            /// </summary>
            public void IncreaseWinCount(int wins = 1)
            {
                _wins += wins;
                UIManager.Instance.UpdateWinsText(_wins);
            }
                
            /// <summary>
            /// Method <c>IncreaseLossCount</c> increases the loss count.
            /// </summary>
            public void IncreaseLossCount(int loses = 1)
            {
                _loses += loses;
                UIManager.Instance.UpdateLosesText(_loses);
            }
        
        #endregion
        
        #region Game Management Methods
            
            /// <summary>
            /// Method <c>RestartGame</c> restarts the game.
            /// </summary>
            public void RestartGame()
            {
                StopAllCoroutines();
                // Do nothing for the moment
            }
            
            /// <summary>
            /// Method <c>PauseGame</c> pauses the game.
            /// </summary>
            /// <param name="pause">The pause state of the game.</param> 
            public void PauseGame(bool pause)
            {
                Time.timeScale = pause ? 0f : 1f;
            }
            
            /// <summary>
            /// Method <c>MainMenu</c> goes to the main menu.
            /// </summary>
            public void MainMenu()
            {
                StopAllCoroutines();
                // Do nothing for the moment
            }

            /// <summary>
            /// Method <c>QuitGame</c> quits the game.
            /// </summary>
            public void QuitGame()
            {
                Application.Quit();
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                #endif
            }
        
        #endregion
    }
}
