using UnityEngine;

namespace Mazecatch
{
    [RequireComponent(typeof(Transform))]
    public class Projectile : MonoBehaviour
    {
        private Vector3 _acceleration;
        private Vector3 _velocity;
        public ulong OwnerID {get; private set;}

        void Update()
        {
            if (_acceleration == null || _velocity == null) return;
            transform.position += _velocity * Time.deltaTime;
            _velocity += _acceleration * Time.deltaTime;
        }

        public void Setup(Vector3 acceleration, Vector3 velocity, ulong ownerID)
        {
            _acceleration = acceleration;
            _velocity = velocity;
            OwnerID = ownerID;
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.GetComponent<UserAvatar>());
            Debug.Log(other.name);
            if (other.GetComponent<UserAvatar>()?.OwnerClientId == OwnerID) return;
            Destroy(gameObject);
        }

    }
}


