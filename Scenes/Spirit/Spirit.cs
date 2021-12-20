using Godot;
using System.Collections.Generic;

public class Spirit : KinematicBody {
    public Master master {set; get;}
    public StateMachine sm {set; get;}
    public Camera camera {set; get;}
    public Hud hud {set; get;}
    public Stats stats {set; get;}
    public Phaser phaser {set; get;}
    public float idleDelta {set; get;}
    public Movement move {set; get;}
    [Export] public int player {set; get;} = -1;
    public Dictionary<MECHEVENT, Event> events {set; get;} = 
        new Dictionary<MECHEVENT, Event>();
    public BTree bTree {set; get;}
    public Reality reality {set; get;}
    public Dictionary<BELIEF, bool> beliefs {set; get;} = new Dictionary<BELIEF, bool>();
    public bool animBool {set; get;} = false;
    public bool stanceBool {set; get;} = false;
    public override void _Ready() {
        master = GetNode<Master>("/root/Master");
        sm = new StateMachine(this);
        reality = new Reality();
        phaser = GetNode<Phaser>("Phaser");
        hud = (Hud)GetTree().GetNodesInGroup("Huds")[player > -1 ? player - 1 : 1];
        stats = new Stats();
        move = new Movement(this);
        
        if(isPlayer()) {
            master.mechanics.Call("basics", this);
            camera = GetNode<Camera>("../Camera");
        }
        else if(player == -1) {
            master.behavior.Call("sample", this);
            reality.target = (Spirit)GetTree().GetNodesInGroup("Players")[0];
        }
    }
    public string pad(string button) {
        string btn = button + "_" + player;
        return btn;
    }
    public bool isPlayer() {
        if(player < 5 && player > 0) {
            return true;
        } else return false;
    }
    public override void _Process(float delta) {
        idleDelta = delta;
        if(stats.lifePoints <= 0) { Visible = false; }
        if(isPlayer()) {
            preProcessState(delta);//Mutates controller variables like inputDir, also calls state change events
            sm.process(delta);//Mutates state variables like speedDir & rotateDir (may use velocity to change them)
        } else if(bTree != null) {
            bTree.process();
        }
    }
    public override void _PhysicsProcess(float delta) {
        //if(player == 1) GD.Print(sm.currentState.name, " ", velocity);
        postProcessState(delta); //Mutates physics variables like Velocity, then resets controller and input variables
    }
    public void frameSignal() {//Game is idle process delta based so I don't need this unless as an option.
        if(phaser.phaseState > 0) {
            phaser.frame++;
        }
    }
    public void preProcessState(float delta) {
        //Handles Event Observer pattern changes state before state processes
        endAnimator(delta);
        //if(!move.moveBool) 
        move.moveInput();//Also preProcesses important variables for state.
        move.jumpInput(delta);
        attack(delta);
        phaser.phase(delta, this);
    }
    public void postProcessState(float delta) {
        //Mandatory logic post state, the state can manipulate how it happens though.
        phaser.solidify(this);
        move.updateVel();
        move.velocity = MoveAndSlide(move.velocity, Vector3.Up); //Always moves but states like idle can manipulate it.
        move.rotate(delta);
        endLoop();
    }
    public void endLoop() {//For restarting state.
        move.moveBool = false;
        move.inputDir = Vector3.Zero;
        move.speedDir = Vector3.Zero;
        move.jumpBool = false;
    }
    public void endAnimator(float delta) {
        if(!sm.animator.IsPlaying() && sm.nextState == null && animBool == true) {
            events[MECHEVENT.ANIMEND].validate(this);
            animBool = false;
        } else {
            animBool = true;
        }
    }
    public void final_frame() {
        sm.finalFrame = true;
    }
    public void enforce_anim() {
        sm.enforceUpdate = true;
    }
    public void attack(float delta) {
        if(Input.IsActionJustPressed(pad("attack"))) {
            //GD.Print("Button has been pressed!");
            events[MECHEVENT.ATTPRESS].validate(this);
        }
    }
    public void hurt() {
        stats.modify(STATS.LIFE, -(20 * (1-stats.phasePoints)));
        hud.healthProgress.Value = stats.lifePoints;
    }
}
//Old
    // public void processState(float delta) {//StateMachine.Update() runs once per frame confirmed.
    //     //Makes update() frames run regardless if state is exited before in preProcessState().
    //     if(currentState.getStage() == EVENT.EXIT) {
    //         currentState = currentState.process(delta);//Update hasn't happened yet, so exit first.
    //     }
    //     currentState = currentState.process(delta);//Now we run this always for the update.
    // }