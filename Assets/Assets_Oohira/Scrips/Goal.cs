using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] private GameManager gameManager = null;

    private void Start()
    {
        gameManager = GameManager.instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Ball") return;
        Debug.Log("Goal!");

        if (GetComponent<Owner>().player == Owners.player0)
        {
            gameManager.Goal(Owners.player0);
        }
        else
        {
            gameManager.Goal(Owners.player1);
        }
    }
}
