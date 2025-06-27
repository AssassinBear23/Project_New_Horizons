using UnityEngine;
using System.Collections;
public class BackgroundMusic : MonoBehaviour
{
    [SerializeField] private AudioSource intro;
    [SerializeField] private AudioSource looping;
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(intro.clip.length);
        looping.Play();
    }
}