using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basket : MonoBehaviour
{
    [SerializeField] private GameObject ballBackLeft;
    [SerializeField] private GameObject ballBackRight;
    [SerializeField] private GameObject ballFront;
    private GameManager gameManager;
    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        if(gameManager.currentScore == 0)
        {
            ballBackLeft.SetActive(false);
            ballBackRight.SetActive(false);
            ballFront.SetActive(false);
        }
        else if(gameManager.currentScore == 1)
        {
            ballBackLeft.SetActive(true);
            ballBackRight.SetActive(false);
            ballFront.SetActive(false);
        }
        else if (gameManager.currentScore == 2)
        {
            ballBackLeft.SetActive(true);
            ballBackRight.SetActive(true);
            ballFront.SetActive(false);
        }
        else
        {
            ballBackLeft.SetActive(true);
            ballBackRight.SetActive(true);
            ballFront.SetActive(true);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Ball"))
        {
            gameManager.IncreaseScore();
            gameManager.shotsInTarget++;
            gameManager.SpawnBallAndBasket();
        }
    }
}
