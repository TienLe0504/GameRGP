using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlackHole_HotKey_Controller : MonoBehaviour
{
    private SpriteRenderer sr;
    private KeyCode myHotkey;
    private TextMeshProUGUI myText;
    private Transform myEnemy;
    private BlackHoleSkillController blackHole;
    public void SetupHotKey(KeyCode _myNewHotKey, Transform _myEnemy, BlackHoleSkillController _myBlackHole)
    {
        sr = GetComponent<SpriteRenderer>();
        myText = GetComponentInChildren<TextMeshProUGUI>();
        myHotkey = _myNewHotKey;
        myText.text = _myNewHotKey.ToString();
        myEnemy = _myEnemy;
        blackHole = _myBlackHole;
    }
    private void Update()
    {
        if (Input.GetKeyDown(myHotkey))
        {
            blackHole.AddEnemyToList(myEnemy);
            myText.color = Color.clear;
            sr.color = Color.clear;
        }
    }
}
