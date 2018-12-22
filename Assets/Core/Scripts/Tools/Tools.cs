using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
#endif

/*Weather*/
[System.Serializable]
public class Weather
{
    public string Name;
    public float FogMaxDensity;
    public float FogMinDensity;
    public int ParticleType;
    public int OneOutOf;
}

/*Nav Mesh Generation*/
[System.Serializable]
public class BuildRegion
{
    public int AreaID;
    public MeshFilter TheMeshFilter;
    public Terrain TheTerrain;
    public Collider TheCollider;
}

/*World Generation*/
[System.Serializable]
public class GenerationBlock
{
    public GameObject Block;
    public int OneOutOf;
}

[System.Serializable]
public class Biome
{
    public string Name;
    public GameObject Chunk;
    public GenerationBlock[] TopBlocks;
    public GenerationBlock[] BottomBlocks;
    public GenerationBlock[] ToplessBlocks;
    public GenerationBlock[] Structures;
    public GenerationBlock[] Trees;
    public GenerationBlock[] Plants;
}

[System.Serializable]
public class TerrainChunk
{
    //layer->block type->blocks
    public List<List<List<GameObject>>> TopChunks = new List<List<List<GameObject>>>();
    public List<List<List<GameObject>>> BottomChunks = new List<List<List<GameObject>>>();
    public List<List<List<GameObject>>> ToplessChunks = new List<List<List<GameObject>>>();
    public List<List<List<GameObject>>> PlantChunks = new List<List<List<GameObject>>>();
}

[System.Serializable]
public class BlockLayer
{
    [Range(0.0f, 1.0f)]
    public float Threshold;
    public Color PreviewColor;
}

/*Tools*/
[System.Serializable]
public class ReplaceObjectData
{
    public GameObject ReplacementPrefab;
    public string NameOfObjectToReplace;
}

public static class Tools
{
    /*General*/
    public static List<GameObject> FindAllChilds(Transform TheGameObject)
    {
        List<GameObject> retval = new List<GameObject>();
        foreach (Transform child in TheGameObject)
        {
            retval.Add(child.gameObject);
            retval.AddRange(FindAllChilds(child));
        }
        return retval;
    }

    public static void ApplyLayerToChilds(Transform TheGameObject, string LayerName)
    {
        foreach (Transform child in TheGameObject)
        {
            child.gameObject.layer = LayerMask.NameToLayer(LayerName);
            ApplyLayerToChilds(child, LayerName);
        }
    }

    public static List<MeshRenderer> FindAllMeshes(Transform TheGameObject)
    {
        List<MeshRenderer> retval = new List<MeshRenderer>();
        foreach (Transform child in TheGameObject)
        {
            if (child.GetComponent<MeshRenderer>() != null) retval.Add(child.GetComponent<MeshRenderer>());
            retval.AddRange(FindAllMeshes(child));
        }
        return retval;
    }

    public static List<MeshFilter> FindAllMeshFilters(Transform TheGameObject)
    {
        List<MeshFilter> retval = new List<MeshFilter>();
        foreach (Transform child in TheGameObject)
        {
            if (child.GetComponent<MeshFilter>() != null) retval.Add(child.GetComponent<MeshFilter>());
            retval.AddRange(FindAllMeshFilters(child));
        }
        return retval;
    }

    /*Nav Mesh*/
    public static NavMeshBuildSource CreateEmptySource()
    {
        NavMeshBuildSource emptySource = new NavMeshBuildSource();
        emptySource.sourceObject = new Mesh();
        return emptySource;
    }

    public static string GetAgentName(int AgentIndex)
    {
        return NavMesh.GetSettingsNameFromID(NavMesh.GetSettingsByIndex(AgentIndex).agentTypeID);
    }

    public static int GetAgentID(int AgentIndex)
    {
        return NavMesh.GetSettingsByIndex(AgentIndex).agentTypeID;
    }

#if UNITY_EDITOR
    public static string GetNavAreaName(int AreaIndex)
    {
        string[] areas = GameObjectUtility.GetNavMeshAreaNames();
        return areas[AreaIndex];
    }

    public static int GetNavAreaID(int AreaIndex)
    {
        string[] areas = GameObjectUtility.GetNavMeshAreaNames();
        return NavMesh.GetAreaFromName(areas[AreaIndex]);
    }
#endif
}
