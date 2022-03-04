using MyBox;
using SoraCore.Extension;
using SoraCore.Manager;
using UnityEngine;

public class Bullet : MonoBehaviour {
    [SerializeField, ReadOnly] private Rigidbody _rigidbody;
    [SerializeField, ReadOnly] private float _despawnTime;

    private void Awake() {
        _rigidbody = transform.GetComponentNullCheck<Rigidbody>();
    }

    private void OnEnable() {
        _despawnTime = Time.realtimeSinceStartup + 3f;
    }

    private void Update() {
        if (Time.realtimeSinceStartup > _despawnTime) {
            GameObjectManager.Destroy(gameObject);
            _despawnTime = float.MaxValue;
        }
    }

    public void Shoot(Vector3 dir, float speed) {
        transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        _rigidbody.AddForce(transform.forward * speed, ForceMode.VelocityChange);
    }
}
