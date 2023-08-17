using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "roomSS", menuName = "room_saveState")]
public class RoomSaveState : ScriptableObject  //This object should be "reseted" in each room or floor
{
    public POI[] pois;                    //Cant use scripts, those are references to the objects (their components) in the scene, and the references to those don't transfer between scenes
    public Vector3Int PlayerPos;
    public int roomNo;                    //The room number in case we need it to calculate difficulty or something like that
    public bool[] doorKeys;               //Wether player has key to each door in room aka door opened/closed
    //public bool[] poiAttempted;           //Wether player has already engaged battle against pois[i]                       //since we use Scriptable objects, we can just use the "done" field in those
    //public bool[] poiDefeated;            //Wether player has defeated pois[i]
    //public Vector3[] chestPos;           //Info on whether the POI is a chest or not can also be stored in the scriptableObject
    //public bool[] openedChests;                                                                                                       //same ^
    public int currentFoe;                //States which POI index the current enemy is

    public bool firstTime;         //Checks out after POIs are generated so map is saved
}
