using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizTabSpriteSelector : MonoBehaviour
{
    public List<Button> tabButtons;   // 1��N
    public Sprite yellowSprite;       // �̼���
    public Sprite whiteSprite;        // ����

    public int CurrentIndex { get; private set; } = -1;

    void Awake()
    {
        // ��ư �⺻ ȿ�� ���� ���� + �̹��� �ʱ�ȭ
        foreach (var btn in tabButtons)
        {
            if (!btn) continue;

            // 1) � Ʈ�����ǵ� ���� �ʵ��� ����
            btn.transition = Selectable.Transition.None;

            // 2) Ȥ�� �������� ���� ��� ���⵵�� 0���� (������)
            var cb = btn.colors;
            cb.normalColor = Color.white;
            cb.highlightedColor = Color.white;
            cb.pressedColor = Color.white;
            cb.selectedColor = Color.white;
            cb.disabledColor = Color.white;
            cb.colorMultiplier = 1f;
            btn.colors = cb;

            // 3) ���� ���� ���� �������� ����
            var img = btn.targetGraphic as Image;
            if (img)
            {
                img.color = Color.white;   // ���� 1, tint ����
                img.material = null;       // Ŀ���� ��Ƽ���� ���� ����
                img.raycastTarget = true;  // Ŭ���� ����
            }

            // Ŭ�� ������
            int idx = tabButtons.IndexOf(btn);
            btn.onClick.AddListener(() => Select(idx));
        }
    }

    void OnEnable()
    {
        Select(0); // â ���� �� �׻� 1��
    }

    public void Select(int index)
    {
        if (tabButtons.Count == 0) return;

        CurrentIndex = Mathf.Clamp(index, 0, tabButtons.Count - 1);

        for (int i = 0; i < tabButtons.Count; i++)
        {
            var img = tabButtons[i].targetGraphic as Image;
            if (img)
            {
                img.sprite = (i == CurrentIndex) ? whiteSprite : yellowSprite;
                img.color = Color.white; // ����/���̶���Ʈ�� ���� �� ���� ����
            }
        }

        // TODO: ���� ���� �ε�
        // QuizManager.Instance.LoadQuestion(CurrentIndex);
    }
}

