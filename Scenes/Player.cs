using Godot;
using System.Collections.Generic;

public class Player : KinematicBody {
    private Dictionary<string, float[]> stats = new Dictionary<string, float[]>();
    //0=current, 1=maximum, 2=alterations
    //Status Aliments will be handled else where
    private StateMachine currentState;
    private Camera camera;
    private float gravity = -9.8f;
    private Vector3 velocity = new Vector3();
    Vector3 inputDir = new Vector3();
    private float speed = 6;
    private float maxSpeed = 8;
    [Export] public int playerNum = 0;
    Dictionary<string, Event> events;
    public override void _Ready() {
        camera = GetNode<Camera>("../Camera");
        //camera.setTarget(GetChild<Spatial>(0));
        float[] defaultVal = new float[3]{10,10,0};
        stats.Add("Health", defaultVal);
        stats.Add("Focus", defaultVal);
        stats.Add("Hollow", defaultVal);
        stats.Add("Power", defaultVal);
        stats.Add("Speed", defaultVal);
        currentState = new Idle(this);
        events = new Dictionary<string, Event>();
        events.Add("InputDirection", new Event(this, "InputDirection", new Dictionary<STATE, STATE>()));
        events["InputDirection"].addCondition(STATE.IDLE, STATE.MOVE);
        events["InputDirection"].addCondition(STATE.MOVE, STATE.NULL);
        events.Add("NoDirection", new Event(this, "NoDirection", new Dictionary<STATE, STATE>()));
        events["NoDirection"].addCondition(STATE.MOVE, STATE.IDLE);
        events["NoDirection"].addCondition(STATE.IDLE, STATE.NULL);

    }
    public StateMachine getCurrentState() {
        return (StateMachine)currentState;
    }
    public float getStat(string index){
        return stats[index][0];
    }
    public float setStat(string index, int change) {
        stats[index][0] = Mathf.Min(stats[index][0] + change,
            stats[index][1] + stats[index][2]);
        return stats[index][0];
    }
    public float getMax(string index) {
        return stats[index][1] + stats[index][2];
    }
    public float alterMax(string index, float alteration) {
        stats[index][2] += alteration;
        return getMax(index);
    }
    public override void _Process(float delta) {
        
    }
    public override void _PhysicsProcess(float delta) {
        checkMove(delta);
        currentState = currentState.process(delta);
    }
    public void checkMove(float delta) {
        inputDir = new Vector3();
        if(playerNum == 0){
            Basis camBasis = camera.GlobalTransform.basis;
            Vector3 fixedBasisX = new Vector3(camBasis.x.x, 0, camBasis.x.z).Normalized();
            Vector3 fixedBasisZ = new Vector3(camBasis.z.x, 0, camBasis.z.z).Normalized(); 
            if (Input.IsActionPressed("move_forward")) inputDir -= fixedBasisZ * Input.GetActionStrength("move_forward");
            if (Input.IsActionPressed("move_backward")) inputDir += fixedBasisZ * Input.GetActionStrength("move_backward");
            if (Input.IsActionPressed("move_left")) inputDir -= fixedBasisX * Input.GetActionStrength("move_left");
            if (Input.IsActionPressed("move_right")) inputDir += fixedBasisX * Input.GetActionStrength("move_right");
        
            if(inputDir != new Vector3(0,0,0)) {//inputDir is true
                events["InputDirection"].validate();
            } else {//inputDir is false
                events["NoDirection"].validate();
            }
        }
    }
    public void movement(float delta) {
        if(playerNum == 0){
            if (inputDir.Length() > 1.0) {
                inputDir = inputDir.Normalized();
            }
            inputDir.y = 0;
            Vector3 origin = GlobalTransform.origin;
            Vector3 velocityNew = inputDir * speed;
            if(velocityNew.Length() > maxSpeed){
                velocityNew = velocityNew.Normalized() * maxSpeed;
            }
            velocityNew.y += velocity.y + gravity * delta;
            velocity = MoveAndSlide(velocityNew, Vector3.Up);
            Vector3 pathLook = new Vector3(velocity.x + origin.x,
                                    origin.y, velocity.z + origin.z);
            if(inputDir.Length() > .1) {
                LookAt(pathLook, Vector3.Up);
            }
        }
    }
}

public class Event {
    //Events switch states, not state code.
    //So state code doesn't need to specify anything about 
    //switching to a specific state. It should be able to however
    //using nextstate variable and it can also make mandatory
    //requirements for switching to any state generally too.
    //I'd prefer to make groups based on event types.
    //So one group of events may interupt while another may queue
    //A state to the next action. Then maybe the state can check
    //for itself if it wants to allow that type of event & process it.
    private string name = "";
    private Player player;
    private EVENTTYPE eventType;
    private Dictionary<STATE, STATE> condition = new Dictionary<STATE, STATE>();
    public Event(Player _player, string _name, Dictionary<STATE, STATE> _condition) {
        player = _player;
        name = _name;
        condition = _condition;
    }
    public void addCondition(STATE curCondition, STATE nextTransition) {
        condition.Add(curCondition, nextTransition);
    }
    public void validate() {
        StateMachine currentState = player.getCurrentState();
        STATE curStateEnum = currentState.name;
        StateMachine nextState;
        if(condition.ContainsKey(curStateEnum)
            && condition[curStateEnum] != STATE.NULL) {
            nextState = currentState.enumToState(condition[curStateEnum]);
            if(nextState != null) {
                currentState.setStage(EVENT.EXIT);
                currentState.setNextState(nextState);
            }
        }
    }
}
public enum STATE {
    IDLE, MOVE, RUN, ATTACK, JUMP, NULL
}
public enum EVENT {
    ENTER, UPDATE, EXIT
}
public enum EVENTTYPE {
    QUEUE, IMMEDIATE, SIMULTANEOUS
} 
public class StateMachine {
    public STATE name; 
    protected EVENT stage;
    protected StateMachine nextState;
    protected Player player;
    public float delta = 0;

    public StateMachine() {
        stage = EVENT.ENTER;
    }
    public void setStage(EVENT _stage) {
        stage = _stage;
    }
    public void setNextState(StateMachine _nextState) {
        nextState = _nextState;
    }
    public StateMachine enumToState(STATE newState) {
        switch (newState){
            case STATE.MOVE:
                return new Move(player);
            case STATE.IDLE:
                return new Idle(player);
            case STATE.NULL:
                return null;
            default: 
                return null;
      }
    }
    public virtual void Enter() { stage = EVENT.UPDATE; }
    public virtual void Update() { stage = EVENT.UPDATE;}
    public virtual void Exit() { stage = EVENT.EXIT; } 
    public StateMachine process(float _delta) { //Here are the if statements that tell all process functions to run
	    delta = _delta;
        if (stage == EVENT.ENTER) Enter(); //Enter will change stage to update
	    if (stage == EVENT.UPDATE) Update(); //Update will do either update or exit
	    if (stage == EVENT.EXIT) {
		    Exit(); //Exit will stop what ever current state is doing & change state; 
		    return nextState;
        }
        return this;
    }
}
public class Idle : StateMachine {
    public Idle(Player _player) {
        name = STATE.IDLE;
        base.player = _player;
    }
    public override void Enter() {
        base.Enter();
    }
    public override void Update() {
        base.Update();
    }
    public override void Exit() {
        base.Exit();
    }
}
public class Move : StateMachine {
    public Move(Player _player) {
        name = STATE.MOVE;
        player = _player;
    }
    public override void Enter() {
        base.Enter();
    }
    public override void Update() {
        base.Update();
        player.movement(delta);
    }
    public override void Exit() {
        base.Exit();
    }
}
public class Attack : StateMachine {
    public Attack(Player _player) {
        name = STATE.ATTACK;
        base.player = _player;
    }
    public override void Enter() {
        base.Enter();
    }
    public override void Update() {
        base.Update();
    }
    public override void Exit() {
        base.Exit();
    }
}