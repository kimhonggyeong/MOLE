using UnityEngine;
using UnityEngine.UI;

public class ToggleSFXSwitch : MonoBehaviour
{
    public Toggle toggle;
    public Image background;
    public RectTransform handle;
    public Image handleImage;

    public Color onColor = new Color(0.25f, 0.5f, 1f);
    public Color offColor = Color.gray;

    public Sprite onHandleSprite;
    public Sprite offHandleSprite;

    private Vector2 handleOnPos = new Vector2(10, 0);
    private Vector2 handleOffPos = new Vector2(-10, 0);

    void Start()
    {
        bool isOn = PlayerPrefs.GetInt("SFX_Toggle", 1) == 1;
        toggle.isOn = isOn;

        toggle.onValueChanged.AddListener(OnToggleChanged);
        UpdateToggleUI(isOn);
        ApplyMute(isOn);
    }

    void OnToggleChanged(bool isOn)
    {
        UpdateToggleUI(isOn);
        ApplyMute(isOn);

        PlayerPrefs.SetInt("SFX_Toggle", isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    void UpdateToggleUI(bool isOn)
    {
        background.color = isOn ? onColor : offColor;
        handle.anchoredPosition = isOn ? handleOnPos : handleOffPos;

        if (handleImage != null)
        {
            handleImage.sprite = isOn ? onHandleSprite : offHandleSprite;
        }
    }

    void ApplyMute(bool isOn)
    {
        if (SoundManager.Instance?.sfxSource != null)
        {
            SoundManager.Instance.sfxSource.mute = !isOn;
        }
    }
}
