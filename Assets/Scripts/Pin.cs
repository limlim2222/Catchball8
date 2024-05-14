using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pin : MonoBehaviour
{
    public float waitTime = 3.0f;
    private int fallenPinCount = 0;
    private int totalPinCount = 10; // 전체 볼링핀의 개수를 여기에 입력하세요.

    private Coroutine myCoroutine = null;

    private Rigidbody PinRigidbody = null;
    private Transform PinTransform = null;

    private Vector3 PinOriginPosition = Vector3.zero;
    private Quaternion PinOriginRotation = Quaternion.identity;

    // Start is called before the first frame update
    void Start()
    {
        PinRigidbody = GetComponent<Rigidbody>();
        PinTransform = GetComponent<Transform>();

        // 초기값 설정
        PinOriginPosition = PinTransform.position;
        PinOriginRotation = PinTransform.rotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Interactable") || collision.gameObject.CompareTag("Pin"))
        {
            if (myCoroutine == null)
            {
                myCoroutine = StartCoroutine(ReturnObject());
            }
        }

        // 쓰러진 볼링핀 수 증가
        if (collision.gameObject.CompareTag("Pin"))
        {
            if (!IsStandingUp())
            {
                fallenPinCount++;
                Debug.Log("쓰러진 볼링핀 수: " + fallenPinCount);

                // 모든 핀이 쓰러졌을 때
                if (fallenPinCount == totalPinCount)
                {
                    // 원래 위치로 볼링핀을 되돌리는 코루틴 시작
                    if (myCoroutine == null)
                    {
                        myCoroutine = StartCoroutine(ReturnObject());
                    }
                }
            }
        }
    }

    IEnumerator ReturnObject()
    {
        yield return new WaitForSeconds(waitTime);

        // 속도 및 각속도 초기화
        PinRigidbody.velocity = Vector3.zero;
        PinRigidbody.angularVelocity = Vector3.zero;

        // 위치 및 회전값 초기 위치로 설정
        PinTransform.position = PinOriginPosition;
        PinTransform.rotation = PinOriginRotation;

        // 코루틴 종료
        myCoroutine = null;
    }

    // 볼링핀이 선상에 서 있는지 확인
    private bool IsStandingUp()
    {
        return transform.position.y < 0.5f;
    }
}
