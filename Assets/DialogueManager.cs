using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public Canvas canvas;
    public TMP_Text text;
    private string[] txt = new string[20];
    private AudioSource src;
    private AudioClip typenoise;
    public int textIndex = 0;
    private bool isRunning = false;
    private Queue<string> queueBuffer = new Queue<string>(); 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        src = GetComponent<AudioSource>();
        typenoise = Resources.Load<AudioClip>("Audio/text");
        //text = canvas.GetComponentInChildren<TMP_Text>();
        txt[0] = "Hey, thanks for coming in on such short notice";
        txt[1] = "Listen, we've got a simple job for you today";
        txt[2] = "We need you to head down into the system and retrieve a few things the last crew left behind";
        txt[3] = "Just head down and I'll contact you again when you reach the second elevator";
        txt[4] = "Remember, you can press Q to ping your surrounding locations";
        txt[5] = "Watch out though, you might attract some unwanted attention from the wildlife";
        txt[6] = "Alright I can see you're getting close. Just be careful around the spike pit, we had a guy fall in last week and it was a real mess to clean up";
        txt[7] = "Damn old rust bucket, the elevator's stuck. Give me a second I'll see what I can do";
        txt[8] = "........................";
        txt[9] = "..........-----llo?------....----ou hear m-----....?";
        txt[10] = "What the hell is-----........JOIN US........";
        txt[11] = ".......................";
        txt[12] = "JOIN US JOIN US JOIN US JOIN US JOIN US";
        txt[13] = "WE WILL BECOME ONE";

    }

    // Update is called once per frame
    void Update()
    {
       

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DialogueTrigger"))
        {
            //if (!isRunning)
            //{
            
            queueBuffer.Enqueue(txt[textIndex]);
            textIndex++;
            if (!isRunning)
            {
                StartCoroutine(TypeText());
            }
            //}
            //else
            //{

            //}
             Destroy(other.gameObject);

        }
    }

    private IEnumerator TypeText()
    {
        isRunning = true;
        while (queueBuffer.Count > 0)
        {
            string currentMessage = queueBuffer.Dequeue();
            text.text = "";
            foreach (char c in currentMessage)
            {
                if(textIndex >= 10){
                    src.pitch = 0.5f;
                }
                else
                {
                    src.pitch = Random.Range(0.9f, 1.1f);
                }
                    
                src.PlayOneShot(typenoise);
                text.text += c;
                if (textIndex >= 10)
                {
                    yield return new WaitForSeconds(0.22f);
                }
                else
                {
                    yield return new WaitForSeconds(0.05f);
                }

            }

            yield return new WaitForSeconds(2f);
        }
        text.text = "";
        
        isRunning = false;
    }
}
