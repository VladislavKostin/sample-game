using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using UnityEngine;

namespace Game.Core
{
    public class GameplayStateSystem : StateSystemBase<GameplayState>
    {

    }

    public enum GameplayState
    {
        None = 0,
        Production = 1,
        BuildModule = 2,
        TestModule = 4,
        BuildFactory = 5,
    }
}
