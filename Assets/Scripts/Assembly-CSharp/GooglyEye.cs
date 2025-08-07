using System.Collections;
using UnityEngine;

public class GooglyEye : MonoBehaviour
{
	public Transform pupil;

	public Transform white;

	public GameObject ouch;

	public GameObject blink;

	public GameObject dead;

	public GameObject eye;

	public Transform target;

	private Vector3 velocity;

	private Vector3 lastPostion;

	private Rigidbody rig;

	[SerializeField]
	[Range(0f, 1f)]
	private float suspicious;

	[SerializeField]
	[Range(0f, 1f)]
	private float fear;

	private float eyeDistanceCap;

	private GooglyEye[] allEyes;

	private bool isDead;

	private void Start()
	{
		rig = base.transform.parent.GetComponent<Rigidbody>();
		allEyes = base.transform.parent.GetComponentsInChildren<GooglyEye>();
	}

	private void FixedUpdate()
	{
		velocity *= 0.95f;
	}

	private void Update()
	{
		if (Random.value > 0.997f)
		{
			Blinks();
		}
		eyeDistanceCap = 0.013f + fear * 0.005f - suspicious * 0.01f;
		SetExpressions();
		if (!target)
		{
			if (lastPostion != Vector3.zero)
			{
				Vector3 vector = lastPostion - base.transform.position;
				if (vector.magnitude > 0.3f)
				{
					vector = vector.normalized * 0.3f;
				}
				velocity -= 500f * vector;
			}
			IdleMovement();
			lastPostion = base.transform.position;
		}
		else
		{
			pupil.transform.position = base.transform.position + (target.position - base.transform.position).normalized * eyeDistanceCap;
		}
		pupil.localPosition = new Vector3(pupil.localPosition.x, pupil.localPosition.y, 0.0035f);
	}

	private void SetExpressions()
	{
		pupil.localScale = Vector3.one * (1f - fear / 2f);
		white.localScale = new Vector3(1f, 1f, 1f - suspicious / 2f);
	}

	private void IdleMovement()
	{
		velocity.y -= 1f;
		if (Vector3.Distance(pupil.transform.position, base.transform.position) > eyeDistanceCap)
		{
			velocity = Mathf.Clamp(velocity.magnitude, 20f, 1000f) * 0.5f * (base.transform.position - pupil.position).normalized;
		}
		pupil.transform.position += velocity * Time.deltaTime * 0.01f;
	}

	private void Blinks()
	{
		for (int i = 0; i < allEyes.Length; i++)
		{
			allEyes[i].Blink();
		}
	}

	public void Blink()
	{
		StartCoroutine(PlayBlink(blink, 0.1f));
	}

	private IEnumerator PlayBlink(GameObject obj, float pause)
	{
		EnableObject(obj);
		yield return new WaitForSeconds(pause);
		EnableObject(eye);
	}

	private void EnableObject(GameObject obj)
	{
		if (!isDead)
		{
			ouch.SetActive(false);
			blink.SetActive(false);
			dead.SetActive(false);
			eye.SetActive(false);
			obj.SetActive(true);
		}
	}

	public void TakeDamage(float ouchTime)
	{
		if (base.gameObject.activeInHierarchy)
		{
			StartCoroutine(PlayBlink(ouch, ouchTime));
		}
	}

	public void Die()
	{
		EnableObject(dead);
		isDead = true;
	}
}
