using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace ClientServer
{
    public class Player : NetworkBehaviour {

        [Header("PLAYER SETTINGS")] public static List<Player> Players = new List<Player>();
        public GameObject cameraPrefab;
        private Camera camera;
        private NetworkObject NetworkObject;
        [SerializeField] private float speed;
        [SerializeField] private Transform cameraTransform;
        private Quaternion _rotation;
        private float gravityValue = -9.81f, v, h;
        private Vector3 playerVelocity;
        [SerializeField] private float rotationHorizontalSpeed;
        [SerializeField] private float rotationVerticalSpeed;
        [SerializeField] private float verticalLimit;
        [SerializeField] private Slider Life;
        [SerializeField] private float hp;
        [SerializeField] private float maxhp;
        [SerializeField] private Transform respawn;
        
        [Header("GUN SETTINGS")]
        private RaycastHit _hit;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Transform gunTransform;
        [SerializeField] private float maxRange;
        private bool _isShooting;
        private float _laserLifeTime;
        

        public override void OnNetworkSpawn() {
            GameObject cam = Instantiate(cameraPrefab);
            CameraPlayer script = cam.GetComponent<CameraPlayer>();
            camera = cam.GetComponent<Camera>();
            script.player = this;
            Players.Add(this);
            hp = maxhp;
            Life.gameObject.SetActive(IsOwner);
            Life.maxValue = maxhp;
        }

        public override void OnNetworkDespawn() {
            Players.Remove(this);
        }

        public void Move() {
            if(!IsOwner) return;
            Vector3 position = transform.position;
            position.x += Input.GetAxis("Horizontal");
            position.z += Input.GetAxis("Vertical");
            transform.position = position;
        }
        
        [ClientRpc]
        void ShootClientRPC() {
            if (!Input.GetButtonDown("Fire1")) return;
            if (!IsOwner) return;
            _isShooting = true;
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, gunTransform.position);
            lineRenderer.SetPosition(1, transform.forward * maxRange);
            if (Physics.Raycast(transform.position, transform.forward, out _hit, maxRange)) {
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
            Vector3 mousepos = new Vector3(Input.mousePosition.x, 0, Input.mousePosition.y);
            mousepos = camera.ScreenToWorldPoint(mousepos);
            mousepos.y = transform.position.y;
            transform.rotation = Quaternion.LookRotation(mousepos);
        }

        
        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag("Zombie")) {
                hp--;
                if (hp <= 0) {
                    transform.position = respawn.position;
                    hp = maxhp;
                }

                Life.value = hp;
            }
        }

        void Update() {
            Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            Move();
            ShootClientRPC();
            HideLaser();
            Rotate();
        }
    }
}
