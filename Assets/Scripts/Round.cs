using System;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class Round
    {
        [SerializeField] private int _roundNumber;
        [SerializeField] private int _successPerRound;

        private int _successes;
        
        public int RoundNumber => _roundNumber;
        
        public void Success()
        {
            _successes++;
            Debug.Log($"Success {_successes}/{_successPerRound} of Round #{_roundNumber}");
            
            if (_successes < _successPerRound) return;
            
            _successes = 0;
            _roundNumber++;
            Debug.Log($"Round {_roundNumber} successful");
        }
        
        public void Fail()
        {
            _successes = 0;
            _roundNumber--;
            
            Debug.Log($"Round #{_roundNumber} failed");
        }
    }
}