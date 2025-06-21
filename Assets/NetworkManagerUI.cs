using Unity.Netcode;
using UnityEngine;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private GameObject parentObject;
    public void StartServer() 
    {
        NetworkManager.Singleton.StartServer();
        parentObject.SetActive(false);
    }

    public void StartHost() 
    {
        NetworkManager.Singleton.StartHost();
        parentObject.SetActive(false);
    }

    public void StartClient() 
    {
        NetworkManager.Singleton.StartClient();
        parentObject.SetActive(false);
    }
}
