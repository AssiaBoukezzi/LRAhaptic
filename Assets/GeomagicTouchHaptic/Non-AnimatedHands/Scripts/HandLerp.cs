// HEADER

// INCLUDES
using System.Collections;
using UnityEngine;

/// <summary>
/// Namespace for classes with general functionalities
/// </summary>
namespace Utils.Animation
{
    /// <summary>
    /// 
    /// </summary>
    public class HandLerp : MonoBehaviour
    {
        [SerializeField]
        GameObject handContainer = null, handContainerEnd = null, handModel = null;
        [SerializeField]
        float normalDuration = .4f;
        [SerializeField]
        AnimationCurve normalSpeedCurve;
        [SerializeField]
        float reverseDuration = .4f;
        [SerializeField]
        AnimationCurve reverseSpeedCurve;
        [SerializeField]
        bool firstPosition = false, firstRotation = true, firstScale = true;
        [SerializeField]
        bool playOnAwake = false;

        GameObject handContainerCopy;
        public bool AtBeginning { get; set; }
        public bool InAnimation { get; set; }

        void Awake()
        {
            AtBeginning = true;
            handContainerCopy = Instantiate(handContainer);
            if (playOnAwake) Play();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Play();
            }
        }

        void OnDestroy()
        {
            StopAllCoroutines();
            Destroy(handContainerCopy);
        }

        public void Play()
        {
            StartCoroutine(Lerp(normalDuration, normalSpeedCurve, handContainerCopy, handContainerEnd));
        }

        // Play if the position is at the beginning, else revert
        public void PlayOrRevert()
        {
            if (AtBeginning)
            {
                Play();
            }
            else
            {
                Revert();
            }
        }

        public void Loop()
        {
            StartCoroutine(LoopCoroutine());
        }

        public void Stop()
        {
            StopAllCoroutines();
        }

        public void Revert()
        {
            StartCoroutine(Lerp(reverseDuration, reverseSpeedCurve, handContainerEnd, handContainerCopy));
            handModel.GetComponent<SkinnedMeshRenderer>().enabled = true;
        }

        public void SetStart()
        {
            LerpGameObject(handContainer, handContainerCopy, handContainerEnd, 0);
            AtBeginning = true;
        }

        public void SetFinish()
        {
            LerpGameObject(handContainer, handContainerCopy, handContainerEnd, 1);
            AtBeginning = false;
        }

        IEnumerator LoopCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                StartCoroutine(Lerp(normalDuration, normalSpeedCurve, handContainerCopy, handContainerEnd));
                yield return new WaitForSeconds(1);
                StartCoroutine(Lerp(reverseDuration, reverseSpeedCurve, handContainerEnd, handContainerCopy));
            }
        }

        IEnumerator Lerp(float duration, AnimationCurve speedCurve, GameObject begin, GameObject end)
        {
            InAnimation = true;
            float timer = 0f;
            while (timer <= duration)
            {
                LerpGameObject(handContainer, begin, end, speedCurve.Evaluate(timer / duration));
                timer += UnityEngine.Time.deltaTime;
                yield return null;
            }
            LerpGameObject(handContainer, begin, end, 1);
            InAnimation = false;
            AtBeginning = !AtBeginning;
        }

        public void LerpTransform(Transform t, Transform tBegin, Transform tEnd, float time)
        {
            t.localPosition = Vector3.Lerp(tBegin.localPosition, tEnd.localPosition, time);
            t.localRotation = Quaternion.Lerp(tBegin.localRotation, tEnd.localRotation, time);
            t.localScale = Vector3.Lerp(tBegin.localScale, tEnd.localScale, time);
        }

        public void LerpTransformRecursively(Transform t, Transform tBegin, Transform tEnd, float time)
        {
            LerpTransform(t, tBegin, tEnd, time);
            for (int i = 0; i < tBegin.childCount; ++i)
            {
                LerpTransformRecursively(t.GetChild(i), tBegin.GetChild(i), tEnd.GetChild(i), time);
            }
        }

        public void LerpGameObject(GameObject hand, GameObject begin, GameObject end, float time)
        {
            // First case
            if (firstPosition) hand.transform.localPosition = Vector3.Lerp(begin.transform.localPosition, end.transform.localPosition, time);
            if (firstRotation) hand.transform.localRotation = Quaternion.Lerp(begin.transform.localRotation, end.transform.localRotation, time);
            if (firstScale) hand.transform.localScale = Vector3.Lerp(begin.transform.localScale, end.transform.localScale, time);

            for (int i = 0; i < begin.transform.childCount; ++i)
            {
                LerpTransformRecursively(hand.transform.GetChild(i), begin.transform.GetChild(i), end.transform.GetChild(i), time);
            }
        }
    }
}