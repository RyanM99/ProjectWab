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
        generatenewriver();

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

    void GenerateRiver()
    {
        // Determine start location
        // In future iterations, should prefer higher altitude locations,, then should also not enter both sides of the screen
        int RiverStart = Random.Range(0, Tiles.Length);
        ConvertTileTo(RiverStart, WaterTilePrefab);

        // North-South or East-West
        int direction = Random.Range(0, 2);
        //int width = Random.Range(1, 4);
        int currentTile = RiverStart;

        switch (direction)
        {
            case 0:
                while (currentTile + columns < Tiles.Length)
                {
                    currentTile = currentTile + columns;
                    ConvertTileTo(currentTile, WaterTilePrefab);
                    if (Random.Range(0,100) < 5)
                    {
                        if (currentTile % columns != 0 && currentTile % columns != columns - 1)
                        {
                            currentTile += Random.Range(-1, 2);
                        }
                    }
                }

                currentTile = RiverStart;

                while (currentTile - columns >= 0)
                {
                    currentTile = currentTile - columns;
                    ConvertTileTo(currentTile, WaterTilePrefab);

                }
                break;

            case 1:
                while ((currentTile + 1) % columns != 0 && currentTile + 1 < Tiles.Length)
                {
                    currentTile++;
                    ConvertTileTo(currentTile, WaterTilePrefab);
                }

                currentTile = RiverStart;

                while ((currentTile - 1) % columns != columns - 1 && currentTile - 1 >= 0)
                {
                    currentTile--;
                    ConvertTileTo(currentTile, WaterTilePrefab);
                }
                break;
        }
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

    void generatenewriver()
    {
        // this is a test function for generating meanders in the river by encouraging the river to divert from the overall direction
        int xValue = 64;
        //ConvertTileTo(xValue, WaterTilePrefab);
        int currentTile = xValue;
        bool MeanderLeft;
        // highly complex left or right generator :)
        switch (Random.Range(0,2))
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

        while (currentTile + columns < Tiles.Length)
        {
            int DistanceFromCentre = Mathf.Abs((currentTile % columns) - xValue);

            if (MeanderLeft)
            {
                if (DistanceFromCentre > RiverScale / 2)
                {
                    if (Random.Range(0, 10) < 6)
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
                if (DistanceFromCentre < RiverScale / 3)
                {
                    currentTile--;
                    ConvertTileTo(currentTile, WaterTilePrefab);
                }
                if (DistanceFromCentre < RiverScale / 8)
                {
                    currentTile--;
                    ConvertTileTo(currentTile, WaterTilePrefab);
                }
                currentTile = currentTile + columns;
                ConvertTileTo(currentTile, WaterTilePrefab);
            }
            else
            {
                if (DistanceFromCentre > RiverScale / 2)
                {
                    if (Random.Range(0, 10) < 6)
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
                if (DistanceFromCentre < RiverScale / 3)
                {
                    currentTile++;
                    ConvertTileTo(currentTile, WaterTilePrefab);
                }
                if (DistanceFromCentre < RiverScale / 8)
                {
                    currentTile++;
                    ConvertTileTo(currentTile, WaterTilePrefab);
                }
                currentTile = currentTile + columns;
                ConvertTileTo(currentTile, WaterTilePrefab);
            }
            
            if (Random.Range(0, RiverScale) < DistanceFromCentre)
            {
                if (MeanderLeft && (currentTile % columns) - xValue < -2 * RiverScale / 3)
                {
                    MeanderLeft = false;
                }
                else if (!MeanderLeft && (currentTile % columns) - xValue > 2 * RiverScale / 3)
                {
                    MeanderLeft = true;
                }
            }

        }
    }
}
