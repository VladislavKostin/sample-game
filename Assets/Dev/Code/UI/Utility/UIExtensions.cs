using UnityEngine.UIElements;

public static class UIExtensions
{
    public static T FindRequired<T>(this VisualElement parent, string name) where T : VisualElement
    {
        T element = parent.Q<T>(name);
        if (element == null)
        {
            Logger.LogError($"Element #{name} not found under parent #{parent.name}");
        }
        return element;
    }
}