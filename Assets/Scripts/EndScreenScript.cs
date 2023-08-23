using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenScript : MonoBehaviour
{
    [SerializeField] AudioSource blip;
    public void exit()
    {
        Application.Quit();
        blip.pitch = Random.Range(0.8f, 1.2f);
        blip.Play();
    }
    public void Menu()
    {
        SceneManager.LoadScene(0);
        blip.pitch = Random.Range(0.8f, 1.2f);
        blip.Play();
    }
}
