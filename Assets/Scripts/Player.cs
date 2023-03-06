using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace ClientServer
{
    public class Player : NetworkBehaviour {
        
        [Header("PLAYER SETTINGS")]
        [SerializeField] private CharacterController _controller;

        public static List<Player> Players = new List<Player>();

        [SerializeField] private float speed;
        [SerializeField] private Transform cameraTransform;
        Quaternion rotation;
        private float gravityValue = -9.81f, v, h;
        private Vector3 playerVelocity;
        [SerializeField] private float rotationHorizontalSpeed;
        [SerializeField] private float rotationVerticalSpeed;
        [SerializeField] private float VerticalLimit;
        
        [Header("GUN SETTINGS")]
        private RaycastHit _hit;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Transform gunTransform;

        [SerializeField] private float maxRange;
        

        public override void OnNetworkSpawn() {
            if (IsOwner) {
                Move();
            }
            GetComponentInChildren<Camera>().enabled = IsLocalPlayer;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Awake() {
            if(IsOwner) Players.Add(this);
        }

        public void Move() {
            if(!IsOwner && !NetworkManager.Singleton.IsServer && !IsLocalPlayer && IsHost) return;
            Vector3 move = Vector3.zero;
            v = 0;
            h = 0;
            v += Input.GetAxis("Vertical");
            h += Input.GetAxis("Horizontal");
            move += cameraTransform.forward * v * speed * Time.deltaTime;
            move += cameraTransform.right * h * speed * Time.deltaTime;
            _controller.Move(move);

            playerVelocity.y += gravityValue * Time.deltaTime;
            _controller.Move(playerVelocity * Time.deltaTime);
        }

        void Shoot() {
            if (!Input.GetButtonDown("Fire1")) return;
            if (!IsLocalPlayer) return;
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out _hit, maxRange)) {
                if (_hit.collider.gameObject.CompareTag("Zombie")) {
                    NetworkObject no = _hit.collider.gameObject.GetComponent<NetworkObject>();
                    no.Despawn();
                }
            }
        }

        void Rotate() {
            if(!IsOwner && !NetworkManager.Singleton.IsServer && !IsLocalPlayer && IsHost) return;
            rotation.x += Input.GetAxis("Mouse X") * rotationHorizontalSpeed;
            rotation.y += Input.GetAxis("Mouse Y") * rotationVerticalSpeed;
            rotation.y = Mathf.Clamp(rotation.y, -VerticalLimit, VerticalLimit);
            Quaternion Xquat = Quaternion.AngleAxis(rotation.x, Vector3.up);
            Quaternion Yquat = Quaternion.AngleAxis(rotation.y, Vector3.left);
            cameraTransform.localRotation = Xquat * Yquat;

            var transformLocalRotation = transform.rotation;
            transformLocalRotation.y = cameraTransform.localRotation.y;
            transform.rotation = transformLocalRotation;
        }

        void Update() {
            Move();
            Shoot();
            Rotate();
        }
    }
}
