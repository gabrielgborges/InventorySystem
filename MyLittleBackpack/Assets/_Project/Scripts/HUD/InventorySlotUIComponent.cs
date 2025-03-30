using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUIComponent : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
   [SerializeField] private Image _icon;
   [SerializeField] private Image _background;
   
   private ItemSlot _item;
   private IEventService _eventService;

   public async void Setup(ItemSlot item)
   {
      _item = item;
      _icon.sprite = item.CurrentSprite;
      _eventService = await ServiceLocator.GetService<IEventService>();
   }

   private void HandleOnDropItem(OnDropItemEvent obj)
   {
      _eventService.TryInvokeEvent(new OnRemoveInventoryItemEvent(_item));
   }

   public void OnPointerEnter(PointerEventData eventData)
   {
      _eventService.AddListener<OnDropItemEvent>(HandleOnDropItem, GetHashCode());
   }

   public void OnPointerExit(PointerEventData eventData)
   {
      _eventService.RemoveListener<OnDropItemEvent>(GetHashCode());
   }
}
