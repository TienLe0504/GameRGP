using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloneSkill : Skill
{
    [Header("Clone info")]
    [SerializeField] private float attackMultiplier;
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;


    [Header("Clone attack")]
    [SerializeField] private UI_SkillTreeSlot cloneAttackunlockButton;
    [SerializeField] private float cloneAttackUltiplier;
    [SerializeField] private bool canAttack;

    [Header("Aggresive clone")]
    [SerializeField] private UI_SkillTreeSlot aggresiveCloneUnlockButton;
    [SerializeField] private float aggresiveCloneAttackMultiplier;
    public bool canApplyOnHitEffect { get; private set; }

    [Header("Multiple mirage")]
    [SerializeField] private UI_SkillTreeSlot multipleUnlockButton;
    [SerializeField] private float multiCloneAttackMultiplier;
    [SerializeField] private bool canDuplicateClone ;
    [SerializeField] private float changeToDuplicate =35;
    [Header("Crystal instead of clone")]
    [SerializeField] private UI_SkillTreeSlot crystalInseadUnlockButton;
    public bool crystalInseadofClone;

    public bool crystalInsteadOfClone;


    protected override void Start()
    {
        base.Start();
        cloneAttackunlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneAttack);
        aggresiveCloneUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockAggresiveClone);
        multipleUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockMultiClone);
        crystalInseadUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalInstead);
    }

    #region Unlock region

    private void UnlockCloneAttack()
    {
        if(cloneAttackunlockButton.unlocked)
        {
            canAttack = true;
            attackMultiplier = cloneAttackUltiplier;
        }
    }
    private void UnlockAggresiveClone()
    {
        if(aggresiveCloneUnlockButton.unlocked)
        {
            canApplyOnHitEffect = true;
            attackMultiplier = aggresiveCloneAttackMultiplier;
        }
    }

    private void UnlockMultiClone()
    {
        if(multipleUnlockButton.unlocked)
        {
            canDuplicateClone = true;
            attackMultiplier = multiCloneAttackMultiplier;
        }
    }

    private void UnlockCrystalInstead()
    {
        if (crystalInseadUnlockButton.unlocked)
        {
            crystalInseadofClone = true;
        }
    }

    #endregion


    public void CreateClone(Transform _clonPosition, Vector3 _offset)
    {
        if (crystalInsteadOfClone)
        {
            SkillManager.instance.crystal.CreateCrystal();
            SkillManager.instance.crystal.CurrentCrystalChooseRandomTarget();
            return;
        }

        GameObject newClone = Instantiate(clonePrefab);
        newClone.GetComponent<CloneSkillController>().SetupClone(_clonPosition, cloneDuration, canAttack, _offset, FindClosestEnemy(newClone.transform), canDuplicateClone, changeToDuplicate,player,attackMultiplier);
    }


    public void CreateCloneWithDelay(Transform _enemyTransform)
    {
            StartCoroutine(CloneDelayCorotine(_enemyTransform, new Vector3(2*player.facingDir,0)));
    }

    private IEnumerator CloneDelayCorotine(Transform _transform, Vector3 _offset)
    {
        yield return new WaitForSeconds(.4f);
        CreateClone(_transform,  _offset);
    }
}
