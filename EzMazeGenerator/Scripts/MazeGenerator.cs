using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [Header("Maze Settings")]
    public Vector3Int mazeScale;
    public float wallLength = 2;
    [Range(0f, 100f), Tooltip("Percentage chance for a wall to spawn (0-100)")]
    public int wallSpawnPercentage = 70;
    [Range(0f, 100f), Tooltip("Percentage chance for a structure to spawn (0-100)")]
    public int structureSpawnPercentage = 30;

    [Space(3)]
    [Header("Objects")]
    public List<GameObject> structures = new List<GameObject>();
    public GameObject lightPrefab;

    [Space(3)]
    [Header("Object Materials")]
    public Material wallMaterial;
    public Material groundMaterial;
    public Material roofMaterial;
    
    [Space(3)]
    [Header("Light Settings")]
    [Tooltip("It is recommended to set to point or area")]
    public LightType lightType = LightType.Rectangle;
    public float intensity = 3;
    public float range = 5;
    public Color color = Color.white;
    [Space(3)]
    [Header("Reflection Probe Settings")]
    public bool spawnReflectionProbes;
    public int probeResolution = 1024;
    public bool boxProjection = true;

    private int[,] maze;
    private List<Vector2Int> visitedCells = new List<Vector2Int>();
    private List<GameObject> generatedMesh = new List<GameObject>();
    private GameObject lightsObject;

    public async Task GenerateMaze()
    {
        ClearMaze();

        if (!IsFloatRound(wallLength))
        {
            Debug.LogWarning("It is not recommended to have the Wall Length to a non-round value, please make it round to prevent errors!");
        }

        GameObject mazeObject = new GameObject("Generated Maze");
        mazeObject.transform.SetParent(transform);

        GameObject wallsObject = new GameObject("Walls");
        wallsObject.transform.SetParent(mazeObject.transform);

        GameObject structureObject = new GameObject("Structures");
        structureObject.transform.SetParent(mazeObject.transform);

        GameObject groundObject = new GameObject("Ground");
        groundObject.transform.SetParent(mazeObject.transform);

        GameObject roofObject = new GameObject("Roof");
        roofObject.transform.SetParent(mazeObject.transform);

        GameObject outerWallsObject = new GameObject("Outer Walls");
        outerWallsObject.transform.SetParent(mazeObject.transform);

        lightsObject = new GameObject("Lights");
        lightsObject.transform.SetParent(mazeObject.transform);

        GameObject reflectionProbeObject = new GameObject("Reflection Probes");
        reflectionProbeObject.transform.SetParent(mazeObject.transform);

        maze = new int[mazeScale.x, mazeScale.z];
        for (int i = 0; i < mazeScale.x; i++)
        {
            for (int j = 0; j < mazeScale.z; j++)
            {
                maze[i, j] = 1;
            }
        }

        await Task.Yield();

        await RecursiveBacktracking(new Vector2Int(0, 0));
        await SpawnGround(groundObject);
        await SpawnRoof(roofObject);
        await SpawnOuterWalls(outerWallsObject);
        await SpawnObstacles(wallsObject, structureObject);

        if (spawnReflectionProbes)
            await SpawnReflectionProbes(reflectionProbeObject);
        else
            DestroyImmediate(reflectionProbeObject);
    }

    public void ClearMaze()
    {
        DestroyImmediate(GameObject.Find($"{gameObject.name}/Generated Maze"));
        generatedMesh.Clear();
    }

    public void MakeMeshStatic()
    {
        foreach (var mesh in generatedMesh)
        {
            mesh.isStatic = true;
        }
    }

    public void MakeMeshInvisible()
    {
        foreach (var mesh in generatedMesh)
        {
            MeshRenderer mr = mesh.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                mr.enabled = false;
            }
        }
    }

    public void MakeMeshVisible()
    {
        foreach (var mesh in generatedMesh)
        {
            MeshRenderer mr = mesh.GetComponentInParent<MeshRenderer>();
            if (mr != null)
            {
                mr.enabled = true;
            }
        }
    }

    async Task RecursiveBacktracking(Vector2Int cell)
    {
        visitedCells.Add(cell);
        List<Vector2Int> neighbors = GetUnvisitedNeighbors(cell);
        while (neighbors.Count > 0)
        {
            int randomIndex = Random.Range(0, neighbors.Count);
            Vector2Int nextCell = neighbors[randomIndex];
            maze[nextCell.x, nextCell.y] = 0;
            maze[(cell.x + nextCell.x) / 2, (cell.y + nextCell.y) / 2] = 0;
            await RecursiveBacktracking(nextCell);
            neighbors = GetUnvisitedNeighbors(cell);
            await Task.Yield();
        }
    }

    List<Vector2Int> GetUnvisitedNeighbors(Vector2Int cell)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        int step = 2;
        int minX = step;
        int maxX = mazeScale.x - step;
        int minY = step;
        int maxY = mazeScale.z - step;

        switch (cell.x)
        {
            case int x when x > minX && !visitedCells.Contains(new Vector2Int(x - step, cell.y)):
                neighbors.Add(new Vector2Int(x - step, cell.y));
                break;
            case int x when x < maxX && !visitedCells.Contains(new Vector2Int(x + step, cell.y)):
                neighbors.Add(new Vector2Int(x + step, cell.y));
                break;
        }

        switch (cell.y)
        {
            case int y when y > minY && !visitedCells.Contains(new Vector2Int(cell.x, y - step)):
                neighbors.Add(new Vector2Int(cell.x, y - step));
                break;
            case int y when y < maxY && !visitedCells.Contains(new Vector2Int(cell.x, y + step)):
                neighbors.Add(new Vector2Int(cell.x, y + step));
                break;
        }

        return neighbors;
    }

    async Task SpawnObstacles(GameObject wallParent, GameObject structureParent)
    {
        for (int i = 0; i < mazeScale.x; i++)
        {
            for (int j = 0; j < mazeScale.z; j++)
            {
                if (maze[i, j] == 1)
                {
                    int randomValue = Random.Range(0, 100);

                    if (randomValue < wallSpawnPercentage)
                    {
                        Vector3 position = new Vector3(i * wallLength, 0, j * wallLength);
                        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        wall.transform.position = position;
                        wall.transform.localScale = new Vector3(0.2f, mazeScale.y, wallLength);
                        wall.transform.Rotate(Vector3.up, Random.Range(0, 4) * 90);
                        wall.transform.SetParent(wallParent.transform);
                        SetupMat(wall, wallMaterial);
                        generatedMesh.Add(wall);

                        Vector3 lightPosition = new Vector3((i + 0.5f) * wallLength, mazeScale.y * 0.5f, (j + 0.5f) * wallLength);
                        GameObject lightMesh = Instantiate(lightPrefab, lightPosition, lightPrefab.transform.rotation);
                        lightMesh.transform.SetParent(lightsObject.transform);
                        Light light = lightMesh.GetComponentInChildren<Light>();
                        light.type = lightType;
                        light.intensity = intensity;
                        light.range = range;
                        light.color = color;
                        generatedMesh.Add(lightMesh);
                    }
                    else if (randomValue < wallSpawnPercentage + structureSpawnPercentage)
                    {
                        Vector3 position = new Vector3(i * 2, 0, j * 2);
                        int randomStructure = Random.Range(0, structures.Count);
                        GameObject structure = Instantiate(structures[randomStructure], position, Quaternion.identity);
                        structure.transform.SetParent(structureParent.transform);
                        structure.transform.localScale = new Vector3(structures[randomStructure].transform.localScale.x, mazeScale.y, structures[randomStructure].transform.localScale.z);
                        structure.transform.Rotate(Vector3.up, Random.Range(0, 4) * 90);
                        generatedMesh.Add(structure);

                        int childCount = structure.transform.childCount;
                        for (int index = 0; index < childCount; index++)
                        {
                            GameObject childObject = structure.transform.GetChild(index).gameObject;
                            generatedMesh.Add(childObject);
                        }

                        Vector3 lightPosition = new Vector3((i + 0.5f) * wallLength, mazeScale.y * 0.5f, (j + 0.5f) * wallLength);
                        GameObject lightMesh = Instantiate(lightPrefab, lightPosition, lightPrefab.transform.rotation);
                        lightMesh.transform.SetParent(lightsObject.transform);
                        Light light = lightMesh.GetComponentInChildren<Light>();
                        light.type = lightType;
                        light.intensity = intensity;
                        light.range = range;
                        light.color = color;
                        generatedMesh.Add(lightMesh);
                    }
                }
            }
            await Task.Yield();
        }
    }

    async Task SpawnGround(GameObject parent)
    {
        float groundWidth = mazeScale.x * wallLength;
        float groundHeight = mazeScale.z * wallLength;
        Vector3 groundPosition = new Vector3(groundWidth / 2f - wallLength / 2f, -mazeScale.y / 2f, groundHeight / 2f - wallLength / 2f);

        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.transform.position = groundPosition;
        ground.transform.SetParent(parent.transform);
        ground.name = "Ground";
        ground.transform.localScale = new Vector3(groundWidth * 0.1f, 0.1f, groundHeight * 0.1f);
        SetupMat(ground, groundMaterial);
        SetAdjustTilling(groundMaterial, new Vector2Int(mazeScale.x, mazeScale.z));
        generatedMesh.Add(ground);
        await Task.Yield();
    }

    async Task SpawnRoof(GameObject parent)
    {
        float roofWidth = mazeScale.x * wallLength;
        float roofDepth = mazeScale.z * wallLength;
        Vector3 roofPosition = new Vector3(roofWidth / 2f - wallLength / 2f, mazeScale.y / 2f, roofDepth / 2f - wallLength / 2f);

        GameObject roof = GameObject.CreatePrimitive(PrimitiveType.Plane);
        roof.transform.position = roofPosition;
        roof.transform.Rotate(Vector3.forward, 180);
        roof.transform.SetParent(parent.transform);
        roof.name = "Roof";
        roof.transform.localScale = new Vector3(roofWidth * 0.1f, 0.1f, roofDepth * 0.1f);
        SetupMat(roof, roofMaterial);
        SetAdjustTilling(roofMaterial, new Vector2Int(mazeScale.x, mazeScale.z));
        generatedMesh.Add(roof);
        await Task.Yield();
    }

    async Task SpawnOuterWalls(GameObject parent)
    {
        for (int i = 0; i <= mazeScale.x; i++)
        {
            Vector3 posPositiveWidth = new Vector3(i * wallLength, 0f, 0f) + Vector3.left + Vector3.back;
            GameObject outerWall1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            outerWall1.name = "Outer Wall";
            outerWall1.transform.position = posPositiveWidth;
            outerWall1.transform.localScale = new Vector3(0.2f, mazeScale.y, wallLength);
            outerWall1.transform.Rotate(Vector3.up, 90);
            outerWall1.transform.parent = parent.transform;
            SetupMat(outerWall1, wallMaterial);
            generatedMesh.Add(outerWall1);

            Vector3 posNegativeWidth = new Vector3(i * wallLength, 0f, mazeScale.z * wallLength) + Vector3.left + Vector3.back;
            GameObject outerWall2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            outerWall2.name = "Outer Wall";
            outerWall2.transform.position = posNegativeWidth;
            outerWall2.transform.localScale = new Vector3(0.2f, mazeScale.y, wallLength);
            outerWall2.transform.Rotate(Vector3.up, 90);
            outerWall2.transform.parent = parent.transform;
            SetupMat(outerWall2, wallMaterial);
            generatedMesh.Add(outerWall2);
            await Task.Yield();
        }

        for (int i = 0; i <= mazeScale.z; i++)
        {
            Vector3 posPositiveHeight = new Vector3(0f, 0f, i * wallLength) + Vector3.left + Vector3.back;
            GameObject outerWall3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            outerWall3.name = "Outer Wall";
            outerWall3.transform.position = posPositiveHeight;
            outerWall3.transform.localScale = new Vector3(0.2f, mazeScale.y, wallLength);
            outerWall3.transform.parent = parent.transform;
            SetupMat(outerWall3, wallMaterial);
            generatedMesh.Add(outerWall3);

            Vector3 posNegativeHeight = new Vector3(mazeScale.x * wallLength, 0f, i * wallLength) + Vector3.left + Vector3.back;
            GameObject outerWall4 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            outerWall4.name = "Outer Wall";
            outerWall4.transform.position = posNegativeHeight;
            outerWall4.transform.localScale = new Vector3(0.2f, mazeScale.y, wallLength);
            outerWall4.transform.parent = parent.transform;
            SetupMat(outerWall4, wallMaterial);
            generatedMesh.Add(outerWall4);
            await Task.Yield();
        }
    }

    async Task SpawnReflectionProbes(GameObject parent)
    {
        int reflectionProbeCount = 5;
        for (int i = 0; i < reflectionProbeCount; i++)
        {
            for (int j = 0; j < reflectionProbeCount; j++)
            {
                Vector3 position = new Vector3(i * reflectionProbeCount, mazeScale.y / 4, j * reflectionProbeCount);
                GameObject spawnedreflectionProbe = new GameObject("ReflectionProbe");
                spawnedreflectionProbe.transform.position = position;
                spawnedreflectionProbe.transform.SetParent(parent.transform);
                ReflectionProbe reflectionProbe = spawnedreflectionProbe.AddComponent<ReflectionProbe>();
                reflectionProbe.resolution = probeResolution;
                reflectionProbe.size = new Vector3(2.1f, 2.1f, 2.1f);
                reflectionProbe.boxProjection = boxProjection;
                await Task.Yield();
            }
        }
    }

    // some of these are kinda unneeded since i can jus copy paste dat shit more then once ig but im lazy
    public bool IsFloatRound(float value, float epsilon = 0.0001f)
    {
        return Mathf.Abs(value - Mathf.Round(value)) < epsilon;
    }

    void SetAdjustTilling(Material material, Vector2Int size)
    {
        material.SetTextureScale("_MainTex", size);
    }

    void SetupMat(GameObject gameObject, Material material)
    {
        gameObject.GetComponent<MeshRenderer>().sharedMaterial = material;
    }
}
