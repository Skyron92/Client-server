using Unity.Netcode.Components;
using UnityEngine;

[DisallowMultipleComponent]
public class NetworkTransformTest : NetworkTransform {
    
    protected override bool OnIsServerAuthoritative() {
        return false;
    }
}