using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Examples.DI
{
    public class AudioService : IAudioService
    {
        public void PlaySound(string soundId)
        {
            // Implementation for playing a sound
            Debug.Log($"Playing sound: {soundId}");
        }
    }

    public class ScoreManager : IScoreManager
    {
        private int _score;

        public int Score => _score;

        public void AddScore(int points)
        {
            _score += points;
            Debug.Log($"Score added: {points}, total score: {_score}");
        }
    }

    public class LevelManager : ILevelManager
    {
        readonly IAudioService _audioService;
        readonly IScoreManager _scoreManager;

        public LevelManager(IAudioService audioService, IScoreManager scoreManager)
        {
            _audioService = audioService;
            _scoreManager = scoreManager;
        }

        public void LoadLevel(int levelIndex)
        {
            // Implementation for loading a level
            _audioService.PlaySound("level_start");
            Debug.Log($"Loading level: {levelIndex}");
            _scoreManager.AddScore(100); // Example of interaction
        }
    }
}