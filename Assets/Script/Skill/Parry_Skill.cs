using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Parry_Skill : Skill
{
    [Header("Parry")]
    [SerializeField] private UI_SkillTreeSlot parryUnlockButton;
    public bool parryUnlocked { get; private set; }

    [Header("Parry restore")]
    [SerializeField] private UI_SkillTreeSlot restoreUnlockButton;
    [Range(0f, 1f)]
    [SerializeField] private float restoreHealthAmount;
    public bool restoreUnlocked { get; private set; }

    [Header("Parry with mirage")]
    [SerializeField] private UI_SkillTreeSlot parryWithMirageUnlokedButton;
    public bool parryWithMirageUnloked { get; private set; } 



    public override void UseSkill()
    {
        base.UseSkill();
        if (restoreUnlocked)
        {
            int restoreAmount = Mathf.RoundToInt( player.stats.GetMaxHealthValue() * restoreHealthAmount);
            player.stats.IncreaseHealthBy(restoreAmount);

        }
    }

    protected override void Start()
    {
        base.Start();

        parryUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockParry);
        restoreUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockParryRestore);
        parryWithMirageUnlokedButton.GetComponent<Button>().onClick.AddListener(UnlockParryWithMirage);

    }

    private void UnlockParry()
    {
        if(parryUnlockButton.unlocked)
            parryUnlocked = true;
    }
    private void UnlockParryRestore()
    {
        if(restoreUnlockButton.unlocked)
            restoreUnlocked = true;
    }
    private void UnlockParryWithMirage()
    {
        if (parryWithMirageUnlokedButton.unlocked)
            parryWithMirageUnloked = true;
    }

    public void makeMirageOnParry(Transform _respawnTransform)
    {
        if (parryWithMirageUnloked)
            SkillManager.instance.clone.CreateCloneWithDelay(_respawnTransform);
    }
}
