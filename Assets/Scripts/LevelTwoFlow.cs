using UnityEngine;
using System.Collections;

public class LevelTwoFlow : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform bossSpawnPoint;
    public Camera mainCamera;

    [Header("Boss Intro")]
    public GameObject boss;
    public Transform bossFaceAnchor; // Empty object at boss's face
    public float introDuration = 4.0f;

    // State
    private Transform originalCamParent;
    private Vector3 originalCamPos;
    private Quaternion originalCamRot;

    void Start()
    {
        // Auto-start if we loaded from a save file
        if (PlayerPrefs.GetInt("SavedLevel", 1) == 2)
        {
            StartBossLevel();
        }
    }

    // This is called by LevelOneFlow when zombies are dead
    public void StartBossLevel()
    {
        Debug.Log("🚀 Starting Level 2 Sequence...");

        // 1. Teleport Player
        TeleportPlayer();

        // 2. Start the Cutscene
        StartCoroutine(PlayBossIntro());
    }

    void TeleportPlayer()
    {
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        player.position = bossSpawnPoint.position;
        player.rotation = bossSpawnPoint.rotation;

        if (cc != null) cc.enabled = true;
    }

    IEnumerator PlayBossIntro()
    {
        // Wait one frame to let physics/teleport settle
        yield return null;

        Debug.Log("🎬 Boss Intro Camera Action!");

        // 1. Hijack Camera
        if (mainCamera != null && bossFaceAnchor != null)
        {
            Transform camTrans = mainCamera.transform;

            // Important: Force-detach from any previous intro parents
            // and attach to player momentarily to get a clean "return" point
            camTrans.SetParent(player);
            camTrans.localPosition = new Vector3(0, 1.6f, 0); // Reset to head height

            // SAVE Return Point
            originalCamParent = camTrans.parent;
            originalCamPos = camTrans.localPosition;
            originalCamRot = camTrans.localRotation;

            // MOVE to Boss
            camTrans.SetParent(bossFaceAnchor);
            camTrans.localPosition = Vector3.zero;
            camTrans.localRotation = Quaternion.identity;
        }

        // 2. Disable Controls
        MonoBehaviour[] playerScripts = player.GetComponents<MonoBehaviour>();
        foreach (var script in playerScripts) script.enabled = false;

        var weapon = player.GetComponentInChildren<SwordDamage>();
        if (weapon != null) weapon.enabled = false;

        // 3. Play Boss Anim
        BossController bossCtrl = null;
        if (boss != null)
        {
            bossCtrl = boss.GetComponent<BossController>();
            if (bossCtrl != null) bossCtrl.enabled = false;

            Animator bossAnim = boss.GetComponentInChildren<Animator>();
            if (bossAnim != null) bossAnim.SetTrigger("Rage");
        }

        // 4. Wait
        yield return new WaitForSeconds(introDuration);

        // 5. Restore Camera
        if (mainCamera != null)
        {
            Transform camTrans = mainCamera.transform;
            camTrans.SetParent(originalCamParent);
            camTrans.localPosition = originalCamPos;
            camTrans.localRotation = originalCamRot;
        }

        // 6. Restore Controls
        foreach (var script in playerScripts) script.enabled = true;
        if (weapon != null) weapon.enabled = true;
        if (bossCtrl != null) bossCtrl.enabled = true;

        Debug.Log("🔔 FIGHT START!");
    }
}