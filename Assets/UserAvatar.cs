using Unity.Netcode;
using UnityEngine;

public class UserAvatar : NetworkBehaviour
{
    [SerializeField] float moveSpeed = 0.5f;

    void Update()
    {
        // Only function if network client owns this player object
        if (!IsOwner) return;

        // Player movement
        if (Input.GetKey(KeyCode.LeftArrow)) 
            transform.position += new Vector3(-moveSpeed * Time.deltaTime, 0 ,0);
        if (Input.GetKey(KeyCode.RightArrow)) 
            transform.position += new Vector3(moveSpeed * Time.deltaTime, 0 ,0);
        if (Input.GetKey(KeyCode.UpArrow)) 
            transform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);
        if (Input.GetKey(KeyCode.DownArrow)) 
            transform.position += new Vector3(0, -moveSpeed * Time.deltaTime, 0);
    }

    [ServerRpc]
    private void TestServerRpc()
    {
        Debug.Log("TestServerRPC" + OwnerClientId);
    }
}


