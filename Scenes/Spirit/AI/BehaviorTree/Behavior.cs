using Godot;
using System.Collections.Generic;

public class Behavior : Object {
    public Behavior() {} //preferences dictionary will have to be created as a godot dictionary.
    public BTree sample(Spirit self, Dictionary<SUBTYPE, int> preferences = null) {
        BTree bTree = new BTree(self, BTYPE.SEQUENCE);//Start Root
        bTree.action(new BAction(self, STATE.IDLE)).chancesPerCycle(5, 5).ascendTree()
        
        .descendTree(new BNode(self, BTYPE.RANDOM_SELECTOR))
        .action(new BAction(self, STATE.MOVE));
        
        return bTree;
    }
}

public enum ARCHITYPE {
    SAMPLE
}

public enum SUBTYPE {//For frequency of existing selectors & conditions in architypes.
    ANTI_HEAL
}

//Old Code
        // BTree bTree = new BTree(self);//Start Root
        // bTree.action(new Approach(self, 5)).chancesPerCycle(5, 5).ascendTree()
        
        // .descendTree(new BNode(self, BTYPE.RANDOM_SELECTOR))
        // .action(new Approach(self, 2));
        
        // return bTree;