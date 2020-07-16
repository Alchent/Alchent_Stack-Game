using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    private CuboidGenerator generator;
    public TextMeshProUGUI scoreText;
    public float widthStart = 10.0f;
    public float heightStart = 10.0f;
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
        generator.GenerateCuboid(widthStart, heightStart, score, startMoveX);
        scoreText.text = score.ToString();
    }
}
