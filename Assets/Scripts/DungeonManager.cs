using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviour
{
    public static bool FirstLoad;                  //for later, if we'll add random generation
    public static POI[] POIs;
    public static Vector3 SavedPlayerPos;

    [SerializeField] private Tilemap dots;
    [SerializeField] private Tile SmallDotNew;
    [SerializeField] private Tile SmallDotDone;
    [SerializeField] private Tile CombatDotNew;
    [SerializeField] private Tile CombatDotDone;
    [SerializeField] private Transform Player;
    [SerializeField] private float PlayerSpeed;
    [SerializeField] private GameObject[] Hints;  //must be assigned as follows: 0 - up; 1 - right; 2 - down; 3 - left;
    [SerializeField] private POIScript FirstPOI;
    private POIScript[] AvailablePOIs;
    [SerializeField] private Vector3Int PlayerGridPos;
    bool Moving;
    POIScript Target;
    int ind;                         //index of the target dot;
    [SerializeField] RoomSaveState dataSave;

    public enum Directions
    {
        up,
        right,
        down,
        left
    }

    private void Start()
    {
        dataSave.roomNo = 0; //We should include a PlaySaveState so we can carry some info across the rooms, including roomNo which should increment
        dataSave.currentFoe = -1;                                                                                   // before advancing to next room
        if (dataSave.firstTime)
        {
            AvailablePOIs = new POIScript[] { FirstPOI };
            dataSave.pois = AvailablePOIs;
        }dataSave.firstTime = false;
        if(POIs != null) print(POIs[0]);
        EnableHints();
    }

    void Load()
    {

    }

    private void Update()
    {
        if (Moving)
        {
            var dot = dots.CellToWorld(Target.contents.LeadingDots[ind]);
            dot = new Vector3(dot.x + 0.5f, dot.y + 0.5f, dot.z);
            var dir = dot - Player.position;
            Player.position += dir * Time.deltaTime * PlayerSpeed;
            if(Vector2.Distance(dot,Player.position) < 0.1f)
            {
                dots.SetTile(Target.contents.LeadingDots[ind], (ind == Target.contents.LeadingDots.Length-1)? CombatDotDone : SmallDotDone);
                ind++;
                if(ind >= Target.contents.LeadingDots.Length)
                {
                    StartEncounter();
                }
            }
        }
    }

    void EnableHints()
    {
        for (int i = 0; i < AvailablePOIs.Length; i++)
        {
            var dir = AvailablePOIs[i].contents.LeadingDots[0] - PlayerGridPos;
            if (dir.y == 1) Hints[0].SetActive(true);
            else if (dir.y == -1) Hints[2].SetActive(true);
            else if (dir.x == 1) Hints[1].SetActive(true);
            else if (dir.x == -1) Hints[3].SetActive(true);
        }
    }

    void StartEncounter()
    {
        POIs = new POI[] { Target.contents };
        EnenemyAI.person = Target.contents.Encounter;
        dataSave.currentFoe = 0; //We'll need to identify POI index
        dataSave.poiAttempted[dataSave.currentFoe] = true;
        SceneManager.LoadScene(1);
    }

    void BeginMovement(POIScript target)
    {
        Moving = true;
        ind = 0;
        Target = target;
        for (int i = 0; i < Hints.Length; i++) Hints[i].SetActive(false);
    }

    public void ChooseDirection(int direction)
    {
        for (int i = 0; i < AvailablePOIs.Length; i++)
        {
            var dir = AvailablePOIs[i].contents.LeadingDots[0] - PlayerGridPos;
            switch (direction)
            {
                case (int)Directions.up:
                    if (dir.y == 1)
                    {
                        BeginMovement(AvailablePOIs[i]);
                        return;
                    }
                    break;
                case (int)Directions.right:
                    if (dir.x == 1)
                    {
                        BeginMovement(AvailablePOIs[i]);
                        return;
                    }
                    break;
                case (int)Directions.down:
                    if (dir.y == -1)
                    {
                        BeginMovement(AvailablePOIs[i]);
                        return;
                    }
                    break;
                case (int)Directions.left:
                    if (dir.x == -1)
                    {
                        BeginMovement(AvailablePOIs[i]);
                        return;
                    }
                    break;
            }
        }
    }
}
