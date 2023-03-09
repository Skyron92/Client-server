using ClientServer;
using UnityEngine;

public class CameraPlayer : MonoBehaviour {

    public Player player;

    void Update() {
        if(player == null) return;
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 25,
            player.transform.position.z);
    }
}
