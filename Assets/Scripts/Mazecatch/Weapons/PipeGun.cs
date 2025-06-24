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

        public override void Use(UserAvatar ua)
        {
            base.Use(ua);
            if (!IsUsable) return; 
            var spawnedPipe = NetworkManager.SpawnManager.InstantiateAndSpawn(projectileObject);
            spawnedPipe.transform.position = ua.transform.position; 
            spawnedPipe.GetComponent<Projectile>().Setup
            (
                new Vector3(0, -projGravity, 0),
                ua.GetLookQuaternion() * Vector3.forward * projSpeed + Vector3.up * arcHeight,
                ua.OwnerClientId
            );
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
