using UnityEngine;
using UnityEngine.UIElements;
using Game.Framework;

namespace Game.UI
{
    [Inject]
    public class UIDocumentProvider : MonoBehaviour
    {
        [field: SerializeField] public UIDocument UIDocument { get; private set; }
    }
}