using AMazeCS;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public enum AudioType
{
    Sword, Archery, March, FallDown
}

public class WorldGrid : MonoBehaviour
{
    [SerializeField]
    private LayerMask ObstacleMask;
    [SerializeField, Tooltip("Don't make this too big. Recommended: [1000, 5000] for each")]
    private Vector2 WorldSize;
    //[SerializeField, Range(0.1f, 30.0f)]
    private float NodeSize = 1;
    [SerializeField]
    private bool ShouldDrawObstacleGizmos = true;
    [SerializeField]
    private GameObject[] Buildings;
    [SerializeField]
    private BoxCollider StadardRoadCollider;
    [SerializeField]
    private TerrainData TerrainData;
    [SerializeField]
    private Vector2 MinRectSize;
    [SerializeField]
    private Vector2 MaxRectSize;
    [SerializeField, Range(50, 15000)]
    private int RoadLength = 10000;
    [SerializeField, Range(0, 100)]
    private int MinPartialLength = 50;
    [SerializeField]
    private GameObject[] Trees;

    [SerializeField]
    private GameObject RoadI;
    [SerializeField]
    private GameObject RoadX;
    [SerializeField]
    private GameObject RoadL;
    [SerializeField]
    private GameObject RoadT;
    [SerializeField]
    private GameObject RoadHI;

    private BoxCollider[] TreeColliders;
    private int[,] RoadMap;

    [SerializeField]
    private GameObject[] Airplanes;
    [SerializeField]
    private GameObject[] Balloons;
    [SerializeField]
    private Car[] Cars;

    public Airport[] Airports { private set; get; }

    private BoxCollider[] BuildingColliders;
    public float GetNodeSize
    {
        get
        {
            return NodeSize;
        }
    }

    private Node[,] Nodes;
    private int Width;
    private int Height;
    public int GetWorldSize()
    {
        return Width * Height;
    }

    public List<Node> Path { set; get; }
    public Node TargetNode;
    // Use this for initialization
    void Start()
    {
        Application.targetFrameRate = 60;
        Width = Mathf.RoundToInt(WorldSize.x / NodeSize);
        Height = Mathf.RoundToInt(WorldSize.y / NodeSize);
        Nodes = new Node[Width, Height];
        RoadLength = Mathf.Min(RoadLength, Width * Height);
        BuildingColliders = new BoxCollider[Buildings.Length];
        for (int i = 0; i < BuildingColliders.Length; i++)
        {
            BuildingColliders[i] = Buildings[i].GetComponent<BoxCollider>();
        }

        TreeColliders = new BoxCollider[Trees.Length];
        for (int i = 0; i < TreeColliders.Length; i++)
        {
            TreeColliders[i] = Trees[i].GetComponent<BoxCollider>();
        }

        CreateWorld();
        var portsInTheWorld = GameObject.FindGameObjectsWithTag("Airport");
        Airports = new Airport[portsInTheWorld.Length];
        for (int i = 0; i < portsInTheWorld.Length; i++)
        {
            Airports[i] = portsInTheWorld[i].GetComponent<Airport>();
        }
        CreateAirplanes(Airports.Length);
        CreateBalloons(Airports.Length);
        CreateCars();
    }

    private void CreateCars()
    {
        if(Cars.Length == 0)
        {
            return;
        }
        var points = new RandomQueue<GameObject>(GameObject.FindGameObjectsWithTag("StartPoint"));
        int numCars = points.Count / 100 + 10;
        numCars = Mathf.Min(numCars, 1050);
        numCars = Mathf.Max(numCars, 1000);
        Debug.Log($"# of cars: {numCars}");
        Debug.Log($"Queue Size: {points.Count}");
        for (int i = 0; i < numCars; i++)
        {
            var newCar = Instantiate<Car>(Cars[Random.Range(0, Cars.Length)]);
            var spawnPoint = points.Dequeue();
            while(spawnPoint != null && spawnPoint.GetComponent<RoadPoint>().IsTrafficLightRoad)
            {
                spawnPoint = points.Dequeue();
            }
            if(spawnPoint == null)
            {
                break;
            }
            // newCar.SetNextPoint(spawnPoint, spawnPoint);
            newCar.SetNextPoint(spawnPoint);
            newCar.transform.position = spawnPoint.transform.position + Vector3.up * 5;
            newCar.transform.rotation = spawnPoint.transform.rotation;
            // Camera.main.transform.position = newCar.transform.position;
        }
    }

    private void CreateAirplanes(int airportCount)
    {
        Vector3 startPoint = transform.position - new Vector3(WorldSize.x / 2, 0, WorldSize.y / 2);
        if (airportCount == 0)
        {
            return;
        }
        for (int i = 0; i < airportCount + 10; i++)
        {
            var newAirplane = Instantiate<GameObject>(Airplanes[Random.Range(0, Airplanes.Length)]);
            newAirplane.transform.position = startPoint + 
                new Vector3(Random.Range(0, Width), 250, Random.Range(0, Height));
        }
    }

    private void CreateBalloons(int airportCount)
    {
        Vector3 startPoint = transform.position - new Vector3(WorldSize.x / 2, 0, WorldSize.y / 2);
        if (airportCount == 0)
        {
            return;
        }
        for (int i = 0; i < airportCount + 50; i++)
        {
            var newBalloon = Instantiate<GameObject>(Balloons[Random.Range(0, Balloons.Length)]);
            newBalloon.transform.position = startPoint +
                new Vector3(Random.Range(0, Width), 120, Random.Range(0, Height));
        }
    }

    public Node WorldPositionToNode(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + WorldSize.x / 2) / WorldSize.x;
        float percentY = (worldPosition.z + WorldSize.y / 2) / WorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt(Width * percentX);
        int y = Mathf.RoundToInt(Height * percentY);
        x = Mathf.Max(x, 0);
        y = Mathf.Max(y, 0);

        x = Mathf.Min(x, Width - 1);
        y = Mathf.Min(y, Height - 1);
        if (IsValidPoint(x, y) && Nodes[x, y].IsObstacle)
        {
            foreach (var item in GetNeighbours(Nodes[x, y]))
            {
                if (!item.IsObstacle)
                {
                    return item;
                }
            }
        }
        return Nodes[x, y];
    }

    public Vector3 NodeToWorldPosition(Node node)
    {
        Vector3 startPoint = transform.position - new Vector3(WorldSize.x / 2, 0, WorldSize.y / 2);
        return startPoint + new Vector3(node.X * NodeSize, 0, node.Y * NodeSize);
    }

    public static readonly int[,] Neighbours = {{ -1, 1 }, { 0, 1 }, { 1, 1 },
                                                { -1, 0 }, /**Point**/{ 1, 0 },
                                                { -1, -1 },{ 0, -1 },{ 1, -1 }};

    public List<Node> GetNeighbours(Node currentNode)
    {
        var result = new List<Node>();
        for (int i = 0; i < Neighbours.GetLength(0); i++)
        {
            int currentX = currentNode.X + Neighbours[i, 0];
            int currentY = currentNode.Y + Neighbours[i, 1];
            if (IsValidPoint(currentX, currentY))
            {
                result.Add(Nodes[currentX, currentY]);
            }
        }
        return result;
    }

    public bool IsValidPoint(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    private void CreateWorld()
    {
        Vector3 startPoint = transform.position - new Vector3(WorldSize.x / 2, 0, WorldSize.y / 2);
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Vector3 objPoint = startPoint + new Vector3(x * NodeSize, 0, y * NodeSize);
                bool isObstacle = Physics.CheckSphere(objPoint, NodeSize / 2, ObstacleMask);
                Nodes[x, y] = new Node(isObstacle, objPoint, x, y);
            }
        }
        int[] roadPoint = new int[2];
        int mapWidth = Width / (int)StadardRoadCollider.size.x;
        int mapHeight = Height / (int)StadardRoadCollider.size.z;
        roadPoint[0] = Random.Range(0, mapWidth);
        roadPoint[1] = Random.Range(0, mapHeight);
        RoadMap = new int[mapWidth, mapHeight];
        int partialLengthCount = MinPartialLength;

        int direction = Random.Range(0, roadPoint.Length);
        int increment = Random.Range(-1, 2);

        int maxSteps = mapHeight * mapWidth;

        for (int i = 0; i < RoadLength; i++)
        {
            //Vector3 objPoint = startPoint + 
            //    new Vector3(StadardRoadCollider.size.x * roadPoint[0],
            //    0, 
            //    StadardRoadCollider.size.z * roadPoint[1]);
            //var newRoad = Instantiate<GameObject>(StadardRoadCollider.gameObject);
            //newRoad.transform.position = objPoint + 
            //    Vector3.up *
            //    (TerrainData.GetHeight(roadPoint[0], 
            //    roadPoint[1]) + 0.52f);

            RoadMap[roadPoint[0], roadPoint[1]] = 1;
            while (RoadMap[roadPoint[0], roadPoint[1]] == 1)
            {
                if (maxSteps-- <= 0)
                {
                    throw new System.Exception("Road Error!");
                }
                if (partialLengthCount-- <= 0)
                {
                    if (roadPoint[direction] % 2 != 0)
                    {
                        roadPoint[direction] -= increment;
                    }
                    direction = Random.Range(0, roadPoint.Length);
                    increment = Random.Range(-1, 1);
                    if(increment == 0)
                    {
                        increment = 1;
                    }
                    partialLengthCount = MinPartialLength;
                }
                else
                {
                    roadPoint[direction] += increment;
                }
                if (roadPoint[0] < 0 || roadPoint[0] >= mapWidth || roadPoint[1] < 0 || roadPoint[1] >= mapHeight)
                {
                    roadPoint[0] = Mathf.Max(roadPoint[0], 0);
                    roadPoint[0] = Mathf.Min(roadPoint[0], mapWidth - 1);
                    roadPoint[1] = Mathf.Max(roadPoint[1], 0);
                    roadPoint[1] = Mathf.Min(roadPoint[1], mapHeight - 1);
                    if (roadPoint[direction] % 2 != 0)
                    {
                        roadPoint[direction] -= increment;
                    }
                    direction = Random.Range(0, roadPoint.Length);
                    increment = Random.Range(-1, 1);
                    if (increment == 0)
                    {
                        increment = 1;
                    }
                    partialLengthCount = MinPartialLength;
                }
            }
        }
        #region Create Roads
        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                if (RoadMap[i, j] != 1)
                {
                    continue;
                }
                Vector3 objPoint = startPoint +
                    new Vector3(StadardRoadCollider.size.x * i,
                    TerrainData.GetHeight(roadPoint[0], roadPoint[1]) + 0.56f,
                    StadardRoadCollider.size.z * j);
                //if (i - 1 >= 0 && i + 1 < mapWidth && j - 1 >= 0 && j + 1 < mapHeight)
                //{
                if (i + 1 < mapWidth && RoadMap[i + 1, j] == 1 &&
                i - 1 >= 0 && RoadMap[i - 1, j] == 1 &&
                j + 1 < mapHeight && RoadMap[i, j + 1] == 1 &&
                j - 1 >= 0 && RoadMap[i, j - 1] == 1)
                {
                    var newRoadX = Instantiate<GameObject>(RoadX);
                    newRoadX.transform.rotation = Quaternion.Euler(0, 0, 0);
                    newRoadX.transform.position = objPoint;
                }//////////////////////////////////////////////////
                else if (i + 1 < mapWidth && RoadMap[i + 1, j] == 1 &&
                i - 1 >= 0 && RoadMap[i - 1, j] == 1 &&
                (j + 1 >= mapHeight || RoadMap[i, j + 1] != 1) &&
                (j - 1 < 0 || RoadMap[i, j - 1] != 1))
                {
                    var newRoadI = Instantiate<GameObject>(RoadI);
                    newRoadI.transform.rotation = Quaternion.Euler(0, 90, 0);
                    newRoadI.transform.position = objPoint;
                }
                else if (j + 1 < mapHeight && RoadMap[i, j + 1] == 1 &&
                j - 1 >= 0 && RoadMap[i, j - 1] == 1 &&
                (i + 1 >= mapWidth || RoadMap[i + 1, j] != 1) &&
                (i - 1 < 0 || RoadMap[i - 1, j] != 1))
                {
                    var newRoadI = Instantiate<GameObject>(RoadI);
                    newRoadI.transform.rotation = Quaternion.Euler(0, 0, 0);
                    newRoadI.transform.position = objPoint;
                }////////////////////////////////////////////////////////////////
                else if (j + 1 < mapHeight && RoadMap[i, j + 1] == 1 &&
                (j - 1 < 0 || RoadMap[i, j - 1] != 1) &&
                i + 1 < mapWidth && RoadMap[i + 1, j] == 1 &&
                (i - 1 < 0 || RoadMap[i - 1, j] != 1))
                {
                    var newRoadL = Instantiate<GameObject>(RoadL);
                    newRoadL.transform.rotation = Quaternion.Euler(0, 180, 0);
                    newRoadL.transform.position = objPoint;
                }
                else if ((j + 1 >= mapHeight || RoadMap[i, j + 1] != 1) &&
                j - 1 >= 0 && RoadMap[i, j - 1] == 1 &&
                (i + 1 >= mapWidth || RoadMap[i + 1, j] != 1) &&
                i - 1 >= 0 && RoadMap[i - 1, j] == 1)
                {
                    var newRoadL = Instantiate<GameObject>(RoadL);
                    newRoadL.transform.rotation = Quaternion.Euler(0, 0, 0);
                    newRoadL.transform.position = objPoint;
                }
                else if (j + 1 < mapHeight && RoadMap[i, j + 1] == 1 &&
                (j - 1 < 0 || RoadMap[i, j - 1] != 1) &&
                (i + 1 >= mapWidth || RoadMap[i + 1, j] != 1) &&
                i - 1 >= 0 && RoadMap[i - 1, j] == 1)
                {
                    var newRoadL = Instantiate<GameObject>(RoadL);
                    newRoadL.transform.rotation = Quaternion.Euler(0, 90, 0);
                    newRoadL.transform.position = objPoint;
                }
                else if ((j + 1 >= mapHeight || RoadMap[i, j + 1] != 1) &&
                j - 1 >= 0 && RoadMap[i, j - 1] == 1 &&
                i + 1 < mapWidth && RoadMap[i + 1, j] == 1 &&
                (i - 1 < 0 || RoadMap[i - 1, j] != 1))
                {
                    var newRoadL = Instantiate<GameObject>(RoadL);
                    newRoadL.transform.rotation = Quaternion.Euler(0, 270, 0);
                    newRoadL.transform.position = objPoint;
                }
                //}/////////////////////////////////////////////////////////////////////
                if ((i + 1 >= mapWidth || RoadMap[i + 1, j] != 1) &&
                    i - 1 >= 0 && RoadMap[i - 1, j] == 1 &&
                    j + 1 < mapHeight && RoadMap[i, j + 1] == 1 &&
                    j - 1 >= 0 && RoadMap[i, j - 1] == 1)
                {
                    var newRoadT = Instantiate<GameObject>(RoadT);
                    newRoadT.transform.rotation = Quaternion.Euler(0, 90, 0);
                    newRoadT.transform.position = objPoint;
                }
                else if (i + 1 < mapWidth && RoadMap[i + 1, j] == 1 &&
                     (i - 1 < 0 || RoadMap[i - 1, j] != 1) &&
                     j + 1 < mapHeight && RoadMap[i, j + 1] == 1 &&
                     j - 1 >= 0 && RoadMap[i, j - 1] == 1)
                {
                    var newRoadT = Instantiate<GameObject>(RoadT);
                    newRoadT.transform.rotation = Quaternion.Euler(0, 270, 0);
                    newRoadT.transform.position = objPoint;
                }
                else if (i + 1 < mapWidth && RoadMap[i + 1, j] == 1 &&
                     i - 1 >= 0 && RoadMap[i - 1, j] == 1 &&
                     (j + 1 >= mapHeight || RoadMap[i, j + 1] != 1) &&
                     j - 1 >= 0 && RoadMap[i, j - 1] == 1)
                {
                    var newRoadT = Instantiate<GameObject>(RoadT);
                    newRoadT.transform.rotation = Quaternion.Euler(0, 0, 0);
                    newRoadT.transform.position = objPoint;
                }
                else if (i + 1 < mapWidth && RoadMap[i + 1, j] == 1 &&
                     i - 1 >= 0 && RoadMap[i - 1, j] == 1 &&
                     j + 1 < mapHeight && RoadMap[i, j + 1] == 1 &&
                     (j - 1 < 0 || RoadMap[i, j - 1] != 1))
                {
                    var newRoadT = Instantiate<GameObject>(RoadT);
                    newRoadT.transform.rotation = Quaternion.Euler(0, 180, 0);
                    newRoadT.transform.position = objPoint;
                } //////////////////////////////////////////////////////////////////////////
                else if ((i + 1 >= mapWidth || RoadMap[i + 1, j] != 1) &&
                    (i - 1 < 0 || RoadMap[i - 1, j] != 1) &&
                    (j + 1 >= mapHeight || RoadMap[i, j + 1] != 1) &&
                    j - 1 >= 0 && RoadMap[i, j - 1] == 1)
                {
                    var newRoadHI = Instantiate<GameObject>(RoadHI);
                    newRoadHI.transform.rotation = Quaternion.Euler(0, 0, 0);
                    newRoadHI.transform.position = objPoint;
                }
                else if ((i + 1 >= mapWidth || RoadMap[i + 1, j] != 1) &&
                     (i - 1 < 0 || RoadMap[i - 1, j] != 1) &&
                     j + 1 < mapHeight && RoadMap[i, j + 1] == 1 &&
                     (j - 1 < 0 || RoadMap[i, j - 1] != 1))
                {
                    var newRoadHI = Instantiate<GameObject>(RoadHI);
                    newRoadHI.transform.rotation = Quaternion.Euler(0, 180, 0);
                    newRoadHI.transform.position = objPoint;
                }
                else if ((i + 1 >= mapWidth || RoadMap[i + 1, j] != 1) &&
                     i - 1 >= 0 && RoadMap[i - 1, j] == 1 &&
                     (j + 1 >= mapHeight || RoadMap[i, j + 1] != 1) &&
                     (j - 1 < 0 || RoadMap[i, j - 1] != 1))
                {
                    var newRoadHI = Instantiate<GameObject>(RoadHI);
                    newRoadHI.transform.rotation = Quaternion.Euler(0, 90, 0);
                    newRoadHI.transform.position = objPoint;
                }
                else if (i + 1 < mapWidth && RoadMap[i + 1, j] == 1 &&
                     (i - 1 < 0 || RoadMap[i - 1, j] != 1) &&
                     (j + 1 >= mapHeight || RoadMap[i, j + 1] != 1) &&
                     (j - 1 < 0 || RoadMap[i, j - 1] != 1))
                {
                    var newRoadHI = Instantiate<GameObject>(RoadHI);
                    newRoadHI.transform.rotation = Quaternion.Euler(0, 270, 0);
                    newRoadHI.transform.position = objPoint;
                }
                // var newRoad = Instantiate<GameObject>(StadardRoadCollider.gameObject);
                // newRoad.transform.position = objPoint + Vector3.up;
            }
        }
        #endregion
        for (int i = 0; i < mapWidth; i += Random.Range(1, 5))
        {
            for (int j = 0; j < mapHeight; j+= Random.Range(1, 5))
            {
                if (RoadMap[i, j] == 1)
                {
                    continue;
                }
                Vector3 objPoint = startPoint +
                        new Vector3(StadardRoadCollider.size.x * i,
                        TerrainData.GetHeight(roadPoint[0], roadPoint[1]) + 0.1f,
                        StadardRoadCollider.size.z * j);
                Quaternion objRotation = new Quaternion();
                int buildingIndex = Random.Range(0, Buildings.Length);
                if (i - 1 > 0 && RoadMap[i - 1, j] == 1)
                {
                    objRotation = Quaternion.Euler(0, 270, 0);
                }
                else if (j - 1 > 0 && RoadMap[i, j - 1] == 1)
                {
                    objRotation = Quaternion.Euler(0, 180, 0);
                }
                else if (i + 1 < mapWidth && RoadMap[i + 1, j] == 1)
                {
                    objRotation = Quaternion.Euler(0, 90, 0);
                }
                else if (j + 1 < mapHeight && RoadMap[i, j + 1] == 1)
                {
                    objRotation = Quaternion.Euler(0, 0, 0);
                }
                else
                {
                    continue;
                }
                objPoint += (objRotation * Vector3.back).normalized *
                    ((BuildingColliders[buildingIndex].size.z * 
                    Buildings[buildingIndex].transform.localScale.z - 
                    StadardRoadCollider.size.z));

                if (!Physics.CheckBox(BuildingColliders[buildingIndex].center + objPoint,
                    BuildingColliders[buildingIndex].size / 2, objRotation, ObstacleMask))
                {
                    var newBuilding = Instantiate<GameObject>(Buildings[buildingIndex]);
                    newBuilding.transform.position = objPoint;
                    newBuilding.transform.rotation = objRotation;
                    RoadMap[i, j] = 2;
                }
            }
        }
        for (int i = 0; i < mapWidth; i+= Random.Range(1, 5))
        {
            for (int j = 0; j < mapHeight; j+= Random.Range(1, 5))
            {
                if (Random.Range(0, 101) < 98 || RoadMap[i, j] == 1)
                {
                    continue;
                }
                Vector3 objPoint = startPoint +
                    new Vector3(StadardRoadCollider.size.x * i,
                    TerrainData.GetHeight(roadPoint[0], roadPoint[1]) + 0.1f,
                    StadardRoadCollider.size.z * j);
                int treeIndex = Random.Range(0, Trees.Length);
                var treeCollider = TreeColliders[treeIndex];
                var objRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                if (!Physics.CheckBox(treeCollider.center + objPoint,
                        treeCollider.size / 2, objRotation, ObstacleMask))
                {
                    var newTree = Instantiate<GameObject>(Trees[treeIndex]);
                    newTree.transform.position = objPoint;
                    newTree.transform.rotation = objRotation;
                    RoadMap[i, j] = 3;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(WorldSize.x, 1, WorldSize.y));
        if (Nodes != null && ShouldDrawObstacleGizmos)
        {
            foreach (var item in Nodes)
            {
                if (item.IsObstacle)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawCube(item.WorldPosition, new Vector3(NodeSize, 1, NodeSize));
                }
            }
        }
    }

    private void Update()
    {
        var angle = RenderSettings.skybox.GetFloat("_Rotation");
        angle += Time.deltaTime * 2;
        if(angle >= 360)
        {
            angle -= 360;
        }
        RenderSettings.skybox.SetFloat("_Rotation", angle);
    }
}
