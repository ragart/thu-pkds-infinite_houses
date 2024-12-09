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
        
        #region State Properties

            /// <value>Property <c>_isGameStarted</c> represents if the game is started.</value>
            private bool _isGameStarted;
            
            /// <value>Property <c>AreAllInteractionsPrevented</c> represents if all interactions are prevented.</value>
            public bool AreAllInteractionsPrevented { get; private set; } = true;
        
        #endregion
        
        #region Game Properties
        
            // TODO: Presets (Scriptable Objects)
            
            /// <value>Property <c>loopBehaviour</c> represents the loop behaviour.</value>
            [Header("Game Properties")]
            [SerializeField]
            private Loop.Behaviour loopBehaviour = Loop.Behaviour.SwitchKey;
            
            /// <value>Property <c>LoopBehaviour</c> represents the loop behaviour.</value>
            public Loop.Behaviour LoopBehaviour => loopBehaviour;
            
            /// <value>Property <c>gameTime</c> represents the time limit of the game.</value>
            [SerializeField]
            private float gameTime = 60f;
            
            /// <value>Property <c>GameTime</c> represents the time limit of the game.</value>
            public float GameTime => gameTime;

            /// <value>Property <c>GameTimeLeft</c> represents the time left of the game.</value>
            public float GameTimeLeft { get; private set; }
            
            /// <value>Property <c>maxRoundTime</c> represents the maximum time of a round.</value>
            [SerializeField]
            private float maxRoundTime = 5f;
            
            /// <value>Property <c>MaxRoundTime</c> represents the maximum time of a round.</value>
            public float MaxRoundTime => maxRoundTime;
            
            /// <value>Property <c>minRoundTime</c> represents the minimum time of a round.</value>
            [SerializeField]
            private float minRoundTime = 2f;
            
            /// <value>Property <c>MinRoundTime</c> represents the minimum time of a round.</value>
            public float MinRoundTime => minRoundTime;

            /// <value>Property <c>round</c> represents the current round.</value>
            public int Round { get; set; }

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
                GameTimeLeft = gameTime;
                
                // Ensure the minimum round time is less than the maximum round time
                minRoundTime = Mathf.Min(minRoundTime, maxRoundTime);
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
            /// Method <c>PreventInteractions</c> prevents all interactions.
            /// </summary>
            public void PreventInteractions(bool prevent = true)
            {
                AreAllInteractionsPrevented = prevent;
            }
        
        #endregion
        
        #region Game Methods
        
            /// <summary>
            /// Method <c>StartGame</c> starts the game.
            /// </summary>
            private void GameStart()
            {
                // Reset the game time and the win/lose counts
                GameTimeLeft = gameTime;
                _wins = 0;
                _loses = 0;
                
                // Update the UI
                UIManager.Instance.UpdateTimeText(GameTimeLeft);
                UIManager.Instance.UpdateWinsText(_wins);
                UIManager.Instance.UpdateLosesText(_loses);
                
                // Start the game
                _isGameStarted = true;
            }

            private void GameUpdate()
            {
                if (!_isGameStarted)
                    return;
                // Update the game timer and the corresponding UI
                GameTimeLeft -= Time.deltaTime;
                GameTimeLeft = Mathf.Clamp(GameTimeLeft, 0f, gameTime);
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
                PreventInteractions();
                _isGameStarted = false;
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
