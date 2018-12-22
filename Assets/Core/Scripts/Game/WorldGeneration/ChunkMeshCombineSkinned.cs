using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkMeshCombineSkinned : MonoBehaviour
{
    /*Settings*/
    public GameObject CombinedMeshPrefab;

    [HideInInspector]
    public List<Material> Materials = new List<Material>();

    public bool debug = false;

    /*Data*/
    private List<GameObject> AllChunks = new List<GameObject>();

    [HideInInspector]
    public MeshFilter[] meshFilters;
    public Material material;

    void Update()
    {
        if (debug)
        {
            if (Input.GetKey(KeyCode.K)) CombineMeshes(true);
            if (Input.GetKeyDown(KeyCode.J)) CombineMeshes(true);
        }
    }

    public void CombineMeshes(bool AddNavMeshRegionTag)
    {
        meshFilters = Tools.FindAllMeshFilters(transform).ToArray();
        material = Materials[0];

        // figure out array sizes
        int vertCount = 0;
        int normCount = 0;
        int triCount = 0;
        int uvCount = 0;

        foreach (MeshFilter mf in meshFilters)
        {
            vertCount += mf.mesh.vertices.Length;
            normCount += mf.mesh.normals.Length;
            triCount += mf.mesh.triangles.Length;
            uvCount += mf.mesh.uv.Length;
        }

        // allocate arrays
        Vector3[] verts = new Vector3[vertCount];
        Vector3[] norms = new Vector3[normCount];
        Transform[] aBones = new Transform[meshFilters.Length];
        Matrix4x4[] bindPoses = new Matrix4x4[meshFilters.Length];
        BoneWeight[] weights = new BoneWeight[vertCount];
        int[] tris = new int[triCount];
        Vector2[] uvs = new Vector2[uvCount];

        int vertOffset = 0;
        int normOffset = 0;
        int triOffset = 0;
        int uvOffset = 0;
        int meshOffset = 0;

        // merge the meshes and set up bones
        foreach (MeshFilter mf in meshFilters)
        {
            foreach (int i in mf.mesh.triangles)
                tris[triOffset++] = i + vertOffset;

            aBones[meshOffset] = mf.transform;
            bindPoses[meshOffset] = Matrix4x4.identity;

            foreach (Vector3 v in mf.mesh.vertices)
            {
                weights[vertOffset].weight0 = 1.0f;
                weights[vertOffset].boneIndex0 = meshOffset;
                verts[vertOffset++] = v;
            }

            foreach (Vector3 n in mf.mesh.normals)
            {
                norms[normOffset++] = n;
            }

            foreach (Vector2 uv in mf.mesh.uv)
            {
                uvs[uvOffset++] = uv;
            }

            meshOffset++;

            MeshRenderer mr = mf.GetComponent<MeshRenderer>();

            if (mr)
                mr.enabled = false;
        }

        // hook up the mesh
        Mesh me = new Mesh();
        me.name = gameObject.name;
        me.vertices = verts;
        me.normals = norms;
        me.boneWeights = weights;
        me.uv = uvs;
        me.triangles = tris;
        me.bindposes = bindPoses;

        // hook up the mesh renderer        
        SkinnedMeshRenderer smr = GetComponent<SkinnedMeshRenderer>();

        smr.sharedMesh = me;
        smr.bones = aBones;
        GetComponent<Renderer>().material = material;
    }
}
