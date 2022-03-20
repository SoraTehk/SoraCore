using MyBox;
using System;
using SoraCore.Extension;
using SoraCore.Manager;
using UnityEngine;

public class Bullet : MonoBehaviour, ISaveable {
    [SerializeField, ReadOnly] private Rigidbody _rigidbody;
    [SerializeField, ReadOnly] private float _despawnTime;

    private void Awake() {
        _rigidbody = transform.GetComponentNullCheck<Rigidbody>();
    }

    private void OnEnable() {
        _despawnTime = Time.time + 3f;
    }

    private void Update() {
        if (Time.time > _despawnTime) {
            GameObjectManager.ReleaseInstance(gameObject);
            _despawnTime = float.MaxValue;
        }
    }

    public void Shoot(Vector3 dir, float speed) {
        transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        _rigidbody.AddForce(transform.forward * speed, ForceMode.VelocityChange);
    }

    #region ISaveable

    public object SaveState() => new SaveData()
    {
        Position = transform.position,
        Rotation = transform.rotation,
        Velocity = _rigidbody.velocity,
        AngularVelocity = _rigidbody.angularVelocity,
    };

    public void LoadState(object state) {
        var saveData = (SaveData)state;

        transform.SetPositionAndRotation(saveData.Position, saveData.Rotation);
        _rigidbody.velocity = saveData.Velocity;
        _rigidbody.angularVelocity = saveData.AngularVelocity;
    }

    [Serializable]
    private struct SaveData {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Velocity;
        public Vector3 AngularVelocity;
    }

    #endregion
}
