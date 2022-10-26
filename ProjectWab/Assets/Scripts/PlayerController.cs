using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    // Movement
    public float baseSpeed = 5f;
    public Transform movePoint;
    public LayerMask isWalkable;
    public LayerMask Collisions;

    // Map
    [SerializeField]
    private Tilemap mapBase;
    [SerializeField]
    private Tilemap mapDetails;
    [SerializeField]
    private Tilemap UI;
    public TileBase SelectedTile;
    public TileBase tilledSoil;
    public TileBase grass;
    public TileBase seeds;
    private Vector3Int prevTilePos;
    private MapManager mapManager;

    // Equipment
    public int EquipmentID;
    public GameObject EquipmentSlots;
    public GameObject _object;
    public GameObject fertiliserCone;
    public ParticleSystem fertiliserParticles;
    public GameObject pivot;

    // Sprites
    public Sprite NorthFacing;
    public Sprite SouthFacing;
    public Sprite EastFacing;
    public Sprite WestFacing;


    // Start is called before the first frame update
    void Start()
    {
        movePoint.parent = null;
        mapManager = FindObjectOfType<MapManager>();
        EquipmentSlots.transform.Find("Item Slot 1").transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
    }

    // Update is called once per frame
    void Update()
    {
        float moveSpeed = baseSpeed * mapManager.GetTileWalkingSpeed(transform.position);

        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);


        // Movement
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            if (Vector3.Distance(transform.position, movePoint.position) <= 0.5f)
            {

                if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
                {
                    if (Physics2D.OverlapCircle(movePoint.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f), .2f, isWalkable) && !Physics2D.OverlapCircle(movePoint.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f), .2f, Collisions))
                    {
                        movePoint.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
                    }

                    if (Input.GetAxisRaw("Horizontal") == 1f)   { GetComponent<SpriteRenderer>().sprite = EastFacing; }
                    if (Input.GetAxisRaw("Horizontal") == -1f)  { GetComponent<SpriteRenderer>().sprite = WestFacing; }
                }

                if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
                {
                    if (Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f), .2f, isWalkable) && !Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f), .2f, Collisions))
                    {
                        movePoint.position += new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);
                    }

                    if (Input.GetAxisRaw("Vertical") == 1f)     { GetComponent<SpriteRenderer>().sprite = NorthFacing; }
                    if (Input.GetAxisRaw("Vertical") == -1f)    { GetComponent<SpriteRenderer>().sprite = SouthFacing; }
                }
            }
        }


        // Display UI

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int gridPosition = mapBase.WorldToCell(mousePosition);

        if (mapBase.GetTile(gridPosition) != SelectedTile && mapBase.GetTile(gridPosition) != null)
        {
            UI.SetTile(prevTilePos, null);
            prevTilePos = gridPosition;
            if (EquipmentSlots.transform.GetChild(EquipmentID - 1).GetComponent<ItemSlotData>().shouldShowSelectedTile)
            {
                UI.SetTile(gridPosition, SelectedTile);
            }
        }


        /////////////////// Controls


        // Selecting Equipment
        if (Input.GetKeyDown("1"))
        {
            EquipmentID = 1;
            foreach (Transform child in EquipmentSlots.transform)
            {
                child.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
            EquipmentSlots.transform.GetChild(EquipmentID - 1).transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        }
        if (Input.GetKeyDown("2"))
        {
            EquipmentID = 2;
            foreach (Transform child in EquipmentSlots.transform)
            {
                child.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
            EquipmentSlots.transform.GetChild(EquipmentID - 1).transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        }
        if (Input.GetKeyDown("3"))
        {
            EquipmentID = 3;
            foreach (Transform child in EquipmentSlots.transform)
            {
                child.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
            EquipmentSlots.transform.GetChild(EquipmentID - 1).transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        }
        if (Input.GetKeyDown("4"))
        {
            EquipmentID = 4;
            foreach (Transform child in EquipmentSlots.transform)
            {
                child.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
            EquipmentSlots.transform.GetChild(EquipmentID - 1).transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        }


        // When mouse button is pressed
        // Used for instantaneous actions
        if (Input.GetMouseButtonDown(0))
        {
            TileBase clickedTileWorld = mapBase.GetTile(gridPosition);
            TileBase clickedTileDetails = mapDetails.GetTile(gridPosition);

            switch (EquipmentID)
            {
                // Hoe
                case 1:
                    if (clickedTileWorld == grass)
                    {
                        mapBase.SetTile(gridPosition, tilledSoil);
                    }
                    break;

                // Seeds
                case 2:
                    mapManager.PlaceSeeds(mousePosition);
                    break;

                // Watering Can
                case 3:
                    if (clickedTileDetails == seeds)
                    {
                        mapManager.PlantSeeds(mousePosition);
                    }
                    break;
            }
        }

        // While mouse button is held
        // Used for channelled actions
        if (Input.GetMouseButton(0))
        {
            switch(EquipmentID)
            {
                default:
                    break;
                case 4:
                    if (fertiliserParticles.isEmitting == false)
                    {
                        fertiliserParticles.Play();
                    }
                    Vector3 targetDirection = new Vector3(mousePosition.x, mousePosition.y, 0f) - transform.position;
                    pivot.transform.up = targetDirection;
                    

                    break;
            }
        }
        else if (!Input.GetMouseButton(0))
        {
            //fertiliserCone.SetActive(false);
            if (fertiliserParticles.isEmitting == true)
            {
                fertiliserParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
            //fertiliserParticles.Pause();
        }

    }
}
