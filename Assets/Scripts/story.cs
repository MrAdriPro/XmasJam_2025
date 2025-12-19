using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class story : MonoBehaviour
{
    public void ChangeScene()
    {
        SceneManager.LoadScene("Lvl1");
    }

}
