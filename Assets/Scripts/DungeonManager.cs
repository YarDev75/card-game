using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviour
{
    [SerializeField] RoomSaveState dataSave;
    [SerializeField] private POI[] POISlots;
    [SerializeField] private Tilemap dots;
    [SerializeField] private Tile SmallDotNew;
    [SerializeField] private Tile SmallDotDone;
    [SerializeField] private Tile CombatDotNew;
    [SerializeField] private Tile CombatDotDone;
    [SerializeField] private Transform Player;
    [SerializeField] private float PlayerSpeed;
    [SerializeField] private GameObject[] Hints;  //must be assigned as follows: 0 - up; 1 - right; 2 - down; 3 - left;
    [SerializeField] private int POIAmount;
    [SerializeField] private Vector3Int PlayerGridPos;
    [SerializeField] private GameObject POIObjectPrefab;
    [SerializeField] private EnemyPerson[] AvailablePersons;
    private POIScript[] AllPOIs;
    bool Moving;
    bool BackTracking;
    POIScript Target;
    int ind;                         //index of the target dot;

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
        // dataSave.currentFoe = -1;                                                                                   // before advancing to next room
        if (dataSave.firstTime)
        {
            Generate(POIAmount);
            Save();
            dataSave.firstTime = false;
        }
        else Load();
        EnableHints();
    }  
    
    private void Update()
    {
        if (Moving)
        {
            Vector3 dot;
            dot = dots.CellToWorld(Target.contents.LeadingDots[ind]);
            dot = new Vector3(dot.x + 0.5f, dot.y + 0.5f, dot.z);
            var dir = dot - Player.position;
            Player.position += dir * Time.deltaTime * PlayerSpeed;

            if(Vector2.Distance(dot,Player.position) < 0.1f)
            {
                PlayerGridPos = Target.contents.LeadingDots[ind];
                if (!BackTracking) dots.SetTile(Target.contents.LeadingDots[ind], (ind == Target.contents.LeadingDots.Length - 1) ? CombatDotDone : SmallDotDone);
                ind += BackTracking ? -1 : 1;
                if(!BackTracking && ind >= Target.contents.LeadingDots.Length)
                {
                    if (!Target.contents.Done) StartEncounter();
                    else
                    {
                        Moving = false;
                        EnableHints();
                    }
                    
                }
                if(ind < 0)
                {
                    for (int i = 0; i < AllPOIs.Length; i++)
                    {
                        var pos = AllPOIs[i].contents.LeadingDots[AllPOIs[i].contents.LeadingDots.Length - 1];
                        dir = pos - PlayerGridPos;
                        if(Mathf.Abs(dir.x) < 2 && Mathf.Abs(dir.y) < 2)
                        {
                            Target = AllPOIs[i];
                            ind = AllPOIs[i].contents.LeadingDots.Length - 1;
                            BackTracking = false;
                        }
                    }
                }
            }
        }
    }

    void Load()   //load the data from the saveFile
    {
        PlayerGridPos = dataSave.PlayerPos;
        var DotsPos = dots.CellToWorld(PlayerGridPos);
        Player.position = new Vector3(DotsPos.x + 0.5f, DotsPos.y + 0.5f, 0);
        AllPOIs = new POIScript[dataSave.pois.Length];
        for (int i = 0; i < dataSave.pois.Length; i++)
        {
            for (int j = 0; j < dataSave.pois[i].LeadingDots.Length; j++)
            {
                dots.SetTile(dataSave.pois[i].LeadingDots[j], dataSave.pois[i].Done ? SmallDotDone : SmallDotNew);
            }
            dots.SetTile(dataSave.pois[i].LeadingDots[dataSave.pois[i].LeadingDots.Length - 1], dataSave.pois[i].Done ? CombatDotDone : CombatDotNew);
            var poi = Instantiate(POIObjectPrefab).GetComponent<POIScript>();
            poi.contents = dataSave.pois[i];
            AllPOIs[i] = poi;
        }
        Target = AllPOIs[dataSave.currentFoe];
    }

    void Save()
    {
        dataSave.PlayerPos = PlayerGridPos;
        dataSave.pois = new POI[AllPOIs.Length];
        for (int i = 0; i < AllPOIs.Length; i++)
        {
            dataSave.pois[i] = AllPOIs[i].contents;
            if (AllPOIs[i] == Target) dataSave.currentFoe = i;
        }
    }

    void Generate(int POIsAmount)
    {
        var generatedPOIs = new List<POIScript>();
        var nextPositions = new List<Vector3Int>();
        nextPositions.Add(PlayerGridPos);
        int PosInd = 0;
        int Hold = 0;
        for (int i = 0; i < POIsAmount; i++)
        {
            var poi = GeneratePOI(nextPositions[PosInd], i);
            if (poi != null)
            {
                nextPositions.Add(poi.contents.LeadingDots[poi.contents.LeadingDots.Length - 1]);
                generatedPOIs.Add(poi);
            }
            if (Random.Range(0, 2) == 0 && Hold < 3 && i > 0) Hold++;
            else
            {
                PosInd++;
                Hold = 0;
            }
            if (PosInd > nextPositions.Count) break;
        }
        AllPOIs = new POIScript[generatedPOIs.Count];
        for (int i = 0; i < generatedPOIs.Count; i++)
        {
            AllPOIs[i] = generatedPOIs[i];
        }
    }

    POIScript GeneratePOI(Vector3Int StartPos, int SlotInd)
    {
        int Dir = Random.Range(0, 4);       //0 - up, then clockwise
        int Dots = Random.Range(3, 6);     //how long the path will be (in dots)
        for (int i = 0; i < 4; i++)         // 4 directions total, if none work - exits loop, returns false
        {
            //checks if there are dots in the way in the current direction
            bool Fine = false;
            for (int j = 1; j < Dots+1; j++)
            {
                //checks each dot, exits the loop therefore changing direction if a position is occupied
                bool Occupied = false;
                switch (Dir)
                {
                    case 0:
                        if (dots.GetTile(new Vector3Int(StartPos.x, StartPos.y + j)) != null) Occupied = true;
                        else if (j == Dots) Fine = true;
                        break;
                    case 1:
                        if (dots.GetTile(new Vector3Int(StartPos.x + j, StartPos.y)) != null) Occupied = true;
                        else if (j == Dots) Fine = true;
                        break;
                    case 2:
                        if (dots.GetTile(new Vector3Int(StartPos.x, StartPos.y - j)) != null) Occupied = true;
                        else if (j == Dots) Fine = true;
                        break;
                    case 3:
                        if (dots.GetTile(new Vector3Int(StartPos.x - j, StartPos.y)) != null) Occupied = true;
                        else if (j == Dots) Fine = true;
                        break;
                }
                if (Occupied) break;
            }
            if (Fine)
            {
                //generating a poi and drawing the path to it
                var poi = Instantiate(POIObjectPrefab).GetComponent<POIScript>();
                poi.contents = POISlots[SlotInd];
                poi.contents.Encounter = AvailablePersons[Random.Range(0, AvailablePersons.Length)];
                poi.contents.Done = false;
                poi.contents.LeadingDots = new Vector3Int[Dots];
                for (int j = 1; j < Dots+1; j++)
                {
                    switch (Dir)
                    {
                        case 0:
                            poi.contents.LeadingDots[j-1] = new Vector3Int(StartPos.x, StartPos.y + j, 0);
                            break;
                        case 1:
                            poi.contents.LeadingDots[j-1] = new Vector3Int(StartPos.x + j, StartPos.y, 0);
                            break;
                        case 2:
                            poi.contents.LeadingDots[j-1] = new Vector3Int(StartPos.x, StartPos.y - j, 0);
                            break;
                        case 3:
                            poi.contents.LeadingDots[j-1] = new Vector3Int(StartPos.x - j, StartPos.y , 0);
                            break;
                    }
                    dots.SetTile(poi.contents.LeadingDots[j-1], j == Dots ? CombatDotNew : SmallDotNew);
                }
                return poi;
            }
            Dir = (Dir + 1) % 4;
        }
        return null;
    }

    void StartEncounter()
    {
        Target.contents.Done = true;
        EnenemyAI.person = Target.contents.Encounter;
        Save();
        //dataSave.currentFoe = 0; //We'll need to identify POI index (no, it's not used anywhere anymore)
        SceneManager.LoadScene(1);
    }

    void EnableHints()
    {
        for (int i = 0; i < AllPOIs.Length; i++)
        {
            Vector3Int dir;
            if (AllPOIs[i] == Target && i > 0) dir = AllPOIs[i].contents.LeadingDots[AllPOIs[i].contents.LeadingDots.Length-2] - PlayerGridPos;
            else dir = AllPOIs[i].contents.LeadingDots[0] - PlayerGridPos;
            if (dir.y == 1 && Mathf.Abs(dir.x) < 2) Hints[0].SetActive(true);
            else if (dir.y == -1 && Mathf.Abs(dir.x) < 2) Hints[2].SetActive(true);
            else if (dir.x == 1 && Mathf.Abs(dir.y) < 2) Hints[1].SetActive(true);
            else if (dir.x == -1 && Mathf.Abs(dir.y) < 2) Hints[3].SetActive(true);
        }
    }

    public void ChooseDirection(int direction)
    {
        for (int i = 0; i < AllPOIs.Length; i++)
        {
            bool backtracking = AllPOIs[i] == Target && i > 0;
            var dir = (!backtracking? AllPOIs[i].contents.LeadingDots[0] : AllPOIs[i].contents.LeadingDots[AllPOIs[i].contents.LeadingDots.Length - 2]) - PlayerGridPos;
            bool matches = false;
            switch (direction)
            {
                case (int)Directions.up:
                    if (dir.y == 1) matches = true;
                    break;
                case (int)Directions.right:
                    if (dir.x == 1) matches = true;
                    break;
                case (int)Directions.down:
                    if (dir.y == -1) matches = true;
                    break;
                case (int)Directions.left:
                    if (dir.x == -1) matches = true;
                    break;
            }
            if (matches)
            {
                BeginMovement(AllPOIs[i], backtracking);
                return;
            }
        }
    }

    void BeginMovement(POIScript target, bool returning)
    {
        Moving = true;
        BackTracking = returning;
        Target = target;
        ind = returning ? (Target.contents.LeadingDots.Length-2) : 0;
        for (int i = 0; i < Hints.Length; i++) Hints[i].SetActive(false);
    }
}
