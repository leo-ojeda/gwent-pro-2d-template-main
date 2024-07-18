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
        cardList.Add(new Card( "Knight Thunderclash", 7, "Leader",  new string[]{"Melee"},  "Torment",new List<EffectActivation>()," "));
    }
    public void Deck2()
    {
        cardList = new List<Card>();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex -2);
        cardList.Add(new Card( "Samurai", 8, "Leader",  new string[]{"Melee"},  "Fire",new List<EffectActivation>()," "));
    }
    public void Deck3()
    {
        cardList = new List<Card>();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex -2);
        cardList.Add(new Card( "Leñador", 7,  "Leader",  new string[]{"Melee"},  "Forest",new List<EffectActivation>()," "));
    }
    IEnumerator Sound()
    {
       yield return new WaitForSeconds(0.4f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }
    IEnumerator Sound1()
    {
        yield return new WaitForSeconds(0.4f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 3);
    }
  
}
