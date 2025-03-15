using UnityEngine;
using Zenject;

namespace Grid
{
    public class GridInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Debug.Log("Grid Install");
            Container.Bind<IGridService>().To<GridService>().AsSingle().NonLazy();
        }
    }
}