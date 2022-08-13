using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.Pagination;
using UnityEngine.Events;
using TMPro;


public class PythonTxtParser : MonoBehaviour
{
    public PythonFaceDetector detector;


    public PagedRect pagedRect;
    public TMP_Text resourceText;

    public UnityEvent age1pop;
    public UnityEvent age2pop;
    public UnityEvent age3pop;
    public UnityEvent age4pop;
    public UnityEvent age5pop;
    public UnityEvent age6pop;
    public UnityEvent idlepop;

    // Start is called before the first frame update
    void Start()
    {
        detector.resultMethod += ChangeInfoCard;

    }

    public void ChangeInfoCard(PythonFaceStructure face)
    {
        if (face.gender.Contains("male") && face.age <= 30)
        {
            age1pop.Invoke();
            pagedRect.SetCurrentPage(5);
            resourceText.text = "젊은 남자분이시군요 추정 나이는 대략 " + face.age + "세";

        }
        else if (face.gender.Contains("male") && face.age <= 50)
        {
            age2pop.Invoke();
            pagedRect.SetCurrentPage(5);
            resourceText.text = "젊은 남자분이시군요 추정 나이는 대략 " + face.age + "세";

        }
        else if (face.gender.Contains("male") && face.age <= 70)
        {
            age3pop.Invoke();
            pagedRect.SetCurrentPage(5);
            resourceText.text = "젊은 남자분이시군요 추정 나이는 대략 " + face.age + "세";
        }
        else if (face.gender.Contains("female") && face.age <= 25)
        {
            age4pop.Invoke();
            pagedRect.SetCurrentPage(1);
            resourceText.text = "젊은 여자분이시군요 추정 나이는 대략 " + face.age + "세";
        }
        else if (face.gender.Contains("female") && face.age <= 45)
        {
            age5pop.Invoke();
            pagedRect.SetCurrentPage(2);
            resourceText.text = "중년 여자분이시군요 추정 나이는 대략 " + face.age + "세";
        }
        else if (face.gender.Contains("female") && face.age <= 70)
        {
            age6pop.Invoke();
            pagedRect.SetCurrentPage(3);
            resourceText.text = "장년 여자분이시군요 추정 나이는 나이는 대략 " + face.age + "세";
        }
        else
        {
            idlepop.Invoke();
        }
    }
}
      
