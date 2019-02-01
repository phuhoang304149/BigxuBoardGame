using UnityEngine;
using System.Collections;

public class ShakeController : MonoBehaviour {
	public enum State{
		none, shake
	}
	public State state{get;set;}

	public float m_shakeIntensity;
	float save_shakeIntensity;
	public float m_shakeDecay = 0.03f;
	public bool isChangePosition, isChangeRotation, canShake;
	private Vector3 m_originPosition, save_originPosition, save_localOriginPosition;
	private Quaternion m_originRotation, save_originRotation, save_localOriginRotation;
	float fixedDeltaTime;
	bool canDecay;
	float timeShake, countTimeShake;

	IEnumerator actionShake;

	// Use this for initialization
	void Start () {
		save_shakeIntensity = m_shakeIntensity;
		fixedDeltaTime = Time.fixedDeltaTime;
	}

	void Awake(){
		if(isChangePosition){
			save_originPosition = transform.position;
			save_localOriginPosition = transform.localPosition;
		}
		if(isChangeRotation){
			save_originRotation = transform.rotation;
			save_localOriginRotation = transform.localRotation;
		}
		ResetData();
	}

	public void ResetData(){
		if(actionShake != null){
			StopCoroutine(actionShake);
			actionShake = null;
		}
		state = State.none;
		timeShake = 0;
		canDecay = false;

		if(isChangePosition){
			transform.position = new Vector3(save_originPosition.x, save_originPosition.y, save_originPosition.z);
			transform.localPosition = new Vector3(save_localOriginPosition.x, save_localOriginPosition.y, save_localOriginPosition.z);
		}
		if(isChangeRotation){
			transform.localRotation = Quaternion.Euler(save_localOriginRotation.x, save_localOriginRotation.y, save_localOriginRotation.z);
			transform.rotation = Quaternion.Euler(save_originRotation.x, save_originRotation.y, save_originRotation.z);
		}
	}

	public void SetUpShakeWorldPoint(float _timeShake, float _shakeIntensity = -1){
		if(state == State.shake
			&& _shakeIntensity < m_shakeIntensity){
			return;
		}
		timeShake = _timeShake;
		canDecay = false;
		countTimeShake = 0;
		if(_shakeIntensity > 0){
			m_shakeIntensity = _shakeIntensity;
		}else{
			m_shakeIntensity = save_shakeIntensity;
		}
		if(state != State.shake && actionShake == null){
			state = State.shake;
			m_originPosition = save_originPosition;
			m_originRotation = save_originRotation;
			actionShake = DoActionShakeWorldPoint();
			StartCoroutine(actionShake);
		}
	}
		

	IEnumerator DoActionShakeWorldPoint(){
		while (state == State.shake){
			if(canShake){
				if(timeShake != -1){ // shake có thời gian
					ShakeWorldPoint();
					if(!canDecay){
						countTimeShake += fixedDeltaTime*10;
						if(countTimeShake >= timeShake){
							SetUpStopShake();
						}
					}
				}else{ // shake hoài
					ShakeWorldPoint();
				}
				if(canDecay){
					m_shakeIntensity -= m_shakeDecay;
					if(m_shakeIntensity <= 0){
						if(isChangePosition){
							transform.position = new Vector3(m_originPosition.x, m_originPosition.y, m_originPosition.z);
						}
						if(isChangeRotation){
							transform.rotation = Quaternion.Euler(m_originRotation.x, m_originRotation.y, m_originRotation.z);
						}
						canDecay = false;
						state = State.none;
						break;
					}
				}
			}
			yield return StartCoroutine(new WaitForSecondsRealtime(fixedDeltaTime));
		}
		actionShake = null;
	}

	public void SetUpStopShake(){
		if(state == State.shake){
			canDecay = true; 
		}
	}

	void ShakeWorldPoint(){
		if(isChangePosition){
			transform.position = m_originPosition + Random.insideUnitSphere * m_shakeIntensity;
		}
		if(isChangeRotation){
			transform.rotation = new Quaternion(
				m_originRotation.x + Random.Range(-m_shakeIntensity, m_shakeIntensity) * 0.1f,
				m_originRotation.y + Random.Range(-m_shakeIntensity, m_shakeIntensity) * 0.1f,
				m_originRotation.z + Random.Range(-m_shakeIntensity, m_shakeIntensity) * 0.1f,
				m_originRotation.w + Random.Range(-m_shakeIntensity, m_shakeIntensity) * 0.1f
				);
		}
	}

	public void SetUpShakeLocalPoint(float _timeShake, float _shakeIntensity = -1){
		if(state == State.shake
			&& _shakeIntensity < m_shakeIntensity){
			return;
		}
		timeShake = _timeShake;
		canDecay = false;
		countTimeShake = 0;
		if(_shakeIntensity > 0){
			m_shakeIntensity = _shakeIntensity;
		}else{
			m_shakeIntensity = save_shakeIntensity;
		}
		if(state != State.shake && actionShake == null){
			state = State.shake;
			m_originPosition = save_localOriginPosition;
			m_originRotation = save_localOriginRotation;
			actionShake = DoActionShakeLocalPoint();
			StartCoroutine(actionShake);
		}
	}

	IEnumerator DoActionShakeLocalPoint(){
		while (state == State.shake){
			if(canShake){
				if(timeShake != -1){ // shake có thời gian
					ShakeLocalPoint();
					if(!canDecay){
						countTimeShake += fixedDeltaTime;
						if(countTimeShake >= timeShake){
							SetUpStopShake();
						}
					}
				}else{ // shake hoài
					ShakeLocalPoint();
				}
				if(canDecay){
					m_shakeIntensity -= m_shakeDecay;
					if(m_shakeIntensity <= 0){
						if(isChangePosition){
							transform.localPosition = new Vector3(m_originPosition.x, m_originPosition.y, m_originPosition.z);
						}
						if(isChangeRotation){
							transform.localRotation = Quaternion.Euler(m_originRotation.x, m_originRotation.y, m_originRotation.z);
						}
						canDecay = false;
						state = State.none;
						break;
					}
				}
			}
			
			yield return StartCoroutine(new WaitForSecondsRealtime(fixedDeltaTime));
		}
		actionShake = null;
	}

	void ShakeLocalPoint(){
		if(isChangePosition){
			transform.localPosition = m_originPosition + Random.insideUnitSphere * m_shakeIntensity;
		}
		if(isChangeRotation){
			transform.localRotation = new Quaternion(
				m_originRotation.x + Random.Range(-m_shakeIntensity, m_shakeIntensity) * 0.1f,
				m_originRotation.y + Random.Range(-m_shakeIntensity, m_shakeIntensity) * 0.1f,
				m_originRotation.z + Random.Range(-m_shakeIntensity, m_shakeIntensity) * 0.1f,
				m_originRotation.w + Random.Range(-m_shakeIntensity, m_shakeIntensity) * 0.1f
				);
		}
	}
}
