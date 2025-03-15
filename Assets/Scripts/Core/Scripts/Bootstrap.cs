using UnityEngine;
using Zenject;

namespace Core
{
    public class Bootstrap : MonoInstaller
    {
        public override void InstallBindings()
        {
            Debug.Log("Bootstrap инсталлер");
            Container.Bind<IInputService>().To<NewInputService>().AsSingle().NonLazy();
            Container.Bind<IGridService>().To<GridService>().AsSingle().NonLazy();
            Container.Bind<ISaveLoadService>().To<FilePersistenceService>().AsSingle().NonLazy();
        }
    }
}