using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerController : MonoBehaviour
{
    public CuboidGenerator generator;
    private GameManager gameManager;
    public Camera mainCamera;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            generator.currentCuboid.Drop();
            
            if( generator.currentCuboid.Separate())
            {
                gameManager.score++;
                gameManager.scoreText.text = gameManager.score.ToString();
                gameManager.moveX = !gameManager.moveX;

                generator.previousCuboid = generator.currentCuboid;
                generator.GenerateCuboid(generator.previousCuboid.width, generator.previousCuboid.height,
                                            gameManager.score, gameManager.moveX);

                mainCamera.transform.position += Vector3.up;
            }
            else
            {
                //game loose
            }
            
        }
    }

   
}
