using Godot;
using System.Collections.Generic;

public class Enemy : KinematicBody {
    BTree bTree;
    BTree currentState;
    private Dictionary<string, float[]> stats = new Dictionary<string, float[]>();
    private Dictionary<BELIEF, bool> beliefs = new Dictionary<BELIEF, bool>();
    public override void _Ready() {
        float[] defaultVal = new float[3]{10,10,0};
        stats.Add("Health", defaultVal);
        stats.Add("Focus", defaultVal);
        stats.Add("Hollow", defaultVal);
        stats.Add("Power", defaultVal);
        stats.Add("Speed", defaultVal);

        bTree = new BTree(this, true, 0, BTYPE.SELECTOR);//Start Root
        bTree.action(new Approach(this, 5, 0)).chancesPerCycle(5, 5).ascendTree()
        
        .descendTree(new BTree(this, BTYPE.RANDOM_SELECTOR, 0))
        .action(new Approach(this, 2));
    }
    public override void _PhysicsProcess(float delta) {
        processBTree(delta);
    }
    public void processBTree(float delta) {
        BTree priorGoal = bTree.goalsProcess(beliefs);
        if(priorGoal != null && priorGoal.getPriorityLvl() 
                            > currentState.getPriorityLvl()) {
           currentState = priorGoal.btProcess(delta, beliefs);
        } else if(currentState.getBType() == BTYPE.ACTION) {
            //Wanted to startAction() with approach but how would you get the arguments for each action...
            //I also can't figure out how to access the approach through currentState... probably can't.
        } else {
            currentState = currentState.btProcess(delta, beliefs);
        }
    }
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
public class BTree {
    //Class Construction Props
    KinematicBody self;
    private bool isRoot = false;
    private BTree parent = null;
    //Tree Construction Props
    List<BTree> children;
    //Root Goal Props (Only works in root)
    private List<BTree> childGoals = new List<BTree>();//Set as soon as they're added but they must be added in depth first order.
    private BTree latestGoalChild;
    //Leaf Goal Props
    private BSIGNAL bSignal = BSIGNAL.STANDBY;
    BELIEF condition = BELIEF.ANY;
    bool leafGoal = true;
    //Process Props
    private BTYPE bType;
    private int priorityLvl = 0;
    //Decorator Props
    List<DECORATOR> decorator; //Added in order of priority.
    int runCount;
    int runLimit = 1;

    //Constructing Methods
    public BTree(KinematicBody _self, bool _isRoot, int _priorityLvl = 0, BTYPE _bType = BTYPE.SELECTOR) {//For Roots
        self = _self;
        bType = _bType;
        priorityLvl = _priorityLvl;
        isRoot = _isRoot;
    }
    public BTree(KinematicBody _self, BTYPE _bType, int _priorityLvl = 0) {//For Composites
        self = _self;
        bType = _bType;
        priorityLvl = _priorityLvl;
    }
    public BTree(KinematicBody _self, int _priorityLvl = 0, BTYPE _bType = BTYPE.ACTION) {//For Actions
        self = _self;
        bType = _bType;
        priorityLvl = _priorityLvl;
    }
    public BTree descendTree(BTree bChild) { //Make it throw an error if type is Action.
        children.Add(bChild);
        bChild.parent = this;
        return bChild;
    }
    public BTree ascendTree() {
        if(!isRoot || parent != null) {
            return parent;
        } else {
            GD.Print("ascendTree Method Error: This is root so no parent.");
            return this;
        }
    }
    public void decorate(DECORATOR _decorator) {
        decorator.Add(_decorator);
        //Added in order of priority. First one that passes wins.
    }
    public Action action(Action action) {
        children.Add(action);
        return action;
    }
    public BTree goal(BELIEF belief, bool _goal = true) {
        condition = belief;
        leafGoal = true;
        findRoot(this).childGoals.Add(this);
        return this;
    }
    //Main Process Methods
    public BTree goalsProcess(Dictionary<BELIEF, bool> beliefs) { //Should only be used by root
        if(parent == null && childGoals.Count > 0) {//Only usable by Root.
            for(int i = 0; i < childGoals.Count; i++) {
                if(!childGoals[i].isGoalMet(beliefs)) {
                    latestGoalChild = childGoals[i];//changes root's state
                    return childGoals[i];
                } else if (Object.ReferenceEquals(childGoals[i], latestGoalChild)) {
                    return null;
                } //Stops at the latestGoal rather than checking every goal.
            }
        }
        return null;
    }
    public BTree btProcess(float delta, Dictionary<BELIEF, bool> beliefs, bool decorated = true) { //Processes all of its own children
        bSignal = BSIGNAL.STANDBY;
        if(!isGoalMet(beliefs)) switch (bType) {
            case BTYPE.SELECTOR: 
                for(int i = 0; i < children.Count; i++) {
                    BTree bResult = children[i].btProcess(delta, beliefs);
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
                    BTree bResult = children[i].btProcess(delta, beliefs);
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
            //Added in order of priority. First one that passes wins.
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
    private BTree findRoot(BTree descendant) {
        BTree dRoot = descendant;
        while(dRoot.isRoot == true) {
            dRoot = dRoot.parent;
        }
        return dRoot;
    }
    private bool isGoalMet(Dictionary<BELIEF, bool> beliefs) {
        if(condition == BELIEF.ANY){
            return false;
        } else {
            latestGoalChild = this;//When ever goal is not ANY, automatically sets as most recent goal regardless if it's index 0 or 19 in childGoals List.
            if(beliefs[condition] == leafGoal) {
                return true;
            }
        } 
        return false;//Belief is not any but it returns false
    }
    //Getters    
    public BTYPE getBType() {
        return bType;
    }
    public int getPriorityLvl() {//To be available for use by calling object
        return priorityLvl;
    }
    public KinematicBody getSelf() {//To be used by actions
        return self;
    }
}

public class Action : BTree {
    private EVENT stage;
    private int frequency = 0;
    private float probability = 0;
    private int priorityLvl;
    private BSIGNAL aSignal = BSIGNAL.STANDBY;
    public Action(KinematicBody _self, int _priorityLvl = 0,
                BTYPE _bType = BTYPE.ACTION) : base(_self, _priorityLvl, _bType) {
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
    public Action chancesPerCycle(int _frequency, float _probability) {
        frequency = _frequency;
        probability = _probability;
        return this;
    }
    public BSIGNAL processAction() {
        if (stage == EVENT.ENTER) Enter(); 
	    if (stage == EVENT.UPDATE) Update();
	    aSignal = base.processDecorate(aSignal);
        decideExit();

        if (stage == EVENT.EXIT) Exit();
        return getASignal();
    }
    public void decideExit() {
        if(aSignal != BSIGNAL.RUNNING) {
            stage = EVENT.EXIT;
        }
    }
}

public class Approach : Action {
    private float speed = 1;
    Vector3 velocity = new Vector3();
    KinematicBody target;
    public Approach (KinematicBody _self, float _speed,
                    int _priorityLvl = 0) : base(_self, _priorityLvl, BTYPE.ACTION) {
        speed = 1;
    }
    public BSIGNAL startAction(KinematicBody _target) {//Wanted to startAction() with approach but how would you get the arguments for each action...
        target = _target;
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