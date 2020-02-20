using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
namespace OhmsLibraries.Pooling {
    [CreateAssetMenu( menuName = "Spawner/Group" )]
    public class SpawnGroup : ScriptableObject {
        [SerializeField, ShowIf( "correct" ), DisableInPlayMode, DisableInEditorMode]
        public int size = 0;

        [ValidateInput( "ValidateConfigurations", "Las configuraciones deben tener la misma cantidad de NPC's" )]
        public SpawnerConfiguration[] configurations;

        public SpawnerConfiguration Configuration {
            get {
                return configurations[Random.Range( 0, configurations.Length )];
            }
        }

#if UNITY_EDITOR
        private bool correct = false;
        private bool ValidateConfigurations ( SpawnerConfiguration[] configs ) {
            size = -1;
            correct = true;
            foreach ( var c in configs ) {
                if ( size == -1 ) {
                    size = Count( c.configurationMatrix );
                }
                else {
                    var aux = Count( c.configurationMatrix );
                    if ( aux != size ) {
                        correct = false;
                    }
                }
            }
            return correct;
        }

        public static int Count ( int[,] matrix ) {
            return ( from int c in matrix where c >= 0 select c ).Count();
        }
#endif
    }
}