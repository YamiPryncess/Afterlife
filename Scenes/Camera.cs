using Godot;
using System;

public class Camera : Godot.Camera {
    private float distance = 6;
    private float height = 6;
    private float curRotation = 1.8f;
    private float verticalOrbit = Mathf.Deg2Rad(45);
    Vector3 offset;
    private Vector3 targetPos;
    private Meeting meeting;
    private float targetDist; //Largest distance of multiple players

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        meeting = GetParent<Meeting>();
    }
    public override void _PhysicsProcess(float delta) {
        if (Input.IsActionPressed("camera_up")) verticalOrbit -= 1f * delta;
        if (Input.IsActionPressed("camera_down")) verticalOrbit += 1f * delta;
        if (Input.IsActionPressed("camera_left")) curRotation -= 1f * delta;
        if (Input.IsActionPressed("camera_right")) curRotation += 1f * delta;
        if(verticalOrbit > Mathf.Deg2Rad(80f)) verticalOrbit = Mathf.Deg2Rad(80f);
        if(verticalOrbit < Mathf.Deg2Rad(10f)) verticalOrbit = Mathf.Deg2Rad(10f);
        
        targetPos = meeting.getTarget();
        targetDist = meeting.getDistance();
        //GD.Print(targetDist);
        offset = new Vector3(0, 0, -1).Rotated(Vector3.Right, verticalOrbit)
                                * (targetDist > 8 ? targetDist * (Mathf.Rad2Deg(verticalOrbit) > 60 
                                            ? verticalOrbit : Mathf.Deg2Rad(60)) : 8);
        Vector3 cameraPos = targetPos + offset.Rotated(Vector3.Up, curRotation);
        
        LookAtFromPosition(cameraPos, targetPos, Vector3.Up);
    }
    public void setTargetPos(Vector3 _targetPos) {
        targetPos = _targetPos;
    }
}
