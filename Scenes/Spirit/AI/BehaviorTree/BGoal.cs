using Godot;
using System.Collections.Generic;

public class BGoal : BNode {
    // BELIEF condition {set; get;} = BELIEF.ANY;
    // bool leafGoal {set; get;} = true;
    public delegate BSIGNAL Goal(Spirit _spirit);
    public Goal condition;
    public BGoal() {}
    public BGoal(Spirit _self, Goal condition) : base(_self, BTYPE.GOAL) {

    }
    public BSIGNAL gProcess() {
        if(self.currentState.frames <= 0) {
            return condition(self);
        }
        return BSIGNAL.PASS;
    }
}

public enum BELIEF {
    ANY, DYING_TARGET, CLOSE_TARGET, MID_TARGET, FAR_TARGET, DYING_SELF
}

public enum DECORATOR {
    INVERTER, SUCCEEDER, LIMITER, UNTILFAIL, NONE
}

// public bool isGoalMet(Dictionary<BELIEF, bool> beliefs) {
//         if(condition == BELIEF.ANY){
//             return false;
//         } else {
//             // BTree root = (BTree)base.findRoot(this);
//             // root.latestGoalChild = this;//When ever goal is not ANY, automatically sets as most recent goal regardless if it's index 0 or 19 in childGoals List.
//             if(beliefs[condition] == leafGoal) {
//                 return true;
//             }
//         } 
//         return false;//Belief is not any but it returns false
//     }