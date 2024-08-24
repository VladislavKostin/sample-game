using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Game.Framework;
using UnityEngine;

namespace Game.Core
{
    [BootstrapAfter(typeof(PlayerProfileSystem))]
    public class SettingsSystem : ManagedSystem
    {
        private static SettingsSystem _instance;

        [Inject] private PlayerProfileSystem _playerProfileSystem;

        private string _settingsPath => Path.Combine(_playerProfileSystem.CurrentProfileDirectory, "Settings.es3");
        private string _settingsKey => "Settings";
        private Coroutine saveCoroutine;

        public SettingsContainer Settings { get; private set; }

        public void Awake()
        {
            _instance = this;
            Settings = new();
        }

        public void Start()
        {
            LoadSettings();
        }

        public void SaveSettings()
        {
            // Redacted code using third party solution
        }

        public void LoadSettings()
        {
            // Redacted code using third party solution
        }

        public void OnSettingsChanged()
        {
            saveCoroutine = StartCoroutine(OnSettingsChangedCoroutine()); // Start coroutines in the correct
        }

        public IEnumerator OnSettingsChangedCoroutine()
        {
            if (saveCoroutine != null)
            {
                StopCoroutine(saveCoroutine);
            }
            yield return new WaitForSeconds(2f);
            SaveSettings();
        }

        [Serializable]
        public class SettingsContainer
        {
            public AudioSettings Audio = new();
            public VideoSettings Video = new();
            public ControlsSettings Controls = new();
            public GameplaySettings Gameplay = new();
            public List<SettingsCategory> Categories = new();

            public SettingsContainer()
            {
                Categories.Add(Audio);
                Categories.Add(Video);
                Categories.Add(Controls);
                Categories.Add(Gameplay);
            }
        }

        [Serializable]
        public class SettingsCategory
        {
            public virtual string Name => "";
            public List<Setting> Settings = new();
        }

        public class AudioSettings : SettingsCategory
        {
            public override string Name => "Audio";
            public SliderSetting MasterVolume = new("Master Volume", _instance.OnSettingsChanged, 100f, 0f, 100f, SliderValueFormat.Percents);
            public SliderSetting MusicVolume = new("Music Volume", _instance.OnSettingsChanged, 100f, 0f, 100f, SliderValueFormat.Percents);
            public SliderSetting SFXVolume = new("SFX Volume", _instance.OnSettingsChanged, 100f, 0f, 100f, SliderValueFormat.Percents);
            public ToggleSetting EnableMusic = new("Enable Music", _instance.OnSettingsChanged, true);

            public AudioSettings()
            {
                Settings.Add(MasterVolume);
                Settings.Add(MusicVolume);
                Settings.Add(SFXVolume);
                Settings.Add(EnableMusic);
            }
        }

        public class VideoSettings : SettingsCategory
        {
            public override string Name => "Video";
            public SliderSetting RenderingQuality = new("Rendering Quality", _instance.OnSettingsChanged, 100f, 0f, 100f, SliderValueFormat.Percents);

            public VideoSettings()
            {
                Settings.Add(RenderingQuality);
            }
        }

        public class ControlsSettings : SettingsCategory
        {
            public override string Name => "Controls";

            public ControlsSettings()
            {
            }
        }

        public class GameplaySettings : SettingsCategory
        {
            public override string Name => "Gameplay";

            public GameplaySettings()
            {
            }
        }
    }

    [Serializable]
    public abstract class Setting
    {
        public string Name { get; private set; }
        public event Action OnValueChange;

        protected void InvokeOnValueChange()
        {
            OnValueChange?.Invoke();
        }

        public Setting(string name, Action onChange)
        {
            OnValueChange = onChange;
            Name = name;
        }
    }

    public abstract class Setting<T> : Setting
    {
        [SerializeField] protected T _value;
        public T OldValue { get; private set; }

        public T Value
        {
            get { return _value; }
            set
            {
                OldValue = _value;
                _value = (T)value;
                InvokeOnValueChange();
            }
        }

        public Setting(string name, Action onChange, T defaultValue) : base(name, onChange)
        {
            _value = defaultValue;
        }
    }

    public class SliderSetting : Setting<float>
    {
        public float MinValue { get; private set; }
        public float MaxValue { get; private set; }
        public int IntValue => (int)Value;
        public int NormalizedValue => (int)((Value - MinValue) / (MaxValue - MinValue));

        public SliderValueFormat ValueFormat { get; private set; }

        public SliderSetting(string name, Action onChange, float defaultValue, float minValue, float maxValue, SliderValueFormat valueFormat) : base(name, onChange, defaultValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            ValueFormat = valueFormat;
        }

        public new float Value
        {
            get { return base._value; }
            set
            {
                if (value < MinValue) value = MinValue;
                if (value > MaxValue) value = MaxValue;
                if (ValueFormat == SliderValueFormat.Integer || ValueFormat == SliderValueFormat.Percents)
                {
                    value = Mathf.Round(value);
                }
                base.Value = value;
            }
        }
    }

    public class ToggleSetting : Setting<bool>
    {
        public ToggleSetting(string name, Action onChange, bool defaultValue) : base(name, onChange, defaultValue)
        {
        }
    }

    public class DropdownSetting<T> : Setting<T> where T : Enum
    {
        public DropdownSetting(string name, Action onChange, T defaultValue) : base(name, onChange, defaultValue)
        {
        }
    }

    public enum SliderValueFormat
    {
        Integer,
        Percents,
        Float
    }
}
