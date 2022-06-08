using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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
    //public GameObject PlayerPrefab;
    public GameObject[,] Tiles;

    public bool WorldGen;


    public Tilemap tilemap;
    public TileBase Ground;
    public TileBase Water;
    public int currentTileX = 0;
    public int currentTileY = 0;


    // Start is called before the first frame update
    void Start()
    {
        // Generating a basic map of just grass tiles
        Tiles = new GameObject[columns, rows];
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                /*
                Tiles[x,y] = Instantiate(GrassTilePrefab, new Vector2(x, -y), Quaternion.identity, GameObject.Find("Tiles").transform);
                Tiles[x,y].name = "Tile "  + x.ToString() + ", " + y.ToString();
                Tiles[x, y].GetComponent<TileManager>().X = x;
                Tiles[x, y].GetComponent<TileManager>().Y = y;
                Tiles[x, y].GetComponent<TileManager>().id = (columns * y) + x;
                if (Tiles[x,y].GetComponentInChildren<UnityEngine.UI.Text>())
                {
                    Tiles[x,y].GetComponentInChildren<UnityEngine.UI.Text>().text = Tiles[x, y].GetComponent<TileManager>().id.ToString();
                }*/


                tilemap.SetTile(new Vector3Int(x, y, 0), Ground);

                
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



        // Spawn Player
        //GameObject Player = Instantiate(PlayerPrefab, new Vector2(0, 0), Quaternion.identity, GameObject.Find("NPCs").transform);
        //Player.name = "Player";
        //Camera.GetComponent<CameraControls>().Player = Player;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ConvertTileTo(int x, int y, TileBase newTileBase)
    {
        tilemap.SetTile(new Vector3Int(x, y, 0), newTileBase);

        currentTileX = x;
        currentTileY = y;
    }

    void GenerateRiver()
    {
        // this is a test function for generating meanders in the river by encouraging the river to divert from the overall direction
        // Currently only functions along the x-axis, needs to function along y as well and then needs funtionality for changing directions

        // Starting location of the river,, currently only on the x-axis
        int xValue = columns / 2;
        currentTileX = columns / 2;
        GameObject currentTile = Tiles[xValue, 0];
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
        while (currentTileX < Tiles.GetLength(0) && currentTileX >= 0 && currentTileY < Tiles.GetLength(1) && currentTileY >= 0)
        {
            // Calculate the current tiles distance from the central axis of the river
            int DistanceFromCentre = currentTileX - xValue;

            if (MeanderLeft)
            {
                if (Mathf.Abs(DistanceFromCentre) > RiverScale / 2)
                {
                    if (Random.Range(0, 10) < 4)
                    {
                        currentTileX--;
                        ConvertTileTo(currentTileX, currentTileY, Water);
                    }
                }
                else
                {
                    currentTileX--;
                    ConvertTileTo(currentTileX, currentTileY, Water);
                }
                if (Mathf.Abs(DistanceFromCentre) < RiverScale / 3)
                {
                    currentTileX--;
                    ConvertTileTo(currentTileX, currentTileY, Water);
                }
                if (Mathf.Abs(DistanceFromCentre) < RiverScale / 8)
                {
                    currentTileX--;
                    ConvertTileTo(currentTileX, currentTileY, Water);
                }
                currentTileY++;
                ConvertTileTo(currentTileX, currentTileY, Water);
            }
            else
            {
                if (Mathf.Abs(DistanceFromCentre) > RiverScale / 2)
                {
                    if (Random.Range(0, 10) < 4)
                    {
                        currentTileX++;
                        ConvertTileTo(currentTileX, currentTileY, Water);
                    }
                }
                else
                {
                    currentTileX++;
                    ConvertTileTo(currentTileX, currentTileY, Water);
                }
                ConvertTileTo(currentTileX, currentTileY, Water);
                if (Mathf.Abs(DistanceFromCentre) < RiverScale / 3)
                {
                    currentTileX++;
                    ConvertTileTo(currentTileX, currentTileY, Water);
                }
                if (Mathf.Abs(DistanceFromCentre) < RiverScale / 8)
                {
                    currentTileX++;
                    ConvertTileTo(currentTileX, currentTileY, Water);
                }
                currentTileY++;
                ConvertTileTo(currentTileX, currentTileY, Water);
            }

            // Update distance from centre
            DistanceFromCentre = currentTileX - xValue;

            // Determine when the river should meander in the other direction
            if (Random.Range(0, RiverScale * RiverScale) - Mathf.Abs(DistanceFromCentre) < RiverScale)
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
