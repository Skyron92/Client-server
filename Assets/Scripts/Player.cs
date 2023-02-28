using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;

namespace ClientServer
{
    public class Player : NetworkBehaviour {
        
        [Header("PLAYER SETTINGS")]
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Vector3 force;
        
        
        [SerializeField] private float speed;
        [SerializeField] private float rotationHorizontalSpeed;
        [SerializeField] private float rotationVerticalSpeed;
        
        [Header("GUN SETTINGS")]
        [SerializeField] private Transform gunTransform;

        [SerializeField] private GameObject bullet;
        [SerializeField] private float bulletSpeed;
        

        public override void OnNetworkSpawn() {
            if (IsOwner) {
                Move();
            }
            GetComponentInChildren<Camera>().enabled = IsLocalPlayer;
        }

        public void Move() {
            if(!IsOwner && !NetworkManager.Singleton.IsServer && !IsLocalPlayer && IsHost) return;
            force = new Vector3(Input.GetAxisRaw("Horizontal"),0 ,Input.GetAxisRaw("Vertical")) * speed * Time.deltaTime;
            _rigidbody.AddForce(force, ForceMode.Acceleration);
        }

        void Shoot() {
            if (!Input.GetButtonDown("Fire1")) return;
            if (!NetworkManager.Singleton.IsServer && !IsOwner && !IsLocalPlayer && IsHost) return;
            GameObject instance = Instantiate(bullet, gunTransform);
            Rigidbody bulRb = instance.GetComponent<Rigidbody>();
            bulRb.AddForce(transform.forward * bulletSpeed * Time.deltaTime, ForceMode.Impulse);
        }

        void Rotate() {
            if(!IsOwner && !NetworkManager.Singleton.IsServer && !IsLocalPlayer && IsHost) return;
            Quaternion rotation = new Quaternion();
            rotation.y += Input.mousePosition.x * rotationHorizontalSpeed;
            rotation.x += Input.mousePosition.y * rotationVerticalSpeed;
            Mathf.Clamp(rotation.x, -35, 45);
            transform.rotation = rotation;
        }

        void Update() {
            Move();
            Shoot();
            Rotate();
        }
        
    }
}
