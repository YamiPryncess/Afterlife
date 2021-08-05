using Godot;
using System.Collections.Generic;

public class Spirit : KinematicBody {
    public Master master {set; get;}
    public Dictionary<string, float[]> stats {set; get;} = new Dictionary<string, float[]>();
    //0=current, 1=maximum, 2=alterations
    //Status Aliments will be handled else where
    public StateMachine sm {set; get;}
    public Camera camera {set; get;}
    public float idleDelta {set; get;}
    public float gravity {set; get;} = -9.8f;
    public float speed {set; get;} = 6;
    public float maxSpeed {set; get;} = 8;
    public Vector3 velocity {set; get;} = new Vector3();
    public Vector3 inputDir {set; get;} = new Vector3();
    [Export] public int player {set; get;} = -1;
    public Dictionary<string, Event> events {set; get;} = 
        new Dictionary<string, Event>();
    public BTree bTree {set; get;}
    public Reality reality {set; get;}
    public Dictionary<BELIEF, bool> beliefs {set; get;} = new Dictionary<BELIEF, bool>();
    public bool animBool {set; get;} = false;
    public bool moveBool {set; get;} = false;
    public bool jumpBool {set; get;} = false;
    public override void _Ready() {
        master = GetNode<Master>("/root/Master");
        sm = new StateMachine(this);
        reality = new Reality();
        
        if(isPlayer()) {
            master.mechanics.Call("basics", this);
            camera = GetNode<Camera>("../Camera");
        }
        else if(player == -1) {
            master.behavior.Call("sample", this);
            reality.target = (Spirit)GetTree().GetNodesInGroup("Players")[0];
        }
        float[] defaultVal = new float[3]{10,10,0};
        stats.Add("Health", defaultVal);
        stats.Add("Focus", defaultVal);
        stats.Add("Hollow", defaultVal);
        stats.Add("Power", defaultVal);
        stats.Add("Speed", defaultVal);
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
    // public override void _Process(float delta) {
        
    // }
    public override void _Process(float delta) {
        idleDelta = delta;
        if(isPlayer()) {
            preProcessState(delta);
            sm.process(delta);
        } else if(bTree != null) {
            bTree.process();
        }
    }
    public override void _PhysicsProcess(float delta) {
        postProcessState(delta);
    }
    public void preProcessState(float delta) {
        //Handles Event Observer pattern changes state before state processes
        endAnimator(delta);
        if(!moveBool) 
        movement(delta);//Also preProcesses important variables for state.
        attack(delta);
    }
    public void postProcessState(float delta) {
        //Mandatory logic post state, the state can manipulate how it happens though.
        velocity = MoveAndSlide(velocity, Vector3.Up); //Always moves but states like idle can manipulate it.
        rotate(delta);
        moveBool = false;
    }
    public void endAnimator(float delta) {
        if(!sm.animator.IsPlaying() && sm.nextState == null && animBool == true) {
            events["AnimEnd"].validate(this);
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
    public void movement(float delta) { //probably won't use but it's beneficial because delta
        inputDir = new Vector3();
        Basis camBasis = camera.GlobalTransform.basis;
        Vector3 fixedBasisX = new Vector3(camBasis.x.x, 0, camBasis.x.z).Normalized();
        Vector3 fixedBasisZ = new Vector3(camBasis.z.x, 0, camBasis.z.z).Normalized(); 
        if (Input.IsActionPressed(pad("move_forward"))) 
            inputDir -= fixedBasisZ * Input.GetActionStrength(pad("move_forward"));
        if (Input.IsActionPressed(pad("move_backward"))) 
            inputDir += fixedBasisZ * Input.GetActionStrength(pad("move_backward"));
        if (Input.IsActionPressed(pad("move_left")))
            inputDir -= fixedBasisX * Input.GetActionStrength(pad("move_left"));
        if (Input.IsActionPressed(pad("move_right")))
            inputDir += fixedBasisX * Input.GetActionStrength(pad("move_right"));
        if (inputDir.Length() > 1.0) {
            inputDir = inputDir.Normalized();
        }
        //inputDir.y = 0;
        velocity = new Vector3(inputDir.x * speed, velocity.y, inputDir.z * speed);
        if(velocity.Length() > maxSpeed){
            velocity = velocity.Normalized() * maxSpeed;
        }
        velocity += new Vector3(velocity.x, 
        velocity.y + gravity, velocity.z); //Keep old gravity velocity

        moveBool = true;
        if(sm.enforceUpdate) return;
        if(inputDir != new Vector3(0,0,0) ) {//velocity is true
            events["InputDirection"].validate(this);
        } else {//velocity is false
            events["NoDirection"].validate(this);
        }
    }

    public void rotate(float delta) {
        Vector3 origin = GlobalTransform.origin;
        Vector3 pathLook = new Vector3(inputDir.x + origin.x,
        origin.y, inputDir.z + origin.z);
        if(inputDir.Length() > .1) {
            LookAt(pathLook, Vector3.Up);
        }
    }
    public void attack(float delta) {
        if(Input.IsActionJustPressed(pad("square"))) {
            GD.Print("Button has been pressed!");
            events["AttackPressed"].validate(this);
        }
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
    //public override void _Input(InputEvent @event) {
    //     if(@event is InputEventKey key) {
    //         GD.Print("Inside input");
    //         if(key.IsActionPressed("move_forward") 
    //         || key.IsActionPressed("move_backward") 
    //         || key.IsActionPressed("move_left") 
    //         || key.IsActionPressed("move_right")) {
    //             GD.Print("Inside direction");
    //             events["InputDirection"].validate();
    //         } else if(key.IsActionReleased("move_forward") 
    //         || key.IsActionReleased("move_backward") 
    //         || key.IsActionReleased("move_left") 
    //         || key.IsActionReleased("move_right")) {
    //             events["NoDirection"].validate();
    //         }

    //         if(key.IsActionPressed("attack")) {

    //         } else if(key.IsActionReleased("attack")) {

    //         }
    //     }
    // }