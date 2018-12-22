using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMeshCombine : MonoBehaviour
{
    /*Settings*/
    public List<Material> materials = new List<Material>();

    /*Data*/
    private List<GameObject> AllChunks = new List<GameObject>();

    public void CombineMeshes()
    {
        //Clears existing chunks if there is any
        for (int i = 0; i < AllChunks.Count; i++)
        {
            Destroy(AllChunks[i]);
            AllChunks.RemoveAt(i);
            i -= 1;
        }

        //Get all child mesh filters
        MeshFilter[] childFilters = Tools.FindAllMeshFilters(transform).ToArray();

        List<Mesh> subMesh = new List<Mesh>();
        List<Material> MaterialIDs = new List<Material>();

        //Get avaliable materials
        List<Material> avaliableMaterials = new List<Material>();
        avaliableMaterials.AddRange(materials);

        //Get all mesh with each material
        for (int materialID = 0; materialID < avaliableMaterials.Count; materialID++)
        {
            bool foundMaterial = false;

            //Combines of a single material
            List<CombineInstance> combiners = new List<CombineInstance>();
            for (int i = 0; i < childFilters.Length; i++)
            {
                //If no ignore script attached
                if (childFilters[i].GetComponent<IgnoreCombine>() == null)
                {
                    //Child mesh renderer
                    MeshRenderer childMeshRenderer = childFilters[i].GetComponent<MeshRenderer>();

                    //Disable rendering
                    childMeshRenderer.enabled = false;

                    for (int j = 0; j < childMeshRenderer.materials.Length; j++)
                    {
                        if (childMeshRenderer.materials[j].name.Contains(avaliableMaterials[materialID].name))
                        {
                            CombineInstance combine = new CombineInstance();
                            foundMaterial = true;
                            combine.mesh = childFilters[i].mesh;
                            combine.subMeshIndex = j; //Maybe change later to material id
                            combine.transform = childFilters[i].transform.localToWorldMatrix;
                            combiners.Add(combine);
                            break; //Get rid of this if object has multiple of the same material attacted
                        }
                    }
                }
            }

            //Export to mesh with materials intact
            int currentVerticeCount = 0;
            List<CombineInstance> tempList = new List<CombineInstance>();
            for (int i = 0; i < combiners.Count; i++)
            {
                if (currentVerticeCount + combiners[i].mesh.vertices.Length < 65534)
                {
                    currentVerticeCount += combiners[i].mesh.vertices.Length;
                    tempList.Add(combiners[i]);
                }
                else
                {
                    Mesh splitedMesh = new Mesh();
                    splitedMesh.CombineMeshes(tempList.ToArray(), true);
                    subMesh.Add(splitedMesh);
                    MaterialIDs.Add(avaliableMaterials[materialID]);

                    //Reset
                    currentVerticeCount = 0;
                    tempList.Clear();
                    i -= 1;
                }
            }
            //Debug.Log(currentVerticeCount);

            Mesh mesh = new Mesh();
            mesh.CombineMeshes(tempList.ToArray(), true);
            subMesh.Add(mesh);
            MaterialIDs.Add(avaliableMaterials[materialID]);

            if (!foundMaterial)
            {
                avaliableMaterials.RemoveAt(materialID);
                materialID -= 1;
            }
        }

        //Final combine
        List<CombineInstance> finalCombiners = new List<CombineInstance>();
        //Loop through each sub mesh
        for (int i = 0; i < subMesh.Count; i++)
        {
            CombineInstance combine = new CombineInstance();
            combine.mesh = subMesh[i];
            combine.subMeshIndex = 0;
            combine.transform = transform.worldToLocalMatrix;
            finalCombiners.Add(combine);
        }

        //Splits the mesh into multiple big chunks aslong as its within the max vertice count
        int currentFinalVerticeCount = 0;
        List<CombineInstance> finalTempList = new List<CombineInstance>();
        List<Material> finalTempMaterialList = new List<Material>();
        for (int i = 0; i < finalCombiners.Count; i++)
        {
            int NextVerticeCount = currentFinalVerticeCount + finalCombiners[i].mesh.vertices.Length;
            if (NextVerticeCount < 65534)
            {
                currentFinalVerticeCount += finalCombiners[i].mesh.vertices.Length;
                finalTempList.Add(finalCombiners[i]);
                if (!finalTempMaterialList.Contains(MaterialIDs[i])) finalTempMaterialList.Add(MaterialIDs[i]);
            }
            else
            {
                //New empty object
                GameObject chunk = new GameObject();
                chunk.AddComponent<IgnoreCombine>();
                chunk.AddComponent<MeshFilter>();
                chunk.AddComponent<MeshRenderer>();
                chunk.AddComponent<NavMeshRegionTag>();
                chunk.transform.parent = transform;
                chunk.transform.localPosition = Vector3.zero;
                chunk.transform.localEulerAngles = Vector3.zero;

                //Apply mesh
                Mesh splitedMesh = new Mesh();
                splitedMesh.CombineMeshes(finalTempList.ToArray(), false);
                chunk.GetComponent<MeshFilter>().mesh = splitedMesh;
                chunk.GetComponent<MeshRenderer>().materials = finalTempMaterialList.ToArray();
                AllChunks.Add(chunk);

                //Reset
                currentFinalVerticeCount = 0;
                finalTempList.Clear();
                finalTempMaterialList.Clear();
                i -= 1;
            }
        }

        //New empty object
        GameObject finalChunk = new GameObject();
        finalChunk.AddComponent<IgnoreCombine>();
        finalChunk.AddComponent<MeshFilter>();
        finalChunk.AddComponent<MeshRenderer>();
        finalChunk.AddComponent<NavMeshRegionTag>();
        finalChunk.transform.parent = transform;
        finalChunk.transform.localPosition = Vector3.zero;
        finalChunk.transform.localEulerAngles = Vector3.zero;

        //Apply mesh
        Mesh finalSplitedMesh = new Mesh();
        finalSplitedMesh.CombineMeshes(finalTempList.ToArray(), false);
        finalChunk.GetComponent<MeshFilter>().mesh = finalSplitedMesh;
        finalChunk.GetComponent<MeshRenderer>().materials = finalTempMaterialList.ToArray();
        AllChunks.Add(finalChunk);
    }
}
