using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public GameObject GameManager;
    public GameObject CurrentTile;
    public GameObject TargetTile;
    public GameObject NextTile;
    public GameObject[,] TileMap;
    GameObject GoalTile;
    List<GameObject> Pathing = new List<GameObject>();
    public float WalkSpeed = 2.0f;

    int columns;
    int rows;

    // Start is called before the first frame update
    void Start()
    {

    }

    void Awake()
    {
        GameManager = GameObject.Find("GameManager");

        TileMap = GameManager.GetComponent<WorldGeneration>().Tiles;
        columns = GameManager.GetComponent<WorldGeneration>().columns;
        rows = GameManager.GetComponent<WorldGeneration>().rows;

        CurrentTile = TileMap[0, 0];
        //transform.position = TileMap[CurrentTile % columns, Mathf.FloorToInt(CurrentTile / rows)].transform.position;
        TargetTile = TileMap[Random.Range(0, TileMap.GetLength(0)), Random.Range(0, TileMap.GetLength(1))];
        Pathfind(TargetTile.GetComponent<TileManager>().id);
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
        //Debug.Log("Left in path: " + Pathing.Count);

        if (NextTile == null)
        {
            NextTile = Pathing[0];
        }

        if (transform.position == NextTile.transform.position) 
        {
            CurrentTile = NextTile;
            if (Pathing.Count > 0)
            {
                NextTile = Pathing[0];
                Pathing.RemoveAt(0);

                //Debug.Log("Move to: " + NextTile);
                //Debug.Log("Currently on: " + CurrentTile);
            }
            else
            {
                TargetTile = TileMap[Random.Range(0, TileMap.GetLength(0)), Random.Range(0, TileMap.GetLength(1))];
                Pathfind(TargetTile.GetComponent<TileManager>().id);
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, NextTile.transform.position, WalkSpeed * Time.deltaTime);
        }
    }




    // Modification of the A* pathfinding algorithm on https://dotnetcoretutorials.com/2020/07/25/a-search-pathfinding-algorithm-in-c/
    class Tile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Cost { get; set; }
        public int Distance { get; set; }
        public int CostDistance => Cost + Distance;
        public Tile Parent { get; set; }

        //The distance is essentially the estimated distance, ignoring walls to our target. 
        //So how many tiles left and right, up and down, ignoring walls, to get there. 
        public void SetDistance(int targetX, int targetY)
        {
            this.Distance = Mathf.Abs(targetX - X) + Mathf.Abs(targetY - Y);
        }
    }

    // Modification of the A* pathfinding algorithm on https://dotnetcoretutorials.com/2020/07/25/a-search-pathfinding-algorithm-in-c/
    void Pathfind(int targetTile)
    {
        var start = new Tile();
        start.Y = CurrentTile.GetComponent<TileManager>().Y;
        start.X = CurrentTile.GetComponent<TileManager>().X;

        TileMap[start.X, start.Y].GetComponent<SpriteRenderer>().color = Color.yellow;

        var finish = new Tile();
        finish.Y = TargetTile.GetComponent<TileManager>().Y;
        finish.X = TargetTile.GetComponent<TileManager>().X;

        start.SetDistance(finish.X, finish.Y);

        var activeTiles = new List<Tile>();
        activeTiles.Add(start);
        var visitedTiles = new List<Tile>();


        while(activeTiles.Any())
        {
            var checkTile = activeTiles.OrderBy(x => x.CostDistance).First();

            if (checkTile.X == finish.X && checkTile.Y == finish.Y)
            {
                // We found the destination and we can be sure (Because the the OrderBy above)
                // That it's the most low cost option. 
                //Debug.Log("We are at the destination!");

                var tile = checkTile;
                //Debug.Log("Retracing steps backwards...");
                Pathing.Clear();

                TileMap[tile.X, tile.Y].GetComponent<SpriteRenderer>().color = Color.green;

                while (true)
                {
                    //Debug.Log($"{tile.X} : {tile.Y}");
                    if (TileMap[tile.X, tile.Y].GetComponent<TileManager>().isWalkable)
                    {
                        // this spot is walkable
                        // do something about it
                        // starts at the last walkable place though
                        // so maybe just add these to a list and move to the tail of the list
                        Pathing.Add(TileMap[tile.X, tile.Y]);
                        //Debug.Log("Added " + TileMap[tile.X, tile.Y] + " to pathing");
                        //Debug.Log(tile + " is parented to " + tile.Parent);
                    }
                    tile = tile.Parent;
                    if (tile == null)
                    {
                        //Debug.Log("Retracing complete");
                        Pathing.Reverse();
                        return;
                    }
                }
            }

            visitedTiles.Add(checkTile);
            activeTiles.Remove(checkTile);

            var walkableTiles = GetWalkableTiles(checkTile, finish);

            foreach(var walkableTile in walkableTiles)
            {
                // We have already visited this tile so we don't need to do so again!
                if (visitedTiles.Any(x => x.X == walkableTile.X && x.Y == walkableTile.Y))
                    continue;

                // It's already in the active list, but that's OK, maybe this new tile has a better value (e.g. We might zigzag earlier but this is now straighter). 
                if (activeTiles.Any(x => x.X == walkableTile.X && x.Y == walkableTile.Y))
                {
                    var existingTile = activeTiles.First(x => x.X == walkableTile.X && x.Y == walkableTile.Y);
                    if (existingTile.CostDistance > checkTile.CostDistance)
                    {
                        activeTiles.Remove(existingTile);
                        activeTiles.Add(walkableTile);
                    }
                } else
                {
                    // We've never seen this tile before so add it to the list. 
                    activeTiles.Add(walkableTile);
                }
            }
        }

        //Debug.Log("No Path Found!");


    }

    // Modification of the A* pathfinding algorithm on https://dotnetcoretutorials.com/2020/07/25/a-search-pathfinding-algorithm-in-c/
    List<Tile> GetWalkableTiles(Tile currentTile, Tile targetTile)
    {
        var possibleTiles = new List<Tile>()
        {
            new Tile { X = currentTile.X, Y = currentTile.Y - 1, Parent = currentTile, Cost = currentTile.Cost + 1 },
            new Tile { X = currentTile.X, Y = currentTile.Y + 1, Parent = currentTile, Cost = currentTile.Cost + 1 },
            new Tile { X = currentTile.X - 1, Y = currentTile.Y, Parent = currentTile, Cost = currentTile.Cost + 1 },
            new Tile { X = currentTile.X + 1, Y = currentTile.Y, Parent = currentTile, Cost = currentTile.Cost + 1 },
        };

        //Debug.Log("Parent set to " + currentTile);

        possibleTiles.ForEach(tile => tile.SetDistance(targetTile.X, targetTile.Y));

        var maxX = columns - 1;
        var maxY = rows - 1;

        // return Tiles that fit the below constraints
        return possibleTiles
                            .Where(tile => tile.X >= 0 && tile.X <= maxX)
                            .Where(tile => tile.Y >= 0 && tile.Y <= maxY)
                            //.Where(tile => TileMap[tile.X, tile.Y].GetComponent<TileManager>().isWalkable || TileMap[tile.X, tile.Y] == GoalTile)
                            .ToList();
    }

}
