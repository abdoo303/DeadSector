using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [Header("GAME STATE CONTROL")]
    [Tooltip("Set this to 1 or 2. Change it while playing to skip levels.")]
    [Range(1, 2)]
    public int currentLevel = 1;

    [Tooltip("If true, the game will load your last Save File instead of using the number above.")]
    public bool useSaveData = true;

    [Header("GLOBAL REFERENCES")]
    public Camera mainCamera;
    public Transform player;
    public Transform gameplayCameraParent; // Where camera sits during gameplay (e.g. Player Head)

    [Header("LEVEL 1 SETUP (The House)")]
    public Transform camStartPos;
    public Transform camEndPos;
    public Transform houseTarget;
    public GameObject level1Zombies;
    public float panDuration = 5.0f;

    [Header("Level 1 Completion Requirements")]
    [Tooltip("Player must pick up the gun to progress to Level 2")]
    public bool requireGunPickup = true;

    [Header("LEVEL 2 SETUP (The Boss)")]
    public Transform bossSpawnPoint;
    public GameObject boss;
    public Transform bossFaceAnchor;
    public float introDuration = 4.0f;

    // Internal State
    private int _internalLevelTracker;
    private bool isLevel1Active = false;
    private bool hasPickedUpGun = false;

    // Camera Restore Data
    private Transform originalCamParent;
    private Vector3 originalCamPos;
    private Quaternion originalCamRot;
    private MonoBehaviour[] cameraScripts; // Store scripts here

    void Awake()
    {
        if (mainCamera != null)
        {
            mainCamera.transform.SetParent(null);
            // 1. POPULATE CAMERA SCRIPTS IMMEDIATELY
            // This ensures we have them ready for Level 1 OR Level 2
            cameraScripts = mainCamera.GetComponents<MonoBehaviour>();
        }

        if (useSaveData)
        {
            currentLevel = PlayerPrefs.GetInt("SavedLevel", currentLevel);
        }

        _internalLevelTracker = currentLevel;

        if (currentLevel == 2)
        {
            StartLevel2();
            return;
        }

        StartLevel1();
    }


    void Update()
    {
        // A. LIVE INSPECTOR CHANGE CHECK
        if (currentLevel != _internalLevelTracker)
        {
            _internalLevelTracker = currentLevel;

            if (currentLevel == 2 && isLevel1Active)
            {
                CompleteLevel1();
                return;
            }
        }

        // B. LEVEL 1 GAMEPLAY CHECK
        if (isLevel1Active)
        {
            // Check if player has completed level objectives
            bool zombiesCleared = GameObject.FindGameObjectsWithTag("Enemy").Length == 0;
            bool canProgress = zombiesCleared && (!requireGunPickup || hasPickedUpGun);

            if (canProgress)
            {
                currentLevel = 2;
                _internalLevelTracker = 2;
                CompleteLevel1();
            }
        }
    }

    // Call this from GunPickup script when player picks up the gun
    public void OnGunPickedUp()
    {
        hasPickedUpGun = true;
        Debug.Log("✅ Gun picked up! Can now progress when zombies are cleared.");
    }

    // ================= LEVEL 1 LOGIC =================

    void StartLevel1()
    {
        if (currentLevel != 1) return;

        Debug.Log("🏁 STARTING LEVEL 1");
        isLevel1Active = true;
        currentLevel = 1;

        if (boss != null) boss.SetActive(false);

        StartCoroutine(Level1IntroRoutine());
    }

    IEnumerator Level1IntroRoutine()
    {
        // 1. DISABLE CAMERA SCRIPTS (Fixes conflict)
        ToggleCameraScripts(false);

        // 2. HIJACK CAMERA
        if (mainCamera != null && camStartPos != null)
        {
            Transform cam = mainCamera.transform;
            originalCamParent = cam.parent;
            originalCamPos = cam.localPosition;
            originalCamRot = cam.localRotation;

            cam.SetParent(null);
            cam.position = camStartPos.position;
            cam.rotation = camStartPos.rotation;
        }

        TogglePlayerControls(false);

        // 3. PAN CAMERA
        float timer = 0f;
        while (timer < panDuration)
        {
            if (currentLevel == 2) yield break;

            timer += Time.deltaTime;
            float t = timer / panDuration;

            if (mainCamera != null && camEndPos != null)
            {
                mainCamera.transform.position = Vector3.Lerp(camStartPos.position, camEndPos.position, t);

                if (houseTarget != null) mainCamera.transform.LookAt(houseTarget);
                else mainCamera.transform.rotation = Quaternion.Lerp(camStartPos.rotation, camEndPos.rotation, t);
            }
            yield return null;
        }

        // 4. RESTORE
        RestoreCameraToPlayer();
        TogglePlayerControls(true);
        ToggleCameraScripts(true); // Re-enable camera scripts

        if (level1Zombies != null) level1Zombies.SetActive(true);
    }

    void CompleteLevel1()
    {
        Debug.Log("✅ LEVEL 1 COMPLETE");
        isLevel1Active = false;
        StopAllCoroutines();

        PlayerPrefs.SetInt("SavedLevel", 2);
        PlayerPrefs.Save();

        StartLevel2();
    }

    // ================= LEVEL 2 LOGIC =================

    void StartLevel2()
    {
        Debug.Log("🚀 STARTING LEVEL 2");
        currentLevel = 2;

        if (level1Zombies != null) level1Zombies.SetActive(false);

        // Teleport player
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        player.position = bossSpawnPoint.position;
        player.rotation = bossSpawnPoint.rotation;

        if (cc != null) cc.enabled = true;

        if (boss != null) boss.SetActive(true);

        StartCoroutine(Level2IntroRoutine());
    }


    IEnumerator Level2IntroRoutine()
    {
        if (currentLevel != 2) yield break;

        yield return null;

        Debug.Log("🎬 LEVEL 2 CUTSCENE START");

        // 1. DISABLE CONTROLS (Player AND Camera)
        TogglePlayerControls(false);
        ToggleCameraScripts(false); // <--- THIS WAS MISSING!

        // 2. HIJACK CAMERA
        Transform cam = mainCamera.transform;

        originalCamParent = cam.parent;
        originalCamPos = cam.localPosition;
        originalCamRot = cam.localRotation;

        // Snap to boss face
        cam.SetParent(null);
        cam.position = bossFaceAnchor.position;
        cam.rotation = bossFaceAnchor.rotation;

        // 3. DISABLE BOSS AI
        BossController bossCtrl = boss.GetComponent<BossController>();
        if (bossCtrl != null) bossCtrl.enabled = false;

        // 4. ANIMATION
        Animator bossAnim = boss.GetComponentInChildren<Animator>();
        if (bossAnim != null) bossAnim.SetTrigger("Rage");

        yield return new WaitForSeconds(introDuration);

        // 5. RESTORE EVERYTHING
        RestoreCameraToPlayer();
        TogglePlayerControls(true);
        ToggleCameraScripts(true); // <--- RE-ENABLE HERE

        if (bossCtrl != null) bossCtrl.enabled = true;

        Debug.Log("🔔 FIGHT START");
    }


    // ================= HELPERS =================

    void ToggleCameraScripts(bool state)
    {
        // Safe check in case array is null
        if (cameraScripts == null && mainCamera != null)
            cameraScripts = mainCamera.GetComponents<MonoBehaviour>();

        if (cameraScripts != null)
        {
            foreach (var script in cameraScripts)
            {
                // Don't disable the AudioListener or THIS script
                if (script != this && !(script is AudioListener))
                    script.enabled = state;
            }
        }
    }

    void RestoreCameraToPlayer()
    {
        if (mainCamera == null || gameplayCameraParent == null) return;

        Transform cam = mainCamera.transform;
        cam.SetParent(gameplayCameraParent);
        cam.localPosition = Vector3.zero;
        cam.localRotation = Quaternion.identity;
    }

    void TogglePlayerControls(bool state)
    {
        if (player != null)
        {
            MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
            foreach (var s in scripts) s.enabled = state;

            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = state;

            this.enabled = true;

            var weapon = player.GetComponentInChildren<SwordDamage>();
            if (weapon != null) weapon.enabled = state;
        }
    }
}