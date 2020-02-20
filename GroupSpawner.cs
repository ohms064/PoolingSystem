using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace OhmsLibraries.Pooling {
    public abstract class GroupSpawner<T, W> : Spawner<T, W> where T : Pool<W> where W : PoolMonoBehaviour {
        [MinMaxSlider( 0, 20f, true )]
        public Vector2 separationMinMax;
        public event System.Action<int> OnGroupSpawn;

        private int currentWave = 0;

        protected abstract SpawnerConfiguration Configuration {
            get;
        }

        private float SeparationY {
            get {
                return Random.Range( separationMinMax.x, separationMinMax.y );
            }
        }
        private bool isSpawning = false;

        protected abstract float Speed { get; }

        public override void SpawnObject () {
            StopSpawning();
            StartCoroutine( SpawnGroup( Configuration.GenerateSpawnObjects ) );
        }

        protected virtual IEnumerator SpawnGroup ( SpawnerGenerator generator, bool resume = true ) {
            W obj = null;
            int row = -1, iterator = -1;
            float separation = SeparationY;
            isSpawning = true;
            currentWave++;
            //ConsoleProDebug.Watch( "Wave", currentWave.ToString() );
            OnGroupSpawn?.Invoke( currentWave );
            foreach ( var cell in generator.Invoke() ) {
                if ( row != cell.row ) {
                    if ( row != -1 ) {
                        yield return new WaitForSeconds( separation / Speed );
                    }
                    row = cell.row;
                    separation = SeparationY;

                    iterator++;
                }
                FetchFromPool( cell, ref obj );
            }
            //while( obj != null && obj.transform.position.y > middleScreenY ) {
            //    yield return new WaitForFixedUpdate();
            //}
            if ( resume ) StartSpawning();
        }

        protected virtual void FetchFromPool ( SpawnerConfigurationData cell, ref W obj ) {
            if ( Pool.RequestPoolMonoBehaviour( out obj ) ) {
#if UNITY_EDITOR
                try {
                    spawnPositions[cell.column].Spawn( obj );
                }
                catch {
                    UnityEditor.Selection.activeGameObject = gameObject;
                    UnityEditor.EditorApplication.isPaused = true;
                    Debug.LogErrorFormat( "Más columnas que spawnPoints en {0}", name );

                }
#else
                spawnPositions[cell.column].Spawn( obj );
#endif
            }
            else {
                Debug.LogErrorFormat( "No hubo objecto disponible en el pool {0} {1}", obj.GetType(), Pool.ToString() );
            }
        }
    }
}