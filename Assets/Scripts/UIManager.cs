using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject gameCamera;
    public GameObject mainUI;
    public GameObject gameUI;
    public Animator spinner;


    public List<Element> collectedPotions = new List<Element>(); // Track collected potions

    public void PauseTime() {
        Time.timeScale = 0;
    }

    public void UnpauseTime() {
        Time.timeScale = 1;
    }

    public void SaveAndQuit() {
        SceneManager.LoadScene("Home");
        Time.timeScale = 1;
    }

    public void StopSpinner() {
        spinner.SetBool("isPlaying", true);
        gameCamera.SetActive(true);
        mainCamera.SetActive(false);
        mainUI.SetActive(false);

        // Start the Wait coroutine
        StartCoroutine(WaitAndEnableGameUI());
    }

    // Coroutine to wait and then enable the game UI after the spinner animation
    private IEnumerator WaitAndEnableGameUI() {
        // Wait for 5 seconds
        yield return new WaitForSeconds(2f);

        // After the wait, enable the game UI
        gameUI.SetActive(true);
    }
}
