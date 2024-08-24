using System.Collections.Generic;
using UnityEngine.UIElements;

public class VisualElementGroup
{
    private List<VisualElement> _elements = new List<VisualElement>();

    public VisualElementGroup(params VisualElement[] elements)
    {
        foreach (var element in elements)
        {
            _elements.Add(element);
        }
    }

    public void Show()
    {
        foreach (var element in _elements)
        {
            element.Show();
        }
    }

    public void Hide()
    {
        foreach (var element in _elements)
        {
            if (element != null)
            {
                element.Hide();
            }
            else
            {
                Logger.LogError($"Element is null");
            }
        }
    }

    public void Add(VisualElement visualElement)
    {
        _elements.Add(visualElement);
    }
}

public class VisualElementGroupToggle
{
    private List<VisualElementGroup> groups;
    private VisualElementGroup activeGroup;

    public VisualElementGroupToggle(params VisualElementGroup[] groups)
    {
        this.groups = new List<VisualElementGroup>(groups);
    }

    public void SetActiveGroup(VisualElementGroup activeGroup)
    {
        foreach (var group in groups)
        {
            if (group != activeGroup)
            {
                group.Hide();
            }
        }

        this.activeGroup = activeGroup;
        activeGroup.Show();
    }
}

public static class VisualElementExtensions
{
    public static void Hide(this VisualElement visualElement)
    {
        if (visualElement.style.display != DisplayStyle.None)
        {
            visualElement.style.display = DisplayStyle.None;
        }
    }

    public static void Show(this VisualElement visualElement)
    {
        if (visualElement.style.display != DisplayStyle.Flex)
        {
            visualElement.style.display = DisplayStyle.Flex;
        }
    }

    public static T Find<T>(this VisualElement root, string name) where T : VisualElement
    {
        T element = root.Q<T>(name);

        if (element == null)
        {
            throw new System.Exception($"Element with name '{name}' not found.");
        }

        return element;
    }
}