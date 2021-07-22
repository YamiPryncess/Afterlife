using Godot;
using System.Collections.Generic;

public class Enemy : KinematicBody {
    BTree bTree;
    State currentState;
    private Dictionary<string, float[]> stats = new Dictionary<string, float[]>();
    private Dictionary<BELIEF, bool> beliefs = new Dictionary<BELIEF, bool>();
    public override void _Ready() {
        float[] defaultVal = new float[3]{10,10,0};
        stats.Add("Health", defaultVal);
        stats.Add("Focus", defaultVal);
        stats.Add("Hollow", defaultVal);
        stats.Add("Power", defaultVal);
        stats.Add("Speed", defaultVal);

        bTree = new BTree(this, BTYPE.SELECTOR, 0);//Start Root
        currentState = new State();
        bTree.action(new Approach(this, 5, 0)).chancesPerCycle(5, 5).ascendTree()
        
        .descendTree(new BTree(this, BTYPE.RANDOM_SELECTOR, 0))
        .action(new Approach(this, 2));
    }
    public override void _PhysicsProcess(float delta) {
        //processBTree(delta);
    }
}
public class State {
    public KinematicBody target;
    public BTree curBNode;
    public Dictionary<BELIEF, bool> beliefs;
}
public enum BTYPE {
    ACTION, SELECTOR, SEQUENCE, RANDOM_SELECTOR, RANDOM_SEQUENCE
}
public enum DECORATOR {
    INVERTER, SUCCEEDER, LIMITER, UNTILFAIL, NONE
}
public enum BELIEF {
    ANY, DYING_TARGET, CLOSE_TARGET, MID_TARGET, FAR_TARGET, DYING_SELF
}
public enum BSIGNAL {
    PASS, FAIL, RUNNING, INTERUPTED, STANDBY
}
public class BTree : BNode {
    BNode constructNode;
    private List<BGoal> childGoals = new List<BGoal>();//Set as soon as they're added but they must be added in depth first order.
    public BNode latestGoalChild;
    //Constructing Methods
    public BTree(KinematicBody _self, BTYPE _bType, int _priority) : base(_self, _bType, _priority) {

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
            for(int i = nextNode.bNode.getChildren().Count-1; i >= 0; i--) {
                nodeStack.Push(new NodeLvl {
                    level = nextNode.level + 1, bNode =
                    nextNode.bNode.getChildren()[i]
                });
            }
        }
        GD.Print(treePrintout);
    }
    // public override BNode btProcess(float delta, State state, bool decorated = true) {
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
    public BTree descendTree(BTree bChild) { //Make it throw an error if type is Action.
        constructNode.setChild(bChild);
        bChild.setParent(constructNode);
        constructNode = bChild;
        return this;
    }
    public BTree ascendTree() {
        if(constructNode.getParent() != null) {
            constructNode = constructNode.getParent();
        } else {
            GD.Print("ascendTree Method Error: This is root so no parent.");
        }
        return this;
    }
    /*public void decorate(DECORATOR _decorator) {
        decorator.Add(_decorator);
        //Added in order of priority. First one that passes wins.
    }*/
    public BTree action(BAction action) {
        constructNode.setChild(action);
        constructNode = action;
        return this;
    }
    public BNode goalsProcess(Dictionary<BELIEF, bool> beliefs) { //Should only be used by root
        for(int i = 0; i < childGoals.Count; i++) {
            if(!childGoals[i].isGoalMet(beliefs)) {
                latestGoalChild = childGoals[i];//changes root's state
                return childGoals[i];
            } else if (Object.ReferenceEquals(childGoals[i], latestGoalChild)) {
                return null;
            } //Stops at the latestGoal rather than checking every goal.
        }
        return null;
    }
    public BTree chancesPerCycle(int _frequency, float _probability) {
        constructNode.frequency = _frequency;
        constructNode.probability = _probability;
        return this;
    }
}
public class BNode {
    //Class Construction Props
    public string name;
    KinematicBody self;
    private BNode parent = null;
    List<BNode> children;
    //Leaf Goal Props
    private BSIGNAL bSignal = BSIGNAL.STANDBY;
    //Process Props
    private BTYPE bType;
    private int priority = 0;
    public int frequency = 0;
    public float probability = 0;
    //Decorator Props
    List<DECORATOR> decorator; //Added in order of priority.
    int runCount;
    int runLimit = 1;
    //Constructing Methods
    public BNode(KinematicBody _self, BTYPE _bType, int _priority = 0) {
        self = _self;
        bType = _bType;
        priority = _priority;
    }
    public BNode(BTYPE _bType, int _priority = 0) {
        bType = _bType;
        priority = _priority;
    }
    //Main Process Methods
    public virtual BNode btProcess(float delta, State state, bool decorated = true) { //Processes all of its own children
        bSignal = BSIGNAL.STANDBY;
        switch (bType) {
            case BTYPE.SELECTOR: 
                for(int i = 0; i < children.Count; i++) {
                    BNode bResult = children[i].btProcess(delta, state);
                    switch(bResult.bSignal) {
                        case BSIGNAL.FAIL:
                        bSignal = (i == children.Count-1 ? BSIGNAL.FAIL : BSIGNAL.RUNNING); 
                        break;
                        case BSIGNAL.PASS:
                        bSignal = BSIGNAL.PASS;
                        break;
                        case BSIGNAL.RUNNING:
                        bSignal = BSIGNAL.RUNNING;
                        break;
                    }
                }
            break;
            case BTYPE.SEQUENCE:
                for(int i = 0; i < children.Count; i++) {
                    BNode bResult = children[i].btProcess(delta, state);
                    switch(bResult.bSignal) {
                        case BSIGNAL.FAIL:
                        bSignal = BSIGNAL.FAIL; 
                        break;
                        case BSIGNAL.PASS:
                        bSignal = (i == children.Count-1 ? BSIGNAL.PASS : BSIGNAL.RUNNING);
                        break;
                        case BSIGNAL.RUNNING:
                        bSignal = BSIGNAL.RUNNING;
                        break;
                    }
                }
            break;
            case BTYPE.RANDOM_SELECTOR:
            break;
            case BTYPE.RANDOM_SEQUENCE:
            break;
            case BTYPE.ACTION:
            BAction action = (BAction) this;
            bSignal = action.startAction(state);
            decorated = false; //Skip decorate since it already happens in action.
            break;
        }
        bSignal = decorated ? processDecorate(bSignal) : bSignal;
        return this;
    }
    public BSIGNAL processDecorate(BSIGNAL pSignal) {
        BSIGNAL dSignal = pSignal;

        //Machinery After Effects
        if(bSignal == BSIGNAL.RUNNING) {
            runCount++;
        } else {
            runCount = 0;
        }

        for(int i = 0; i < decorator.Count; i++) {
            //Added in order of priority. First one that passes wins. Update: Not true, they all win.
            switch (decorator[i]) {
                case DECORATOR.SUCCEEDER:
                    dSignal = BSIGNAL.PASS;
                    break;
                case DECORATOR.INVERTER:
                    if(bSignal == BSIGNAL.PASS) {
                        dSignal = BSIGNAL.FAIL;
                        break;
                    } else if (bSignal == BSIGNAL.FAIL) {
                        dSignal = BSIGNAL.PASS;
                        break;
                    }
                    break;
                case DECORATOR.UNTILFAIL:
                    if(bSignal != BSIGNAL.FAIL) {
                        dSignal = BSIGNAL.RUNNING;
                    }
                    break;
                case DECORATOR.LIMITER:
                    if(runCount >= runLimit) {
                        dSignal = BSIGNAL.FAIL;
                    }
                    break;
            }
        }
        return dSignal;
    }
    //Supporting Methods for Construction or Processing
    public BNode findRoot(BNode descendant) {
        BNode dRoot = descendant;
        while(dRoot.parent != null) {
            dRoot = dRoot.parent;
        }
        return dRoot;
    }
    //Getters & Setters
    public BNode getParent() {
        return parent;
    }
    public void setParent(BNode _parent) {
        parent = _parent;
    }
    public List<BNode> setChild(BNode bChild) {
        children.Add(bChild);
        return children;
    }
    public BNode getChild(int index) {
        return children[index];
    }
    public List<BNode> getChildren() {
        return children;
    }
    public BTYPE getBType() {
        return bType;
    }
    public int getPriority() {//To be available for use by calling object
        return priority;
    }
    public KinematicBody getSelf() {//To be used by actions
        return self;
    }
}
public class BGoal : BNode {
    BELIEF condition = BELIEF.ANY;
    bool leafGoal = true;
    public BGoal(BTYPE _bType) : base(_bType) {

    }
    public bool isGoalMet(Dictionary<BELIEF, bool> beliefs) {
        if(condition == BELIEF.ANY){
            return false;
        } else {
            BTree root = (BTree)base.findRoot(this);
            root.latestGoalChild = this;//When ever goal is not ANY, automatically sets as most recent goal regardless if it's index 0 or 19 in childGoals List.
            if(beliefs[condition] == leafGoal) {
                return true;
            }
        } 
        return false;//Belief is not any but it returns false
    }
}

public class BAction : BTree {
    private EVENT stage;
    private BSIGNAL aSignal = BSIGNAL.STANDBY;
    public BAction(KinematicBody _self, int _priority = 0,
                BTYPE _bType = BTYPE.ACTION) : base(_self, _bType, _priority) {
    }
    public BSIGNAL getASignal() {
        return aSignal;
    }
    public void setASignal(BSIGNAL _aSignal) {
        aSignal =_aSignal;
    }
    public virtual void Enter() { stage = EVENT.UPDATE; setASignal(BSIGNAL.RUNNING); }
    public virtual void Update() { stage = EVENT.UPDATE; }
    public virtual void Exit() { stage = EVENT.EXIT; } 
    public BSIGNAL processAction() {
        if (stage == EVENT.ENTER) Enter(); 
	    if (stage == EVENT.UPDATE) Update();
	    aSignal = base.processDecorate(aSignal);
        decideExit();

        if (stage == EVENT.EXIT) Exit();
        return getASignal();
    }
    public virtual BSIGNAL startAction(State state) {
        return BSIGNAL.STANDBY;
    }
    public void decideExit() {
        if(aSignal != BSIGNAL.RUNNING) {
            stage = EVENT.EXIT;
        }
    }
}

public class Approach : BAction {
    private float speed = 1;
    Vector3 velocity = new Vector3();
    KinematicBody target;
    public Approach (KinematicBody _self, float _speed,
                    int _priorityLvl = 0) : base(_self, _priorityLvl, BTYPE.ACTION) {
        speed = 1;
    }
    public override BSIGNAL startAction(State state) {//Wanted to startAction() with approach but how would you get the arguments for each action...
        target = state.target;
        return base.processAction();
    }
    public override void Enter() {
        base.Enter();
        
    }
    public override void Update() {
        base.Update();
        Vector3 targetOrigin = target.GlobalTransform.origin;
        Vector3 selfOrigin = base.getSelf().GlobalTransform.origin;
        velocity = targetOrigin - selfOrigin;
        base.getSelf().MoveAndSlide(velocity, Vector3.Up);//Needs nav mesh
        if(selfOrigin.DistanceTo(targetOrigin) < 1) {
            setASignal(BSIGNAL.PASS);
        } else if(selfOrigin.DistanceTo(targetOrigin) > 20) {
            setASignal(BSIGNAL.FAIL);
        } else {
            setASignal(BSIGNAL.RUNNING);
        }
    }
    public override void Exit() {
        base.Exit();
    }
}