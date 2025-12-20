using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Needed to load Main Menu
using TMPro;
using System.Collections;

public class OutroSequence : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera mainCamera;
    public float initialDelay = 3.0f; // Wait time after boss dies
    public float rotationSpeed = 15.0f;
    public float riseSpeed = 2.0f;

    [Header("Credits Content")]
    public string gameTitle = "DEAD SECTOR";
    [TextArea] public string[] developerNames; // Type names here in Inspector
    public string thankYouMessage = "Thank You For Playing";

    [Header("Sequence Settings")]
    public int mainMenuBuildIndex = 0; // Usually 0 is Main Menu
    public float fadeDuration = 1.5f;
    public float displayDuration = 3.0f;

    // Internal UI References
    private Canvas outroCanvas;
    private Image blackScreenPanel;
    private TextMeshProUGUI centerTextObj; // We will reuse this single text object

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        CreateOutroUI();
    }

    public void StartOutro()
    {
        StartCoroutine(PlayOutroRoutine());
    }

    IEnumerator PlayOutroRoutine()
    {
        // 1. INITIAL WAIT (Watch boss die)
        yield return new WaitForSeconds(initialDelay);

        Debug.Log("🎬 OUTRO SEQUENCE STARTED");

        // 2. DETACH CAMERA
        if (mainCamera != null)
        {
            mainCamera.transform.SetParent(null);

            // Disable gameplay scripts
            MonoBehaviour[] scripts = mainCamera.GetComponents<MonoBehaviour>();
            foreach (var s in scripts)
            {
                if (!(s is AudioListener) && !(s is Camera) && s != this)
                    s.enabled = false;
            }
        }

        // 3. START CAMERA MOTION (Parallel)
        StartCoroutine(CameraMotionRoutine());

        // 4. START TEXT SEQUENCE

        // A. Fade to Black
        yield return FadeCanvasGroup(blackScreenPanel, 0f, 1f, fadeDuration);
        yield return new WaitForSeconds(0.5f);

        // B. Show Title: "DEAD SECTOR"
        centerTextObj.text = gameTitle;
        centerTextObj.fontSize = 80;
        yield return FadeTextInOut(centerTextObj, displayDuration);

        // C. Show Credits: "A Game By..."
        string names = string.Join("\n", developerNames);
        centerTextObj.text = $"A Game By\n\n<size=60>{names}</size>";
        centerTextObj.fontSize = 40; // Base size for "A Game By"
        yield return FadeTextInOut(centerTextObj, displayDuration + 1.0f); // Give extra time to read names

        // D. Show: "Thank You"
        centerTextObj.text = thankYouMessage;
        centerTextObj.fontSize = 60;
        yield return FadeTextInOut(centerTextObj, displayDuration);

        yield return new WaitForSeconds(1.0f);

        // 5. LOAD MAIN MENU
        Debug.Log("🏠 Returning to Main Menu...");
        SceneManager.LoadScene(mainMenuBuildIndex);
    }

    IEnumerator CameraMotionRoutine()
    {
        if (mainCamera == null) yield break;

        Quaternion startRot = mainCamera.transform.rotation;
        // Target: Look straight up
        Quaternion targetRot = Quaternion.Euler(-90f, startRot.eulerAngles.y, 0f);

        while (true)
        {
            // Rotate Up
            mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, targetRot, Time.deltaTime * 0.5f);
            // Float Up
            mainCamera.transform.position += Vector3.up * riseSpeed * Time.deltaTime;

            yield return null;
        }
    }

    // --- FADE HELPERS ---

    // Fades Text IN -> WAITS -> Fades OUT
    IEnumerator FadeTextInOut(TextMeshProUGUI txt, float holdTime)
    {
        // Fade IN
        yield return FadeText(txt, 0f, 1f, fadeDuration);

        // Hold
        yield return new WaitForSeconds(holdTime);

        // Fade OUT
        yield return FadeText(txt, 1f, 0f, fadeDuration);

        // Small pause between texts
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator FadeText(TextMeshProUGUI txt, float startAlpha, float endAlpha, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(startAlpha, endAlpha, t / duration);
            txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, a);
            yield return null;
        }
        txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, endAlpha);
    }

    IEnumerator FadeCanvasGroup(Image img, float startAlpha, float endAlpha, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(startAlpha, endAlpha, t / duration);
            img.color = new Color(0, 0, 0, a);
            yield return null;
        }
        img.color = new Color(0, 0, 0, endAlpha);
    }

    // --- UI BUILDER ---
    void CreateOutroUI()
    {
        // 1. Create Canvas
        GameObject canvasObj = new GameObject("OutroCanvas");
        outroCanvas = canvasObj.AddComponent<Canvas>();
        outroCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        outroCanvas.sortingOrder = 999;
        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObj.AddComponent<GraphicRaycaster>();

        // 2. Create Black Panel
        GameObject panelObj = new GameObject("BlackScreen");
        panelObj.transform.SetParent(canvasObj.transform, false);
        blackScreenPanel = panelObj.AddComponent<Image>();
        blackScreenPanel.color = new Color(0, 0, 0, 0); // Start transparent

        RectTransform panelRect = blackScreenPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;

        // 3. Create SINGLE Center Text Object (We will change its content)
        GameObject txtObj = new GameObject("CenterText");
        txtObj.transform.SetParent(panelObj.transform, false);
        centerTextObj = txtObj.AddComponent<TextMeshProUGUI>();

        // Default Styling
        centerTextObj.alignment = TextAlignmentOptions.Center;
        centerTextObj.color = new Color(1, 1, 1, 0); // Start invisible
        centerTextObj.enableWordWrapping = false;

        // Center it perfectly
        RectTransform rt = centerTextObj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0.5f);
        rt.anchorMax = new Vector2(1, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(0, 500); // Tall enough for list of names
        rt.anchoredPosition = Vector2.zero;
    }
}