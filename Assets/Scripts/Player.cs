using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.Serialization;

namespace ClientServer
{
    public class Player : NetworkBehaviour {
        
        [Header("PLAYER SETTINGS")]
        [SerializeField] private CharacterController _controller;

        public static List<Player> Players = new List<Player>();
        private NetworkObject NetworkObject;
        [SerializeField] private float speed;
        [SerializeField] private Transform cameraTransform;
        private Quaternion _rotation;
        private float gravityValue = -9.81f, v, h;
        private Vector3 playerVelocity;
        [SerializeField] private float rotationHorizontalSpeed;
        [SerializeField] private float rotationVerticalSpeed;
        [SerializeField] private float verticalLimit;
        
        [Header("GUN SETTINGS")]
        private RaycastHit _hit;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Transform gunTransform;
        [SerializeField] private float maxRange;
        private bool _isShooting;
        private float _laserLifeTime;
        

        public override void OnNetworkSpawn() {
            GetComponentInChildren<Camera>().enabled = IsOwner;
            GetComponentInChildren<AudioListener>().enabled = IsOwner;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public override void OnNetworkDespawn() {
            Players.Remove(this);
        }

        private void Awake() {
            Players.Add(this);
        }

        public void Move() {
            if(!IsOwner) return;
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
            if (!IsOwner) return;
            _isShooting = true;
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, gunTransform.position);
            lineRenderer.SetPosition(1, cameraTransform.forward * maxRange);
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out _hit, maxRange)) {
                if (_hit.collider.gameObject.CompareTag("Zombie")) {
                    NetworkObject no = _hit.collider.gameObject.GetComponent<NetworkObject>();
                    no.Despawn();
                }
            }
        }

        void HideLaser() {
            if(!IsOwner) return;
            _laserLifeTime += Time.deltaTime;
            if (_laserLifeTime >= 0.2f) {
                _laserLifeTime = 0;
                lineRenderer.enabled = false;
                _isShooting = false;
            }
        }

        void Rotate() {
            if(!IsOwner) return;
            _rotation.x += Input.GetAxis("Mouse X") * rotationHorizontalSpeed;
            _rotation.y += Input.GetAxis("Mouse Y") * rotationVerticalSpeed;
            _rotation.y = Mathf.Clamp(_rotation.y, -verticalLimit, verticalLimit);
            Quaternion Xquat = Quaternion.AngleAxis(_rotation.x, Vector3.up);
            Quaternion Yquat = Quaternion.AngleAxis(_rotation.y, Vector3.left);
            cameraTransform.localRotation = Xquat * Yquat;

            var transformLocalRotation = transform.rotation;
            transformLocalRotation.y = cameraTransform.localRotation.y;
            transform.rotation = transformLocalRotation;
        }

        void Update() {
            Move();
            Shoot();
            HideLaser();
            Rotate();
        }
    }
}
