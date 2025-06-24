using System;
using Mazecatch.Weapons;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Mazecatch
{
    public class UserAvatar : NetworkBehaviour
    {
        private Transform cameraTransform;
        [SerializeField] private Vector2 camSens;

        [SerializeField] CharacterController characterController;
        private TextMeshProUGUI debugText;
        private bool jumpPressed = false;
        private bool attackPressed = false;
        private bool groundedLastTick = false;

        [SerializeField] private GameObject _heldWeaponPrefab;
        // Set in script
        [NonSerialized] private Weapons.Weapon _heldWeapon;

        // TODO move movement constants to server variables
        [SerializeField] private float _groundAcceleration = 0.5f;
        [SerializeField] private float _airAcceleration = 0.25f;
        [SerializeField] private float _groundMaxVel = 10f;
        [SerializeField] private float _airMaxVel = 20f;
        [SerializeField] private float _friction = 1f;
        [SerializeField] private float _gravity = -0.01f;
        [SerializeField] private float _jumpVel = 10f;
        [SerializeField] private bool _alwaysApplyFriction = true;

        private Vector3 _moveInputDir = Vector3.zero;
        private Vector3 _rawMoveInputDir = Vector3.zero;

        public void SetupWeapon(NetworkObject weaponPrefab)
        {
            NetworkObject _heldWeaponObject = NetworkManager.SpawnManager.InstantiateAndSpawn(weaponPrefab);
            FollowTransform ft = _heldWeaponObject.AddComponent<FollowTransform>();
            ft.SetToFollow(transform);
            _heldWeapon = _heldWeaponObject.GetComponent<Weapon>();

        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;

            Cursor.lockState = CursorLockMode.Locked;

            debugText = GameObject.Find("DebugText").GetComponent<TextMeshProUGUI>();

            // Get weapon script reference
            SetupWeapon(_heldWeaponPrefab.GetComponent<NetworkObject>());

            // Set up camera follower
            cameraTransform = Camera.main.transform;
            cameraTransform.forward = transform.forward;
            Camera.main.GetComponent<FollowTransform>().SetToFollow(transform);
            Camera.main.GetComponent<FollowTransform>().enabled = true;
        }

        void FixedUpdate()
        {
            // Only function if network client owns this player object
            if (!IsOwner) return;

            debugText.text = string.Format(
                "CCVel:\t{0}\nInputDir:\t{1}\n{2}\n{3}\n{4}", 
                characterController.velocity, 
                _moveInputDir, jumpPressed, 
                characterController.isGrounded, 
                Move(_moveInputDir, characterController.velocity).y
            );  

            // Weapon controls
            if (attackPressed) _heldWeapon.Use(this);

            // Player movement
            Vector3 delta = Move(_moveInputDir, characterController.velocity);

            // Apply gravity and jump
            if (characterController.isGrounded && jumpPressed)
            {
                delta.y = _jumpVel * Time.fixedDeltaTime;
                jumpPressed = false;
            }
            delta.y += _gravity * Time.fixedDeltaTime;
            groundedLastTick = characterController.isGrounded;

            characterController.Move(delta);
        }

        private Vector3 AddAcceleration(Vector3 inputDir, Vector3 currentVel, float acceleration, float maxVel)
        {
            float projectedVel = Vector3.Dot(currentVel * Time.fixedDeltaTime, inputDir);
            float accelVel = acceleration * Time.fixedDeltaTime;
            maxVel *= Time.fixedDeltaTime;

            // Cap max accel
            if (projectedVel + accelVel > maxVel)
            {
                accelVel = maxVel - projectedVel;
            }
            
            return currentVel * Time.fixedDeltaTime + inputDir * accelVel;
        }

        private Vector3 Move(Vector3 inputDir, Vector3 currentVel)
        {
            bool useGroundPhys = groundedLastTick && characterController.isGrounded;
            // Apply friction
            Vector3 lateralVel = Vector3.Scale(currentVel, new Vector3(1, 0, 1));
            if (lateralVel.magnitude != 0 && (useGroundPhys|| _alwaysApplyFriction))
            {
                float d = lateralVel.magnitude * _friction * Time.fixedDeltaTime;
                currentVel.x *= Mathf.Max(lateralVel.magnitude - d, 0) / lateralVel.magnitude;
                currentVel.z *= Mathf.Max(lateralVel.magnitude - d, 0) / lateralVel.magnitude;
            }

            return AddAcceleration(
                inputDir,
                currentVel,
                useGroundPhys ? _groundAcceleration : _airAcceleration,
                useGroundPhys ? _groundMaxVel : _airMaxVel
                );
        }

        public void OnLook(InputValue value)
        {
            Vector2 v = value.Get<Vector2>();
            // Rotate user and cam to with mouse x movement
            transform.Rotate(Vector3.up, v.x * camSens.x, Space.World);
            cameraTransform.Rotate(Vector3.up, v.x * camSens.x, Space.World);

            // Rotate only cam with mouse y movement
            cameraTransform.Rotate(Vector3.right, v.y * camSens.y, Space.Self);
            calculateMoveInputDir();
        }

        public void OnMove(InputValue value)
        {
            Vector2 v = value.Get<Vector2>();
            _rawMoveInputDir.x = v.x;
            _rawMoveInputDir.z = v.y;
            _rawMoveInputDir.Normalize();
            calculateMoveInputDir();
        }

        // Rotate desired move dir with cam
        public void calculateMoveInputDir()
        {
            _moveInputDir = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * _rawMoveInputDir;
        }

        public void OnJump(InputValue value)
        {
            jumpPressed = value.isPressed;
        }

        public void OnRestart(InputValue value)
        {
            characterController.enabled = false;
            transform.position = new Vector3(0, 2, 0);
            characterController.enabled = true;
        }

        public void OnAttack(InputValue value)
        {
            if (_heldWeapon != null) 
                attackPressed = value.isPressed;
        }

        public Quaternion GetLookQuaternion()
        {
            return cameraTransform.rotation;
        }

        [ServerRpc]
        private void TestServerRpc()
        {
            Debug.Log("TestServerRPC" + OwnerClientId);
        }
    }
}
