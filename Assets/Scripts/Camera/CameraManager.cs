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

    #region Pan Camera

    public void PanCameraOnContact(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
    {
        panCameraCoroutine = StartCoroutine(PanCamera(panDistance, panTime, panDirection, panToStartingPos));
    }

    private IEnumerator PanCamera(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
    {
        Vector2 endPos = Vector2.zero;
        Vector2 startingPos = Vector2.zero;

        // handle pan from trigger
        if (!panToStartingPos)
        {
            // set the direction and distance
            switch (panDirection)
            {
                case PanDirection.Up:
                    endPos = Vector2.up;
                    break;
                case PanDirection.Down:
                    endPos = Vector2.down;
                    break;
                case PanDirection.Left:
                    endPos = Vector2.left;
                    break;
                case PanDirection.Right:
                    endPos = Vector2.right;
                    break;
                default:
                    break;
            }

            endPos *= panDistance;

            startingPos = startingTrackedObjectOffset;

            endPos += startingPos;

        }
        else
        {
            startingPos = framingTransposer.TargetOffset;
            endPos = startingTrackedObjectOffset;
        }

        float elapsedTime = 0f;
        while (elapsedTime < panTime)
        {
            elapsedTime += Time.deltaTime;

            Vector3 panLerp = Vector3.Lerp(startingPos, endPos, (elapsedTime / panTime));
            framingTransposer.TargetOffset = panLerp;

            yield return null;
        }
    }

    #endregion

    #region Swap Cameras

    public void SwapCamera(CinemachineCamera cameraFromLeft, CinemachineCamera cameraFromRight, Vector2 triggerExitDirection)
    {
        if (currentCamera == cameraFromLeft && triggerExitDirection.x > 0f)
        {
            // activate the new camera
            cameraFromRight.enabled = true;

            // deactivate the old camera
            cameraFromLeft.enabled = false;

            // set the new camera as the current camera
            currentCamera = cameraFromRight;

            // update the composer variable
            framingTransposer = currentCamera.GetComponent<CinemachinePositionComposer>();
        }

        else if (currentCamera == cameraFromRight && triggerExitDirection.x < 0f)
        {
            // activate the new camera
            cameraFromLeft.enabled = true;

            // deactivate the old camera
            cameraFromRight.enabled = false;

            // set the new camera as the current camera
            currentCamera = cameraFromLeft;

            // update the composer variable
            framingTransposer = currentCamera.GetComponent<CinemachinePositionComposer>();
        }
    }

    #endregion

}
