using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEditor.Build.Reporting;
using UnityEngine;
//using UnityEngine.Assertions;
using UnityEngine.Tilemaps;

public class POI_2 : ScriptableObject                //made it a Scriptable object, so it can be saved when unloading the scene, if you have a better solution, I'm all ears
{                                                   //If we can make this work we just need to save it in between scenes and dungeonmanager.cs will do the rest
    public Vector3Int[] LeadingDots;
    public POI[] FollowingPOIs;
    public POI PreviousPOI;
    public EnemyPerson Encounter;
    public bool Done;
    Vector3 previousPOI;

    private void OnEnable()
    {
        previousPOI = GameObject.FindWithTag("Origin").GetComponent<Transform>().position;
    }
    public void Start()
    {
        Tilemap floorTilemap = GameObject.FindGameObjectWithTag("Floor").GetComponent<Tilemap>();
        var iterator = floorTilemap.cellBounds.allPositionsWithin.GetEnumerator();
        Vector3 POIposition = new Vector3(float.NaN, float.NaN, float.NaN);
        while(POIposition == new Vector3(float.NaN, float.NaN, float.NaN))
        {
            if (1 == Random.Range(0, 50))
            {
                POIposition = iterator.Current;  //POI position gets selected randomly with 2% possibility each (cannot get the iterator size)
                break;
            }
            if(!iterator.MoveNext()) iterator.Reset();
        }
        LeadingDots = generatePath(previousPOI, POIposition);
    }


    private Vector3Int[] generatePath(Vector3 previous, Vector3 thisPOI) //This is just cartesian difference
    {
        int deltaX = (int)(thisPOI.x - previous.x), deltaY = (int)(thisPOI.y - previous.y);
        Vector3Int[] points = new Vector3Int[deltaX+deltaY];
        int step = deltaY >= 0 ? 1 : -1;
        for(int i = 1; i < deltaY+1; i = i+1)
        {
            points[i-1] = new Vector3Int((int)previous.x, (int)previous.y + i * step);
        }
        step = deltaX >= 0 ? 1 : -1;
        for(int i = 1; i < deltaY+1; i = i+1)
        {
            points[i+deltaX-1] = new Vector3Int((int)previous.x, (int)previous.y + i * step);
        }
        return points;  //We will also need to use the wall layer of the tile palette to check wether a leading point is going through a wall
        
        //Tilemap walltilemap = GameObject.FindGameObjectWithTag("Wall").GetComponent<Tilemap>();  //Something like this
        //foreach(Vector3Int point in points)
        //{
        //    if(walltilemap.GetTile(point) != null) changePath()
        //}
    }
}
