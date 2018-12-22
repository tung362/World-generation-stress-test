using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(TestWorldGeneration))]
public class TestWorldGenerationEditor : Editor
{
    SerializedProperty BlockParent;
    SerializedProperty BlockOffset;
    SerializedProperty ChunkSize;
    SerializedProperty Biomes;
    SerializedProperty Seed;
    SerializedProperty IslandXZ;
    SerializedProperty IslandY;
    SerializedProperty IslandScale;
    SerializedProperty IslandOctaves;
    SerializedProperty IslandPersistance;
    SerializedProperty IslandLacunarity;
    SerializedProperty IslandOffset;
    SerializedProperty BlockLayers;
    SerializedProperty UnusedLayer;
    SerializedProperty DoubleBaseLayer;
    SerializedProperty PreviewGenerationMask;
    SerializedProperty MaskRadius;

    void OnEnable()
    {
        BlockParent = serializedObject.FindProperty("BlockParent");
        BlockOffset = serializedObject.FindProperty("BlockOffset");
        ChunkSize = serializedObject.FindProperty("ChunkSize");
        Biomes = serializedObject.FindProperty("Biomes");
        Seed = serializedObject.FindProperty("Seed");
        IslandXZ = serializedObject.FindProperty("IslandXZ");
        IslandY = serializedObject.FindProperty("IslandY");
        IslandScale = serializedObject.FindProperty("IslandScale");
        IslandOctaves = serializedObject.FindProperty("IslandOctaves");
        IslandPersistance = serializedObject.FindProperty("IslandPersistance");
        IslandLacunarity = serializedObject.FindProperty("IslandLacunarity");
        IslandOffset = serializedObject.FindProperty("IslandOffset");
        BlockLayers = serializedObject.FindProperty("BlockLayers");
        UnusedLayer = serializedObject.FindProperty("UnusedLayer");
        DoubleBaseLayer = serializedObject.FindProperty("DoubleBaseLayer");
        PreviewGenerationMask = serializedObject.FindProperty("PreviewGenerationMask");
        MaskRadius = serializedObject.FindProperty("MaskRadius");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //Cache block layers
        if (BlockLayers.arraySize != IslandY.intValue) BlockLayers.arraySize = IslandY.intValue;
        List<BlockLayer> blockLayers = new List<BlockLayer>();
        for(int i = 0; i < IslandY.intValue; i++)
        {
            BlockLayer blockLayer = new BlockLayer();
            blockLayer.Threshold = BlockLayers.GetArrayElementAtIndex(i).FindPropertyRelative("Threshold").floatValue;
            blockLayer.PreviewColor = BlockLayers.GetArrayElementAtIndex(i).FindPropertyRelative("PreviewColor").colorValue;
            blockLayers.Add(blockLayer);
        }

        //Generation options
        EditorGUILayout.PropertyField(BlockParent, true);
        EditorGUILayout.PropertyField(BlockOffset, true);
        EditorGUILayout.PropertyField(ChunkSize, true);
        EditorGUILayout.PropertyField(Biomes, true);
        EditorGUILayout.PropertyField(Seed, true);
        EditorGUILayout.PropertyField(IslandXZ, true);
        EditorGUILayout.PropertyField(IslandY, true);
        EditorGUILayout.PropertyField(IslandScale, true);
        EditorGUILayout.PropertyField(IslandOctaves, true);
        EditorGUILayout.PropertyField(IslandPersistance, true);
        EditorGUILayout.PropertyField(IslandLacunarity, true);
        EditorGUILayout.PropertyField(IslandOffset, true);
        EditorGUILayout.PropertyField(DoubleBaseLayer, true);
        EditorGUILayout.PropertyField(PreviewGenerationMask, true);

        //Noise gradient mask options
        switch (PreviewGenerationMask.enumValueIndex)
        {
            case 0:
                break;
            case 1:
                EditorGUILayout.PropertyField(MaskRadius, true);
                break;
            case 2:
                EditorGUILayout.PropertyField(MaskRadius, true);
                break;
        }

        //Previews
        Texture2D noiseMap = new Texture2D(IslandXZ.intValue, IslandXZ.intValue);
        Texture2D baseMap = new Texture2D(IslandXZ.intValue, IslandXZ.intValue);
        Texture2D completeMap = new Texture2D(IslandXZ.intValue, IslandXZ.intValue);
        GenerateBaseLayerPreview(ref noiseMap, ref baseMap, ref completeMap, blockLayers, Seed.stringValue, IslandXZ.intValue, IslandScale.floatValue, IslandOctaves.intValue, IslandPersistance.floatValue, IslandLacunarity.floatValue, IslandOffset.vector2Value);

        EditorGUILayout.LabelField("Base Layer Noise Preview");
        EditorGUI.DrawPreviewTexture(new Rect(EditorGUILayout.GetControlRect().x, EditorGUILayout.GetControlRect().y - 20, EditorGUILayout.GetControlRect().width, EditorGUILayout.GetControlRect().width), noiseMap);
        GUILayout.Space(EditorGUIUtility.currentViewWidth - 100);
        GUILayout.Space(10);

        EditorGUILayout.LabelField("Base Layer Preview");
        EditorGUI.DrawPreviewTexture(new Rect(EditorGUILayout.GetControlRect().x, EditorGUILayout.GetControlRect().y - 20, EditorGUILayout.GetControlRect().width, EditorGUILayout.GetControlRect().width), baseMap);
        GUILayout.Space(EditorGUIUtility.currentViewWidth - 100);
        EditorGUILayout.PropertyField(BlockLayers.GetArrayElementAtIndex(0), true);
        EditorGUILayout.PropertyField(UnusedLayer, true);
        GUILayout.Space(10);

        EditorGUILayout.LabelField("Complete Preview");
        EditorGUI.DrawPreviewTexture(new Rect(EditorGUILayout.GetControlRect().x, EditorGUILayout.GetControlRect().y - 20, EditorGUILayout.GetControlRect().width, EditorGUILayout.GetControlRect().width), completeMap);
        GUILayout.Space(EditorGUIUtility.currentViewWidth - 100);
        EditorGUILayout.PropertyField(BlockLayers, true);


        serializedObject.ApplyModifiedProperties();
    }

    void GenerateBaseLayerPreview(ref Texture2D NoiseTexture, ref Texture2D BaseTexture, ref Texture2D CompleteTexture, List<BlockLayer> MapBlockLayers, string MapSeed, int MapXZSize, float MapScale, int MapOctaves, float MapPersistance, float MapLacunarity, Vector2 MapOffset)
    {
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
                for(int i = 0; i < MapOctaves; i++)
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
                switch (PreviewGenerationMask.enumValueIndex)
                {
                    case 0:
                        break;
                    case 1:
                        textureCoord[x, y] = GradientNoise(x, y, MapXZSize, MaskRadius.floatValue, textureCoord[x, y], 0);
                        break;
                    case 2:
                        textureCoord[x, y] = GradientNoise(x, y, MapXZSize, MaskRadius.floatValue, textureCoord[x, y], 1);
                        break;
                }

                //Apply to texture
                NoiseTexture.SetPixel(x, y, Color.Lerp(Color.black, Color.white, textureCoord[x, y]));

                if (textureCoord[x, y] >= MapBlockLayers[0].Threshold) BaseTexture.SetPixel(x, y, MapBlockLayers[0].PreviewColor);
                else BaseTexture.SetPixel(x, y, UnusedLayer.colorValue);

                CompleteTexture.SetPixel(x, y, UnusedLayer.colorValue);
                for (int i = 0; i < MapBlockLayers.Count; i++)
                {
                    if (textureCoord[x, y] >= MapBlockLayers[i].Threshold) CompleteTexture.SetPixel(x, y, MapBlockLayers[i].PreviewColor);
                }
            }
        }

        NoiseTexture.wrapMode = TextureWrapMode.Clamp;
        NoiseTexture.filterMode = FilterMode.Point;
        NoiseTexture.Apply();

        BaseTexture.wrapMode = TextureWrapMode.Clamp;
        BaseTexture.filterMode = FilterMode.Point;
        BaseTexture.Apply();

        CompleteTexture.wrapMode = TextureWrapMode.Clamp;
        CompleteTexture.filterMode = FilterMode.Point;
        CompleteTexture.Apply();
    }

    float GradientNoise(int X, int Y, int Size, float Radius, float AdditiveNoise, int GradientType)
    {
        //Gradient
        float distanceX = Mathf.Abs(X - Size * 0.5f);
        float distanceY = Mathf.Abs(Y - Size * 0.5f);

        //Circular mask
        float distance = Mathf.Sqrt(distanceX * distanceX + distanceY * distanceY);

        //square mask
        if(GradientType == 1) distance = Mathf.Max(distanceX, distanceY);

        float maxWidth = Size * Radius - 10.0f;
        float delta = distance / maxWidth;
        float gradient = delta * delta;

        AdditiveNoise *= Mathf.Max(0.0f, 1.0f - gradient);

        return AdditiveNoise;
    }
}
