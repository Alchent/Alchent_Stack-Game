using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuboidGenerator : MonoBehaviour
{
    public Cuboid previousCuboid;
    public Cuboid currentCuboid;
    public Cuboid cuboidPrefab;


    public void GenerateCuboid(float width, float height, int score, bool moveX)
    {
        currentCuboid = Instantiate<Cuboid>(cuboidPrefab);
        currentCuboid.transform.position = previousCuboid.transform.position;
        currentCuboid.SetUp(width, height, score, moveX);
        currentCuboid.Generate(true);
        currentCuboid.MoveToMaxMove();
    }
    
}
