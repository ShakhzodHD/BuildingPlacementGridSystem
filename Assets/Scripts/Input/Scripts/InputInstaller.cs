using UnityEngine;
using Zenject;

namespace Input
{
    public class InputInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Debug.Log("Input Install");
            Container.Bind<IInputService>().To<NewInputService>().AsSingle().NonLazy();
        }
    }
}