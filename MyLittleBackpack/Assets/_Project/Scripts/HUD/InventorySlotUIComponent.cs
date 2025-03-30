using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUIComponent : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
   [SerializeField] private Image _icon;
   [SerializeField] private Image _background;
   [SerializeField] private Color _selectedColor = Color.red;
   
   private ItemSlot _item;
   private IEventService _eventService;
   private Color _initialColor;

   public async void Setup(ItemSlot item)
   {
      _item = item;
      _icon.sprite = item.CurrentSprite;
      _initialColor = _background.color;
      _eventService = await ServiceLocator.GetService<IEventService>();
   }
   
   public void OnPointerEnter(PointerEventData eventData)
   {
      _eventService.AddListener<OnDropItemEvent>(HandleOnDropItem, GetHashCode());
      _background.color = _selectedColor;
   }

   public void OnPointerExit(PointerEventData eventData)
   {
      _eventService.RemoveListener<OnDropItemEvent>(GetHashCode());
      _background.color = _initialColor;
   }

   private void HandleOnDropItem(OnDropItemEvent obj)
   {
      if (_item.ItemData != null)
      {
         _eventService.TryInvokeEvent(new OnRemoveInventoryItemEvent(_item));
      }
   }
}
