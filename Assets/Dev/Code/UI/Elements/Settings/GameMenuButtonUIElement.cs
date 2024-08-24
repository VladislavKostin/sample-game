using UnityEngine;
using System;
using UnityEngine.UIElements;
using Game.Core;

namespace Game.UI
{
    public class GameMenuButtonUIElement : UIElement
    {
        public Button Button => this.Q<Button>();

        public void Bind(string title, Action action)
        {
            Button.text = title;
            Button.clicked += action;
        }

        public new class UxmlFactory : UxmlFactory<GameMenuButtonUIElement, UxmlTraits> { }
    }
}