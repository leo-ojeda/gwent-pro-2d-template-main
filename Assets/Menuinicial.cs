using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menuinicial : MonoBehaviour
{
    public static List<Card> cardList = new List<Card>();
    private AudioSource AudioSource;
    public void Jugar()
    {
        AudioSource = GetComponent<AudioSource>();
        AudioSource.Play();
        StartCoroutine(Sound1());
    }
    public void Card()
    {
        AudioSource = GetComponent<AudioSource>();
        AudioSource.Play();
        StartCoroutine(Sound());
    }
    public void Atras()
    {
        AudioSource = GetComponent<AudioSource>();
        AudioSource.Play();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2);
    }
    public void Terminar()
    {
        AudioSource = GetComponent<AudioSource>();
        AudioSource.Play();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
    public void Salir()
    {
        AudioSource = GetComponent<AudioSource>();
        AudioSource.Play();
        Debug.Log("Salir...");
        Application.Quit();
    }

    public void Deck1()
    {
        cardList = new List<Card>();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex -2);
        cardList.Add(new Card(9, "Knight Thunderclash", 7, "4", "Leader", Resources.Load<Sprite>("13"), "Leader", 1, "Torment"));
    }
    public void Deck2()
    {
        cardList = new List<Card>();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex -2);
        cardList.Add(new Card(29, "Samurai", 8, "1", "Leader", Resources.Load<Sprite>("32"), "Leader", 1, "Fire"));
    }
    public void Deck3()
    {
        cardList = new List<Card>();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex -2);
        cardList.Add(new Card(40, "Leñador", 7, "1", "Leader", Resources.Load<Sprite>("43"), "Leader", 1, "Forest"));
    }
    IEnumerator Sound()
    {
       yield return new WaitForSeconds(0.3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }
    IEnumerator Sound1()
    {
        yield return new WaitForSeconds(0.3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 3);
    }
  
}
