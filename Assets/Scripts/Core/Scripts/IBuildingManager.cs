public interface IBuildingManager
{
    void SelectBuilding(int index);
    void SetPlaceMode();
    void SetRemoveMode();
    void OnMouseOverUIChanged(bool isOverUI);
}