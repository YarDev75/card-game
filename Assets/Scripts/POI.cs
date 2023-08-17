using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "POI slot", menuName = "POI slot")]
public class POI : ScriptableObject                //made it a Scriptable object, so it can be saved when unloading the scene, if you have a better solution, I'm all ears
{
    public Vector3Int[] LeadingDots;
    public EnemyPerson Encounter;
    public bool Done;
    //public bool IsUsed;
}
