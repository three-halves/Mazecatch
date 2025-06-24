using Unity.Netcode;
using UnityEngine;

namespace Mazecatch.Weapons
{
    abstract public class Weapon : NetworkBehaviour
    {
        [Tooltip("Name")]
        [SerializeField] public string weaponName = "Gun";
        
        [Tooltip("Cooldown (Seconds)")]
        [SerializeField] public float cooldownTime = 0.5f;
        private float _currentCooldown = 0.0f;
        public bool IsUsable {get; private set;}= true;

        public virtual void Update()
        {
            _currentCooldown = Mathf.Max(_currentCooldown - Time.deltaTime, 0f);
        }

        public virtual void Use(UserAvatar ua)
        {
            // Weapon cooldown fail state
            IsUsable = _currentCooldown == 0f;
            if (IsUsable) _currentCooldown = cooldownTime;
        }
    }
}
