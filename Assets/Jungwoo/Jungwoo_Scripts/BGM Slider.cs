using UnityEngine;
using UnityEngine.UI;

public class BGMVolumeSlider : MonoBehaviour
{
    public Slider slider;

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("BGM_Volume", 0.2f);
        slider.value = savedVolume;

        if (SoundManager.Instance?.bgmSource != null)
            SoundManager.Instance.bgmSource.volume = savedVolume;

        slider.onValueChanged.AddListener(AdjustVolume);
    }

    void AdjustVolume(float value)
    {
        if (SoundManager.Instance?.bgmSource != null)
            SoundManager.Instance.bgmSource.volume = value;

        PlayerPrefs.SetFloat("BGM_Volume", value);
        PlayerPrefs.Save();
    }
}

