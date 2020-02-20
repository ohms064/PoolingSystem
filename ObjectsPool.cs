
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
namespace OhmsLibraries.Pooling {
    public class ObjectsPool<T> : Pool<T> where T : PoolMonoBehaviour {

        protected T PoolMonoBehaviour {
            get {
                return PoolMonoBehaviours[Random.Range( 0, PoolMonoBehaviours.Length )];
            }
        }

        [Tooltip( "Crea la misma cantidad de objetos por tipo." )]
        public bool evenlyCreate;

        protected virtual void Awake () {
            InstantiateObjects();
        }

        private void OnDestroy () {
            for ( int i = 0; i < pool.Count; i++ ) {
                if ( pool[i] == null )
                    continue;
                Destroy( pool[i].gameObject );
            }
        }

        /// <summary>
        /// Creates all the objects to be used in the pool.
        /// </summary>
        protected override void InstantiateObjects () {
            poolQueue = new Queue<T>();
            if ( evenlyCreate ) {
                pool = new List<T>( poolSize * PoolMonoBehaviours.Length );
                for ( int i = 0; i < PoolMonoBehaviours.Length; i++ ) {
                    for ( int j = 0; j < poolSize; j++ ) {
                        var obj = InstantiatePoolObject( PoolMonoBehaviours[i] );
                        obj.Available = true;
                        obj.OnPoolReturnRequest = ReturnToQueue;
                        poolQueue.Enqueue( obj );
                        pool.Add( obj );
                    }
                }
            }
            else {
                pool = new List<T>( poolSize );
                for ( int i = 0; i < poolSize; i++ ) {
                    var obj = InstantiatePoolObject( PoolMonoBehaviour );
                    obj.Available = true;
                    obj.OnPoolReturnRequest = ReturnToQueue;
                    pool.Add( obj );
                    poolQueue.Enqueue( pool[i] );
                }
            }
        }
    }


}