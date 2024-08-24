using System;
using UnityEngine;
using UnityEngine.UIElements;
using Game.Core;
using TMPro;

namespace Game.UI
{
    public abstract class SettingUIElement<T> : UIElement where T : Setting
    {
        protected Label _nameLabel;
        protected T _setting;

        public SettingUIElement()
        {
            _nameLabel = this.FindRequired<Label>("name-label");
            _nameLabel.text = "Setting Name";
        }

        public virtual void BindSetting(T setting)
        {
            _setting = setting;
            _nameLabel.text = setting.Name;
        }
    }
}

public class BindToSetting : Attribute
{
    public BindToSetting(Type settingType)
    {

    }
}