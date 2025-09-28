using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditModeMenu : MonoBehaviour
{
    int lastEnabledToolbar;
    public GameObject[] tooltips;

    [SerializeField] EditMode editMode;

    [Header("Edit Player")]
    [SerializeField] Slider moveRangeSlider;
    [SerializeField] TextMeshProUGUI moveRangeText;
    [SerializeField] Slider attackRangeSlider;
    [SerializeField] TextMeshProUGUI attackRangeText;
    [Header("Edit Map")]
    [SerializeField] TMP_InputField xCoordInput;
    [SerializeField] TMP_InputField yCoordInput;
    [SerializeField] Toggle obstaclesToggle;

    private void Start()
    {
        editMode.onChangeState.AddListener(OnChangeState);
        ChangeDisplayedToolbar(0);
    }

    public void OnChangeState() {
        int newState = (int)editMode.currentState;
        ChangeDisplayedToolbar(newState);
    }

    public void ChangeDisplayedToolbar(int stateID)
    {
        tooltips[lastEnabledToolbar].SetActive(false);
        lastEnabledToolbar = stateID;
        tooltips[stateID].SetActive(true);
    }

    public void UpdateMoveSlider()
    {
        moveRangeText.text = moveRangeSlider.value.ToString();
        editMode.UpdateMoveRange(Mathf.RoundToInt(moveRangeSlider.value));
    }

    public void UpdateAttackSlider()
    {
        attackRangeText.text = attackRangeSlider.value.ToString();
        editMode.UpdateAttackRange(Mathf.RoundToInt(attackRangeSlider.value));
    }

    public void GenerateMap()
    {
        Vector2Int mapSize = new Vector2Int(int.Parse(xCoordInput.text), int.Parse(yCoordInput.text));
        if (mapSize == null) { Debug.LogError("Something happened"); return; }
        editMode.UpdateMap(mapSize, obstaclesToggle.isOn);
    }
}
