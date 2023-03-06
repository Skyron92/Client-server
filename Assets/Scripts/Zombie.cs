using System.Linq;
using ClientServer;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : NetworkBehaviour {
    
    private NavMeshAgent _navMeshAgent;
    private Vector3 _targetPosition;
    private Vector3 _targetDirection;
    private Vector3 _targetPoint;
    private float _x, _z, _distanceWithPlayer;
    private bool HasReachedDestination => Vector3.Distance(transform.position, _targetPoint) < 0.1f;

    public void Awake() {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update() {
        SetUpDestination();
    }

    private void SetUpDestination() {
        if(HasReachedDestination) return;
        Player firstOrDefault = Player.Players.First();
        _targetPosition = firstOrDefault.transform.position;
        _targetDirection = _targetPosition - transform.position;
        _distanceWithPlayer = Vector3.Distance(_targetDirection, transform.position);
        _targetPoint = _targetDirection.normalized * _distanceWithPlayer;
        _navMeshAgent.SetDestination(_targetPoint);
    }
}
