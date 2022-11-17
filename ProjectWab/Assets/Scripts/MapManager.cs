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

    float nightimePercentage = .5f;
    public GameObject clockHand;


    // Player
    PlayerController playerController;


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

        playerController = FindObjectOfType<PlayerController>();
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
    public bool PlaceSeeds(Vector2 worldPosition)
    {
        Vector3Int gridPosition = map.WorldToCell(worldPosition);
        TileBase tile = map.GetTile(gridPosition);

        if (tile == null || !behaviourFromTiles[tile].canBeSeeded)
            return false;

        if (!Crops.ContainsKey(gridPosition))
        {
            Details.SetTile(gridPosition, Seeded);
            CropManager newCrop = Instantiate(CropPrefab);
            newCrop.tilePos = gridPosition;
            newCrop.SetCollider();
            Crops.Add(gridPosition, newCrop);

            print("new seeds added");
            return true;
        }
        return false;
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

    public void Harvest(Vector2 worldPosition)
    {
        Vector3Int gridPosition = map.WorldToCell(worldPosition);
        TileBase tileBase = map.GetTile(gridPosition);
        TileBase tileDetails = Details.GetTile(gridPosition);

        if (tileBase == null)
            return;

        if (tileDetails != Seeded && Crops.ContainsKey(gridPosition))
        {
            if (Crops[gridPosition].harvest())
            {
                /*print("Setting tile at " + gridPosition + " to null");
                Details.SetTile(gridPosition, null);
                print("Destroying " + Crops[gridPosition].name);
                Destroy(Crops[gridPosition]);*/

                Details.SetTile(gridPosition, null);
                Crops[gridPosition].removeCrop();
                Crops.Remove(gridPosition);

                playerController.incMoney(4);
                //print("+2 mullah");
            }
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
        int maxClockValue = 500;
        int sunriseTime = (int)(maxClockValue * (nightimePercentage / 2f));
        int sunsetTime = (int)(maxClockValue - maxClockValue * (nightimePercentage  / 2f));
        int time = maxClockValue / 2;
        while (true)
        {
            // Loop time 0 - 100
            // Using a value to determine sunset and sunrise and another value for sunset/rise duration until max/min darkness is reached
            
            print(time.ToString());

            if (time == sunriseTime)
            {
                // initiate sunrise
                StartCoroutine(SunriseCoroutine(true));
            }
            else if (time == sunsetTime)
            {
                // initiate sunset
                StartCoroutine(SunriseCoroutine(false));
            }

            // adjust clock hand
            clockHand.transform.eulerAngles = new Vector3(
                clockHand.transform.eulerAngles.x,
                clockHand.transform.eulerAngles.y,
                136.8f - ((360f / (float)maxClockValue) * (float)time)
                );


            // Start the clock over at midnight
            if (time >= maxClockValue - 1)
            {
                time = 0;
            }
            // Increment the time
            else
            {
                time++;
            }

            yield return new WaitForSeconds(DayNightInterval);
        }
    }

    private IEnumerator SunriseCoroutine(bool RiseSetBool = false)
    {
        while (true)
        {
            switch (RiseSetBool)
            {
                // Increase alpha during sunset
                case false:
                    print("sun is setting");
                    DayNightOverlay.color = new Color(DayNightOverlay.color.r, DayNightOverlay.color.g, DayNightOverlay.color.b, DayNightOverlay.color.a + DayNightAlphaGradient);
                    if (DayNightOverlay.color.a >= DayNightAlphaMax)
                    {
                        yield break;
                    }
                break;

                // Decrease alpha during sunrise
                case true:
                    print("sun is rising");
                    DayNightOverlay.color = new Color(DayNightOverlay.color.r, DayNightOverlay.color.g, DayNightOverlay.color.b, DayNightOverlay.color.a - DayNightAlphaGradient);
                    if (DayNightOverlay.color.a <= DayNightAlphaMin)
                    {
                        yield break;
                    }
                    break;
            }

            yield return new WaitForSeconds(0.05f);
        }
    }
}
