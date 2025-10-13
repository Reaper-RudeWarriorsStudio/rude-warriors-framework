using UnityEngine;

namespace RudeWarriors.Framework.Core
{
    public interface IInputRouter
    {
        Vector2 GetMovement();
        bool GetJump();
    }

    public class InputRouter : MonoBehaviour, IInputRouter
    {
        public Vector2 GetMovement()
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");
            return new Vector2(x, y).normalized;
        }

        public bool GetJump() => Input.GetButtonDown("Jump");
    }
}
