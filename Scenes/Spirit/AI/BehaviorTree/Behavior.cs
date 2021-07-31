using Godot;
using System.Collections.Generic;

public class Behavior : Object {
    public Behavior() {} //preferences dictionary will have to be created as a godot dictionary.
    public void sample(Spirit self) {
        BTree bTree = new BTree(self, BTYPE.SELECTOR);//Start Root
        bTree.action(new BAction(self, STATE.IDLE)).chancesPerCycle(5, 5).ascendTree()
        
        .descendTree(new BNode(self, BTYPE.SEQUENCE))
        .action(new BAction(self, STATE.MOVE));
        BGoal bGoal = new BGoal(self, myGoal);
        
        self.bTree = bTree;
    }
    public BSIGNAL myGoal(Spirit _spirit) {
        return BSIGNAL.PASS;
    }
}

public enum ARCHITYPE {
    SAMPLE
}

public enum SUBTYPE {//For frequency of existing selectors & conditions in architypes.
    ANTI_HEAL
}

//Old Code
// Dictionary<SUBTYPE, int> preferences = null

        // BTree bTree = new BTree(self);//Start Root
        // bTree.action(new Approach(self, 5)).chancesPerCycle(5, 5).ascendTree()
        
        // .descendTree(new BNode(self, BTYPE.RANDOM_SELECTOR))
        // .action(new Approach(self, 2));
        
        // return bTree;