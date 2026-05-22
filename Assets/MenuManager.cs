using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public CanvasGroup Title;
    public CanvasGroup PlayButton;
    public CanvasGroup OptionsButton;
    public CanvasGroup ExitButton;
    private float opacityTime = 5f;
    private float timer = 0;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        Title.alpha = Mathf.Lerp(0f, 1f, timer+1 / opacityTime);
        PlayButton.alpha = Mathf.Lerp(0f, 1f, timer-1 / opacityTime);
        OptionsButton.alpha = Mathf.Lerp(0f, 1f, timer-2/ opacityTime);
        ExitButton.alpha = Mathf.Lerp(0f, 1f, timer-3 / opacityTime);
        
    }

    public void OnClickStart()
    {
        SceneManager.LoadScene("game");
    }

    public void OnClickOptions()
    {

    }
    public void OnClickExit()
    {
        Application.Quit();
    }
    
}
