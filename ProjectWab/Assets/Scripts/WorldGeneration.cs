using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    // Map Generation Variables
    public int rows = 16;
    public int columns = 16;
    static public int tileSize = 16;
    public int WorldGenIterations = 1;
    public int RiverScale = 16;

    // Tile Prefabs
    public GameObject GrassTilePrefab;
    public GameObject WaterTilePrefab;


    public GameObject Camera;
    public GameObject[] Tiles;

    // Start is called before the first frame update
    void Start()
    {
        // Generating a basic map of just grass tiles
        Tiles = new GameObject[rows * columns];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                int id = j + i * columns;
                Tiles[id] = Instantiate(GrassTilePrefab, new Vector2(j, -i), Quaternion.identity, GameObject.Find("Tiles").transform);

                Tiles[id].name = "Tile "  + (id).ToString();
                if (Tiles[id].GetComponentInChildren<UnityEngine.UI.Text>())
                {
                    Tiles[id].GetComponentInChildren<UnityEngine.UI.Text>().text = id.ToString();
                }
                
            }
        }

        // I'm generating landscape stuff here, after a field has already been made
        // dont know if thats the best way to do it, but it seemed easy enough so i did it
        GenerateRiver();

        Camera.transform.position = new Vector3(columns / 2, -rows / 2, -10);
        Camera.GetComponent<Camera>().orthographicSize = 17;

        for (int i = 0; i < WorldGenIterations; i++)
        {
            //GenerateRiver();
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ConvertTileTo(int id, GameObject Prefab)
    {
        GameObject NewTile = Instantiate(Prefab, Tiles[id].transform.position, Quaternion.identity, GameObject.Find("Tiles").transform);
        string NewTileName = Tiles[id].name;
        if (NewTile.GetComponentInChildren<UnityEngine.UI.Text>())
        {
            NewTile.GetComponentInChildren<UnityEngine.UI.Text>().text = id.ToString();
        }

        Destroy(Tiles[id]);
        NewTile.name = NewTileName;
        Tiles[id] = NewTile;
    }

    void GenerateRiver()
    {
        // this is a test function for generating meanders in the river by encouraging the river to divert from the overall direction
        // Currently only functions along the x-axis, needs to function along y as well and then needs funtionality for changing directions

        // Starting location of the river,, currently only on the x-axis
        int xValue = 64;
        int currentTile = xValue;
        bool MeanderLeft;


        // highly complex left or right generator :)
        switch (Random.Range(0, 2))
        {
            case 0:
                MeanderLeft = true;
                break;
            case 1:
                MeanderLeft = false;
                break;
            default:
                MeanderLeft = true;
                break;
        }

        // River Generation Loop
        while (currentTile + columns < Tiles.Length)
        {
            // Calculate the current tiles distance from the central axis of the river
            int DistanceFromCentre = (currentTile % columns) - xValue;

            if (MeanderLeft)
            {
                if (Mathf.Abs(DistanceFromCentre) > RiverScale / 2)
                {
                    if (Random.Range(0, 10) < 4)
                    {
                        currentTile--;
                        ConvertTileTo(currentTile, WaterTilePrefab);
                    }
                }
                else
                {
                    currentTile--;
                    ConvertTileTo(currentTile, WaterTilePrefab);
                }
                if (Mathf.Abs(DistanceFromCentre) < RiverScale / 3)
                {
                    currentTile--;
                    ConvertTileTo(currentTile, WaterTilePrefab);
                }
                if (Mathf.Abs(DistanceFromCentre) < RiverScale / 8)
                {
                    currentTile--;
                    ConvertTileTo(currentTile, WaterTilePrefab);
                }
                currentTile = currentTile + columns;
                ConvertTileTo(currentTile, WaterTilePrefab);
            }
            else
            {
                if (Mathf.Abs(DistanceFromCentre) > RiverScale / 2)
                {
                    if (Random.Range(0, 10) < 4)
                    {
                        currentTile++;
                        ConvertTileTo(currentTile, WaterTilePrefab);
                    }
                }
                else
                {
                    currentTile++;
                    ConvertTileTo(currentTile, WaterTilePrefab);
                }
                ConvertTileTo(currentTile, WaterTilePrefab);
                if (Mathf.Abs(DistanceFromCentre) < RiverScale / 3)
                {
                    currentTile++;
                    ConvertTileTo(currentTile, WaterTilePrefab);
                }
                if (Mathf.Abs(DistanceFromCentre) < RiverScale / 8)
                {
                    currentTile++;
                    ConvertTileTo(currentTile, WaterTilePrefab);
                }
                currentTile = currentTile + columns;
                ConvertTileTo(currentTile, WaterTilePrefab);
            }

            // Update distance from centre
            //DistanceFromCentre = (currentTile % columns) - xValue;

            // Determine when the river should meander in the other direction
            if (Random.Range(0, RiverScale * 10) < RiverScale)
            {
                if (MeanderLeft && DistanceFromCentre < -2 * RiverScale / 3)
                {
                    MeanderLeft = false;
                }
                else if (!MeanderLeft && DistanceFromCentre > 2 * RiverScale / 3)
                {
                    MeanderLeft = true;
                }
            }
        }
    }
}
