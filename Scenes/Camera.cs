using Godot;
using System;

public class Camera : Godot.Camera {
    public float distance {set; get;} = 6;
    public float height {set; get;} = 6;
    public float curRotation {set; get;} = 1.8f;
    public float verticalOrbit {set; get;} = Mathf.Deg2Rad(45);
    public Vector3 offset {set; get;}
    public Vector3 targetPos {set; get;}
    public Meeting meeting {set; get;}
    public float slope {set; get;} //Largest distance of multiple players

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        meeting = GetParent<Meeting>();
    }
    public override void _Process(float delta) {
        if (Input.IsActionPressed("camera_up")) verticalOrbit -= 1f * delta;
        if (Input.IsActionPressed("camera_down")) verticalOrbit += 1f * delta;
        if (Input.IsActionPressed("camera_left")) curRotation -= 1f * delta;
        if (Input.IsActionPressed("camera_right")) curRotation += 1f * delta;

        if(verticalOrbit > Mathf.Deg2Rad(80f)) verticalOrbit = Mathf.Deg2Rad(80f);
        if(verticalOrbit < Mathf.Deg2Rad(10f)) verticalOrbit = Mathf.Deg2Rad(10f);
        float vertOrbDeg = Mathf.Rad2Deg(verticalOrbit);
        float minOffset = 8;
        float tooCloseAngle = 60;
        float pushCamera = Mathf.Deg2Rad(tooCloseAngle); //Forgot what this is

        targetPos = meeting.target;
        slope = meeting.largestDist;//largest distance between 2 players
        //GD.Print(targetDist);
        offset = new Vector3(0, 0, -1).Rotated(Vector3.Right, verticalOrbit) * 
                                                (slope > minOffset ?
                                                slope * (vertOrbDeg > tooCloseAngle ? 
                                                verticalOrbit : pushCamera) 
                                                : minOffset);
        Vector3 cameraPos = targetPos + offset.Rotated(Vector3.Up, curRotation);
        
        LookAtFromPosition(cameraPos, targetPos, Vector3.Up);
    }
    public void setTargetPos(Vector3 _targetPos) {
        targetPos = _targetPos;
    }
}
