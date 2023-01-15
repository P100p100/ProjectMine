using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;

public class MapGeneration_M : MonoBehaviour
{
    public Gradient gradient;
    Color[] colors;
    Color[] colors2;
    Color[] colors4;
    Color[] colors8;
    Color[] colors16;
    Color[] colors32;
    Color[] colors64;
    Color[] colors128;

    Mesh mesh;
    Texture texture;

    Vector3[] vertices;
    int[] triangles;

    public int lod = 1;
    public int tempLod = 1;

    public bool generate = false;

    public int xSize = 20;
    public int zSize = 20;
    [Range(0f, 1f)]
    public float freq = 0.03f;
    [Range(0f, 1f)]
    public float freqMount = 0.03f;
    [Range(0f, 10000f)]
    public float mountHeight = 100f;
    public float octaves = 5;
    [Range(0f, 10f)]
    public float pow = 1f;
    [Range(0f, 1f)]
    public List<float> octaveFreq = new List<float>();
    [Range(-10f, 10f)]
    public List<float> octaveTX = new List<float>();
    [Range(-10f, 10f)]
    public List<float> octaveTZ = new List<float>();    
    [Range(-10f, 1000f)]
    public List<float> heights = new List<float>();

    public Transform player;
    int genAll = 0;

    int[] lod1MeshTriangles;
    Vector3[] lod1Vertices;

    int[] lod2MeshTriangles;
    Vector3[] lod2Vertices;

    int[] lod4MeshTriangles;
    Vector3[] lod4Vertices;

    int[] lod8MeshTriangles;
    Vector3[] lod8Vertices;

    int[] lod16MeshTriangles;
    Vector3[] lod16Vertices;

    int[] lod32MeshTriangles;
    Vector3[] lod32Vertices;

    int[] lod64MeshTriangles;
    Vector3[] lod64Vertices;

    int[] lod128MeshTriangles;
    Vector3[] lod128Vertices;

    public GameObject lodMesh;

    LOD[] lods = new LOD[8];
    Renderer[] renderers0 = new Renderer[1];
    Renderer[] renderers1 = new Renderer[1];
    Renderer[] renderers2 = new Renderer[1];
    Renderer[] renderers3 = new Renderer[1];
    Renderer[] renderers4 = new Renderer[1];
    Renderer[] renderers5 = new Renderer[1];
    Renderer[] renderers6 = new Renderer[1];
    Renderer[] renderers7 = new Renderer[1];

    public LODGroup group;
    float max = 150;
    float min = 0;

    int genLod = 1;
    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void Update()
    {
        if(generate)
        {
            genAll = 0;
            generate = false;
        }

        if (genAll == 0)
        {
            vertices = new Vector3[(xSize + 1) * (zSize + 1)];
            colors = new Color[(xSize + 1) * (zSize + 1)];

            for (int i = 0, z = 0; z <= zSize; z++)
            {
                for (int x = 0; x <= xSize; x++)
                {
                    float height = 0;
                    for (int octave = 1; octave < octaveFreq.Count + 1; octave++)
                    {
                        height += Mathf.PerlinNoise((transform.position.x + x + octaveTX[octave - 1]) * octaveFreq[octave - 1], (transform.position.z + z + octaveTZ[octave - 1]) * octaveFreq[octave - 1]) * octave / 10 * heights[octave - 1] - 12;
                        //height += noise.snoise(new float2((transform.position.x + x + octaveTX[octave - 1]) * octaveFreq[octave - 1], (transform.position.z + z + octaveTZ[octave - 1]))) *octave / 10 * heights[octave - 1] - 12;
                    }
                    if (height < 0)
                    {
                        height = 0;
                    }
                    if (height > 30)
                    {
                        height -= noise.cellular(new float2((transform.position.x + x) * freqMount, (transform.position.z + z) * freqMount)).y * mountHeight * 0.7f * (height - 30) * -1;
                        height -= noise.cellular(new float2((transform.position.x + x) * freqMount * 3, (transform.position.z + z) * freqMount * 3)).y * (mountHeight/20) * (height - 30) * -1;

                    }
                    vertices[i] = new Vector3(x, height, z);
                    colors[i] = gradient.Evaluate(Mathf.InverseLerp(max, min, height));
                    i++;
                }
            }
            genAll++;
        }
        else if (genAll == 1)
        {
            genLod = 1;
            lod1MeshTriangles = new int[xSize * zSize * 6];

            int vert = 0;
            int tris = 0;

            for (int z = 0; z < zSize / genLod; z++)
            {
                for (int x = 0; x < xSize / genLod; x++)
                {
                    lod1MeshTriangles[tris + 0] = (vert + 0);
                    lod1MeshTriangles[tris + 1] = (vert + xSize * genLod + 1 * genLod);
                    lod1MeshTriangles[tris + 2] = (vert + 1 * genLod);
                    lod1MeshTriangles[tris + 3] = (vert + 1 * genLod);
                    lod1MeshTriangles[tris + 4] = (vert + xSize * genLod + 1 * genLod);
                    lod1MeshTriangles[tris + 5] = (vert + xSize * genLod + 2 * genLod);

                    vert += genLod;
                    tris += 6;
                }
                vert += xSize * (genLod - 1) + genLod;
            }
            genAll++;
        }
        else if (genAll == 2)
        {
            genLod = 2;
            lod2MeshTriangles = new int[(xSize / genLod) * (zSize / genLod) * 6];

            int vert = 0;
            int tris = 0;

            for (int z = 0; z < zSize / genLod; z++)
            {
                for (int x = 0; x < xSize / genLod; x++)
                {
                    lod2MeshTriangles[tris + 0] = (vert + 0);
                    lod2MeshTriangles[tris + 1] = (vert + (xSize / genLod) + 1);
                    lod2MeshTriangles[tris + 2] = (vert + 1);
                    lod2MeshTriangles[tris + 3] = (vert + 1);
                    lod2MeshTriangles[tris + 4] = (vert + (xSize / genLod) + 1);
                    lod2MeshTriangles[tris + 5] = (vert + (xSize / genLod) + 2);

                    vert += 1;
                    tris += 6;
                }
                vert += 1;
            }
            genAll++;
        }
        else if (genAll == 3)
        {
            genLod = 4;
            lod4MeshTriangles = new int[(xSize / genLod) * (xSize / genLod) * 6];

            int vert = 0;
            int tris = 0;

            for (int z = 0; z < zSize / genLod; z++)
            {
                for (int x = 0; x < xSize / genLod; x++)
                {
                    lod4MeshTriangles[tris + 0] = (vert + 0);
                    lod4MeshTriangles[tris + 1] = (vert + (xSize / genLod) + 1);
                    lod4MeshTriangles[tris + 2] = (vert + 1);
                    lod4MeshTriangles[tris + 3] = (vert + 1);
                    lod4MeshTriangles[tris + 4] = (vert + (xSize / genLod) + 1);
                    lod4MeshTriangles[tris + 5] = (vert + (xSize / genLod) + 2);

                    vert += 1;
                    tris += 6;
                }
                vert += 1;
            }
            genAll++;
        }
        else if (genAll == 4)
        {
            genLod = 8;
            lod8MeshTriangles = new int[(xSize / genLod) * (xSize / genLod) * 6];

            int vert = 0;
            int tris = 0;

            for (int z = 0; z < zSize / genLod; z++)
            {
                for (int x = 0; x < xSize / genLod; x++)
                {
                    lod8MeshTriangles[tris + 0] = (vert + 0);
                    lod8MeshTriangles[tris + 1] = (vert + (xSize / genLod) + 1);
                    lod8MeshTriangles[tris + 2] = (vert + 1);
                    lod8MeshTriangles[tris + 3] = (vert + 1);
                    lod8MeshTriangles[tris + 4] = (vert + (xSize / genLod) + 1);
                    lod8MeshTriangles[tris + 5] = (vert + (xSize / genLod) + 2);

                    vert += 1;
                    tris += 6;
                }
                vert += 1;
            }
            genAll++;
        }
        else if (genAll == 5)
        {
            genLod = 16;
            lod16MeshTriangles = new int[(xSize / genLod) * (xSize / genLod) * 6];

            int vert = 0;
            int tris = 0;

            for (int z = 0; z < zSize / genLod; z++)
            {
                for (int x = 0; x < xSize / genLod; x++)
                {
                    lod16MeshTriangles[tris + 0] = (vert + 0);
                    lod16MeshTriangles[tris + 1] = (vert + (xSize / genLod) + 1);
                    lod16MeshTriangles[tris + 2] = (vert + 1);
                    lod16MeshTriangles[tris + 3] = (vert + 1);
                    lod16MeshTriangles[tris + 4] = (vert + (xSize / genLod) + 1);
                    lod16MeshTriangles[tris + 5] = (vert + (xSize / genLod) + 2);

                    vert += 1;
                    tris += 6;
                }
                vert += 1;
            }
            genAll++;
        }
        else if (genAll == 6)
        {
            genLod = 32;
            lod32MeshTriangles = new int[(xSize / genLod) * (xSize / genLod) * 6];

            int vert = 0;
            int tris = 0;

            for (int z = 0; z < zSize / genLod; z++)
            {
                for (int x = 0; x < xSize / genLod; x++)
                {
                    lod32MeshTriangles[tris + 0] = (vert + 0);
                    lod32MeshTriangles[tris + 1] = (vert + (xSize / genLod) + 1);
                    lod32MeshTriangles[tris + 2] = (vert + 1);
                    lod32MeshTriangles[tris + 3] = (vert + 1);
                    lod32MeshTriangles[tris + 4] = (vert + (xSize / genLod) + 1);
                    lod32MeshTriangles[tris + 5] = (vert + (xSize / genLod) + 2);

                    vert += 1;
                    tris += 6;
                }
                vert += 1;
            }
            genAll++;
        }
        else if (genAll == 7)
        {
            genLod = 64;
            lod64MeshTriangles = new int[(xSize / genLod) * (xSize / genLod) * 6];

            int vert = 0;
            int tris = 0;

            for (int z = 0; z < zSize / genLod; z++)
            {
                for (int x = 0; x < xSize / genLod; x++)
                {
                    lod64MeshTriangles[tris + 0] = (vert + 0);
                    lod64MeshTriangles[tris + 1] = (vert + (xSize / genLod) + 1);
                    lod64MeshTriangles[tris + 2] = (vert + 1);
                    lod64MeshTriangles[tris + 3] = (vert + 1);
                    lod64MeshTriangles[tris + 4] = (vert + (xSize / genLod) + 1);
                    lod64MeshTriangles[tris + 5] = (vert + (xSize / genLod) + 2);

                    vert += 1;
                    tris += 6;
                }
                vert += 1;
            }
            genAll++;
        }
        else if (genAll == 8)
        {
            genLod = 128;
            lod128MeshTriangles = new int[(xSize / genLod) * (xSize / genLod) * 6];

            int vert = 0;
            int tris = 0;

            for (int z = 0; z < zSize / genLod; z++)
            {
                for (int x = 0; x < xSize / genLod; x++)
                {
                    lod128MeshTriangles[tris + 0] = (vert + 0);
                    lod128MeshTriangles[tris + 1] = (vert + (xSize / genLod) + 1);
                    lod128MeshTriangles[tris + 2] = (vert + 1);
                    lod128MeshTriangles[tris + 3] = (vert + 1);
                    lod128MeshTriangles[tris + 4] = (vert + (xSize / genLod) + 1);
                    lod128MeshTriangles[tris + 5] = (vert + (xSize / genLod) + 2);

                    vert += 1;
                    tris += 6;
                }
                vert += 1;
            }
            genAll++;
        }
        else if (genAll == 9)
        {
            genLod = 2;
            lod2Vertices = new Vector3[(xSize / genLod + 1) * (zSize / genLod + 1)];
            colors2 = new Color[(xSize / genLod + 1) * (zSize / genLod + 1)];

            for (int i = 0, z = 0; z <= zSize; z += genLod)
            {
                for (int x = 0; x <= xSize; x += genLod)
                {
                    float height = 0;
                    for (int octave = 1; octave < octaveFreq.Count + 1; octave++)
                    {
                        height += Mathf.PerlinNoise((transform.position.x + x + octaveTX[octave - 1]) * octaveFreq[octave - 1], (transform.position.z + z + octaveTZ[octave - 1]) * octaveFreq[octave - 1]) * octave / 10 * heights[octave - 1] - 12;
                        //height += noise.snoise(new float2((transform.position.x + x + octaveTX[octave - 1]) * octaveFreq[octave - 1], (transform.position.z + z + octaveTZ[octave - 1]))) *octave / 10 * heights[octave - 1] - 12;
                    }
                    if (height < 0)
                    {
                        height = 0;
                    }
                    if (height > 30)
                    {
                        height -= noise.cellular(new float2((transform.position.x + x) * freqMount, (transform.position.z + z) * freqMount)).y * mountHeight * 0.7f * (height - 30) * -1;
                        height -= noise.cellular(new float2((transform.position.x + x) * freqMount * 3, (transform.position.z + z) * freqMount * 3)).y * (mountHeight / 20) * (height - 30) * -1;

                    }
                    lod2Vertices[i] = new Vector3(x, height, z);
                    colors2[i] = gradient.Evaluate(Mathf.InverseLerp(max, min, height));
                    i++;
                }
            }
            genAll++;
        }
        else if (genAll == 10)
        {
            genLod = 4;
            lod4Vertices = new Vector3[(xSize / genLod + 1) * (zSize / genLod + 1)];
            colors4 = new Color[(xSize / genLod + 1) * (zSize / genLod + 1)];

            for (int i = 0, z = 0; z <= zSize; z += genLod)
            {
                for (int x = 0; x <= xSize; x += genLod)
                {
                    float height = 0;
                    for (int octave = 1; octave < octaveFreq.Count + 1; octave++)
                    {
                        height += Mathf.PerlinNoise((transform.position.x + x + octaveTX[octave - 1]) * octaveFreq[octave - 1], (transform.position.z + z + octaveTZ[octave - 1]) * octaveFreq[octave - 1]) * octave / 10 * heights[octave - 1] - 12;
                        //height += noise.snoise(new float2((transform.position.x + x + octaveTX[octave - 1]) * octaveFreq[octave - 1], (transform.position.z + z + octaveTZ[octave - 1]))) *octave / 10 * heights[octave - 1] - 12;
                    }
                    if (height < 0)
                    {
                        height = 0;
                    }
                    if (height > 30)
                    {
                        height -= noise.cellular(new float2((transform.position.x + x) * freqMount, (transform.position.z + z) * freqMount)).y * mountHeight * 0.7f * (height - 30) * -1;
                        height -= noise.cellular(new float2((transform.position.x + x) * freqMount * 3, (transform.position.z + z) * freqMount * 3)).y * (mountHeight / 20) * (height - 30) * -1;

                    }
                    lod4Vertices[i] = new Vector3(x, height, z);
                    colors4[i] = gradient.Evaluate(Mathf.InverseLerp(max, min, height));
                    i++;
                }
            }
            genAll++;
        }
        else if (genAll == 11)
        {
            genLod = 8;
            lod8Vertices = new Vector3[(xSize / genLod + 1) * (zSize / genLod + 1)];
            colors8 = new Color[(xSize / genLod + 1) * (zSize / genLod + 1)];

            for (int i = 0, z = 0; z <= zSize; z += genLod)
            {
                for (int x = 0; x <= xSize; x += genLod)
                {
                    float height = 0;
                    for (int octave = 1; octave < octaveFreq.Count + 1; octave++)
                    {
                        height += Mathf.PerlinNoise((transform.position.x + x + octaveTX[octave - 1]) * octaveFreq[octave - 1], (transform.position.z + z + octaveTZ[octave - 1]) * octaveFreq[octave - 1]) * octave / 10 * heights[octave - 1] - 12;
                        //height += noise.snoise(new float2((transform.position.x + x + octaveTX[octave - 1]) * octaveFreq[octave - 1], (transform.position.z + z + octaveTZ[octave - 1]))) *octave / 10 * heights[octave - 1] - 12;
                    }
                    if (height < 0)
                    {
                        height = 0;
                    }
                    if (height > 30)
                    {
                        height -= noise.cellular(new float2((transform.position.x + x) * freqMount, (transform.position.z + z) * freqMount)).y * mountHeight * 0.7f * (height - 30) * -1;
                        height -= noise.cellular(new float2((transform.position.x + x) * freqMount * 3, (transform.position.z + z) * freqMount * 3)).y * (mountHeight / 20) * (height - 30) * -1;

                    }
                    lod8Vertices[i] = new Vector3(x, height, z);
                    colors8[i] = gradient.Evaluate(Mathf.InverseLerp(max, min, height));
                    i++;
                }
            }
            genAll++;
        }
        else if (genAll == 12)
        {
            genLod = 16;
            lod16Vertices = new Vector3[(xSize / genLod + 1) * (zSize / genLod + 1)];
            colors16 = new Color[(xSize / genLod + 1) * (zSize / genLod + 1)];

            for (int i = 0, z = 0; z <= zSize; z += genLod)
            {
                for (int x = 0; x <= xSize; x += genLod)
                {
                    float height = 0;
                    for (int octave = 1; octave < octaveFreq.Count + 1; octave++)
                    {
                        height += Mathf.PerlinNoise((transform.position.x + x + octaveTX[octave - 1]) * octaveFreq[octave - 1], (transform.position.z + z + octaveTZ[octave - 1]) * octaveFreq[octave - 1]) * octave / 10 * heights[octave - 1] - 12;
                        //height += noise.snoise(new float2((transform.position.x + x + octaveTX[octave - 1]) * octaveFreq[octave - 1], (transform.position.z + z + octaveTZ[octave - 1]))) *octave / 10 * heights[octave - 1] - 12;
                    }
                    if (height < 0)
                    {
                        height = 0;
                    }
                    if (height > 30)
                    {
                        height -= noise.cellular(new float2((transform.position.x + x) * freqMount, (transform.position.z + z) * freqMount)).y * mountHeight * 0.7f * (height - 30) * -1;
                        height -= noise.cellular(new float2((transform.position.x + x) * freqMount * 3, (transform.position.z + z) * freqMount * 3)).y * (mountHeight / 20) * (height - 30) * -1;

                    }
                    lod16Vertices[i] = new Vector3(x, height, z);
                    colors16[i] = gradient.Evaluate(Mathf.InverseLerp(max, min, height));
                    i++;
                }
            }
            genAll++;
        }
        else if (genAll == 13)
        {
            genLod = 32;
            lod32Vertices = new Vector3[(xSize / genLod + 1) * (zSize / genLod + 1)];
            colors32 = new Color[(xSize / genLod + 1) * (zSize / genLod + 1)];

            for (int i = 0, z = 0; z <= zSize; z += genLod)
            {
                for (int x = 0; x <= xSize; x += genLod)
                {
                    float height = 0;
                    for (int octave = 1; octave < octaveFreq.Count + 1; octave++)
                    {
                        height += Mathf.PerlinNoise((transform.position.x + x + octaveTX[octave - 1]) * octaveFreq[octave - 1], (transform.position.z + z + octaveTZ[octave - 1]) * octaveFreq[octave - 1]) * octave / 10 * heights[octave - 1] - 12;
                        //height += noise.snoise(new float2((transform.position.x + x + octaveTX[octave - 1]) * octaveFreq[octave - 1], (transform.position.z + z + octaveTZ[octave - 1]))) *octave / 10 * heights[octave - 1] - 12;
                    }
                    if (height < 0)
                    {
                        height = 0;
                    }
                    if (height > 30)
                    {
                        height -= noise.cellular(new float2((transform.position.x + x) * freqMount, (transform.position.z + z) * freqMount)).y * mountHeight * 0.7f * (height - 30) * -1;
                        height -= noise.cellular(new float2((transform.position.x + x) * freqMount * 3, (transform.position.z + z) * freqMount * 3)).y * (mountHeight / 20) * (height - 30) * -1;

                    }
                    lod32Vertices[i] = new Vector3(x, height, z);
                    colors32[i] = gradient.Evaluate(Mathf.InverseLerp(max, min, height));
                    i++;
                }
            }
            genAll++;
        }
        else if (genAll == 14)
        {
            genLod = 64;
            lod64Vertices = new Vector3[(xSize / genLod + 1) * (zSize / genLod + 1)];
            colors64 = new Color[(xSize / genLod + 1) * (zSize / genLod + 1)];

            for (int i = 0, z = 0; z <= zSize; z += genLod)
            {
                for (int x = 0; x <= xSize; x += genLod)
                {
                    float height = 0;
                    for (int octave = 1; octave < octaveFreq.Count + 1; octave++)
                    {
                        height += Mathf.PerlinNoise((transform.position.x + x + octaveTX[octave - 1]) * octaveFreq[octave - 1], (transform.position.z + z + octaveTZ[octave - 1]) * octaveFreq[octave - 1]) * octave / 10 * heights[octave - 1] - 12;
                        //height += noise.snoise(new float2((transform.position.x + x + octaveTX[octave - 1]) * octaveFreq[octave - 1], (transform.position.z + z + octaveTZ[octave - 1]))) *octave / 10 * heights[octave - 1] - 12;
                    }
                    if (height < 0)
                    {
                        height = 0;
                    }
                    if (height > 30)
                    {
                        height -= noise.cellular(new float2((transform.position.x + x) * freqMount, (transform.position.z + z) * freqMount)).y * mountHeight * 0.7f * (height - 30) * -1;
                        height -= noise.cellular(new float2((transform.position.x + x) * freqMount * 3, (transform.position.z + z) * freqMount * 3)).y * (mountHeight / 20) * (height - 30) * -1;

                    }
                    lod64Vertices[i] = new Vector3(x, height, z);
                    colors64[i] = gradient.Evaluate(Mathf.InverseLerp(max, min, height));
                    i++;
                }
            }
            genAll++;
        }
        else if (genAll == 15)
        {
            genLod = 128;
            lod128Vertices = new Vector3[(xSize / genLod + 1) * (zSize / genLod + 1)];
            colors128 = new Color[(xSize / genLod + 1) * (zSize / genLod + 1)];

            for (int i = 0, z = 0; z <= zSize; z += genLod)
            {
                for (int x = 0; x <= xSize; x += genLod)
                {
                    float height = 0;
                    for (int octave = 1; octave < octaveFreq.Count + 1; octave++)
                    {
                        height += Mathf.PerlinNoise((transform.position.x + x + octaveTX[octave - 1]) * octaveFreq[octave - 1], (transform.position.z + z + octaveTZ[octave - 1]) * octaveFreq[octave - 1]) * octave / 10 * heights[octave - 1] - 12;
                        //height += noise.snoise(new float2((transform.position.x + x + octaveTX[octave - 1]) * octaveFreq[octave - 1], (transform.position.z + z + octaveTZ[octave - 1]))) *octave / 10 * heights[octave - 1] - 12;
                    }
                    if (height < 0)
                    {
                        height = 0;
                    }
                    if (height > 30)
                    {
                        height -= noise.cellular(new float2((transform.position.x + x) * freqMount, (transform.position.z + z) * freqMount)).y * mountHeight * 0.7f * (height - 30) * -1;
                        height -= noise.cellular(new float2((transform.position.x + x) * freqMount * 3, (transform.position.z + z) * freqMount * 3)).y * (mountHeight / 20) * (height - 30) * -1;

                    }
                    lod128Vertices[i] = new Vector3(x, height, z);
                    colors128[i] = gradient.Evaluate(Mathf.InverseLerp(max, min, height));
                    i++;
                }
            }
            genAll++;
        }
        else if (genAll == 16)
        {
            GameObject temp;
            temp = Instantiate(lodMesh, transform);
            temp.GetComponent<MeshFilter>().mesh.Clear();
            temp.GetComponent<MeshFilter>().mesh.vertices = vertices;
            temp.GetComponent<MeshFilter>().mesh.triangles = lod1MeshTriangles;
            temp.GetComponent<MeshFilter>().mesh.colors = colors;
            temp.GetComponent<MeshFilter>().mesh.RecalculateBounds();
            temp.GetComponent<MeshFilter>().mesh.RecalculateNormals();
            temp.GetComponent<MeshCollider>().sharedMesh = temp.GetComponent<MeshFilter>().mesh;
            temp.name = "lod0";
            renderers0[0] = temp.GetComponent<Renderer>();
            lods[0] = new LOD(1.0F, renderers0);

            genAll++;
        }
        else if (genAll == 17)
        {
            GameObject temp;
            temp = Instantiate(lodMesh, transform);
            temp.GetComponent<MeshFilter>().mesh.Clear();
            temp.GetComponent<MeshFilter>().mesh.vertices = lod2Vertices;
            temp.GetComponent<MeshFilter>().mesh.triangles = lod2MeshTriangles;
            temp.GetComponent<MeshFilter>().mesh.colors = colors2;
            temp.GetComponent<MeshFilter>().mesh.RecalculateBounds();
            temp.GetComponent<MeshFilter>().mesh.RecalculateNormals();
            temp.GetComponent<MeshCollider>().sharedMesh = temp.GetComponent<MeshFilter>().mesh;
            temp.name = "lod1";
            renderers1[0] = temp.GetComponent<Renderer>();
            lods[1] = new LOD(0.6F, renderers1);
            genAll++;
        }
        else if (genAll == 18)
        {
            GameObject temp;
            temp = Instantiate(lodMesh, transform);
            temp.GetComponent<MeshFilter>().mesh.Clear();
            temp.GetComponent<MeshFilter>().mesh.vertices = lod4Vertices;
            temp.GetComponent<MeshFilter>().mesh.triangles = lod4MeshTriangles;
            temp.GetComponent<MeshFilter>().mesh.colors = colors4;
            temp.GetComponent<MeshFilter>().mesh.RecalculateBounds();
            temp.GetComponent<MeshFilter>().mesh.RecalculateNormals();
            temp.GetComponent<MeshCollider>().sharedMesh = temp.GetComponent<MeshFilter>().mesh;
            temp.name = "lod2";
            renderers2[0] = temp.GetComponent<Renderer>();
            lods[2] = new LOD(0.4F, renderers2);
            genAll++;
        }
        else if (genAll == 19)
        {
            GameObject temp;
            temp = Instantiate(lodMesh, transform);
            temp.GetComponent<MeshFilter>().mesh.Clear();
            temp.GetComponent<MeshFilter>().mesh.vertices = lod8Vertices;
            temp.GetComponent<MeshFilter>().mesh.triangles = lod8MeshTriangles;
            temp.GetComponent<MeshFilter>().mesh.colors = colors8;
            temp.GetComponent<MeshFilter>().mesh.RecalculateBounds();
            temp.GetComponent<MeshFilter>().mesh.RecalculateNormals();
            temp.GetComponent<MeshCollider>().sharedMesh = temp.GetComponent<MeshFilter>().mesh;
            temp.name = "lod3";
            renderers3[0] = temp.GetComponent<Renderer>();
            lods[3] = new LOD(0.25F, renderers3);
            genAll++;
        }
        else if (genAll == 20)
        {
            GameObject temp;
            temp = Instantiate(lodMesh, transform);
            temp.GetComponent<MeshFilter>().mesh.Clear();
            temp.GetComponent<MeshFilter>().mesh.vertices = lod16Vertices;
            temp.GetComponent<MeshFilter>().mesh.triangles = lod16MeshTriangles;
            temp.GetComponent<MeshFilter>().mesh.colors = colors16;
            temp.GetComponent<MeshFilter>().mesh.RecalculateBounds();
            temp.GetComponent<MeshFilter>().mesh.RecalculateNormals();
            temp.GetComponent<MeshCollider>().sharedMesh = temp.GetComponent<MeshFilter>().mesh;
            temp.name = "lod4";
            renderers4[0] = temp.GetComponent<Renderer>();
            lods[4] = new LOD(0.2f, renderers4);
            genAll++;
        }
        else if (genAll == 21)
        {
            GameObject temp;
            temp = Instantiate(lodMesh, transform);
            temp.GetComponent<MeshFilter>().mesh.Clear();
            temp.GetComponent<MeshFilter>().mesh.vertices = lod32Vertices;
            temp.GetComponent<MeshFilter>().mesh.triangles = lod32MeshTriangles;
            temp.GetComponent<MeshFilter>().mesh.colors = colors32;
            temp.GetComponent<MeshFilter>().mesh.RecalculateBounds();
            temp.GetComponent<MeshFilter>().mesh.RecalculateNormals();
            temp.GetComponent<MeshCollider>().sharedMesh = temp.GetComponent<MeshFilter>().mesh;
            temp.name = "lod5";
            renderers5[0] = temp.GetComponent<Renderer>();
            lods[5] = new LOD(0.15f, renderers5);
            genAll++;
        }
        else if (genAll == 22)
        {
            GameObject temp;
            temp = Instantiate(lodMesh, transform);
            temp.GetComponent<MeshFilter>().mesh.Clear();
            temp.GetComponent<MeshFilter>().mesh.vertices = lod64Vertices;
            temp.GetComponent<MeshFilter>().mesh.triangles = lod64MeshTriangles;
            temp.GetComponent<MeshFilter>().mesh.colors = colors64;
            temp.GetComponent<MeshFilter>().mesh.RecalculateBounds();
            temp.GetComponent<MeshFilter>().mesh.RecalculateNormals();
            temp.GetComponent<MeshCollider>().sharedMesh = temp.GetComponent<MeshFilter>().mesh;
            temp.name = "lod6";
            renderers6[0] = temp.GetComponent<Renderer>();
            lods[6] = new LOD(0.05f, renderers6);
            genAll++;
        }
        else if (genAll == 23)
        {
            GameObject temp;
            temp = Instantiate(lodMesh, transform);
            temp.GetComponent<MeshFilter>().mesh.Clear();
            temp.GetComponent<MeshFilter>().mesh.vertices = lod128Vertices;
            temp.GetComponent<MeshFilter>().mesh.triangles = lod128MeshTriangles;
            temp.GetComponent<MeshFilter>().mesh.colors = colors128;
            temp.GetComponent<MeshFilter>().mesh.RecalculateBounds();
            temp.GetComponent<MeshFilter>().mesh.RecalculateNormals();
            temp.GetComponent<MeshCollider>().sharedMesh = temp.GetComponent<MeshFilter>().mesh;
            temp.name = "lod7";
            renderers7[0] = temp.GetComponent<Renderer>();
            lods[7] = new LOD(0.01f, renderers7);
            genAll++;
        }
        else if (genAll == 24)
        {
            group = gameObject.AddComponent<LODGroup>();

            group.SetLODs(lods);
            group.RecalculateBounds();
            genAll++;
        }
    }
    void changeLod()
    {
        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize / lod; z++)
        {
            for (int x = 0; x < xSize / lod; x++)
            {
                triangles[tris + 0] = (vert + 0);
                triangles[tris + 1] = (vert + xSize * lod + 1 * lod);
                triangles[tris + 2] = (vert + 1 * lod);
                triangles[tris + 3] = (vert + 1 * lod);
                triangles[tris + 4] = (vert + xSize * lod + 1 * lod);
                triangles[tris + 5] = (vert + xSize * lod + 2 * lod);

                vert += lod;
                tris += 6;
            }
            vert += xSize * (lod - 1) + lod;
        }
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;

        //IJob job = new genLODs()
        //{
//
        //};
    }
}

struct genLODs : IJob
{
    float3 pos;
    int size;
    float freqMount;
    float mountHeight;
    NativeArray<float> octaveFreq;
    NativeArray<float> octaveTX;
    NativeArray<float> octaveTZ;
    NativeArray<float> heights;
    NativeArray<Vector3> vertices;
    NativeArray<Color> colors;
    Gradient gradient;
    public void Execute()
    {
        //vertices = new Vector3[(size + 1) * (size + 1)];
        //colors = new Color[(size + 1) * (size + 1)];

        for (int i = 0, z = 0; z <= size; z++)
        {
            for (int x = 0; x <= size; x++)
            {
                float height = 0;
                for (int octave = 1; octave < octaveFreq.Length + 1; octave++)
                {
                    height += Mathf.PerlinNoise((pos.x + x + octaveTX[octave - 1]) * octaveFreq[octave - 1], (pos.z + z + octaveTZ[octave - 1]) * octaveFreq[octave - 1]) * octave / 10 * heights[octave - 1] - 12;
                    //height += noise.snoise(new float2((transform.position.x + x + octaveTX[octave - 1]) * octaveFreq[octave - 1], (transform.position.z + z + octaveTZ[octave - 1]))) *octave / 10 * heights[octave - 1] - 12;
                }
                if (height < 0)
                {
                    height = 0;
                }
                if (height > 30)
                {
                    height -= noise.cellular(new float2((pos.x + x) * freqMount, (pos.z + z) * freqMount)).y * mountHeight * 0.7f * (height - 30) * -1;
                    height -= noise.cellular(new float2((pos.x + x) * freqMount * 3, (pos.z + z) * freqMount * 3)).y * (mountHeight / 20) * (height - 30) * -1;

                }
                vertices[i] = new Vector3(x, height, z);
                colors[i] = gradient.Evaluate(Mathf.InverseLerp(150, 0, height));
                i++;
            }
        }
    }
}