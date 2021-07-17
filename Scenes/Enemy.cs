using Godot;
using System.Collections.Generic;

public class Enemy : KinematicBody {
    BTree bTree;
    private Dictionary<string, float[]> stats = new Dictionary<string, float[]>();
    private Dictionary<BELIEF, bool> beliefs = new Dictionary<BELIEF, bool>();
    public override void _Ready() {
        float[] defaultVal = new float[3]{10,10,0};
        stats.Add("Health", defaultVal);
        stats.Add("Focus", defaultVal);
        stats.Add("Hollow", defaultVal);
        stats.Add("Power", defaultVal);
        stats.Add("Speed", defaultVal);

        bTree = new BTree(this);//Start Root
        bTree.newAction(new Approach(this, 5)).chancesPerCycle(5, 5).ascendTree()
        
        .descendTree(new BTree(this, BTYPE.RANDOM_SELECTOR))
        .newAction(new Approach(this, 2));
    }
    public override void _PhysicsProcess(float delta) {

    }
}
public enum BTYPE {
    ROOT, ACTION, SELECTOR, SEQUENCE, RANDOM_SELECTOR, RANDOM_SEQUENCE
}
public enum DECORATOR {
    INVERTER, SUCCEEDER, LIMITER, UNTILFAIL, NONE
}
public enum BELIEF {
    DYING_TARGET, CLOSE_TARGET, MID_TARGET, FAR_TARGET, DYING_SELF
}
public enum BPROCESS {
    PASS, FAIL, RUNNING, INTERUPTED, STANDBY
}
public class BTree {
    KinematicBody self;
    string name;
    BELIEF goal;
    DECORATOR decorator;
    BTYPE bType;
    List<BTree> children;
    public BTree(KinematicBody _self, BTYPE _bType = BTYPE.SELECTOR) {
        self = _self;
        decorator = DECORATOR.NONE;
        bType = _bType;
    }
    public void decorate(DECORATOR _decorator) {
        decorator = _decorator;
    }
    public Action newAction(Action action) {
        children.Add(action);
        return action;
    }
    public BTree descendTree(BTree bChild) { //Make it throw an error if type is Action.
        children.Add(bChild);
        return bChild;
    }
}

public class Action : BTree {
    KinematicBody self;
    private Action action;
    private BTree parent;
    private int frequency = 0;
    private float probability = 0;
    public Action(KinematicBody _self, BTYPE bType = BTYPE.ACTION, BTree _parent = null) : base(_self, bType) {
        self = _self;
        parent = _parent;
    }
    public void setAction(Action _action) {
        action = _action;
    }
    public BTree ascendTree() {
        return parent;
    }
    public Action chancesPerCycle(int _frequency, float _probability) {
        frequency = _frequency;
        probability = _probability;
        return this;
    }
    public virtual BPROCESS act(KinematicBody target = null) {
        return BPROCESS.STANDBY;
    }
}

public class Approach : Action {
    private KinematicBody self;
    private float speed = 1;
    Vector3 velocity = new Vector3();
    public Approach (KinematicBody _self, float _speed, 
                    BTYPE bType = BTYPE.ACTION) : base(_self, bType) {
        self = _self;
        speed = 1;
        setAction(this);
    }
    public override BPROCESS act(KinematicBody target) {
        velocity = target.GlobalTransform.origin - self.GlobalTransform.origin;
        return BPROCESS.RUNNING;
    }
}