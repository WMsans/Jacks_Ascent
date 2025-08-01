using Michsky.LSS;
using UnityEngine;

public class StartButton : MonoBehaviour
{
    [SerializeField] private LSS_Manager lssManager;
    [SerializeField] private SceneField gameScene;
    public void StartGame()
    {
        lssManager.LoadScene(gameScene);
    }
}
