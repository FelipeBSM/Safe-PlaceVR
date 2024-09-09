using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsManager : MonoBehaviour
{

    private HudManager hudManager;
    private void Start()
    {
        hudManager = GetComponent<HudManager>();
    }
    public void ThumbsTutorialAnimationEvent()
    {
        StartCoroutine("ThumbsCoroutine");
    }
    private IEnumerator ThumbsCoroutine()
    {
        yield return new WaitForSeconds(1f);
        if(hudManager.isOnThumbsTutorial)
            hudManager.InitSecondTutorial();
        else
        {
            hudManager.InitFourthTutorial();
        }
    }


}
