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
        
        [Header("GUN SETTINGS")]
        [SerializeField] private Transform gunTransform;

        [SerializeField] private GameObject bullet;
        [SerializeField] private float bulletSpeed;
        

        public override void OnNetworkSpawn() {
            if (IsOwner) {
                Move();
            }
            GetComponentInChildren<Camera>().enabled = IsLocalPlayer ? true : false;
        }

        public void Move() {
            if(!IsOwner && !NetworkManager.Singleton.IsServer && !IsLocalPlayer && IsHost) return;
            force = new Vector3(Input.GetAxisRaw("Horizontal"),0 ,Input.GetAxisRaw("Vertical")) * speed * Time.deltaTime;
            _rigidbody.AddForce(force, ForceMode.Acceleration);
        }

        void Shoot() {
            if (!Input.GetButtonDown("Fire1")) return;
            if (!NetworkManager.Singleton.IsServer && !IsOwner) return;
            GameObject instance = Instantiate(bullet, gunTransform);
            Rigidbody bulRb = instance.GetComponent<Rigidbody>();
            bulRb.AddForce(transform.forward * bulletSpeed * Time.deltaTime, ForceMode.Impulse);
        }

        void Update() {
            Move();
            Shoot();
        }
        
    }
}
