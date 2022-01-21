using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoraCore;

public class RanPosMeshTest : BenchmarkableMono {
    public Mesh mesh;
    public override void Action() {
        Vector3 ranPos = Math.GetRandomPointOnMesh(mesh);
        Debug.Log(ranPos);
    }
}
