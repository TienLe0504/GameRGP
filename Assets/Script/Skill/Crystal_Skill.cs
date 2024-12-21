using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crystal_Skill : Skill
{
    [SerializeField] private float crystalDuration;
    [SerializeField] private GameObject crystalPrefab;
    private GameObject currentCrystal;


    [Header("Crystal mirage")]
    [SerializeField] private UI_SkillTreeSlot unlockCloneInstaedButton;
    [SerializeField] private bool cloneInsteadOfCrystal;


    [Header("Crystal simple")]
    [SerializeField] private UI_SkillTreeSlot unlockCrystalButton;
    public bool crystalUnlocked { get; private set; }

    [Header("Explosive crystal")]
    [SerializeField] private UI_SkillTreeSlot unlockExplosiveButton;
    [SerializeField] private bool canExplode;

    [Header("Moving crystal")]
    [SerializeField] private UI_SkillTreeSlot unlockMovingCrystalButton;
    [SerializeField] private bool canMoveToEnemy;
    [SerializeField] private float moveSpeed;


    [Header("Multi stacking crystal")]
    [SerializeField] private UI_SkillTreeSlot unlockMultiStackButton;
    [SerializeField] private bool canUseMultiStacks;
    [SerializeField] private int amountOfStacks;
    [SerializeField] private float multiStackCooldown;
    [SerializeField] private float useTimeWondow;

    [SerializeField] private List<GameObject> crystalLeft = new List<GameObject>();

    private void UnlockCrystal()
    {
        if (unlockCrystalButton.unlocked)
            crystalUnlocked = true;
    }

    private void UnclocCrystalMirage()
    {
        if(unlockCloneInstaedButton.unlocked)
            cloneInsteadOfCrystal = true;
    }

    private void UnlockedExplosiveCrystal()
    {
        if (unlockExplosiveButton.unlocked)
            canExplode = true;
    }

    private void UnlockMovingCrystal()
    {
        if (unlockMovingCrystalButton.unlocked)
            canMoveToEnemy = true;
    }

    private void UnlockMultiStack()
    {
        if (unlockMovingCrystalButton.unlocked)
            canUseMultiStacks = true;
    }


    protected override void Start()
    {
        base.Start();
        unlockCrystalButton.GetComponent<Button>().onClick.AddListener(UnlockCrystal);
        unlockCloneInstaedButton.GetComponent<Button>().onClick.AddListener(UnclocCrystalMirage);
        unlockMovingCrystalButton.GetComponent<Button>().onClick.AddListener(UnlockMovingCrystal);
        unlockMultiStackButton.GetComponent<Button>().onClick.AddListener(UnlockMultiStack);
        unlockExplosiveButton.GetComponent<Button>().onClick.AddListener(UnlockedExplosiveCrystal);



    }

    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }
    private void Awake()
    {
        RefilCrystal();
    }
    public override void UseSkill()
    {
        base.UseSkill();
        if (CanUseMultiCrystal())
            return;
        if(currentCrystal == null)
        {
            CreateCrystal();
        }
        else
        {
            if (canMoveToEnemy)
                return;

            Vector2 playerPos = player.transform.position;
            player.transform.position = currentCrystal.transform.position;
            currentCrystal.transform.position = playerPos;


            if (cloneInsteadOfCrystal)
            {
                SkillManager.instance.clone.CreateClone(currentCrystal.transform, Vector3.zero);
                Destroy(currentCrystal);
            }
            else
            {
                currentCrystal.GetComponent<CrystalSillController>()?.FininshCrystal();

            }


        }

    }

    public void CreateCrystal()
    {
        currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
        CrystalSillController currentCrystalScript = currentCrystal.GetComponent<CrystalSillController>();
        currentCrystal.SetActive(true);
        currentCrystalScript.SetupScrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(currentCrystal.transform), player);
        
        currentCrystalScript.ChooseRandomEnemy();
    }
    public void CurrentCrystalChooseRandomTarget() => currentCrystal.GetComponent<CrystalSillController>().ChooseRandomEnemy();


   

    protected override void Update()
    {
        base.Update();
    }

    private bool CanUseMultiCrystal()
    {
        if (canUseMultiStacks)
        {
            if(crystalLeft.Count>0)
            {
                
                if (crystalLeft.Count == amountOfStacks)
                    Invoke("ResetAbility",useTimeWondow);

                coolDown = 0;
                GameObject crystalToSpawn = crystalLeft[crystalLeft.Count - 1];
                GameObject newCrystal = Instantiate(crystalToSpawn, player.transform.position, Quaternion.identity);
                newCrystal.SetActive(true);
                crystalLeft.Remove(crystalToSpawn);
                newCrystal.GetComponent<CrystalSillController>().SetupScrystal(crystalDuration,canExplode,canMoveToEnemy,moveSpeed,FindClosestEnemy(newCrystal.transform), player);
                if (crystalLeft.Count <= 0)
                {
                    coolDown = multiStackCooldown;
                    RefilCrystal();
                }
            return true;
            }
        }
        return false;
    }

    private void RefilCrystal()
    {
        int amountToAdd = amountOfStacks - crystalLeft.Count;

        for(int i =0;i< amountToAdd; i++)
        {
            crystalLeft.Add(crystalPrefab);
        }
    }
    private void ResetAbility()
    {
        if (coolDownTimer > 0)
            return;
        coolDownTimer = multiStackCooldown;
        RefilCrystal();
    }
}
