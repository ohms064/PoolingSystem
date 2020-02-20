using UnityEngine;
using System.Collections;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
namespace OhmsLibraries.Pooling {
    public class SpawnPoint : MonoBehaviour {
        public SpawnType spawnType;
#if ODIN_INSPECTOR
        [ShowIf( "BurstIntervalCondition" )]
        public float burstIntervalTime;
        [ShowIf( "BurstIntervalCondition" )]
        public int burstCount = 5;

#if UNITY_EDITOR
        public bool BurstIntervalCondition {
            get {
                return spawnType == SpawnType.BURST;
            }
        }
#endif
#else
    public float burstIntervalTime;
    public int burstCount = 5;
#endif

        public Vector3 Position {
            get {
                return transform.position;
            }
        }

        public void Spawn<T> ( T PoolMonoBehaviour ) where T : PoolMonoBehaviour {
            switch ( spawnType ) {
                case SpawnType.ONE_BY_ONE:
                    PoolMonoBehaviour.Spawn( Position );
                    break;
                case SpawnType.BURST:
                    for ( int i = 0; i < burstCount; i++ ) {
                        PoolMonoBehaviour.Spawn( Position );
                    }
                    break;
            }

        }
    }
}