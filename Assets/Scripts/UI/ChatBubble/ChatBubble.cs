using System.Collections;
using TMPro;
using UnityEngine;

public class ChatBubble : MonoBehaviour
{
    private SpriteRenderer chatBubbleBackgroundSpriteRenderer;
    private TextMeshPro chatMessage;

    private Coroutine hideChatBubbleCoroutine;

    private void Awake()
    {
        chatBubbleBackgroundSpriteRenderer = transform.Find("ChatBubbleBackground").GetComponent<SpriteRenderer>();
        chatMessage = transform.Find("ChatMesage").GetComponent<TextMeshPro>();
    }

    private void Start()
    {
        
    }

    public void ChatBubbleSetChatMessage(string text)
    {
        if (hideChatBubbleCoroutine != null)
        {
            StopCoroutine(hideChatBubbleCoroutine);
        }

        hideChatBubbleCoroutine = StartCoroutine(HideChatAfterDelay());

        chatMessage.text = text;
        chatMessage.ForceMeshUpdate();
        Vector2 chatMessageTextSize = chatMessage.GetRenderedValues(false);

        Vector2 padding = new Vector2(3.0f, 2.0f);
        chatBubbleBackgroundSpriteRenderer.size = chatMessageTextSize + padding;        
    }

    private IEnumerator HideChatAfterDelay()
    {
        yield return new WaitForSeconds(2f);

        gameObject.SetActive(false);
        
        hideChatBubbleCoroutine = null;
    }
}