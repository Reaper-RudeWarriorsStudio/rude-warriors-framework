using UnityEngine;

namespace RudeWarriors.Framework.Core
{
    public interface IExampleSystem
    {
        void SayHello();
    }

    [AutoRegister]
    public class ExampleSystem : MonoBehaviour, IExampleSystem
    {
        public void SayHello()
        {
            Debug.Log("Hello from ExampleSystem!");
        }
    }
}
