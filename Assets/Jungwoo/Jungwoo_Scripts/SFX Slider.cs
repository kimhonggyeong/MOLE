using UnityEngine;
using UnityEngine.UI;

public class SFXVolumeSlider : MonoBehaviour
{
    public Slider slider;

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("SFX_Volume", 1.0f);
        slider.value = savedVolume;

        if (SoundManager.Instance?.sfxSource != null)
            SoundManager.Instance.sfxSource.volume = savedVolume;

        slider.onValueChanged.AddListener(AdjustVolume);
    }

    void AdjustVolume(float value)
    {
        if (SoundManager.Instance?.sfxSource != null)
            SoundManager.Instance.sfxSource.volume = value;

        PlayerPrefs.SetFloat("SFX_Volume", value);
        PlayerPrefs.Save();
    }
}
