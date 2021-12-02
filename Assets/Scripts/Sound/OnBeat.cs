using UnityEngine;

public class OnBeat : StateMachineBehaviour {
    private bool _movedThisBeat;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
        _movedThisBeat = false;
    }
    
}