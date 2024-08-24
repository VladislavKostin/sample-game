using UnityEngine.UIElements;
using UnityEngine.AddressableAssets;
using Game.Core;
using System;

namespace Game.UI
{
    public class SliderSettingUIElement : SettingUIElement<SliderSetting>
    {
        private Slider _slider;
        private Label _valueLabel;

        public SliderSettingUIElement()
        {
            _slider = this.FindRequired<Slider>("slider");
            _valueLabel = this.FindRequired<Label>("value-label");
        }

        public override void BindSetting(SliderSetting setting)
        {
            base.BindSetting(setting);
            _slider.lowValue = setting.MinValue;
            _slider.highValue = setting.MaxValue;
            _slider.value = setting.Value;
            UpdateValueLabel();
            _slider.RegisterValueChangedCallback(evt =>
            {
                setting.Value = evt.newValue;
                UpdateValueLabel();
            });
        }

        private void UpdateValueLabel()
        {
            if (_setting.ValueFormat == SliderValueFormat.Integer)
            {
                _valueLabel.text = ((int)_slider.value).ToString();
            }
            else if (_setting.ValueFormat == SliderValueFormat.Float)
            {
                _valueLabel.text = _slider.value.ToString("0.00");
            }
            else if (_setting.ValueFormat == SliderValueFormat.Percents)
            {
                _valueLabel.text = $"{(int)_slider.value}%";
            }
        }

        public new class UxmlFactory : UxmlFactory<SliderSettingUIElement, UxmlTraits> { }
    }
}

