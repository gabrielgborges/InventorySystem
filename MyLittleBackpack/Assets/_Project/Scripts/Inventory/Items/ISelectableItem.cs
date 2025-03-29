using UnityEngine;

public interface ISelectableItem
{
    public void Hover();
    public void Select();
    public void OnDrag(Vector3 position);
    public GameObject Deselect();
}
