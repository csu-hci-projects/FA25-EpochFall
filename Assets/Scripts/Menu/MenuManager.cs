using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject optionsPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (mainPanel != null)
        {
            mainPanel.SetActive(true);
        }

        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartGame()
    {
        Debug.Log("Start button pressed! (Will not start in editor)");
        // replace with actual starting scene name
        SceneManager.LoadScene("Level One");
    }

    public void OpenOptions()
    {
        if (mainPanel != null)
        {
            mainPanel.SetActive(false);
        }

        if (optionsPanel != null)
        {
            optionsPanel.SetActive(true);
        }
    }

    public void CloseOptions()
    {
        if (mainPanel != null)
        {
            mainPanel.SetActive(true);
        }

        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quit button pressed! (Will not quit in editor)");
        Application.Quit();
    }
}
