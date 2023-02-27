using System;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class NetworkTransformTest : NetworkTransform {
    
    protected override bool OnIsServerAuthoritative() {
        return false;
    }
}