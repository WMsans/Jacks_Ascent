using System;
using UnityEngine;

public class LevelMusicStart : MonoBehaviour
{
    [SerializeField] private SoundManager soundManager;
    private void Start()
    {
        soundManager.IntroStop();
        soundManager.BackgroundStart();
    }
}
