using Godot;

public class Movement {
    private Spirit self;
    public float gravity {set; get;} = -9.8f/3;
    public float jumpImpulse {set; get;} = 50;
    public float yVelocity {set; get;} = 0;
    public bool moveBool {set; get;} = false;
    public bool jumpBool {set; get;} = false;
    public bool stanceBool {set; get;} = false;
    public Vector3 velocity {set; get;} = new Vector3();
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
    public void crouchInput() {
        if (Input.IsActionJustPressed(self.pad("crouch"))) {
            self.events[MECHEVENT.CROUCHPRESS].validate(self);       
        }
    }
    public void dashInput() {
        if (Input.IsActionJustPressed(self.pad("dash"))) {
            self.events[MECHEVENT.DASHPRESS].validate(self);
        }
    }
    public void stanceInput() {
        if (Input.IsActionJustPressed(self.pad("stance"))) {
            self.events[MECHEVENT.STANCEPRESS].validate(self);       
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

        moveBool = true;
        if(self.sm.enforceUpdate) return;
        if(inputDir != Vector3.Zero ) {//velocity is true
            self.events[MECHEVENT.INPUTDIR].validate(self);
        } else if(velocity < new Vector3(1,0,1) && velocity > new Vector3(-1,0,-1)) {//velocity is false
            self.events[MECHEVENT.NODIR].validate(self);
        }
    }
    public void jumpInput(float delta) {
        if(self.sm.enforceUpdate) return;
        if(Input.IsActionJustPressed(self.pad("jump")) && (coyote())) {
            self.events[MECHEVENT.JUMPPRESS].validate(self);
        }
        
        //jumpBool = true;
        //else jumpBool = false;
        //if(Input.IsActionJustReleased(pad("jump"))) jumpBool = false;
        //if(jumpBool) jumpSum = Mathf.Clamp(jumpSum + (jumpImpulse * delta), -gravity, 50f);

        //if(jumpBool) {
        //    //events["Jump"].validate(this);
        //} else {
        //    //events[NoJump].validate(this);
        //}
    }
    public void calcMove(Vector3 direction, Vector3 rotate, float speed, float max, int gravMode = 0) {
        pRotDir = rotate;
        pMoveDir = chooseVel(direction, speed, max, VELOCITY.CONTROL);
        if(pMoveDir.Length() > max){
            pMoveDir = pMoveDir.Normalized() * max;
        }
        if(gravMode == 1) yVelocity = gravity + velocity.y;
        else if(gravMode == 0) yVelocity = 0;
        //newGravity = self.stats.phasePoints >= self.phaser.phasable ? 0 : gravity;
    }
    public void airUpdate(float delta) {
        Godot.Collections.Array overlap = self.floor.GetOverlappingBodies();
        bool onFloor = false;
        for (int i = 0; i < overlap.Count; i++) {
            if(overlap[i] != self){
                onFloor = true;
                break;
            }
        }
        if(!onFloor) { //May work with a raycast in the future.
            airTime = Mathf.Clamp(airTime + delta, 0, 10);
            if(airTime > 0.016666) self.events[MECHEVENT.AIR].validate(self);
        } else airTime = 0;
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