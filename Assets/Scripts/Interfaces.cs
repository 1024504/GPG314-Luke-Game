using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IControllable
{
    public void Move(Vector2 direction);
    public void Aim(Vector2 direction);
    public void Interact();
    public void Action1();
    public void Action2();
}

public interface IKickable
{
    public void GetKicked(float angle);
}
