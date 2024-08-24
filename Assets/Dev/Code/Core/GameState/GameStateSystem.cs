using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Game.Framework;
using UnityEngine;

namespace Game.Core
{
    public class GameStateSystem : StateSystemBase<GameState>
    {

    }

    public enum GameState
    {
        Undefined = -1,
        Loading = 1,
        MainMenu = 2,
        SettingsMenu = 3,
        Gameplay = 4,
        PauseMenu = 8,
        ResearchMenu = 9,
    }
}
