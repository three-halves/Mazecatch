using Unity.Netcode;
using UnityEngine;

namespace Mazecatch.Weapons
{
    public class PipeGun : Weapon
    {
        [SerializeField] private NetworkObject projectileObject;
        [SerializeField] private float projGravity = 10f;
        [SerializeField] private float projSpeed = 15f;
        [SerializeField] private float arcHeight = 1f;
        private UserAvatar _ua;

        public override void Use(UserAvatar ua)
        {
            Debug.Log("USE BASE");
            base.Use(ua);
            _ua = ua;
            UseServerRPC();
        }

        [ServerRpc]
        private void UseServerRPC()
        {
            Debug.Log("UseServerRPC Call");
            if (!IsUsable) return;
            var spawnedPipe = Instantiate(projectileObject);
            spawnedPipe.transform.position = _ua.transform.position; 
            spawnedPipe.GetComponent<Projectile>().Setup
            (
                new Vector3(0, -projGravity, 0),
                _ua.GetLookQuaternion() * Vector3.forward * projSpeed + Vector3.up * arcHeight,
                _ua.OwnerClientId
            );
            spawnedPipe.GetComponent<NetworkObject>().Spawn();
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
