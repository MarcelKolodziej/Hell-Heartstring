using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TerrainChecker : MonoBehaviour
{
    // Start is called before the first frame update
    private float[] GetTextureMix(Vector3 playerPos, Terrain t) // return array of floats
     {
        Vector3 tPos = t.transform.position;
        TerrainData tData = t.terrainData;
        int mapX = Mathf.RoundToInt((playerPos.x - tPos.x) / tData.size.x * tData.alphamapWidth);
        int mapZ = Mathf.RoundToInt((playerPos.z - tPos.z) / tData.size.z * tData.alphamapHeight);
        float[,,] splatmapData = tData.GetAlphamaps(mapX, mapZ, 1, 1); // get values of terrain 

        float[] cellmix = new float[splatmapData.GetUpperBound(2) + 1];
        for (int i = 0; i < cellmix.Length; i++) {
            cellmix[i] = splatmapData[0, 0, i];
        }
        return cellmix;
    }

    public string GetLayerName(Vector3 playerPos, Terrain t)
        {
        float[] cellMix = GetTextureMix(playerPos, t);
        float strongest = 0;
        int maxIndex = 0;
        for (int i = 0; i < cellMix.Length; i++)
        {
            if (cellMix[i] > strongest)
            {
                maxIndex = i;
                strongest = cellMix[i];
            }
        }
        return t.terrainData.terrainLayers[maxIndex].name;
    }



    }
