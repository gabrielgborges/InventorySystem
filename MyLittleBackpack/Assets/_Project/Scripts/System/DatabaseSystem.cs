using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class DatabaseSystem : MonoBehaviour
{
    private IEventService _eventService;
    
    private readonly string _serverAddress = "https://wadahub.manerai.com/api/inventory/status";
    private readonly string _authKey = "kPERnYcWAY46xaSy8CEzanosAgsWM84Nx7SKM4QBSqPq6c7StWfGxzhxPfDh8MaP";

    private UnityEvent<InventoryResponse> OnInventoryRequestFinished; //Could be used by other classes, specially if 
    //DatabaseSystem were a Service (like ScreenService), but since in this specificproject the EventService is already being
    //used for the same purposes, I personally do not find a benefit on using it outside this script.
    
    private void Start()
    {
        Initialize();
    }

    private void OnDestroy()
    {
        _eventService.RemoveListener<OnRemoveInventoryItemEvent>(GetHashCode());
    }

    private async void Initialize()
    {
        _eventService = await ServiceLocator.GetService<IEventService>();
        _eventService.AddListener<OnRemoveInventoryItemEvent>(HandleRemoveItem, GetHashCode());
        _eventService.AddListener<OnAddItemEvent>(HandlAddItem, GetHashCode());
    }

    private void HandlAddItem(OnAddItemEvent obj)
    {
        if (obj.Item != null)
        {
            InventoryRequestData data = new InventoryRequestData()
            {
                item_id = obj.Item.ID,
                request = InventoryRequestType.POST
            };
            
            StartCoroutine(SendInventoryRequest(data));
        }    }

    private void HandleRemoveItem(OnRemoveInventoryItemEvent obj)
    {
        if (obj.Item.ItemData != null)
        {
            InventoryRequestData data = new InventoryRequestData()
            {
                item_id = obj.Item.ItemData.ID,
                request = InventoryRequestType.DELETE
            };
            
            StartCoroutine(SendInventoryRequest(data));
        }
    }
    
    private IEnumerator SendInventoryRequest(InventoryRequestData requestData)
    {
        string jsonData = JsonUtility.ToJson(requestData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new UnityWebRequest(_serverAddress, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {_authKey}");

        yield return request.SendWebRequest();

        
        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;
            InventoryResponse response = JsonUtility.FromJson<InventoryResponse>(jsonResponse);
            OnInventoryRequestFinished?.Invoke(response);
            Debug.Log($"Status: {response.status}, Item Sent: {response.data_submitted.item_id}, Qty: {response.data_submitted.request.ToString()}");
        }
        else
        {
            Debug.LogError("Error sending request: " + request.error);
        }
    }
    
}
internal enum InventoryRequestType
{
    SET = 0,
    GET = 1,
    DELETE = 2,
    POST = 3
}

[Serializable]
internal struct InventoryRequestData
{
    public string item_id;
    public InventoryRequestType request;
}

[Serializable]
internal struct InventoryResponse
{
    public string response;
    public string status;
    public InventoryRequestData data_submitted;
}