using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public GameObject GameManager;
    public GameObject CurrentTile;
    public GameObject[] TileMap;
    public float WalkSpeed = 2.0f;
    Vector3 MoveToLocation;

    // Start is called before the first frame update
    void Start()
    {

    }

    void Awake()
    {
        GameManager = GameObject.Find("GameManager");

        TileMap = GameManager.GetComponent<WorldGeneration>().Tiles;

        CurrentTile = TileMap[Random.Range(0, TileMap.Length)];

        MoveToLocation = CurrentTile.transform.position;
    }


    // Update is called once per frame
    void Update()
    {
        switch (DetermineBehaviour())
        {
            case "Wander":
                RandomWander();
                break;

            default:
                break;
        }

    }

    string DetermineBehaviour()
    {
        string[] jobs = new string[] { "Wander" };


        return jobs[Random.Range(0, jobs.Length)];
    }

    void RandomWander()
    {
        if (transform.position == MoveToLocation)
        {
            MoveToLocation = TileMap[Random.Range(0, TileMap.Length)].transform.position;
        }

        transform.position = Vector3.MoveTowards(transform.position, MoveToLocation, WalkSpeed * Time.deltaTime);
    }
}
