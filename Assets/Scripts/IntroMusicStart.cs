using UnityEngine;

public class IntroMusicStart : MonoBehaviour
{
    [SerializeField] private SoundManager soundManager;
    private void Start()
    {
        soundManager.IntroPlay();
    }
}
