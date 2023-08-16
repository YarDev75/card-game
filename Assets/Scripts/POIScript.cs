using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POIScript : MonoBehaviour
{
    [SerializeField] private EnemyPerson person;
    public POI contents;
    public Transform startTransform;
    

    private void Start()
    {
        contents = ScriptableObject.CreateInstance<POI>();
        //contents.LeadingDots = new Vector3Int[] {new Vector3Int(-5,-4), new Vector3Int(-5, -3), new Vector3Int(-5, -2), new Vector3Int(-4, -2), new Vector3Int(-3, -2), new Vector3Int(-2, -2), new Vector3Int(-1, -2), };
        contents.Encounter = person;
    }
}
