using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(fileName = "roomSS", menuName = "room_saveState")]
public class RoomSaveState : ScriptableObject  //This object should be "reseted" in each room or floor
{
    public POIScript[] pois;
    public int roomNo; //The room number in case we need it to calculate difficulty or something like that
    public bool[] doorKeys; //Wether player has key to each door in room aka door opened/closed
    public bool[] poiAttempted; //Wether player has already engaged battle against pois[i]
    public bool[] poiDefeated; //Wether player has defeated pois[i]
    public Vector3[] chestPos;
    public bool[] openedChests;
    public int currentFoe; //States which POI index the current enemy is

    public bool firstTime = true; //Checks out after POIs are generated so map is saved
}
