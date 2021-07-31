using Godot;
using System.Collections.Generic;

public class BTree : BNode {
    public BNode constructNode {set; get;}
    public List<BGoal> childGoals {set; get;} = new List<BGoal>();//Set as soon as they're added but they must be added in depth first order.
    public BTree() {constructNode = this;}
    //Constructing Methods
    public BTree(Spirit _self, BTYPE _bType) : base(_self, _bType) {
        constructNode = this;
    }
    struct NodeLvl {
        public int level;
        public BNode bNode;
    }
    public void printTree() {
        string treePrintout = "";
        Stack<NodeLvl> nodeStack = new Stack<NodeLvl>();
        BNode currentNode = this;
        nodeStack.Push(new NodeLvl { level = 0, bNode = currentNode});

        while(nodeStack.Count != 0) {
            NodeLvl nextNode = nodeStack.Pop();
            treePrintout += new string('-', nextNode.level) + nextNode.bNode.name + "\n";
            for(int i = nextNode.bNode.children.Count-1; i >= 0; i--) {
                nodeStack.Push(new NodeLvl {
                    level = nextNode.level + 1, bNode =
                    nextNode.bNode.children[i]
                });
            }
        }
        GD.Print(treePrintout);
    }
    public BTree descendTree(BNode bChild) { //Make it throw an error if type is Action.
        constructNode.children.Add(bChild);
        bChild.parent = constructNode;
        constructNode = bChild;
        return this;
    }
    public BTree ascendTree() {
        if(constructNode.parent != null) {
            constructNode = constructNode.parent;
        } else {
            GD.Print("ascendTree Method Error: This is root so no parent.");
        }
        return this;
    }
    public void decorate(DECORATOR _decorator) {
        constructNode.decorator.Add(_decorator);
        //Added in order of priority. First one that passes wins.
    }
    public BTree action(BAction action) {
        constructNode.children.Add(action);
        action.parent = constructNode;
        constructNode = action;
        return this;
    }
    public BTree chancesPerCycle(int _frequency, float _probability) {
        constructNode.frequency = _frequency;
        constructNode.probability = _probability;
        return this;
    }
}

    //     //if(!isGoalMet(state.beliefs)) 
    //     return base.btProcess(delta, state, decorated);
    // }
    // public void processBTree(float delta) {
    //     BTree priorGoal = bTree.goalsProcess(beliefs);
    //     if(priorGoal != null && priorGoal.getPriorityLvl() 
    //                         > currentState.curBNode.getPriorityLvl()) {
    //        currentState.curBNode = priorGoal.btProcess(delta, currentState);
    //     } else {
    //         currentState.curBNode = currentState.curBNode.btProcess(delta, currentState);
    //     }
    // }

    //  public BNode goalsProcess(Dictionary<BELIEF, bool> beliefs) { //Should only be used by root
    //     for(int i = 0; i < childGoals.Count; i++) {
    //         if(!childGoals[i].isGoalMet(beliefs)) {
    //             // latestGoalChild = childGoals[i];//changes root's state
    //             return childGoals[i];
    //         } //else if (Object.ReferenceEquals(childGoals[i], latestGoalChild)) {
    //             //return null;
    //         //} //Stops at the latestGoal rather than checking every goal.
    //     }
    //     return null;
    // }