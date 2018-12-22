using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class Comments : MonoBehaviour
//{
//}

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class ChunkMeshCombine : MonoBehaviour
//{
//    //Notes: Could be improved in the future by bypassing final combine if theres only 1 material (Create 2 functions with a material count check in main function)

//    /*Settings*/
//    public GameObject CombinedMeshPrefab;

//    [HideInInspector]
//    public List<Material> Materials = new List<Material>();

//    /*Data*/
//    private List<GameObject> AllChunks = new List<GameObject>();

//    public void CombineMeshes(bool AddNavMeshRegionTag)
//    {
//        //Clears existing chunks if there is any
//        for (int i = 0; i < AllChunks.Count; i++)
//        {
//            Destroy(AllChunks[i]);
//            AllChunks.RemoveAt(i);
//            i -= 1;
//        }

//        //Get all child mesh filters
//        MeshFilter[] childFilters = Tools.FindAllMeshFilters(transform).ToArray();

//        //Get layer mask to use
//        LayerMask layer;
//        if (childFilters.Length != 0) layer = childFilters[0].gameObject.layer;
//        else return;

//        //Disabled renderer
//        for (int i = 0; i < childFilters.Length; i++) childFilters[i].GetComponent<MeshRenderer>().enabled = false;

//        //All Combined meshes
//        List<Mesh> subMesh = new List<Mesh>();

//        /*First Combine*/
//        //Combine each material id
//        for (int materialID = 0; materialID < Materials.Count; materialID++)
//        {
//            //Combines of a single material
//            List<CombineInstance> combiners = new List<CombineInstance>();

//            for (int childID = 0; childID < childFilters.Length; childID++)
//            {
//                //Combine data
//                CombineInstance combine = new CombineInstance();
//                combine.mesh = childFilters[childID].mesh;
//                combine.subMeshIndex = materialID;
//                combine.transform = childFilters[childID].transform.localToWorldMatrix;
//                combiners.Add(combine);
//            }

//            //Export to mesh with materials intact
//            int currentVerticeCount = 0;
//            List<CombineInstance> tempCombiners = new List<CombineInstance>();
//            for (int combineID = 0; combineID < combiners.Count; combineID++)
//            {
//                //Check if under vertice limit
//                if (currentVerticeCount + combiners[combineID].mesh.vertices.Length < 65534)
//                {
//                    currentVerticeCount += combiners[combineID].mesh.vertices.Length;
//                    tempCombiners.Add(combiners[combineID]);
//                }
//                else
//                {
//                    //Combine to mesh
//                    Mesh splitedMesh = new Mesh();
//                    splitedMesh.CombineMeshes(tempCombiners.ToArray(), true);
//                    subMesh.Add(splitedMesh);

//                    //Reset
//                    currentVerticeCount = 0;
//                    tempCombiners.Clear();
//                    combineID -= 1;
//                }
//            }

//            //Combine any remaining to mesh
//            Mesh mesh = new Mesh();
//            mesh.CombineMeshes(tempCombiners.ToArray(), true);
//            subMesh.Add(mesh);
//        }

//        /*Final Combine*/
//        //Combines of a multiple materials
//        List<CombineInstance> finalCombiners = new List<CombineInstance>();
//        for (int subMeshID = 0; subMeshID < subMesh.Count; subMeshID++)
//        {
//            //Combine data
//            CombineInstance combine = new CombineInstance();
//            combine.mesh = subMesh[subMeshID];
//            combine.subMeshIndex = 0;
//            combine.transform = transform.worldToLocalMatrix;
//            finalCombiners.Add(combine);
//        }

//        //Export to chunk with materials intact
//        int finalCurrentVerticeCount = 0;
//        List<CombineInstance> finalTempCombiners = new List<CombineInstance>();
//        for (int combineID = 0; combineID < finalCombiners.Count; combineID++)
//        {
//            //Check if under vertice limit
//            if (finalCurrentVerticeCount + finalCombiners[combineID].mesh.vertices.Length < 65534)
//            {
//                finalCurrentVerticeCount += finalCombiners[combineID].mesh.vertices.Length;
//                finalTempCombiners.Add(finalCombiners[combineID]);
//            }
//            else
//            {
//                //Chunk
//                GameObject chunk = Instantiate(CombinedMeshPrefab, Vector3.zero, Quaternion.identity, transform.root);
//                chunk.name = CombinedMeshPrefab.name;
//                chunk.layer = layer;
//                if (AddNavMeshRegionTag) chunk.AddComponent<NavMeshRegionTag>();

//                //Combine to mesh
//                Mesh splitedMesh = new Mesh();
//                splitedMesh.CombineMeshes(finalTempCombiners.ToArray(), false);
//                chunk.GetComponent<MeshFilter>().mesh = splitedMesh;
//                chunk.GetComponent<MeshRenderer>().materials = Materials.ToArray();
//                AllChunks.Add(chunk);

//                //Reset
//                finalCurrentVerticeCount = 0;
//                finalTempCombiners.Clear();
//                combineID -= 1;
//            }
//        }

//        //Combine any remaining to chunk
//        GameObject finalChunk = Instantiate(CombinedMeshPrefab, Vector3.zero, Quaternion.identity, transform.root);
//        finalChunk.name = CombinedMeshPrefab.name;
//        finalChunk.layer = layer;
//        if (AddNavMeshRegionTag) finalChunk.AddComponent<NavMeshRegionTag>();

//        //Combine to mesh
//        Mesh finalSplitedMesh = new Mesh();
//        finalSplitedMesh.CombineMeshes(finalTempCombiners.ToArray(), false);
//        finalChunk.GetComponent<MeshFilter>().mesh = finalSplitedMesh;
//        finalChunk.GetComponent<MeshRenderer>().materials = Materials.ToArray();
//        AllChunks.Add(finalChunk);
//    }
//}
