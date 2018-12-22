using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceAtObject : MonoBehaviour
{
    /*Setting*/
    public List<ReplaceObjectData> ReplacementDatas = new List<ReplaceObjectData>();
    public bool DestroyAfterSpawn = true;

    public void DoTask()
    {
        List<GameObject> childs = Tools.FindAllChilds(transform);
        List<GameObject> objectsToDestroy = new List<GameObject>();

        for (int i = 0; i < childs.Count; i++)
        {
            for(int j = 0; j < ReplacementDatas.Count; j++)
            {
                if(childs[i].name == ReplacementDatas[j].NameOfObjectToReplace)
                {
                    GameObject spawnedObject = Instantiate(ReplacementDatas[j].ReplacementPrefab, childs[i].transform.position, childs[i].transform.rotation, childs[i].transform.parent);
                    spawnedObject.name = ReplacementDatas[j].ReplacementPrefab.name;
                    objectsToDestroy.Add(childs[i]);
                }
            }
        }

        if(DestroyAfterSpawn)
        {
            for (int i = 0; i < objectsToDestroy.Count; i++) DestroyImmediate(objectsToDestroy[i]);
        }
    }
}
