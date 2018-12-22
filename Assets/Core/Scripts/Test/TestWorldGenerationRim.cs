using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TestWorldGenerationRim : MonoBehaviour
{
    public enum GenerationMaskTypeEnum { None, Circle, Square, SharpSquare }
    public enum BlockTypeEnum { TopBlock, BottomBlock, ToplessBlock }

    /*Settings*/
    [Header("Settings")]
    [SerializeField]
    public GameObject BlockParent;
    [SerializeField]
    public float BlockOffset = 0.43f;
    [SerializeField]
    public float BlockOffsetY = 0.1f;
    [SerializeField]
    public int ChunkSize = 16;
    [SerializeField]
    public int SubChunkSize = 16;
    [SerializeField]
    List<string> LayerNames = new List<string>();
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
                    case GenerationMaskTypeEnum.SharpSquare:
                        textureCoord[x, y] = GradientNoise(x, y, MapXZSize, MaskRadius, textureCoord[x, y], 2);
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
        //Smooth circle and square
        if(GradientType == 0 || GradientType == 1)
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
        }
        return AdditiveNoise;
    }
    #endregion

    void GenerateTerrain()
    {
        //The data used to determine the terrain chunks used to control occlusion and mesh combine
        int maxChunkSize = Mathf.CeilToInt((float)IslandXZ / (float)ChunkSize);
        TerrainChunk[,] chunkDatas = new TerrainChunk[maxChunkSize, maxChunkSize];

        //Prepare the chunk data (chunk->layer->block type)
        for (int x = 0; x < maxChunkSize; x++)
        {
            for (int z = 0; z < maxChunkSize; z++)
            {
                chunkDatas[x, z] = new TerrainChunk();

                //Each layer
                for (int layerID = 0; layerID < LayerNames.Count; layerID++)
                {

                    //Add list of layers
                    chunkDatas[x, z].TopChunks.Add(new List<List<GameObject>>());
                    chunkDatas[x, z].BottomChunks.Add(new List<List<GameObject>>());
                    chunkDatas[x, z].ToplessChunks.Add(new List<List<GameObject>>());
                    chunkDatas[x, z].PlantChunks.Add(new List<List<GameObject>>());

                    //Each block type
                    for (int i = 0; i < Biomes[0].TopBlocks.Length; i++) chunkDatas[x, z].TopChunks[layerID].Add(new List<GameObject>());
                    for (int i = 0; i < Biomes[0].BottomBlocks.Length; i++) chunkDatas[x, z].BottomChunks[layerID].Add(new List<GameObject>());
                    for (int i = 0; i < Biomes[0].ToplessBlocks.Length; i++) chunkDatas[x, z].ToplessChunks[layerID].Add(new List<GameObject>());
                    for (int i = 0; i < Biomes[0].Plants.Length; i++) chunkDatas[x, z].PlantChunks[layerID].Add(new List<GameObject>());
                }
            }
        }

        //The data used to determine the terrain's tree and plant placement
        bool[,] treeData = new bool[IslandXZ, IslandXZ];

        //Generation
        for (int x = 0; x < IslandXZ; x++)
        {
            for (int z = 0; z < IslandXZ; z++)
            {
                int chunkX = Mathf.FloorToInt((float)x / (float)ChunkSize);
                int chunkZ = Mathf.FloorToInt((float)z / (float)ChunkSize);

                //Elevation
                for (int y = 0; y < CurrentTerrainData[x, z]; y++)
                {
                    //If water block continue onto the next block
                    if (CurrentTerrainData[x, z] == -1) break;

                    //Tree placement verification
                    bool isValidTreePlacement = true;
                    if (y == 0 || y == 1 || y == 3 || y == 4 || y == 6)
                    {
                        //3x3 placement verification
                        for (int i = -1; i < 2; i++)
                        {
                            for (int j = -1; j < 2; j++)
                            {
                                //Don't check the center
                                if (i == 0 && j == 0) continue;

                                if ((x + i < IslandXZ && x + i > -1) && (z + j < IslandXZ && z + j > -1))
                                {
                                    //check to make sure the point is on flat ground and there isnt already a tree nearby
                                    if (CurrentTerrainData[x + i, z + j] - 1 > y || treeData[x + i, z + j])
                                    {
                                        isValidTreePlacement = false;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else isValidTreePlacement = false;

                    //Block placement Position
                    float middleMap = IslandXZ * 0.5f;
                    Vector3 position = new Vector3(transform.position.x + ((x - middleMap) * BlockOffset), transform.position.y + ((y + 1) * BlockOffsetY), transform.position.z + ((z - middleMap) * BlockOffset));

                    //Block info
                    GameObject block = null;
                    BlockTypeEnum blockType = BlockTypeEnum.TopBlock;
                    int BlockLayerID = 0;
                    int blockID = 0;

                    //Tree info
                    GameObject tree = null;
                    int treeID = 0;

                    //Plant info
                    GameObject plant = null;
                    int plantID = 0;

                    //Base layers
                    if (y == 0 || y == 3 || y == 6)
                    {
                        if(y == CurrentTerrainData[x, z] - 1)
                        {
                            //Roll
                            int blockRoll = Random.Range((int)0, Biomes[0].TopBlocks.Length);
                            //Roll chance of success
                            int success = Random.Range((int)0, Biomes[0].TopBlocks[blockRoll].OneOutOf);

                            //Top Block
                            if (success == 0)
                            {
                                block = Instantiate(Biomes[0].TopBlocks[blockRoll].Block, position, Quaternion.identity);
                                block.name = Biomes[0].TopBlocks[blockRoll].Block.name;
                                blockID = blockRoll;
                            }
                            else
                            {
                                block = Instantiate(Biomes[0].TopBlocks[0].Block, position, Quaternion.identity);
                                block.name = Biomes[0].TopBlocks[0].Block.name;
                            }
                            blockType = BlockTypeEnum.TopBlock;

                            //Tree placement
                            if(isValidTreePlacement)
                            {
                                //Roll
                                int treeRoll = Random.Range((int)0, Biomes[0].Trees.Length);
                                //Roll chance of success
                                int treeSuccess = Random.Range((int)0, Biomes[0].Trees[treeRoll].OneOutOf);

                                //Tree Block
                                if (treeSuccess == 0)
                                {
                                    tree = Instantiate(Biomes[0].Trees[treeRoll].Block, new Vector3(position.x, position.y + BlockOffsetY, position.z), Quaternion.identity);
                                    tree.transform.eulerAngles = new Vector3(0, Random.Range(0.0f, 360.0f), 0);
                                    tree.name = Biomes[0].Trees[treeRoll].Block.name;
                                    treeID = treeRoll;
                                    treeData[x, z] = true;
                                }
                            }

                            //Plant placement
                            if(!treeData[x, z])
                            {
                                //Roll
                                int plantRoll = Random.Range((int)0, Biomes[0].Plants.Length);
                                //Roll chance of success
                                int plantSuccess = Random.Range((int)0, Biomes[0].Plants[plantRoll].OneOutOf);

                                //Tree Block
                                if (plantSuccess == 0)
                                {
                                    plant = Instantiate(Biomes[0].Plants[plantRoll].Block, new Vector3(position.x, position.y + BlockOffsetY, position.z), Quaternion.identity);
                                    plant.name = Biomes[0].Plants[plantRoll].Block.name;
                                    plantID = plantRoll;
                                }
                            }
                        }
                        else
                        {
                            //Roll
                            int blockRoll = Random.Range((int)0, Biomes[0].BottomBlocks.Length);
                            //Roll chance of success
                            int success = Random.Range((int)0, Biomes[0].BottomBlocks[blockRoll].OneOutOf);

                            //Bottom Block
                            if (success == 0)
                            {
                                block = Instantiate(Biomes[0].BottomBlocks[blockRoll].Block, position, Quaternion.identity);
                                block.name = Biomes[0].BottomBlocks[blockRoll].Block.name;
                                blockID = blockRoll;
                            }
                            else
                            {
                                block = Instantiate(Biomes[0].BottomBlocks[0].Block, position, Quaternion.identity);
                                block.name = Biomes[0].BottomBlocks[0].Block.name;
                            }
                            blockType = BlockTypeEnum.BottomBlock;
                        }
                    }
                    //Any non base layer with a top block
                    else if (y == CurrentTerrainData[x, z] - 1)
                    {
                        //Roll
                        int blockRoll = Random.Range((int)0, Biomes[0].TopBlocks.Length);
                        //Roll chance of success
                        int success = Random.Range((int)0, Biomes[0].TopBlocks[blockRoll].OneOutOf);

                        //Top Block
                        if (success == 0)
                        {
                            block = Instantiate(Biomes[0].TopBlocks[blockRoll].Block, position, Quaternion.identity);
                            block.name = Biomes[0].TopBlocks[blockRoll].Block.name;
                            blockID = blockRoll;
                        }
                        else
                        {
                            block = Instantiate(Biomes[0].TopBlocks[0].Block, position, Quaternion.identity);
                            block.name = Biomes[0].TopBlocks[0].Block.name;
                        }
                        blockType = BlockTypeEnum.TopBlock;

                        //Tree placement
                        if (isValidTreePlacement)
                        {
                            //Roll
                            int treeRoll = Random.Range((int)0, Biomes[0].Trees.Length);
                            //Roll chance of success
                            int treeSuccess = Random.Range((int)0, Biomes[0].Trees[treeRoll].OneOutOf);

                            //Tree Block
                            if (treeSuccess == 0)
                            {
                                tree = Instantiate(Biomes[0].Trees[treeRoll].Block, new Vector3(position.x, position.y + BlockOffsetY, position.z), Quaternion.identity);
                                tree.transform.eulerAngles = new Vector3(0, Random.Range(0.0f, 360.0f), 0);
                                tree.name = Biomes[0].Trees[treeRoll].Block.name;
                                treeID = treeRoll;
                                treeData[x, z] = true;
                            }
                        }

                        //Plant placement
                        if (!treeData[x, z])
                        {
                            //Roll
                            int plantRoll = Random.Range((int)0, Biomes[0].Plants.Length);
                            //Roll chance of success
                            int plantSuccess = Random.Range((int)0, Biomes[0].Plants[plantRoll].OneOutOf);

                            //Tree Block
                            if (plantSuccess == 0)
                            {
                                plant = Instantiate(Biomes[0].Plants[plantRoll].Block, new Vector3(position.x, position.y + BlockOffsetY, position.z), Quaternion.identity);
                                plant.name = Biomes[0].Plants[plantRoll].Block.name;
                                plantID = plantRoll;
                            }
                        }
                    }
                    //Any other layer non base layer
                    else
                    {
                        //Roll
                        int blockRoll = Random.Range((int)0, Biomes[0].ToplessBlocks.Length);
                        //Roll chance of success
                        int success = Random.Range((int)0, Biomes[0].ToplessBlocks[blockRoll].OneOutOf);

                        //Topless Block
                        if (success == 0)
                        {
                            block = Instantiate(Biomes[0].ToplessBlocks[blockRoll].Block, position, Quaternion.identity);
                            block.name = Biomes[0].ToplessBlocks[blockRoll].Block.name;
                            blockID = blockRoll;
                        }
                        else
                        {
                            block = Instantiate(Biomes[0].ToplessBlocks[0].Block, position, Quaternion.identity);
                            block.name = Biomes[0].ToplessBlocks[0].Block.name;
                        }
                        blockType = BlockTypeEnum.ToplessBlock;
                    }

                    //Assign base layers
                    if (y < 3) BlockLayerID = 0;
                    else if (y >= 3 && y < 6) BlockLayerID = 1;
                    else if (y >= 6) BlockLayerID = 2;
                    block.layer = LayerMask.NameToLayer(LayerNames[BlockLayerID]);

                    //Add to chunk
                    switch (blockType)
                    {
                        case BlockTypeEnum.TopBlock:
                            ChunkSizeCheck(block, ref chunkDatas[chunkX, chunkZ].TopChunks, BlockLayerID, blockID, true);
                            break;
                        case BlockTypeEnum.BottomBlock:
                            ChunkSizeCheck(block, ref chunkDatas[chunkX, chunkZ].BottomChunks, BlockLayerID, blockID, true);
                            break;
                        case BlockTypeEnum.ToplessBlock:
                            ChunkSizeCheck(block, ref chunkDatas[chunkX, chunkZ].ToplessChunks, BlockLayerID, blockID, false);
                            break;
                    }
                    if (tree != null)
                    {
                        tree.layer = LayerMask.NameToLayer(LayerNames[BlockLayerID]);
                    }
                    if (plant != null)
                    {
                        plant.layer = LayerMask.NameToLayer(LayerNames[BlockLayerID]);
                        ChunkSizeCheck(plant, ref chunkDatas[chunkX, chunkZ].PlantChunks, BlockLayerID, plantID, false);
                    }
                }
            }
        }

        //Create chunk for any remainder blocks
        for (int x = 0; x < maxChunkSize; x++)
        {
            for (int z = 0; z < maxChunkSize; z++)
            {
                for (int layerID = 0; layerID < LayerNames.Count; layerID++)
                {
                    for (int i = 0; i < chunkDatas[x, z].TopChunks[layerID].Count; i++) CreateChunk(chunkDatas[x, z].TopChunks[layerID][i], true);
                    for (int i = 0; i < chunkDatas[x, z].BottomChunks[layerID].Count; i++) CreateChunk(chunkDatas[x, z].BottomChunks[layerID][i], true);
                    for (int i = 0; i < chunkDatas[x, z].ToplessChunks[layerID].Count; i++) CreateChunk(chunkDatas[x, z].ToplessChunks[layerID][i], false);
                    for (int i = 0; i < chunkDatas[x, z].PlantChunks[layerID].Count; i++) CreateChunk(chunkDatas[x, z].PlantChunks[layerID][i], false);
                }
            }
        }
    }

    void ChunkSizeCheck(GameObject Block, ref List<List<List<GameObject>>> ChunkList, int BlockLayerID, int BlockID, bool CanAddNavMeshRegionTag)
    {
        ChunkList[BlockLayerID][BlockID].Add(Block);
        if (ChunkList[BlockLayerID][BlockID].Count >= SubChunkSize)
        {
            CreateChunk(ChunkList[BlockLayerID][BlockID], CanAddNavMeshRegionTag);
            ChunkList[BlockLayerID][BlockID].Clear();
        }
    }

    void CreateChunk(List<GameObject> pieces, bool CanAddNavMeshRegionTag)
    {
        //Prevents empty chunks
        if (pieces.Count == 0) return;

        GameObject chunk = Instantiate(Biomes[0].Chunk, Vector3.zero, Quaternion.identity);
        chunk.name = Biomes[0].Chunk.name;
        chunk.transform.SetParent(BlockParent.transform);

        for (int i = 0; i < pieces.Count; i++) pieces[i].transform.SetParent(chunk.transform);
        ChunkMeshCombine theMeshCombiner = chunk.GetComponent<ChunkMeshCombine>();
        theMeshCombiner.Materials = pieces[0].GetComponent<MeshRenderer>().sharedMaterials.ToList();
        theMeshCombiner.CombineMeshes(CanAddNavMeshRegionTag);
    }
}
