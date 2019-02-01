using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FpsDisplay : MonoBehaviour
{   
    private static FpsDisplay ins;
    [SerializeField] private float updateFrequency = 1f;

    public Text text;

    private void Awake ()
    {
        #if !TEST
		Destroy(this.gameObject);
		return;
		#endif

        if (ins != null && ins != this) { 
			Destroy(this.gameObject); 
			return;
		}
		ins = this;
		DontDestroyOnLoad (this.gameObject);
    }

    private void Start ()
    {
        StartCoroutine(UpdateCounter());
    }

    private IEnumerator UpdateCounter ()
    {
        var waitForDelay = new WaitForSeconds(updateFrequency);

        while (true)
        {
            var lastFrameCount = Time.frameCount;
            var lastTime = Time.realtimeSinceStartup;

            yield return waitForDelay;

            var timeDelta = Time.realtimeSinceStartup - lastTime;
            var frameDelta = Time.frameCount - lastFrameCount;

            text.text = string.Format("{0:0.} FPS", frameDelta / timeDelta);
        }
    }
}