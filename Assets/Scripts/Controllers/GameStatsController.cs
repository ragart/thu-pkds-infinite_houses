using UnityEngine;
using TMPro;

namespace PKDS.Controllers
{
    /// <summary>
    /// Method <c>GameStatsController</c> handles the game stats' logic.
    /// </summary>
    public class GameStatsController : MonoBehaviour
    {
        /// <value>Property <c>timerContainer</c> represents the container of the timer.</value>
        [Header("Game Stats Properties")]
        [SerializeField]
        private GameObject timerContainer;
            
        /// <value>Property <c>timerText</c> represents the text containing the time left of the game.</value>
        [SerializeField]
        private TextMeshProUGUI timerText;
            
        /// <value>Property <c>scoreContainer</c> represents the container of the score.</value>
        [SerializeField]
        private GameObject scoreContainer;
            
        /// <value>Property <c>winsText</c> represents the text containing the number of wins.</value>
        [SerializeField]
        private TextMeshProUGUI winsText;
            
        /// <value>Property <c>losesText</c> represents the text containing the number of loses.</value>
        [SerializeField]
        private TextMeshProUGUI losesText;

        /// <summary>
        /// Method <c>ShowTimer</c> shows or hides the timer.
        /// </summary>
        /// <param name="show">The boolean value to show or hide the timer.</param>
        public void ShowTimer(bool show)
        {
            timerContainer.SetActive(show);
        }
        
        /// <summary>
        /// Method <c>ShowScore</c> shows or hides the score.
        /// </summary>
        /// <param name="show">The boolean value to show or hide the score.</param>
        public void ShowScore(bool show)
        {
            scoreContainer.SetActive(show);
        }
        
        /// <summary>
        /// Method <c>UpdateTimeText</c> updates the time text.
        /// </summary>
        /// <param name="timeLeft">The time left.</param>
        public void UpdateTimeText(float timeLeft)
        {
            var minutes = Mathf.FloorToInt(timeLeft / 60);
            var seconds = Mathf.FloorToInt(timeLeft % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
        
        /// <summary>
        /// Method <c>UpdateWinsText</c> updates the wins text.
        /// </summary>
        /// <param name="wins">The number of wins.</param>
        public void UpdateWinsText(int wins)
        {
            winsText.text = wins.ToString();
        }
        
        /// <summary>
        /// Method <c>UpdateLosesText</c> updates the loses text.
        /// </summary>
        /// <param name="loses">The number of loses.</param>
        public void UpdateLosesText(int loses)
        {
            losesText.text = loses.ToString();
        }
    }
}
