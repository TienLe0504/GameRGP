using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UI_SkillTreeSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private UI ui;
    [SerializeField] private int skillPrice;
    [SerializeField] private string skillName;
    [TextArea]
    [SerializeField] private string skillDescription;
    [SerializeField] private Color lockedSkillColor;
    public bool unlocked;
    [SerializeField] private UI_SkillTreeSlot[] shouldBeUnlocked;
    [SerializeField] private UI_SkillTreeSlot[] shouldBeLocked;
     private Image skillImage;

    private void Awake()
    {
         GetComponent<Button>().onClick.AddListener(() => UnClockSkillSlot());

    }

    private void OnValidate()
    {
        gameObject.name = "SKillTreeSlot_UI - " + skillName;
        ui = GetComponentInParent<UI>();
    }

    private void Start()
    {
        skillImage = GetComponent<Image>();
        Color color;
        if (ColorUtility.TryParseHtmlString("#6F5555", out color))
        {
            skillImage.color = color; // Đặt màu đã chuyển đổi
        }
    }

    public void UnClockSkillSlot()
    {
        if (PlayerManager.instance.HaveEnoughMoney(skillPrice) == false)
            return;

        for (int i = 0; i < shouldBeUnlocked.Length; i++)
        {
            if (shouldBeUnlocked[i].unlocked == false)
            {
                Debug.Log("Cannot unclock skill");
                return;
            }
        }

        for (int i = 0; i < shouldBeLocked.Length; i++)
        {
            if (shouldBeLocked[i].unlocked == true)
            {
                Debug.Log("Cannot unclock skill");
                return;
            }
        }
        unlocked = true;
        skillImage.color = Color.white;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillTooltip.ShowToolTip(skillDescription,skillName);
        Vector2 mousePostition = Input.mousePosition;
        float xOffset = 0;
        float yOffset = 0;
        if (mousePostition.x > 380)
            xOffset = -50;
        else
            xOffset = 50;
        if (mousePostition.y > 320)
            yOffset = -50;
        else
        {
            yOffset = 50;
        }

        ui.skillTooltip.transform.position = new Vector2(mousePostition.x + xOffset, mousePostition.y + yOffset);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillTooltip.HideToolTip();
    }
}
