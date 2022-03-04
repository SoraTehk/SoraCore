using MyBox;
using SoraCore;
using SoraCore.Manager;
using UnityEngine;

public class BulletSpawner : MonoBehaviour {
    [SerializeField] private PrefabSO _bulletPrefab;
    [SerializeField] private AudioSO _shotAudio;
    [OverrideLabel("Spawn Rate (x/s)")]
    [Range(1, 100)]
    [SerializeField] private float _spawnRate;
    [MinMaxRange(0.1f, 200f)]
    [SerializeField] private RangedFloat _bulletSpeed = new(10f, 20f);
    [SerializeField] private Mesh _mesh;

    private float _nextSpawnTime;

    private void Update() {
        if (Time.timeSinceLevelLoad >= _nextSpawnTime) {
            // Spawn a disabled bullet
            GameObject gObj = GameObjectManager.Instantiate(_bulletPrefab);
            // Calculate random pos in mesh (local & normalized)
            Vector3 pointOnMeshLocPosNormalized = Math.GetRandomPointOnMesh(_mesh);
            // Set bullet pos
            gObj.transform.position = transform.position + transform.TransformVector(pointOnMeshLocPosNormalized);

            // Clear trail
            gObj.GetComponent<TrailRenderer>().Clear();

            // Shoot the bullet
            float speed = Random.Range(_bulletSpeed.Min, _bulletSpeed.Max);
            gObj.GetComponent<Bullet>().Shoot(transform.forward, speed);
            // Shoot SFX
            AudioManager.PlayAudio(_shotAudio, gObj.transform.position);

            _nextSpawnTime = Time.timeSinceLevelLoad + 1 / _spawnRate;
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = new Color(1f, 0.5f, 0.5f, 0.5f);
        if (_mesh != null) Gizmos.DrawMesh(_mesh, transform.position, transform.rotation, transform.lossyScale);
    }
}
