using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleSkillController : MonoBehaviour
{
    [SerializeField] private GameObject hotKeyPrefab;
    [SerializeField] private List<KeyCode> keyCodeList;
    public float maxSize;
    public float growSpeed;
    public float shrinkSpeed;
    public bool canGrow = true;
    public bool canShrink = false;
    private float blackHoleTimer;
    private bool playerCanDisappear =true;

    private bool canCreateHotKeys = true;
    private bool cloneAttackRepleased;
    public int amountOfAttacks;
    public float cloneAttackCooldown;
    private float cloneAttackTimer;
    private List<Transform> targets = new List<Transform>();
    private List<GameObject> createdHotKey = new List<GameObject> ();
    public bool playerCanExitState {  get; private set; }

    public void SetupBlackHole(float _maxSize, float _growSpeed, float _shrinkSpeed, int _amountofAttacks, float _cloneAttackCoolDown, float _blackHoleDuration)
    {
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeed;
        amountOfAttacks = _amountofAttacks;
        cloneAttackCooldown = _cloneAttackCoolDown;
        blackHoleTimer = _blackHoleDuration;


        if (SkillManager.instance.clone.crystalInsteadOfClone)
            playerCanDisappear = false;
            

    }

    private void Awake()
    {
        canGrow = true;
        canShrink = false;
    }
    private void Update()
    {
        cloneAttackTimer -= Time.deltaTime;
        blackHoleTimer -= Time.deltaTime;
        if (blackHoleTimer < 0)
        {

            blackHoleTimer = Mathf.Infinity;
            if (targets.Count > 0)
            {
                ReleaseCloneAttack();
            }
            else
            {
                FinishBlackHoleAbility();
            }
        }
        CloneAttackLogic();
        if (canGrow && !canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);
        }
        if (canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-1, -1), shrinkSpeed * Time.deltaTime);
            if (transform.localScale.x < 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void CloneAttackLogic()
    {
        if (cloneAttackTimer < 0 && cloneAttackRepleased && amountOfAttacks >0)
        {
            cloneAttackTimer = cloneAttackCooldown;
            int randomindex = Random.Range(0, targets.Count);
            float xOffset;
            if (Random.Range(0, 100) > 50)
                xOffset = 1;
            else
                xOffset = -1;

            if (SkillManager.instance.clone.crystalInsteadOfClone)
            {
                SkillManager.instance.crystal.CreateCrystal();
                SkillManager.instance.crystal.CurrentCrystalChooseRandomTarget();
            }
            else
            {
                SkillManager.instance.clone.CreateClone(targets[randomindex], new Vector3(xOffset, 0));

            }
            amountOfAttacks--;
            if (amountOfAttacks <= 0)
            {
                Invoke("FinishBlackHoleAbility", 1.7f);
            }
        }
    }

    private void FinishBlackHoleAbility()
    {
        DestroyHotKeys();
        playerCanExitState = true;
        canShrink = true;
        cloneAttackRepleased = false;
        //PlayerManager.instance.player.ExitBlackHoleAbility();
    }

    private void ReleaseCloneAttack()
    {
        if (targets.Count <= 0)
            return;
        DestroyHotKeys();
        cloneAttackRepleased = true;
        canCreateHotKeys = false;
        if (playerCanDisappear)
        {
            playerCanDisappear = false;
            PlayerManager.instance.player.MakeTransprent(true);
        }
    }

    private void DestroyHotKeys()
    {
        if (createdHotKey.Count > 0)
        {
            for(int i = 0; i < createdHotKey.Count; i++)
            {
                Destroy(createdHotKey[i]);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            collision.GetComponent<Enemy>().FreezeTime(true);
            CreateHotKey(collision);

        }
    }

    private void OnTriggerExit2D(Collider2D collision)=> collision.GetComponent<Enemy>()?.FreezeTime(false);

    private void CreateHotKey(Collider2D collision)
    {
        if (keyCodeList.Count <= 0)
        {
            Debug.LogWarning("Not enough hot keys in a key code list");
            return;
        }
        if (!canCreateHotKeys)
            return;
        GameObject newHotKey = Instantiate(hotKeyPrefab, collision.transform.position + new Vector3(0, 2), Quaternion.identity);
        createdHotKey.Add(newHotKey);
        KeyCode choosenKey = keyCodeList[Random.Range(0, keyCodeList.Count)];
        keyCodeList.Remove(choosenKey);

        BlackHole_HotKey_Controller newHotKeyScript = newHotKey.GetComponent<BlackHole_HotKey_Controller>();
        newHotKeyScript.SetupHotKey(choosenKey, collision.transform, this);
    }

    public void AddEnemyToList(Transform _enemyTransform) =>targets.Add(_enemyTransform);
}
