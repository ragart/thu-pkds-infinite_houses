using UnityEngine;
using PKDS.Properties;

namespace PKDS.Entities
{
    [CreateAssetMenu(fileName = "GameMode", menuName = "Custom/Game Mode", order = 1)]
    public class GameMode : ScriptableObject
    {
        /// <value>Property <c>modeName</c> represents the name of the game mode.</value>
        public string modeName;
        
        /// <value>Property <c>loopBehaviour</c> represents the loop behaviour.</value>
        public Loop.Behaviour loopBehaviour = Loop.Behaviour.None;
        
        /// <value>Property <c>gameTime</c> represents the time limit of the game.</value>
        public int gameTime = 60;
        
        /// <value>Property <c>maxRoundTime</c> represents the maximum time of a round.</value>
        public int maxRoundTime = 5;
        
        /// <value>Property <c>minRoundTime</c> represents the minimum time of a round.</value>
        public int minRoundTime = 1;
        
        /// <value>Property <c>showScore</c> represents if the score is shown.</value>
        public bool showScore = true;
        
        /// <value>Property <c>zoomDelay</c> represents the delay of the zoom effect.</value>
        public float zoomDelay = 0.5f;
    }
}
