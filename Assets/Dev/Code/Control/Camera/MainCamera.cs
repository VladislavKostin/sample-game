using System.Collections;
using System.Collections.Generic;
using Game.Core;
using UnityEngine;
using Game.Framework;

namespace Game.Core
{
    [Inject]
    public class MainCamera : MonoBehaviour
    {
        [field: SerializeField] public Camera Camera { get; private set; }
    }
}