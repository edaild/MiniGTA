using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathUI : MonoBehaviour
{
    public void RestartScene()
    {
        Time.timeScale = 1f; // Á×À» ¶§ ¸ØÃè´Ù¸é ¹Ýµå½Ã º¹±¸
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
