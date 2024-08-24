using UnityEngine;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;
using Game.Core;

namespace Game.UI
{
    public class UIElement : VisualElement
    {
        private VisualTreeAsset _template;

        public UIElement()
        {
            LoadTemplateAsset();
            CloneTemplate();
        }

        private void LoadTemplateAsset()
        {
            string assetPath = $"Assets/Dev/UI/Templates/{GetType().Name}.uxml";
            try
            {
                _template = Addressables.LoadAssetAsync<VisualTreeAsset>(assetPath).WaitForCompletion();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error loading asset at #{assetPath}: {ex.Message}");
                throw new Exception($"Error loading asset at {assetPath}", ex);
            }
        }

        private void LoadTemplateAssetAsync(VisualTreeAsset visualTreeAsset)
        {
            // Addressables.LoadAssetAsync<VisualTreeAsset>(nameof(FloatSetting)).Completed += op =>
            // {
            //     if (op.Status == AsyncOperationStatus.Succeeded)
            //     {
            //         assets.Add(op.Result);
            //     }
            // };
        }

        private void CloneTemplate()
        {
            // Logger.LogDebug($"Cloning template: #{_template}");
            _template.CloneTree(this);
            this.Show();
            // Logger.LogDebug($"This has button: #{this.Q<Button>() != null}");
        }

        // public new class UxmlFactory : UxmlFactory<UIElement, UxmlTraits> { }
    }

}
