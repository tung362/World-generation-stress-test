using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWorldGeneration : MonoBehaviour
{
    public enum GenerationMaskTypeEnum { None, Circle, Square }

    /*Settings*/
    [Header("Settings")]
    [SerializeField]
    public GameObject BlockParent;
    [SerializeField]
    public float BlockOffset = 0.43f;
    [SerializeField]
    public int ChunkSize = 16;
    [SerializeField]
    public List<Biome> Biomes = new List<Biome>();
    [SerializeField]
    public string Seed = "Seed";
    [SerializeField]
    [Range(1, 200)]
    public int IslandXZ = 40;
    [SerializeField]
    [Range(1, 10)]
    public int IslandY = 6;
    [SerializeField]
    [Range(0.001f, 100)]
    public float IslandScale = 20;
    [SerializeField]
    [Range(1, 10)]
    public int IslandOctaves = 4;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    public float IslandPersistance = 0.5f;
    [SerializeField]
    [Range(1.0f, 10.0f)]
    public float IslandLacunarity = 1.87f;
    [SerializeField]
    public Vector2 IslandOffset = Vector2.zero;
    [SerializeField]
    public List<BlockLayer> BlockLayers = new List<BlockLayer>();
    [SerializeField]
    public Color UnusedLayer = Color.blue;
    [SerializeField]
    public bool DoubleBaseLayer = true;

    [Header("Gradient Settings")]
    [SerializeField]
    public GenerationMaskTypeEnum PreviewGenerationMask = GenerationMaskTypeEnum.None;
    [SerializeField]
    [Range(-2.0f, 2.0f)]
    public float MaskRadius = 0.75f;

    /*Data*/
    //The data used to determine the terrain's elevation and block placement
    private int[,] CurrentTerrainData;

    void Start ()
    {
        //Generates the terrain data
        NoiseGenerator(ref CurrentTerrainData, BlockLayers, Seed, IslandXZ, IslandScale, IslandOctaves, IslandPersistance, IslandLacunarity, IslandOffset);

        //Generates the terrain
        GenerateTerrain();
    }

    #region Terrain Data Generator
    void NoiseGenerator(ref int[,] MapTerrainData, List<BlockLayer> MapBlockLayers, string MapSeed, int MapXZSize, float MapScale, int MapOctaves, float MapPersistance, float MapLacunarity, Vector2 MapOffset)
    {
        MapTerrainData = new int[MapXZSize, MapXZSize];
        float[,] textureCoord = new float[MapXZSize, MapXZSize];

        //Pick a seeded offset and regular offset
        System.Random rand = new System.Random(MapSeed.GetHashCode());
        Vector2[] mapOctavesOffsets = new Vector2[MapOctaves];
        for (int i = 0; i < MapOctaves; i++)
        {
            float offsetX = rand.Next(-100000, 100000) + MapOffset.x;
            float offsetY = rand.Next(-100000, 100000) + MapOffset.y;
            mapOctavesOffsets[i] = new Vector2(offsetX, offsetY);
        }

        float highestNoiseHeight = float.MinValue;
        float lowestNoiseHeight = float.MaxValue;

        //Calculates scaling into center instead of top right corner
        float half = MapXZSize / 2f;


        //Apply texture coordinate values
        for (int x = 0; x < MapXZSize; x++)
        {
            for (int y = 0; y < MapXZSize; y++)
            {
                float amplitude = 1;
                float frequency = 1;

                //Value
                float noiseHeight = 0;

                //Modify values
                for (int i = 0; i < MapOctaves; i++)
                {
                    float mapX = (x - half) / MapScale * frequency + mapOctavesOffsets[i].x;
                    float mapY = (y - half) / MapScale * frequency + mapOctavesOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(mapX, mapY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= MapPersistance;
                    frequency *= MapLacunarity;
                }

                //Get highest and lowest noise height
                if (noiseHeight > highestNoiseHeight) highestNoiseHeight = noiseHeight;
                else if (noiseHeight < lowestNoiseHeight) lowestNoiseHeight = noiseHeight;

                //Apply value to texture coord
                textureCoord[x, y] = noiseHeight;
            }
        }

        //Normalize texture coordinates and apply to texture
        for (int x = 0; x < MapXZSize; x++)
        {
            for (int y = 0; y < MapXZSize; y++)
            {
                textureCoord[x, y] = Mathf.InverseLerp(lowestNoiseHeight, highestNoiseHeight, textureCoord[x, y]);

                //Gradient
                switch (PreviewGenerationMask)
                {
                    case GenerationMaskTypeEnum.None:
                        break;
                    case GenerationMaskTypeEnum.Circle:
                        textureCoord[x, y] = GradientNoise(x, y, MapXZSize, MaskRadius, textureCoord[x, y], 0);
                        break;
                    case GenerationMaskTypeEnum.Square:
                        textureCoord[x, y] = GradientNoise(x, y, MapXZSize, MaskRadius, textureCoord[x, y], 1);
                        break;
                }

                //Water block as default
                MapTerrainData[x, y] = -1;

                for (int i = 0; i < MapBlockLayers.Count; i++)
                {
                    //Set Elevation
                    if (textureCoord[x, y] >= MapBlockLayers[i].Threshold) MapTerrainData[x, y] = i + 1;
                }
            }
        }
    }

    float GradientNoise(int X, int Y, int Size, float Radius, float AdditiveNoise, int GradientType)
    {
        //Gradient
        float distanceX = Mathf.Abs(X - Size * 0.5f);
        float distanceY = Mathf.Abs(Y - Size * 0.5f);

        //Circular mask
        float distance = Mathf.Sqrt(distanceX * distanceX + distanceY * distanceY);

        //square mask
        if (GradientType == 1) distance = Mathf.Max(distanceX, distanceY);

        float maxWidth = Size * Radius - 10.0f;
        float delta = distance / maxWidth;
        float gradient = delta * delta;

        AdditiveNoise *= Mathf.Max(0.0f, 1.0f - gradient);

        return AdditiveNoise;
    }
    #endregion

    void GenerateTerrain()
    {
        //Chunk layers depending on height of map
        List<List<GameObject>> layerChunks = new List<List<GameObject>>();
        for (int i = 0; i < IslandY; i++) layerChunks.Add(new List<GameObject>());

        for(int x = 0; x < IslandXZ; x++)
        {
            for (int z = 0; z < IslandXZ; z++)
            {
                //Elevation
                for(int y = 0; y < CurrentTerrainData[x, z]; y++)
                {
                    //If water block continue onto the next block
                    if (CurrentTerrainData[x, z] == -1) break;

                    //Block placement Position
                    Vector3 position = new Vector3(transform.position.x + (x * BlockOffset), transform.position.y + ((y + 1) * BlockOffset), transform.position.z + (z * BlockOffset));
                    GameObject block;

                    //Top block
                    if (y == CurrentTerrainData[x, z] - 1)
                    {
                        //Roll
                        int blockRoll = Random.Range((int)0, Biomes[0].TopBlocks.Length);
                        //Roll chance of success
                        int success = Random.Range((int)0, Biomes[0].TopBlocks[blockRoll].OneOutOf);

                        if (success == 0)
                        {
                            block = Instantiate(Biomes[0].TopBlocks[blockRoll].Block, position, Quaternion.identity);
                            block.name = Biomes[0].TopBlocks[blockRoll].Block.name;
                        }
                        else
                        {
                            block = Instantiate(Biomes[0].TopBlocks[0].Block, position, Quaternion.identity);
                            block.name = Biomes[0].TopBlocks[0].Block.name;
                        }

                    }
                    //BottomBlock
                    else
                    {
                        //Roll
                        int blockRoll = Random.Range((int)0, Biomes[0].BottomBlocks.Length);
                        //Roll chance of success
                        int success = Random.Range((int)0, Biomes[0].BottomBlocks[blockRoll].OneOutOf);

                        if (success == 0)
                        {
                            block = Instantiate(Biomes[0].BottomBlocks[blockRoll].Block, position, Quaternion.identity);
                            block.name = Biomes[0].BottomBlocks[blockRoll].Block.name;
                        }
                        else
                        {
                            block = Instantiate(Biomes[0].BottomBlocks[0].Block, position, Quaternion.identity);
                            block.name = Biomes[0].BottomBlocks[0].Block.name;
                        }
                    }

                    //Add to chunk
                    layerChunks[y].Add(block);
                    if (layerChunks[y].Count >= ChunkSize)
                    {
                        CreateChunk(layerChunks[y]);
                        layerChunks[y].Clear();
                    }

                    //Place another block below this block if on the base layer
                    if (y == 0 && DoubleBaseLayer)
                    {
                        position = new Vector3(transform.position.x + (x * BlockOffset), transform.position.y + (y * BlockOffset), transform.position.z + (z * BlockOffset));
                        block = Instantiate(Biomes[0].BottomBlocks[0].Block, position, Quaternion.identity);
                        block.name = Biomes[0].BottomBlocks[0].Block.name;

                        //Add to chunk
                        layerChunks[y].Add(block);
                        if (layerChunks[y].Count >= ChunkSize)
                        {
                            CreateChunk(layerChunks[y]);
                            layerChunks[y].Clear();
                        }
                    }
                }
            }
        }

        //Create chunk for any remainder blocks
        for (int i = 0; i < layerChunks.Count; i++) CreateChunk(layerChunks[i]);
    }

    void CreateChunk(List<GameObject> pieces)
    {
        GameObject chunk = Instantiate(Biomes[0].Chunk, Vector3.zero, Quaternion.identity);
        chunk.transform.SetParent(BlockParent.transform);

        for (int i = 0; i < pieces.Count; i++) pieces[i].transform.SetParent(chunk.transform);
        chunk.GetComponent<TestMeshCombine>().CombineMeshes();
    }
}
