using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PKDS.Properties
{
    /// <summary>
    /// Class <c>Loop</c> represents a loop.
    /// </summary>
    public static class Loop
    {
        /// <value>Property <c>Behaviour</c> represents the behaviour of the loop.</value>
        public enum Behaviour
        {
            None,
            Start,
            Auto,
            Click,
            SwitchKey,
            Random
        }
        
        /// <value>Property <c>BehaviourWeights</c> represents the weights of the behaviours.</value>
        private static readonly Dictionary<Behaviour, float> BehaviourWeights = new Dictionary<Behaviour, float>
        {
            { Behaviour.None, 0.0f },
            { Behaviour.Start, 0.0f },
            { Behaviour.Auto, 0.1f },
            { Behaviour.Click, 0.2f },
            { Behaviour.SwitchKey, 0.2f },
            { Behaviour.Random, 0.0f }
        };
        
        /// <summary>
        /// Method <c>GetRandomBehaviour</c> gets a random behaviour.
        /// </summary>
        public static Behaviour GetRandomBehaviour()
        {
            var totalWeight = BehaviourWeights.Values.Sum();
            var randomValue = Random.Range(0.0f, totalWeight);
            var cumulativeWeight = 0.0f;
            foreach (var behaviour in BehaviourWeights.Keys)
            {
                cumulativeWeight += BehaviourWeights[behaviour];
                if (randomValue <= cumulativeWeight)
                    return behaviour;
            }
            return Behaviour.Auto;
        }
    }
}
