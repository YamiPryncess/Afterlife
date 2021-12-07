using Godot;
using System.Collections.Generic;

public class Spirit : KinematicBody {
    public Master master {set; get;}
    public Dictionary<string, float[]> stats {set; get;} = new Dictionary<string, float[]>();
    //0=current, 1=maximum, 2=alterations
    //Status Aliments will be handled else where
    public StateMachine sm {set; get;}
    public Camera camera {set; get;}
    public Hud hud {set; get;}
    public Phaser phaser {set; get;}
    public float idleDelta {set; get;}
    public float gravity {set; get;} = -9.8f/3;
    public float speed {set; get;} = 12;
    public float jumpImpulse {set; get;} = 50;
    public float maxSpeed {set; get;} = 16;
    public Vector3 velocity {set; get;} = new Vector3();
    public Vector3 inputDir {set; get;} = new Vector3();
    //public float jumpSum {set; get;}
    public Vector3 rotateDir {set; get;} = new Vector3();
    //public bool autoInput {set; get;} = false;
    [Export] public int player {set; get;} = -1;
    public Dictionary<string, Event> events {set; get;} = 
        new Dictionary<string, Event>();
    public BTree bTree {set; get;}
    public Reality reality {set; get;}
    public Dictionary<BELIEF, bool> beliefs {set; get;} = new Dictionary<BELIEF, bool>();
    public bool animBool {set; get;} = false;
    public bool moveBool {set; get;} = false;
    public bool jumpBool {set; get;} = false;
    public float health = 100;
    public override void _Ready() {
        master = GetNode<Master>("/root/Master");
        sm = new StateMachine(this);
        reality = new Reality();
        phaser = GetNode<Phaser>("Phaser");
        hud = (Hud)GetTree().GetNodesInGroup("Huds")[player > -1 ? player - 1 : 1];
        GD.Print(player, hud);
        
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
    public override void _Process(float delta) {
        idleDelta = delta;
        health = Mathf.Clamp(health, 0, 100);
        if(health <= 0) { Visible = false; }
        if(isPlayer()) {
            preProcessState(delta);
            sm.process(delta);
        } else if(bTree != null) {
            bTree.process();
        }
    }
    public override void _PhysicsProcess(float delta) {
        //if(player == 1) GD.Print(sm.currentState.name, " ", velocity);
        postProcessState(delta);
    }
    public void frameSignal() {//Game is idle process delta based so I don't need this unless as an option.
        if(phaser.phaseState > 0) {
            phaser.frame++;
        }
    }
    public void preProcessState(float delta) {
        //Handles Event Observer pattern changes state before state processes
        endAnimator(delta);
        if(!moveBool) 
        moveInput();//Also preProcesses important variables for state.
        jumpInput(delta);
        attack(delta);
        phaser.phase(delta, this);
    }
    public void postProcessState(float delta) {
        //Mandatory logic post state, the state can manipulate how it happens though.
        phaser.solidify(this);
        moveVel();
        velocity = MoveAndSlide(velocity, Vector3.Up); //Always moves but states like idle can manipulate it.
        rotate(delta);
        endLoop();
    }
    public void endLoop() {//For restarting state.
        moveBool = false;
        inputDir = Vector3.Zero;
        jumpBool = false;
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
    public void updateDirection(Vector3 vector) {
        inputDir = vector;
        rotateDir = vector;
    }
    public void moveInput() {
        inputDir = new Vector3();
        Basis camBasis = camera.GlobalTransform.basis;
        Vector3 fixedBasisX = new Vector3(camBasis.x.x, 0, camBasis.x.z).Normalized();
        Vector3 fixedBasisZ = new Vector3(camBasis.z.x, 0, camBasis.z.z).Normalized(); 
        //bool inputPressed = false;
        if (Input.IsActionPressed(pad("move_forward"))) { //inputPressed = true;
            inputDir -= fixedBasisZ * Input.GetActionStrength(pad("move_forward"));}
        if (Input.IsActionPressed(pad("move_backward"))) { //inputPressed = true;
            inputDir += fixedBasisZ * Input.GetActionStrength(pad("move_backward"));}
        if (Input.IsActionPressed(pad("move_left"))) { //inputPressed = true;
            inputDir -= fixedBasisX * Input.GetActionStrength(pad("move_left"));}
        if (Input.IsActionPressed(pad("move_right"))) { //inputPressed = true;
            inputDir += fixedBasisX * Input.GetActionStrength(pad("move_right"));}
        
        //if(!inputPressed) inputDir = Vector3.Zero;
        if (inputDir.Length() > 1.0) {
            inputDir = inputDir.Normalized();
        }
        rotateDir = inputDir;

        moveBool = true;
        if(sm.enforceUpdate) return;
        if(inputDir != Vector3.Zero ) {//velocity is true
            events["InputDirection"].validate(this);
        } else {//velocity is false
            events["NoDirection"].validate(this);
        }
    }
    public void jumpInput(float delta) {
        if(sm.enforceUpdate) return;
        if(Input.IsActionJustPressed(pad("jump")) && IsOnFloor()) jumpBool = true;
        else jumpBool = false;
        //if(Input.IsActionJustReleased(pad("jump"))) jumpBool = false;
        //if(jumpBool) jumpSum = Mathf.Clamp(jumpSum + (jumpImpulse * delta), -gravity, 50f);

        if(jumpBool) {
            //events["Jump"].validate(this);
        } else {
            //events[NoJump].validate(this);
        }
    }
    public Vector3 chooseVel(VELOCITY choice = VELOCITY.CONTROL) {
        switch(choice) {
            case VELOCITY.CONTROL:
                return inputDir * speed;
            case VELOCITY.SKII:
                return velocity + (inputDir * speed);
            case VELOCITY.PINBALL:
                return (velocity + inputDir)  * speed;
            case VELOCITY.SOAP:
                return (inputDir != Vector3.Zero ? velocity : Vector3.Zero) + (inputDir * speed);
            case VELOCITY.RUBBER:
                return ((inputDir != Vector3.Zero ? velocity : Vector3.Zero) + inputDir)  * speed;
        }
        return Vector3.Zero;
    }
    public void moveVel() {
        Vector3 speedDir = chooseVel(VELOCITY.CONTROL);
        if(speedDir.Length() > maxSpeed){
            speedDir = speedDir.Normalized() * maxSpeed;
        }
        float newGravity = IsOnFloor() ? 0 : gravity; //I should do jump states for whether you're
        newGravity = phaser.phaseVal >= phaser.phasable ? 0 : gravity; //Phased or not.
        velocity = new Vector3(speedDir.x, jumpBool ? jumpImpulse : 0 + //Why plus zero?
        velocity.y + newGravity, speedDir.z); //I need to debug what's happening here better. inc w/ phase.
        //if(player == -1) GD.Print(velocity);
    }
    public void rotate(float delta) {
        Vector3 origin = GlobalTransform.origin;
        Vector3 pathLook = new Vector3(rotateDir.x + origin.x,
        origin.y, rotateDir.z + origin.z);
        if(rotateDir.Length() > .1) {
            LookAt(pathLook, Vector3.Up);
        }
    }
    public void attack(float delta) {
        if(Input.IsActionJustPressed(pad("square"))) {
            //GD.Print("Button has been pressed!");
            events["AttackPressed"].validate(this);
        }
    }
    public void hurt() {
        health = health - (20 * (1-phaser.phaseVal));
        hud.healthProgress.Value = health;
    }
}
public enum VELOCITY {
    CONTROL, ACCELERATE, DEACCELERATE, PINBALL, SKII, RUBBER, SOAP
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
    //moveVel() {
        //Old Code that is redundant
        // //inputDir.y = 0;
        // velocity = new Vector3(inputDir.x * speed, velocity.y, inputDir.z * speed);
        // if(velocity.Length() > maxSpeed){
        //     velocity = velocity.Normalized() * maxSpeed;
        // }
        // velocity += new Vector3(velocity.x, 
        // velocity.y + gravity, velocity.z); //Keep old gravity velocity
        //velocity.y plus velocity.y (double gravity?) gives an 
        //interesting effect on resulting linearVelocity.y from move and slide 


        //Blood Souls on Ice
        //         velocity = new Vector3(velocity.x+inputDir.x * speed, velocity.y, velocity.z +inputDir.z * speed);
        // if(velocity.Length() > maxSpeed){
        //     velocity = velocity.Normalized() * maxSpeed;
        // }
        // velocity += new Vector3(velocity.x, 
        // velocity.y + gravity, velocity.z); //Keep old gravity velocity
    //}