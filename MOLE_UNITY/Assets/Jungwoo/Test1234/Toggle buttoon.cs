using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ToggleVisual : MonoBehaviour
{
    [Header("�ʼ�")]
    public Toggle toggle;
    public Image background;
    public RectTransform handle;
    public Image handleImage;

    [Header("��/��������Ʈ")]
    public Color onColor = new Color(0.25f, 0.5f, 1f);
    public Color offColor = Color.gray;
    public Sprite onHandleSprite;
    public Sprite offHandleSprite;

    [Header("�ڵ� ��ġ (���� ��Ŀ��)")]
    public Vector2 handleOnPos = new Vector2(10, 0);
    public Vector2 handleOffPos = new Vector2(-10, 0);

    [Header("���� ���� �ɼ�")]
    public bool usePersistence = true;         // PlayerPrefs�� ��������
    public string prefsKey = "Toggle_Generic"; // ���� Ű
    public bool defaultOn = true;              // ���尪 ���� �� �⺻��

    [Header("���� �ɼ�")]
    public bool interactive = true;            // �б����� ���(�����)�� false
    public UnityEvent<bool> onToggle;          // ����: �ܺ� ���� ȣ��(���Ұ� ��)

    void Start()
    {
        if (!toggle) toggle = GetComponent<Toggle>();
        if (toggle == null) { Debug.LogError("[ToggleVisual] Toggle�� �����ϴ�."); return; }

        // �б����� ���
        toggle.interactable = interactive;

        // ����� �� �ҷ�����
        bool isOn = usePersistence
            ? PlayerPrefs.GetInt(prefsKey, defaultOn ? 1 : 0) == 1
            : defaultOn;

        // �ʱⰪ ����(�ݹ� ȣ�� ����)
        toggle.SetIsOnWithoutNotify(false);
        UpdateToggleUI(false);
        //toggle.SetIsOnWithoutNotify(isOn);
        //UpdateToggleUI(isOn);

        // ������ ����
        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener(HandleChanged);
    }

    void HandleChanged(bool isOn)
    {
        UpdateToggleUI(isOn);

        // ����: �ܺ� �̺�Ʈ ȣ��(���ϸ� ����� ���Ұ�/���� �ݿ� �� ����)
        onToggle?.Invoke(isOn);

        // ����
        if (usePersistence)
        {
            PlayerPrefs.SetInt(prefsKey, isOn ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    void UpdateToggleUI(bool isOn)
    {
        if (background) background.color = isOn ? onColor : offColor;
        if (handle) handle.anchoredPosition = isOn ? handleOnPos : handleOffPos;
        if (handleImage) handleImage.sprite = isOn ? onHandleSprite : offHandleSprite;
    }

    // �ڵ�� ���¸� �ٲٰ� ���� ��(�ݹ���� ����)
    public void Set(bool isOn)
    {
        if (!toggle) return;
        toggle.isOn = isOn;
    }
}

