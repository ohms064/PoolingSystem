using UnityEngine;
using System.Collections;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
namespace OhmsLibraries.Pooling {
    public abstract class Spawner<T, W> : MonoBehaviour where T : Pool<W> where W : PoolMonoBehaviour {
        protected const string SPAWNER_LABEL = "Spawner Configurations";
#if ODIN_INSPECTOR
    public abstract T Pool { get; set; }
    public SpawnPoint[] spawnPositions;
    [MinMaxSlider( 1f, 100f, true ), BoxGroup( SPAWNER_LABEL )]
    public Vector2 randomSpawnTime;
    [SerializeField, BoxGroup( SPAWNER_LABEL )]
    private bool _startSpawnCoroutineOnStart;
    [BoxGroup( SPAWNER_LABEL )]
    public bool spawnAllOnStart;
#else
        public T pool;
        public SpawnPoint[] spawnPositions;
        public Vector2 randomSpawnTime;
        [SerializeField]
        public bool _automaticTimedSpawn;
        public bool spawnAllOnStart;
#endif

        public virtual float SpawnTime {
            get {
                return Random.Range( randomSpawnTime.x, randomSpawnTime.y );
            }
        }
        public SpawnPoint SpawnPosition {
            get {
                return spawnPositions[Random.Range( 0, spawnPositions.Length )];
            }
        }

        private Coroutine spawnCycle;

#if UNITY_EDITOR
        private void Reset () {
            Pool = GetComponent<T>();
            spawnPositions = GetComponentsInChildren<SpawnPoint>();
        }
        private void OnDrawGizmosSelected () {
            Gizmos.color = Color.blue;
            for ( int i = 0; i < spawnPositions.Length; i++ ) {
                if ( spawnPositions[i] == null ) {
                    continue;
                }
                Gizmos.DrawLine( transform.position, spawnPositions[i].transform.position );
                Gizmos.DrawSphere( spawnPositions[i].transform.position, 0.2f );
            }
        }
#endif

        private void Start () {
            if ( spawnAllOnStart ) {
                DebugManager.Log( "Spawn all on start" );
                for ( int i = 0; i < spawnPositions.Length; i++ ) {
                    SpawnObject();
                }
            }

            if ( _startSpawnCoroutineOnStart ) {
                DebugManager.Log( "Automatic Timed Spawn enabled" );
                spawnCycle = StartCoroutine( SpawnCycle() );
            }
        }

        protected virtual IEnumerator SpawnCycle () {
            while ( Application.isPlaying ) {
                yield return new WaitForSeconds( SpawnTime );
                SpawnObject();
            }
        }

        [Button( "Spawn Once" )]
        public virtual void SpawnObject () {
            W PoolMonoBehaviour;
            if ( Pool.RequestPoolMonoBehaviour( out PoolMonoBehaviour ) ) {
                SpawnPosition.Spawn( PoolMonoBehaviour );
            }
        }

        private void OnDisable () {
            StopSpawning();
        }

        public void StopSpawning () {
            if ( spawnCycle != null ) {
                StopCoroutine( spawnCycle );
                spawnCycle = null;
            }
        }

        public void StartSpawning () {
            if ( spawnCycle == null ) {
                spawnCycle = StartCoroutine( SpawnCycle() );
            }
        }
#if ODIN_INSPECTOR
    [HideInEditorMode, Button]
#endif
        public void DispawnAll () {
            foreach ( var obj in Pool.GetUnavailableObjects() ) {
                obj.Despawn();
            }
        }

    }

    public enum SpawnType {
        ONE_BY_ONE, BURST
    }
}