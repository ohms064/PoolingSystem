using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace OhmsLibraries.Pooling {
    public class PoolMonobehaviours_UnityEvents : MonoBehaviour {
        public UnitySpawnEvent OnSpawn, OnDespawn;
        protected PoolMonoBehaviour poolMono;

        private void Awake () {
            poolMono = GetComponent<PoolMonoBehaviour>();
        }

        protected virtual void OnEnable () {
            poolMono.OnSpawn += OnSpawn.Invoke;
            poolMono.OnDespawn += OnDespawn.Invoke;
        }

        protected virtual void OnDisable () {
            poolMono.OnSpawn -= OnSpawn.Invoke;
            poolMono.OnDespawn -= OnDespawn.Invoke;
        }
    }

    [System.Serializable]
    public class UnitySpawnEvent : UnityEvent<PoolMonoBehaviour> { }
}