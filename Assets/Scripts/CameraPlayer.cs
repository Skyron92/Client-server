using ClientServer;
using UnityEngine;

public class CameraPlayer : MonoBehaviour {

    public Player player;

    void Update() {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 10,
            player.transform.position.z - 1.5f);
    }
}
