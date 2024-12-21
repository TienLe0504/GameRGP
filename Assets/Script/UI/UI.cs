using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] private GameObject characterUI;
    [SerializeField] private GameObject skillTreeUI;
    [SerializeField] private GameObject craftUI;
    [SerializeField] private GameObject optionUI;
    public UI_ItemToolTip itemToolTip;
    public UI_StatTooltip statTooltip;
    public UI_CraftWindow craftWindow;
    public UI_SkillToolTip skillTooltip;
    void Start()
    {
        SwitchTo(null);
        itemToolTip.gameObject.SetActive(false);
        statTooltip.gameObject.SetActive(false);
    }

    private void Awake()
    {
        SwitchTo(skillTreeUI);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            SwitchWithKeyTo(characterUI);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            SwitchWithKeyTo(craftUI);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            SwitchWithKeyTo(skillTreeUI);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            SwitchWithKeyTo(optionUI);
        }
    }

    public void SwitchTo(GameObject _menu)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        if (_menu != null)
            _menu.SetActive(true);
    }

    public void SwitchWithKeyTo(GameObject _menu)
    {
        if (_menu != null && _menu.activeSelf)
        {
            _menu.SetActive(false);
            return;

        }
        SwitchTo(_menu);
    }
}
