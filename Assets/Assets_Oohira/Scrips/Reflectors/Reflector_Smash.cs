using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reflector_Smash : MonoBehaviour
{
    GameObject ball = null;
    Ball_BasicMove ballMove = null;
    [SerializeField] GameObject[] goals_Array = null;
    [SerializeField] List<GameObject> goals_List = null;
    [SerializeField] float smashSpeed = 20;

    private void Start()
    {
        Init_Smash();
    }

    private void Init_Smash()
    {
        goals_Array = GameObject.FindGameObjectsWithTag("Goal");
        foreach (GameObject goal in goals_Array)
        {
            if (goal.GetComponent<Owner>().player != GetComponent<Owner>().player)
                goals_List.Add(goal);
        }
        ball = GameObject.FindGameObjectWithTag("Ball");
        ballMove = ball.GetComponent<Ball_BasicMove>();
    }

    public void Smash()
    {
        Debug.Log("ÉSÅ[ÉãÇÃêî" + goals_List.Count);
        int index_Random = Random.Range(0, goals_List.Count);
        Vector3 goalPosition_Random = goals_List[index_Random].transform.position;

        Vector3 SmashDirection_Random = goalPosition_Random - ball.transform.position;
        goals_List.Clear();

        ball.GetComponent<Rigidbody>().velocity = SmashDirection_Random * smashSpeed;
        ballMove.changeSpeed = smashSpeed;
    }
    private IEnumerator A(float smashTime)
    {
        yield return new WaitForSeconds(smashTime);
    }
}
