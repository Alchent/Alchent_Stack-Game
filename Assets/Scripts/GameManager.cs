using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    public const float GAME_DIFICULTY = .1f;
    private CuboidGenerator generator;
    public TextMeshProUGUI scoreText;
    public float widthStart = 10.0f;
    public float depthStart = 10.0f;
    public bool startMoveX = true;
    public bool moveX = true;
    public int score = 0;

    void Start()
    {
        StartGame();
    }

    void Update()
    {
        
    }

    public void StartGame()
    {
        generator = FindObjectOfType<CuboidGenerator>();
        generator.GenerateCuboid(widthStart, depthStart, score, startMoveX);
        scoreText.text = score.ToString();
    }
}
