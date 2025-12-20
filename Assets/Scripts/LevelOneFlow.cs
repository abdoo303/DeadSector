using UnityEngine;
using System.Collections;

public class LevelOneFlow : MonoBehaviour
{
    [Header("Level 1 Intro Settings")]
    public Camera mainCamera;
    public Transform camStartPos;
    public Transform camEndPos;
    public Transform houseTarget;
    public float panDuration = 5.0f;

    [Header("Level 1 Gameplay")]
    public GameObject player;
    public string zombieTag = "Enemy";
    public GameObject level1Zombies;

    [Header("Debug")]
    [Tooltip("Check this BEFORE play to start at Boss, or DURING play to skip.")]
    public bool forceLevelComplete = false;

    // Internal State
    private LevelTwoFlow levelTwoScript;
    private bool isFinished = false;

    // Camera Restore State
    private Transform originalCamParent;
    private Vector3 originalCamPos;
    private Quaternion originalCamRot;

    void Start()
    {
        levelTwoScript = GetComponent<LevelTwoFlow>();

        // 1. PRIORITY CHECK: Is Force Complete checked? OR Is Save Data Level 2?
        int savedLevel = PlayerPrefs.GetInt("SavedLevel", 1);

        if (savedLevel == 2 || forceLevelComplete)
        {
            Debug.Log("⏩ Skipping Level 1 (Force Complete or Save File)");

            // Ensure we don't accidentally start the coroutine
            DisableLevel1();

            // Jump straight to Level 2
            if (levelTwoScript != null) levelTwoScript.StartBossLevel();
        }
        else
        {
            // START Level 1 with the Intro Pan
            StartCoroutine(PlayLevel1Intro());
        }
    }

    IEnumerator PlayLevel1Intro()
    {
        Debug.Log("🎬 Level 1 Intro: Panning Camera...");

        // A. SETUP CAMERA
        if (mainCamera != null && camStartPos != null)
        {
            Transform camTrans = mainCamera.transform;

            // 1. Save Player Camera position
            originalCamParent = camTrans.parent;
            originalCamPos = camTrans.localPosition;
            originalCamRot = camTrans.localRotation;

            // 2. Detach Camera
            camTrans.SetParent(null);
            camTrans.position = camStartPos.position;
            camTrans.rotation = camStartPos.rotation;
        }

        // B. DISABLE PLAYER CONTROLS
        SetPlayerControls(false);

        // C. PANNING ANIMATION
        float timer = 0f;
        while (timer < panDuration)
        {
            // CRITICAL: If user checks box MID-CUTSCENE, stop this loop immediately
            if (forceLevelComplete) yield break;

            timer += Time.deltaTime;
            float t = timer / panDuration;

            if (mainCamera != null && camEndPos != null)
            {
                mainCamera.transform.position = Vector3.Lerp(camStartPos.position, camEndPos.position, t);

                if (houseTarget != null)
                    mainCamera.transform.LookAt(houseTarget);
                else
                    mainCamera.transform.rotation = Quaternion.Lerp(camStartPos.rotation, camEndPos.rotation, t);
            }
            yield return null;
        }

        // D. RESTORE CAMERA TO PLAYER
        RestoreCamera();

        // E. ENABLE PLAYER CONTROLS
        SetPlayerControls(true);

        // F. ACTIVATE ZOMBIES
        if (level1Zombies != null) level1Zombies.SetActive(true);

        Debug.Log("🏁 Level 1 Start!");
    }

    void Update()
    {
        if (isFinished) return;

        // Check if we skipped (mid-game) or finished normally
        if (forceLevelComplete || (GameObject.FindGameObjectsWithTag(zombieTag).Length == 0))
        {
            CompleteLevel();
        }
    }

    void CompleteLevel()
    {
        if (isFinished) return;
        isFinished = true;

        Debug.Log("✅ Level 1 Complete!");

        // 1. STOP THE INTRO if it's still running
        StopAllCoroutines();

        // 2. FORCE RESTORE CAMERA (In case we skipped mid-intro)
        RestoreCamera();

        // 3. Save & Transition
        PlayerPrefs.SetInt("SavedLevel", 2);
        PlayerPrefs.Save();

        if (levelTwoScript != null)
        {
            levelTwoScript.StartBossLevel();
        }

        DisableLevel1();
    }

    // Helper to safely restore camera
    void RestoreCamera()
    {
        if (mainCamera != null && originalCamParent != null)
        {
            mainCamera.transform.SetParent(originalCamParent);
            mainCamera.transform.localPosition = originalCamPos;
            mainCamera.transform.localRotation = originalCamRot;
        }
    }

    // Helper to toggle player scripts
    void SetPlayerControls(bool state)
    {
        if (player != null)
        {
            MonoBehaviour[] playerScripts = player.GetComponents<MonoBehaviour>();
            foreach (var script in playerScripts) script.enabled = state;

            var weapon = player.GetComponentInChildren<SwordDamage>();
            if (weapon != null) weapon.enabled = state;
        }
    }

    void DisableLevel1()
    {
        if (level1Zombies != null) level1Zombies.SetActive(false);
        this.enabled = false;
    }
}