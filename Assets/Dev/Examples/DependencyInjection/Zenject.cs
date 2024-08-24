using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Examples.DI
{
    public interface IAudioService
    {
        void PlaySound(string soundId);
    }

    public interface IScoreManager
    {
        int Score { get; }
        void AddScore(int points);
    }

    public interface ILevelManager
    {
        void LoadLevel(int levelIndex);
    }

    public class GameInstaller // : MonoInstaller
    {
        public void InstallBindings()
        {
            //Container.Bind<IAudioService>().To<AudioService>().AsSingle();
            //Container.Bind<IScoreManager>().To<ScoreManager>().AsSingle();
            //Container.Bind<ILevelManager>().To<LevelManager>().AsSingle();
        }
    }
}