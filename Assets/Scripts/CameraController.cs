using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public static CameraController Instance;

    private void Awake() {
        if(Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void ShakeCamera(float intensity, float duration) {
        CinemachineBasicMultiChannelPerlin basicChannelPerlin = GetComponent<CinemachineBasicMultiChannelPerlin>();
        basicChannelPerlin.AmplitudeGain = intensity;
        StartCoroutine(SetCameraShakeOff(duration, basicChannelPerlin));
    }

    private IEnumerator SetCameraShakeOff(float duration, CinemachineBasicMultiChannelPerlin basicChannelPerlin) {
        yield return new WaitForSecondsRealtime(duration);
        basicChannelPerlin.AmplitudeGain = 0;   
    }

}