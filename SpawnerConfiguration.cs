using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
namespace OhmsLibraries.Pooling {
    public delegate IEnumerable<SpawnerConfigurationData> SpawnerGenerator ();

    [CreateAssetMenu( menuName = "Spawner/Configuration" )]
    public class SpawnerConfiguration : SerializedScriptableObject {

        public bool variableSpeeds;
        [ShowIf( "variableSpeeds" ), MinMaxSlider( 0.1f, 2f, ShowFields = true )]
        public Vector2 speedVariation = new Vector2( 1f, 1f );

        public float SpeedMultiplier {
            get {
                return Random.Range( speedVariation.x, speedVariation.y );
            }
        }

        [ValidateInput( "MatrixValidation" ), OnValueChanged( "OnMatrixChanged" )]
        [InfoBox( "Matriz que indica que NPC está en cierta posición." )]
        public int[,] configurationMatrix = new int[3, 5];

#if UNITY_EDITOR
        [DisableInPlayMode, DisableInEditorMode]
        public int size;
        protected bool MatrixValidation ( int[,] matrix ) {
            return matrix.GetLength( 0 ) == 3;
        }

        [Button]
        private void SetDefaults () {
            for ( int i = 0; i < configurationMatrix.GetLength( 0 ); i++ ) {
                for ( int j = 0; j < configurationMatrix.GetLength( 1 ); j++ ) {
                    configurationMatrix[i, j] = -1;
                }
            }
            size = SpawnGroup.Count( configurationMatrix );
        }

        private void OnMatrixChanged ( int[,] ma ) {
            size = SpawnGroup.Count( ma );
        }
#endif

        /// <summary>
        /// Lista que contiene los datos cuya condición fue exitosa de acuerdo a la probabilidad.
        /// TODO: Se puede mejorar, se pueden obtener los datos mayores a cero en el editor y aquí obtener los resultados de la probabilidad.
        /// </summary>
        public virtual List<SpawnerConfigurationData> Objects2Spawn {
            get {
                var list = new List<SpawnerConfigurationData>();
                for ( int i = 0; i < configurationMatrix.GetLength( 0 ); i++ ) {
                    for ( int j = 0; j < configurationMatrix.GetLength( 1 ); j++ ) {
                        var data = configurationMatrix[i, j];
                        list.Add( new SpawnerConfigurationData { row = j, column = i, type = data } );
                    }
                }
                return list;
            }
        }

        /// <summary>
        /// Generador que retorna un SpawnerConfigurationData con su posición en la matriz dependiendo de la probabilidad de esa celda.
        /// </summary>
        /// <returns>Elemento que pasó la probabilidad de crearse.</returns>
        public virtual IEnumerable<SpawnerConfigurationData> GenerateSpawnObjectsMirrored () {
            for ( int i = configurationMatrix.GetLength( 1 ) - 1; i >= 0; i-- ) {
                for ( int j = 0; j < configurationMatrix.GetLength( 0 ); j++ ) {
                    var data = configurationMatrix[j, i];
                    if ( data == -1 ) {
                        continue;
                    }
                    yield return new SpawnerConfigurationData { row = i, column = j, type = data };
                }
            }
        }

        public virtual IEnumerable<SpawnerConfigurationData> GenerateSpawnObjects () {
            for ( int i = configurationMatrix.GetLength( 1 ) - 1; i >= 0; i-- ) {
                for ( int j = 0; j < configurationMatrix.GetLength( 0 ); j++ ) {
                    var data = configurationMatrix[j, i];
                    if ( data == -1 ) {
                        continue;
                    }
                    yield return new SpawnerConfigurationData { row = i, column = j, type = data };
                }
            }
        }
    }

    /// <summary>
    /// Contiene información sobre el objeto a crear.
    /// </summary>
    [System.Serializable]
    public class SpawnerConfigurationData {
        public int row, column;
        public int type;

        public Vector2Int Matrix {
            get {
                return new Vector2Int( row, column );
            }
        }
    }
}