using Godot;

public class Movement {
    private Spirit self;
    public float gravity {set; get;} = -0f;
    //public float yVelocity {set; get;} = 0; //Only gets changed in IdleProcess/PreProcess/ProcessState
    public Vector3 velocity {set; get;} = new Vector3(); //Only gets changed in PhysicsProcess/PostProcess
    public float modVelocity {set; get;} = 0;
    public Vector3 inputDir {set; get;} = new Vector3();
    //public float jumpSum {set; get;}
    public Vector3 pMoveDir {set; get;} = new Vector3();
    public Vector3 pRotDir {set; get;} = new Vector3();
    private float airTime = 0;
    //public bool autoInput {set; get;} = false;

    public Movement (Spirit spirit) {
        self = spirit;
    }

    public void updateDirection(Vector3 vector) {
        pMoveDir = vector;
        pRotDir = vector;
    }
    public void crouchInput() {//Tap
        if (Input.IsActionJustPressed(self.pad("crouch"))) {
            self.events[MECHEVENT.CROUCHPRESS].validate(self);       
        }
    }
    public void dashInput() {//Tap
        if (Input.IsActionJustPressed(self.pad("dash"))) {
            self.events[MECHEVENT.DASHPRESS].validate(self);
        }
    }
    public void stanceInput() {//Hold
        if (Input.IsActionJustPressed(self.pad("stance"))) {
            self.events[MECHEVENT.STANCEPRESS].validate(self);       
        } else if (!Input.IsActionPressed(self.pad("stance"))) {
            self.events[MECHEVENT.STANCERELEASE].validate(self);       
        }
    }
    public void moveInput() {
        inputDir = new Vector3();
        Basis camBasis = self.camera.GlobalTransform.basis;
        Vector3 fixedBasisX = new Vector3(camBasis.x.x, 0, camBasis.x.z).Normalized();
        Vector3 fixedBasisZ = new Vector3(camBasis.z.x, 0, camBasis.z.z).Normalized(); 
        //bool inputPressed = false;
        if (Input.IsActionPressed(self.pad("moveUp"))) { //inputPressed = true;
            inputDir -= fixedBasisZ * Input.GetActionStrength(self.pad("moveUp"));}
        if (Input.IsActionPressed(self.pad("moveDown"))) { //inputPressed = true;
            inputDir += fixedBasisZ * Input.GetActionStrength(self.pad("moveDown"));}
        if (Input.IsActionPressed(self.pad("moveLeft"))) { //inputPressed = true;
            inputDir -= fixedBasisX * Input.GetActionStrength(self.pad("moveLeft"));}
        if (Input.IsActionPressed(self.pad("moveRight"))) { //inputPressed = true;
            inputDir += fixedBasisX * Input.GetActionStrength(self.pad("moveRight"));}
 
        //if(!inputPressed) inputDir = Vector3.Zero;
        if (inputDir.Length() > 1.0) {
            inputDir = inputDir.Normalized();
        }
    }
    public void calcMove(Vector3 direction, Vector3 rotate, float speed, float max) { //Process, inside of state
        pRotDir = rotate;
        pMoveDir = chooseVel(direction, speed, max, VELOCITY.CONTROL);
        if(pMoveDir.Length() > max){
            pMoveDir = pMoveDir.Normalized() * max;
        }
        //newGravity = self.stats.phasePoints >= self.phaser.phasable ? 0 : gravity;
    }
    public void preProcess(float delta) { //Prior to state
        STATE state = self.sm.currentState.name;
        //Air
        Godot.Collections.Array overlap = self.floor.GetOverlappingBodies();
        bool areaFloor = false;
        for (int i = 0; i < overlap.Count; i++) {
            if(overlap[i] != self) {
                areaFloor = true;
                break;
            }
        }
        airTime = areaFloor ? 0 : airTime = Mathf.Clamp(airTime + delta, 0, 10);
        if(state == STATE.JUMP) {
            if(self.IsOnFloor() && self.sm.currentState.physics == true) {
                self.events[MECHEVENT.LANDED].validate(self);
            } else if(modVelocity <= 0) { //velocity.y <= 0) {
                self.events[MECHEVENT.AIR].validate(self);
            }
        } else if(state == STATE.AIR) {
            if(self.IsOnFloor()) {
                self.events[MECHEVENT.LANDED].validate(self);
            }
        } else {//Not included in falling.
            if(Input.IsActionJustPressed(self.pad("jump")) && ((coyote() || areaFloor) || self.IsOnFloor())) {
                self.events[MECHEVENT.JUMPPRESS].validate(self);
            } else if(!self.IsOnFloor()) { //Successfully reduced IsOnFloor == false with snap and cylinder collision but player still slides off some stuff. 
                self.events[MECHEVENT.AIR].validate(self); //Might need to switch this to areaFloor and do !isOnFloor gravity in walk/idle etc instead?
                //Must apply gravity if !isonfloor either way because snap needs you to be on the floor.
            } else if(self.IsOnFloor()) { //Unimportant. Test to make sure it doesn't break stuff but it shouldn't.
                self.move.velocity = new Vector3(self.move.velocity.x, 0, self.move.velocity.z);
            }
        }
    }
    public bool coyote() {
        if(airTime < 0.1) {
            return true;
        } return false;
    }
    public Vector3 chooseVel(Vector3 direction, float _speed, float _maxSpeed, VELOCITY choice = VELOCITY.CONTROL) {
        switch(choice) {
            case VELOCITY.CONTROL:
                return direction * _speed;
            case VELOCITY.SKII:
                return velocity + (direction * _speed);
            case VELOCITY.PINBALL:
                return (velocity + direction)  * _speed;
            case VELOCITY.SOAP:
                return (direction != Vector3.Zero ? velocity : Vector3.Zero) + (direction * _speed);
            case VELOCITY.RUBBER:
                return ((direction != Vector3.Zero ? velocity : Vector3.Zero) + direction)  * _speed;
            case VELOCITY.INFLUENCE:
                return (direction != Vector3.Zero ? velocity + (direction * _speed) : velocity - (Vector3.One * _speed));
            case VELOCITY.CANNON:
                return (direction != Vector3.Zero ? (velocity + direction) * _speed : velocity - (Vector3.One * (_maxSpeed / _speed)));
        }
        return Vector3.Zero;
    }
    public void rotate(float delta) {
        Vector3 origin = self.GlobalTransform.origin;
        Vector3 pathLook = new Vector3(pRotDir.x + origin.x,
        origin.y, pRotDir.z + origin.z);
        if(pRotDir.Length() > .1) {
            self.LookAt(pathLook, Vector3.Up);
        }
    }

}
public enum VELOCITY {
    CONTROL, INFLUENCE, CANNON, PINBALL, SKII, RUBBER, SOAP
}

//Old

// if(self.sm.enforceUpdate) return;
// if(inputDir != Vector3.Zero ) {//velocity is true
//     self.events[MECHEVENT.INPUTDIR].validate(self);
// } else if(velocity < new Vector3(1,0,1) && velocity > new Vector3(-1,0,-1)) {//velocity is false
//     self.events[MECHEVENT.NODIR].validate(self);
// }
//else if(!areaFloor) { // && airTime > 0.02) {//Helps with preventing state switching from air to idle/walk when applying no gravity to standing player.
// else if(!self.IsOnFloor()) { //If not in air or transitioning to air. Adjust gravity as needed :/
//                 gravity = -.1f;
//             }
    // public void jumpInput(float delta) {
    //     //jumpBool = true;
    //     //else jumpBool = false;
    //     //if(Input.IsActionJustReleased(pad("jump"))) jumpBool = false;
    //     //if(jumpBool) jumpSum = Mathf.Clamp(jumpSum + (jumpImpulse * delta), -gravity, 50f);

    //     //if(jumpBool) {
    //     //    //events["Jump"].validate(this);
    //     //} else {
    //     //    //events[NoJump].validate(this);
    //     //}
    // }
//Attempt 2 for air update
        // if(!self.IsOnFloor()) { //May work with a raycast in the future but difference is said to be marginal.
        //     airTime = Mathf.Clamp(airTime + delta, 0, 10);
        //     if((self.sm.currentState.name != STATE.JUMP && airTime > .016666) 
        //         || (self.sm.currentState.name == STATE.JUMP && yVelocity <= 0)) {
        //         self.events[MECHEVENT.AIR].validate(self);
        //     }
        // } else if(self.IsOnFloor()) { //Using areaFloor to get cool effects but may be better if they def collide KinematicBody.IsOnFloor() to even/ground character heights?
        //     airTime = 0;
        //     if(self.sm.currentState.name == STATE.JUMP || self.sm.currentState.name == STATE.AIR) { //Definitely works best with KinematicBody.IsOnFloor()
        //         self.events[MECHEVENT.LANDED].validate(self);
        //     }
        // }
//Attempted 1 for air update
//  public void airUpdate(float delta) {
//         Godot.Collections.Array overlap = self.floor.GetOverlappingBodies();
//         bool onFloor = false;
//         for (int i = 0; i < overlap.Count; i++) {
//             if(overlap[i] != self){
//                 onFloor = true;
//                 break;
//             }
//         }
//         if(!onFloor) { //May work with a raycast in the future.
//             airTime = Mathf.Clamp(airTime + delta, 0, 10);
//             if(airTime > 0.016666) self.events[MECHEVENT.AIR].validate(self);
//         } else airTime = 0;
//     }
// public void updateVel() { //Only place velocity should be updated from since it's called from physics process when final calculations are set.
//     velocity = new Vector3(pMoveDir.x, yVelocity, pMoveDir.z); //I need to debug what's happening here better. inc w/ phase.
//     //if(player == -1) GD.Print(velocity);
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