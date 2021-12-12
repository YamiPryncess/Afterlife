using Godot;

public class Movement {
    private Spirit self;
    public float gravity {set; get;} = -9.8f/3;
    public float speed {set; get;} = 12;
    public float jumpImpulse {set; get;} = 50;
    public float maxSpeed {set; get;} = 16;
    public bool moveBool {set; get;} = false;
    public bool jumpBool {set; get;} = false;
    public Vector3 velocity {set; get;} = new Vector3();
    public Vector3 inputDir {set; get;} = new Vector3();
    //public float jumpSum {set; get;}
    public Vector3 rotateDir {set; get;} = new Vector3();
    //public bool autoInput {set; get;} = false;

    public Movement (Spirit spirit) {
        self = spirit;
    }

    public void updateDirection(Vector3 vector) {
        inputDir = vector;
        rotateDir = vector;
    }
    public void moveInput() {
        inputDir = new Vector3();
        Basis camBasis = self.camera.GlobalTransform.basis;
        Vector3 fixedBasisX = new Vector3(camBasis.x.x, 0, camBasis.x.z).Normalized();
        Vector3 fixedBasisZ = new Vector3(camBasis.z.x, 0, camBasis.z.z).Normalized(); 
        //bool inputPressed = false;
        if (Input.IsActionPressed(self.pad("move_forward"))) { //inputPressed = true;
            inputDir -= fixedBasisZ * Input.GetActionStrength(self.pad("move_forward"));}
        if (Input.IsActionPressed(self.pad("move_backward"))) { //inputPressed = true;
            inputDir += fixedBasisZ * Input.GetActionStrength(self.pad("move_backward"));}
        if (Input.IsActionPressed(self.pad("move_left"))) { //inputPressed = true;
            inputDir -= fixedBasisX * Input.GetActionStrength(self.pad("move_left"));}
        if (Input.IsActionPressed(self.pad("move_right"))) { //inputPressed = true;
            inputDir += fixedBasisX * Input.GetActionStrength(self.pad("move_right"));}
        
        //if(!inputPressed) inputDir = Vector3.Zero;
        if (inputDir.Length() > 1.0) {
            inputDir = inputDir.Normalized();
        }
        rotateDir = inputDir;

        moveBool = true;
        if(self.sm.enforceUpdate) return;
        if(inputDir != Vector3.Zero ) {//velocity is true
            self.events["InputDirection"].validate(self);
        } else {//velocity is false
            self.events["NoDirection"].validate(self);
        }
    }
    public void jumpInput(float delta) {
        if(self.sm.enforceUpdate) return;
        if(Input.IsActionJustPressed(self.pad("jump")) && self.IsOnFloor()) jumpBool = true;
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
        float newGravity = self.IsOnFloor() ? 0 : gravity; //I should do jump states for whether you're
        newGravity = self.stats.phasePoints >= self.phaser.phasable ? 0 : gravity; //Phased or not.
        velocity = new Vector3(speedDir.x, jumpBool ? jumpImpulse : 0 + //Why plus zero?
        velocity.y + newGravity, speedDir.z); //I need to debug what's happening here better. inc w/ phase.
        //if(player == -1) GD.Print(velocity);
    }
    public void rotate(float delta) {
        Vector3 origin = self.GlobalTransform.origin;
        Vector3 pathLook = new Vector3(rotateDir.x + origin.x,
        origin.y, rotateDir.z + origin.z);
        if(rotateDir.Length() > .1) {
            self.LookAt(pathLook, Vector3.Up);
        }
    }

}
public enum VELOCITY {
    CONTROL, ACCELERATE, DEACCELERATE, PINBALL, SKII, RUBBER, SOAP
}

//Old
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