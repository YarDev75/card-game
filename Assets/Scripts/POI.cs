using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POI : ScriptableObject                //made it a Scriptable object, so it can be saved when unloading the scene, if you have a better solution, I'm all ears
{
    public Vector3Int[] LeadingDots;
    public POI[] FollowingPOIs;
    public POI PreviousPOI;
    public EnemyPerson Encounter;
    public bool Done;
}
