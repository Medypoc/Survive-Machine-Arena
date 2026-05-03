using UnityEngine;
using UnityEngine.SceneManagement;

public class HubManager : MonoBehaviour
{
    public void StartArena()
    {
        // Убедись, что сцена с ареной добавлена в Build Settings
        SceneManager.LoadScene("ArenaScene"); 
    }
}