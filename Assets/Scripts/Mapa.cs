using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mapa : MonoBehaviour
{
    public GameObject[,] mapPoints = new GameObject[27,30];
    bool didStartDeath = false;
    GameObject[] ghosts;
    GameObject pacMan;
    private SoundManager soundManager;
    void Awake()
    {
        InitializeMapPoints();
	}

    private void Start()
    {
        ghosts = GameObject.FindGameObjectsWithTag("Ghost");
        pacMan = GameObject.FindGameObjectWithTag("Pacman");
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }

    public void Restart()
    {
        pacMan.GetComponent<PacWoman>().Restart();
        pacMan.GetComponent<PacWoman>().lives--;
        foreach (GameObject ghostElement in ghosts)
        {
            ghostElement.GetComponent<Ghost>().Restart();
        }
        didStartDeath = false;
        soundManager.PlayClipOnLoop(soundManager.ghostAS, soundManager.ghostMove);
    }

    public void StartDeath()
    {
        if (!didStartDeath)
        {
            didStartDeath = true;
            foreach (GameObject ghostElement in ghosts)
            {
                ghostElement.GetComponent<Ghost>().canMove = false;
            }
            pacMan.GetComponent<PacWoman>().canMove = false;
            pacMan.GetComponent<Animator>().enabled = false;
            soundManager.ghostAS.Stop();
            //Stop background music

            StartCoroutine(ProcessDeathAfter(2.0f));
        }
    }

    IEnumerator ProcessDeathAfter(float _delay)
    {
        yield return new WaitForSeconds(_delay);
        foreach (GameObject ghostElement in ghosts)
        {
            ghostElement.GetComponent<SpriteRenderer>().enabled = false;
        }
        StartCoroutine(ProcessDeathAnimation(1.9f));
    }

    IEnumerator ProcessDeathAnimation(float _delay)
    {
        pacMan.GetComponent<Animator>().runtimeAnimatorController = pacMan.GetComponent<PacWoman>().pacManDeath;
        pacMan.GetComponent<Animator>().enabled = true;
        soundManager.oneShotAS.PlayOneShot(soundManager.pacManDies);

        yield return new WaitForSeconds(_delay);
        StartCoroutine(ProcessRestart(2));
    }

    IEnumerator ProcessRestart(float _delay)
    {
        pacMan.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(_delay);
        Restart();
    }

    void InitializeMapPoints()
    {
        Object[] allObjects = FindObjectsOfType(typeof(GameObject));

        foreach (GameObject GO in allObjects)
        {
            if (GO.CompareTag("TurningPoints"))
            {
                mapPoints[(int)GO.transform.position.x, (int)GO.transform.position.y] = GO;
            }
        }
    }
}
