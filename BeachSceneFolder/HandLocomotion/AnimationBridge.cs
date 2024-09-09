using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationBridge : MonoBehaviour
{
    public AnimationsManager AnimManager;

    public void EventFunctionThumbs()
    {
        AnimManager.ThumbsTutorialAnimationEvent();
    }

}
