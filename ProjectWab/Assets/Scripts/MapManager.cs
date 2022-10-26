using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;




public class MapManager : MonoBehaviour
{
    // Tile Maps
    [SerializeField]
    private Tilemap map;
    [SerializeField]
    private Tilemap collisions;
    [SerializeField]
    private Tilemap Details;

    // Tile Behaviour (For all tiles of a type)
    [SerializeField]
    private List<TileBehaviour> tileBehaviours;
    private Dictionary<TileBase, TileBehaviour> behaviourFromTiles;


    // Tile Data (Individual Tiles)


    // Crops
    public TileBase Seeded;
    public CropManager CropPrefab;
    public float GrowthInterval = 1f;
    private Dictionary<Vector3Int, CropManager> Crops = new Dictionary<Vector3Int, CropManager>();


    // Day Night Cycle
    public SpriteRenderer DayNightOverlay;
    public float DayNightInterval = 10f;
    public float DayNightAlphaMin = 5f;
    public float DayNightAlphaMax = 150f;
    public float DayNightAlphaGradient = 1f;


    private void Awake()
    {
        behaviourFromTiles = new Dictionary<TileBase, TileBehaviour>();

        foreach (var tileBehaviour in tileBehaviours)
        {
            foreach (var tile in tileBehaviour.tiles)
            {
                behaviourFromTiles.Add(tile, tileBehaviour);
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GrowCrops());
        StartCoroutine(DayNightCycle());
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPosition = map.WorldToCell(mousePosition);

            TileBase clickedTile = map.GetTile(gridPosition);

            float walkingSpeed = behaviourFromTiles[clickedTile].walkingSpeed;
        }
    }

    public float GetTileWalkingSpeed(Vector2 worldPosition)
    {
        Vector3Int gridPosition = map.WorldToCell(worldPosition);

        TileBase tile = map.GetTile(gridPosition);

        if (tile == null)
            return 1f;

        float walkingSpeed = behaviourFromTiles[tile].walkingSpeed;

        return walkingSpeed;
    }
    public void PlaceSeeds(Vector2 worldPosition)
    {
        Vector3Int gridPosition = map.WorldToCell(worldPosition);
        TileBase tile = map.GetTile(gridPosition);

        if (tile == null || !behaviourFromTiles[tile].canBeSeeded)
            return;

        if (!Crops.ContainsKey(gridPosition))
        {
            Details.SetTile(gridPosition, Seeded);
            CropManager newCrop = Instantiate(CropPrefab);
            newCrop.tilePos = gridPosition;
            Crops.Add(gridPosition, newCrop);

            print("new seeds added");
        }
    }

    public void PlantSeeds(Vector2 worldPosition)
    {
        Vector3Int gridPosition = map.WorldToCell(worldPosition);
        TileBase tileBase = map.GetTile(gridPosition);
        TileBase tileDetails = Details.GetTile(gridPosition);

        if (tileBase == null)
            return;

        if (tileDetails == Seeded && Crops.ContainsKey(gridPosition))
        {
            Crops[gridPosition].PlantSeeds();
            print("seeds planted");
        }
    }

    public void FertiliseSeeds(Vector2 worldPosition, float fertFactor)
    {
        Vector3Int gridPosition = map.WorldToCell(worldPosition);
        TileBase tileBase = map.GetTile(gridPosition);
        TileBase tileDetails = Details.GetTile(gridPosition);

        if (tileBase == null)
            return;

        if (tileDetails != Seeded && Crops.ContainsKey(gridPosition))
        {
            Crops[gridPosition].fertilise(fertFactor);
            print("growth speed increased");
        }
    }
    
    public void UpdateCropTiles(Vector3Int tilePos, TileBase tile)
    {
        Details.SetTile(tilePos, tile);
    }

    private IEnumerator GrowCrops()
    {
        while (true)
        {
            foreach (var crop in Crops)
            {
                crop.Value.Grow(1f);
                
            }

            yield return new WaitForSeconds(GrowthInterval);
        }    
    }

    private IEnumerator DayNightCycle()
    {
        bool DayToNight = true;
        while (true)
        {
            switch (DayToNight)
            {
                case true:
                    DayNightOverlay.color = new Color(DayNightOverlay.color.r, DayNightOverlay.color.g, DayNightOverlay.color.b, DayNightOverlay.color.a + DayNightAlphaGradient);
                    if (DayNightOverlay.color.a >= DayNightAlphaMax)
                    {
                        DayToNight = false;
                    }
                break;

                case false:
                    DayNightOverlay.color = new Color(DayNightOverlay.color.r, DayNightOverlay.color.g, DayNightOverlay.color.b, DayNightOverlay.color.a - DayNightAlphaGradient);
                    if (DayNightOverlay.color.a <= DayNightAlphaMin)
                    {
                        DayToNight = true;
                    }
                    break;
            }

            yield return new WaitForSeconds(DayNightInterval);
        }
    }
}
