using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button[] buildingButtons;
    [SerializeField] private Button placeButton;
    [SerializeField] private Button removeButton;
    [SerializeField] private Button clearButton;

    private BuildingManager buildingManager;
    private ISaveLoadService saveLoadService;
    private UIMouseTracker uiMouseTracker;

    [Inject]
    public void Construct(BuildingManager building, ISaveLoadService saveLoad)
    {
        buildingManager = building;
        saveLoadService = saveLoad;

        uiMouseTracker = gameObject.AddComponent<UIMouseTracker>();
        uiMouseTracker.Initialize(buildingManager.OnMouseOverUIChanged);

        for (int i = 0; i < buildingButtons.Length; i++)
        {
            int index = i;
            buildingButtons[i].onClick.AddListener(() => buildingManager.SelectBuilding(index));
        }

        placeButton.onClick.AddListener(() => buildingManager.SetPlaceMode());
        removeButton.onClick.AddListener(() => buildingManager.SetRemoveMode());

        clearButton.onClick.AddListener(ClearSaves);
    }

    private void ClearSaves()
    {
        saveLoadService.ClearSaves();
    }
}