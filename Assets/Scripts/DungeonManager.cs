using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviour
{
    [SerializeField] RunSaveState huh;
    [SerializeField] RoomSaveState dataSave;
    [SerializeField] private POI[] POISlots;
    [SerializeField] private Tilemap dots;
    [SerializeField] private Tile SmallDotNew;
    [SerializeField] private Tile SmallDotDone;
    [SerializeField] private Tile CombatDotNew;
    [SerializeField] private Tile CombatDotDone;
    [SerializeField] private Tile BossDot;
    [SerializeField] private Tile[] Walls;
    [SerializeField] private Tile[] Decor;
    [SerializeField] private Tile[] Door;
    [SerializeField] private Transform Player;
    [SerializeField] private float PlayerSpeed;
    [SerializeField] private GameObject[] Hints;  //must be assigned as follows: 0 - up; 1 - right; 2 - down; 3 - left;
    [SerializeField] private int POIAmount;
    [SerializeField] private Vector3Int PlayerGridPos;
    [SerializeField] private GameObject POIObjectPrefab;
    [SerializeField] private EnemyPerson[] AvailablePersons;
    [SerializeField] private EnemyPerson[] AvailableBosses;
    private POIScript[] AllPOIs;
    bool Moving;
    bool BackTracking;
    POIScript Target;
    POIScript BossPOI;
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
            for (int i = 0; i < 5; i++)
            {
                Generate(POIAmount);
                if (BossPOI != null) break;
            }
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
            if (!dataSave.pois[i].IsBoss) dots.SetTile(dataSave.pois[i].LeadingDots[dataSave.pois[i].LeadingDots.Length - 1], dataSave.pois[i].Done ? CombatDotDone : CombatDotNew);
            else dots.SetTile(dataSave.pois[i].LeadingDots[dataSave.pois[i].LeadingDots.Length - 1], BossDot);
            var poi = Instantiate(POIObjectPrefab).GetComponent<POIScript>();
            poi.contents = dataSave.pois[i];
            AllPOIs[i] = poi;
            if (poi.contents.IsBoss) BossPOI = poi;
        }
        Target = AllPOIs[dataSave.currentFoe];
        GenerateWalls();
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
        bool BossAdded = false;
        var generatedPOIs = new List<POIScript>();
        var nextPositions = new List<Vector3Int>();
        nextPositions.Add(PlayerGridPos);
        int PosInd = 0;
        int Hold = 0;
        for (int i = 0; i < POIsAmount; i++)
        {
            var poi = GeneratePOI(nextPositions[PosInd], i, (i >= POIAmount - 3) && !BossAdded);
            if (poi != null)
            {
                if (!poi.contents.IsBoss) nextPositions.Add(poi.contents.LeadingDots[poi.contents.LeadingDots.Length - 1]);
                else BossAdded = true;
                generatedPOIs.Add(poi);
            }
            if (Random.Range(0, 2) == 0 && Hold < 3 && i > 0) Hold++;
            else
            {
                PosInd++;
                Hold = 0;
            }
            if (PosInd >= nextPositions.Count) break;
        }
        AllPOIs = new POIScript[generatedPOIs.Count];
        for (int i = 0; i < generatedPOIs.Count; i++)
        {
            AllPOIs[i] = generatedPOIs[i];
        }
        GenerateWalls();
    }

    POIScript GeneratePOI(Vector3Int StartPos, int SlotInd, bool ForceBoss)
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
                poi.contents.IsBoss = false;
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
                if (BossPOI == null && Dir != 2 && ForceBoss)
                {
                    var Pos = poi.contents.LeadingDots[poi.contents.LeadingDots.Length - 1];
                    bool Doorable = true;
                    for (int y = 1; y < 6; y++)
                    {
                        for (int x = -1; x < 2; x++)
                        {
                            var dot = dots.GetTile(new Vector3Int(Pos.x + x, Pos.y + y, 0));
                            if (dot == SmallDotDone || dot == SmallDotNew || dot == CombatDotDone || dot == CombatDotNew) Doorable = false;
                        }
                    }
                    if (Doorable)
                    {
                        poi.contents.IsBoss = true;
                        BossPOI = poi;
                        dots.SetTile(poi.contents.LeadingDots[poi.contents.LeadingDots.Length - 1], BossDot);
                        poi.contents.Encounter = AvailableBosses[Random.Range(0, AvailableBosses.Length)];
                    }
                }
                return poi;
            }
            Dir = (Dir + 1) % 4;
        }
        return null;
    }

    void GenerateWalls()
    {
        for (int i = 0; i < AllPOIs.Length; i++)
        {
            foreach (var dot in AllPOIs[i].contents.LeadingDots)
            {
                for (int y = -2; y < 3; y++)
                {
                    for (int x = -2; x < 3; x++)
                    {
                        var pos = new Vector3Int(dot.x + x, dot.y + y, 0);
                        var daDot = dots.GetTile(pos);
                        if(daDot != SmallDotDone && daDot != SmallDotNew && daDot != CombatDotDone && daDot != CombatDotNew && daDot != BossDot) SmartWall(pos, (y < 2 && y > -2) ? 3 : 1);
                    }
                    
                }
            }
        }
        for (int i = 0; i < AllPOIs.Length; i++)
        {
            foreach (var dot in AllPOIs[i].contents.LeadingDots)
            {
                for (int y = -1; y < 2; y++)
                {
                    for (int x = -1; x < 2; x++)
                    {
                        var pos = new Vector3Int(dot.x + x, dot.y + y, 0);
                        var daDot = dots.GetTile(pos);
                        if (daDot != SmallDotDone && daDot != SmallDotNew && daDot != CombatDotDone && daDot != CombatDotNew && daDot != BossDot) SmartWall(pos, 8);
                    }

                }
            }
        }

        //piece of code for generating daDoor next to the bossDot
        if (BossPOI != null)
        {
            var Pos = BossPOI.contents.LeadingDots[BossPOI.contents.LeadingDots.Length - 1];
            ind = 0;
            for (int y = 2; y < 4; y++)
            {
                for (int x = -1; x < 2; x++)
                {
                    var Dpos = new Vector3Int(Pos.x + x, Pos.y + y);
                    dots.SetTile(Dpos, Door[ind]);
                    ind++;
                }
            }
        }
    }

    /// <summary>
    /// sets a wall with correct sprite, default orientations: 1 - horizontal, 3 - vertical, 8 - clearTile
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="DefOrient"></param>
    void SmartWall(Vector3Int pos, int DefOrient)
    {
        if (DefOrient < 8)
        {
            var availableWalls = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 };
            int nullCounter = 0;                                                    //if this hits 4 - there are no wall around, use defOrient as guidence
            if (!IsWall(dots.GetTile(new Vector3Int(pos.x, pos.y + 1))))
            {
                availableWalls.Remove(3);
                availableWalls.Remove(5);
                availableWalls.Remove(6);
                availableWalls.Remove(7);
                nullCounter++;
            }

            if (!IsWall(dots.GetTile(new Vector3Int(pos.x + 1, pos.y))))
            {
                availableWalls.Remove(0);
                availableWalls.Remove(1);
                availableWalls.Remove(4);
                availableWalls.Remove(6);
                nullCounter++;
            }
            else
            {
                availableWalls.Remove(2);
                availableWalls.Remove(3);
                availableWalls.Remove(5);
                availableWalls.Remove(7);
            }

            if (!IsWall(dots.GetTile(new Vector3Int(pos.x, pos.y - 1))))
            {
                availableWalls.Remove(0);
                availableWalls.Remove(2);
                availableWalls.Remove(3);
                availableWalls.Remove(4);
                nullCounter++;
            }
            else
            {
                availableWalls.Remove(1);
                availableWalls.Remove(5);
                availableWalls.Remove(6);
                availableWalls.Remove(7);
            }

            if (!IsWall(dots.GetTile(new Vector3Int(pos.x - 1, pos.y))))
            {
                availableWalls.Remove(1);
                availableWalls.Remove(2);
                availableWalls.Remove(4);
                availableWalls.Remove(7);
                nullCounter++;
            }
            else
            {
                availableWalls.Remove(0);
                availableWalls.Remove(3);
                availableWalls.Remove(5);
                availableWalls.Remove(6);
            }

            if (nullCounter == 4 || availableWalls.Count == 0) availableWalls = new List<int>() { DefOrient };
            dots.SetTile(pos, Walls[availableWalls[0]]);
        }
        else
        {
            dots.SetTile(pos, null);
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    var Pos = new Vector3Int(pos.x + x, pos.y + y, 0);
                    var DotToCheck = dots.GetTile(Pos);
                    for (int i = 0; i < Walls.Length; i++)
                    {
                        if (DotToCheck == Walls[i]) SmartWall(Pos, y == 0 ? 3 : 1);
                    }
                }
            }
            dots.SetTile(pos, Decor[Random.Range(0, Decor.Length)]);
        }
    }
    
    bool IsWall(TileBase tile)
    {
        for (int i = 0; i < Walls.Length; i++)
        {
            if (tile == Walls[i]) return true;
        }
        return false;
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
                    if (dir.y == 1 && Mathf.Abs(dir.y) < 2) matches = true;
                    break;
                case (int)Directions.right:
                    if (dir.x == 1 && Mathf.Abs(dir.y) < 2) matches = true;
                    break;
                case (int)Directions.down:
                    if (dir.y == -1 && Mathf.Abs(dir.x) < 2) matches = true;
                    break;
                case (int)Directions.left:
                    if (dir.x == -1 && Mathf.Abs(dir.x) < 2) matches = true;
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

    public void EditDeck()
    {
        SceneManager.LoadScene(2);
    }
}
