using Godot;
using System.Collections.Generic;

public class BNode {
    //Class Construction Props
    public string name {set; get;}
    public Spirit self {set; get;}
    public BNode parent {set; get;} = null;
    public List<BNode> children {set; get;} = new List<BNode>();
    public int currentChild {set; get;} = 0;
    //Process Props
    public BTYPE bType {set; get;} = BTYPE.SEQUENCE;
    public int priority {set; get;} = 0;
    public int frequency {set; get;} = 0;
    public float probability {set; get;} = 0;
    //Decorator Props
    public List<DECORATOR> decorator {set; get;} = new List<DECORATOR>();
    public int runCount {set; get;}
    public int runLimit {set; get;} = 1;
    //Constructing Methods
    public BNode() {}
    public BNode(Spirit _self) {
        self = _self;
    }
    public BNode(Spirit _self, BTYPE _bType) {
        self = _self;
        bType = _bType;
    }
    //Main Process Methods
    public BSIGNAL process() { //Processes all of its own children
        BSIGNAL bSignal = BSIGNAL.STANDBY;
        switch (bType) {
            case BTYPE.SELECTOR: 
                bSignal = children[currentChild].process();
                switch(bSignal) {
                    case BSIGNAL.FAIL:
                        bSignal = (currentChild >= children.Count-1 ? BSIGNAL.FAIL : BSIGNAL.RUNNING);
                        if(bSignal == BSIGNAL.FAIL) reset();
                        else currentChild++;
                        break;
                    case BSIGNAL.PASS:
                        bSignal = BSIGNAL.PASS;
                        reset();
                        break;
                    case BSIGNAL.RUNNING:
                        bSignal = BSIGNAL.RUNNING;
                        break;
                }
            break;
            case BTYPE.SEQUENCE:
                bSignal = children[currentChild].process();
                switch(bSignal) {
                    case BSIGNAL.FAIL:
                        bSignal = BSIGNAL.FAIL;
                        reset();
                        break;
                    case BSIGNAL.PASS:
                        bSignal = (currentChild == children.Count-1 ? BSIGNAL.PASS : BSIGNAL.RUNNING);
                        if(bSignal == BSIGNAL.PASS) reset();
                        else currentChild++;
                        break;
                    case BSIGNAL.RUNNING:
                        bSignal = BSIGNAL.RUNNING;
                        break;
                }
                break;
            case BTYPE.RANDOM_SELECTOR:
                switch (bSignal) {
                    case BSIGNAL.PASS:
                        break;
                    case BSIGNAL.FAIL:
                        break;
                    case BSIGNAL.RUNNING:
                        break;
                }
                break;
            case BTYPE.RANDOM_SEQUENCE:
                switch (bSignal) {
                    case BSIGNAL.PASS:
                        break;
                    case BSIGNAL.FAIL:
                        break;
                    case BSIGNAL.RUNNING:
                        break;
                }
                break;
            case BTYPE.ACTION:
                BAction action = (BAction) this;
                bSignal = action.aProcess();
                if(bSignal != BSIGNAL.RUNNING) reset();
                break;
            case BTYPE.GOAL:
                BGoal goal = (BGoal) this;
                bSignal = goal.gProcess();
                if(bSignal != BSIGNAL.FAIL) reset();
                break;
        }
        GD.Print(bType, "-", bSignal);
        return processDecorate(bSignal);//My Choice: Reset happens before decorate.
    }
    protected BSIGNAL processDecorate(BSIGNAL pSignal) {
        BSIGNAL dSignal = pSignal;
        //Machinery After Effects
        if(pSignal == BSIGNAL.RUNNING) {
            runCount++;//Set back to 0 by reset.
        }

        for(int i = 0; i < decorator.Count; i++) {
            //Added in order of priority. First one that passes wins. Update: Not true, they all win.
            switch (decorator[i]) {
                case DECORATOR.SUCCEEDER:
                    dSignal = BSIGNAL.PASS;
                    break;
                case DECORATOR.INVERTER:
                    if(pSignal == BSIGNAL.PASS) {
                        dSignal = BSIGNAL.FAIL;
                        break;
                    } else if (pSignal == BSIGNAL.FAIL) {
                        dSignal = BSIGNAL.PASS;
                        break;
                    }
                    break;
                case DECORATOR.UNTILFAIL:
                    if(pSignal != BSIGNAL.FAIL) {
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
    protected BNode findRoot(BNode descendant) {
        BNode dRoot = descendant;
        while(dRoot.parent != null) {
            dRoot = dRoot.parent;
        }
        return dRoot;
    }
    public void reset() {
        currentChild = 0;
        runCount = 0;
        for(int i = 0; i < children.Count; i++) {
            children[i].reset();
        }
    }
}
public enum BTYPE {
    ACTION, GOAL, SELECTOR, SEQUENCE, RANDOM_SELECTOR, RANDOM_SEQUENCE
}
public enum BSIGNAL {
    PASS, FAIL, RUNNING, INTERUPTED, STANDBY
}

//Old Code

    //Getters & Setters
    // public BNode getParent() {
    //     return parent;
    // }
    // public void setParent(BNode _parent) {
    //     parent = _parent;
    // }
    // public BNode getChild(int index) {
    //     return children[index];
    // }
    //     public void setDecorator(DECORATOR _dec) {
    //     decorator.Add(_dec);
    // }
    // public List<BNode> getChildren() {
    //     return children;
    // }
    // public List<BNode> setChild(BNode bChild) {
    //     children.Add(bChild);
    //     return children;
    // }
    // 
    // public BTYPE getBType() {
    //     return bType;
    // }
    // public int getPriority() {//To be available for use by calling object
    //     return priority;
    // }
    // public KinematicBody getSelf() {//To be used by actions
    //     return self;
    // }