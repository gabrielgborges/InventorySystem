using MLB.Inventory.Items;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUIComponent : MonoBehaviour, IPointerUpHandler
{
   [SerializeField] private Image _icon;
   [SerializeField] private Image _background;
   
   private ItemSlot _item;

   public void Setup(ItemSlot item)
   {
      _item = item;
      _icon.sprite = item.CurrentSprite;
   }

   public async void OnPointerUp(PointerEventData eventData)
   {
      IEventService eventService = await ServiceLocator.GetService<IEventService>();
      eventService.TryInvokeEvent(new OnSelectItemSlotEvent(_item));
   }
}
