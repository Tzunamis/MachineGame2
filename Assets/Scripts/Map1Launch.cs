using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Map1Launch : MonoBehaviour
{
    public void mapOne()
    {
        SceneManager.LoadScene("TriangleTestSceneTristan");
        SceneManager.UnloadSceneAsync("StartMenu");
    }

    public void mapTwo()
    {
        SceneManager.LoadScene("CompetitiveScene");
        SceneManager.UnloadSceneAsync("StartMenu");
    }

    public void mapThree()
    {
        SceneManager.LoadScene("NewLevelDiffrent");
        SceneManager.UnloadSceneAsync("StartMenu");
    }

    public void mapMetrics()
    {
        SceneManager.LoadScene("MetricGym");
        SceneManager.UnloadSceneAsync("StartMenu");
    }

    public void exitGame()
    {
        //Debug.Log("QUIT!");
        Application.Quit();
    }
}
