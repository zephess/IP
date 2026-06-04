using TMPro;


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
    public Slider slider;
    public CanvasRenderer optionsMenu;
    public TMP_Text difficultyText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        slider.onValueChanged.AddListener(delegate { ChangeDifficulty(); });
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime; 
        Title.alpha = Mathf.Lerp(0f, 1f, timer+1 / opacityTime); // Title fades in first, so it starts at timer+1 to give it a head start
        PlayButton.alpha = Mathf.Lerp(0f, 1f, timer-1 / opacityTime); // Play button fades in second, so it starts at timer-1 to give it a head start after the title
        OptionsButton.alpha = Mathf.Lerp(0f, 1f, timer-2/ opacityTime); // Options button fades in third, so it starts at timer-2 to give it a head start after the play button
        ExitButton.alpha = Mathf.Lerp(0f, 1f, timer-3 / opacityTime); // Exit button fades in last, so it starts at timer-3 to give it a head start after the options button
        
    }

    public void OnClickStart() // This function is called when the play button is clicked. It loads the game scene.
    {
        SceneManager.LoadScene("game");
    }

    public void OnClickOptions() // This function is called when the options button is clicked. It shows the options menu.
    {
        optionsMenu.gameObject.SetActive(true);
    }
    public void OnExitOptions() // This function is called when the back button in the options menu is clicked. It hides the options menu.
    {
        optionsMenu.gameObject.SetActive(false);
    }
    public void OnClickExit() // This function is called when the exit button is clicked. It quits the application.
    {
        Application.Quit();
    }

    public void ChangeDifficulty() // This function is called when the difficulty slider value is changed. It updates the game difficulty settings.
    {
        switch(slider.value)
        {
            case 0:
                SonarPulseManager.Instance.SetPulseSpeed(0f);
                SonarPulseManager.Instance.SetPulseWidth(400f);
                difficultyText.text = "Difficulty: Peaceful\nSonar will light up the entire cave. Not recommended for players that want the full experience.";
                break;
            case 1:
                SonarPulseManager.Instance.SetPulseSpeed(10f);
                SonarPulseManager.Instance.SetPulseWidth(10f);
                difficultyText.text = "Difficulty: Easy\nSonar will light up a moderate area and travel slowly. Suitable for players who want an easy experience.";
                break;
            case 2:
                SonarPulseManager.Instance.SetPulseSpeed(15f);
                SonarPulseManager.Instance.SetPulseWidth(7.5f);
                difficultyText.text = "Difficulty: Medium\nSonar will light up a smaller area and travel at a moderate speed. The intended way to play the game.";
                break;
            case 3:
                SonarPulseManager.Instance.SetPulseSpeed(30f);
                SonarPulseManager.Instance.SetPulseWidth(7.5f);
                difficultyText.text = "Difficulty: Hard\nSonar will light up a very small area and travel quickly. Suitable for players seeking a challenging experience.";
                break;
            case 4:
                SonarPulseManager.Instance.SetPulseSpeed(60f);
                SonarPulseManager.Instance.SetPulseWidth(7.5f);
                difficultyText.text = "Difficulty: Insane\nSonar will light up an extremely small area and travel at an extremely fast speed. Not recommended.";
                break;
            }
        }
    
}
