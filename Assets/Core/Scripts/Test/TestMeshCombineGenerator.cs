using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TestMeshCombineGenerator : MonoBehaviour
{
    public GameObject BlockParent;
    public GameObject ChunkPrefab;
    public GameObject ChunkSkinnedPrefab;
    public GameObject BlockPrefab;
    public string LayerName = "Layer1";
    public Vector2Int Dimensions = Vector2Int.zero;
    public float Offset = 0.43f;
    public bool UseSkinnedMeshCombine = true;
    public bool UseNothing = false;

	void Start()
    {
        List<GameObject> chunk = new List<GameObject>();
		for(int x = 0; x < Dimensions.x; x++)
        {
            for(int z = 0; z < Dimensions.y; z++)
            {
                Vector3 position = new Vector3(transform.position.x + (x * Offset), transform.position.y + Offset, transform.position.z + (z * Offset));
                GameObject block = Instantiate(BlockPrefab, position, Quaternion.identity);
                block.name = BlockPrefab.name;
                block.layer = LayerMask.NameToLayer(LayerName);
                chunk.Add(block);
            }
        }

        if (!UseNothing)
        {
            if(UseSkinnedMeshCombine) CreateChunkSkinned(chunk, true);
            else CreateChunk(chunk, true);
        }
    }

    void CreateChunkSkinned(List<GameObject> pieces, bool CanAddNavMeshRegionTag)
    {
        //Prevents empty chunks
        if (pieces.Count == 0) return;

        GameObject chunk = Instantiate(ChunkSkinnedPrefab, Vector3.zero, Quaternion.identity);
        chunk.name = ChunkSkinnedPrefab.name;
        chunk.transform.SetParent(BlockParent.transform);
        for (int i = 0; i < pieces.Count; i++) pieces[i].transform.SetParent(chunk.transform);

        ChunkMeshCombineSkinned theMeshCombiner = chunk.GetComponent<ChunkMeshCombineSkinned>();
        theMeshCombiner.Materials = pieces[0].GetComponent<MeshRenderer>().sharedMaterials.ToList();
        theMeshCombiner.CombineMeshes(CanAddNavMeshRegionTag);
    }

    void CreateChunk(List<GameObject> pieces, bool CanAddNavMeshRegionTag)
    {
        //Prevents empty chunks
        if (pieces.Count == 0) return;

        GameObject chunk = Instantiate(ChunkPrefab, Vector3.zero, Quaternion.identity);
        chunk.name = ChunkPrefab.name;
        chunk.transform.SetParent(BlockParent.transform);
        for (int i = 0; i < pieces.Count; i++) pieces[i].transform.SetParent(chunk.transform);

        ChunkMeshCombine theMeshCombiner = chunk.GetComponent<ChunkMeshCombine>();
        theMeshCombiner.Materials = pieces[0].GetComponent<MeshRenderer>().sharedMaterials.ToList();
        theMeshCombiner.CombineMeshes(CanAddNavMeshRegionTag);
    }
}
