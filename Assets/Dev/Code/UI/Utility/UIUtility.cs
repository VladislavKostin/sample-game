using UnityEngine.EventSystems;

namespace Game.UI
{
    public static class UIUtility
    {
        public static bool IsPointerOverUI()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }
    }
}