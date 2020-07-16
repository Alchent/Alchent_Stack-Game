using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuboidGenerator : MonoBehaviour
{
    
    public float cuboidHeight = 1.5f;
    public Cuboid previousCuboid;
    public Cuboid currentCuboid;
    public Cuboid cuboidPrefab;


    public void GenerateCuboid(float width, float depth, int score, bool moveX)
    {
        currentCuboid = Instantiate<Cuboid>(cuboidPrefab);
        currentCuboid.transform.position = previousCuboid.transform.position;
        currentCuboid.SetUp(width, depth, score * cuboidHeight, moveX, cuboidHeight);
        currentCuboid.Generate(true);
        currentCuboid.MoveToMaxMove();
    }
    
}
