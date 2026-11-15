using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

using Unity.Cinemachine;
using System.Collections;
using UnityEditor.Experimental.GraphView;
public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    [Header("Camera Array")]
    [SerializeField] private CinemachineCamera[] allVirtualCameras;

    #region Lerping Y Values Variable Declerations

    [Header("Controls for Lerping the Camera Y Damping")]
    [SerializeField] private float fallPanAmount = 0.25f;
    [SerializeField] private float fallYPanTime = 0.35f;
    public float fallSpeedYDampingChangeThreshold = -15f;

    public bool IsLerpingYDamping { get; private set; }
    public bool LerpedFromPlayerFalling { get; set; }

    private Coroutine lerpYPanCoroutine;

    #endregion

    private CinemachineCamera currentCamera;
    private CinemachinePositionComposer framingTransposer;

    private float normPanAmount;

    private Vector2 startingTrackedObjectOffset;

    private Coroutine panCameraCoroutine;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        for (int i = 0; i < allVirtualCameras.Length; i++)
        {
            if (allVirtualCameras[i] != null)
            {
                currentCamera = allVirtualCameras[i];
                if (currentCamera.enabled)
                {
                    framingTransposer = currentCamera.GetComponent<CinemachinePositionComposer>();
                }
            }
        }

        normPanAmount = framingTransposer.Damping.y;

        startingTrackedObjectOffset = framingTransposer.TargetOffset;
    }

    #region Lerp The Y Damping

    public void lerpYDamping(bool isPlayerFalling)
    {
        lerpYPanCoroutine = StartCoroutine(lerpYAction(isPlayerFalling));
    }

    private IEnumerator lerpYAction(bool isPlayerFalling)
    {
        IsLerpingYDamping = true;

        float startDampAmount = normPanAmount;
        float endDampAmount = 0f;

        if (isPlayerFalling)
        {
            endDampAmount = fallPanAmount;
            LerpedFromPlayerFalling = true;
        }
        else
        {
            endDampAmount = normPanAmount;
        }
        float elapsedTime = 0f;
        while (elapsedTime < fallYPanTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, (elapsedTime / fallYPanTime));
            framingTransposer.Damping.y = lerpedPanAmount;

            yield return null;
        }

        IsLerpingYDamping = false;
    }

    #endregion
}
