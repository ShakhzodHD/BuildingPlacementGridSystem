using Grid;
using Buildings;
using SaveData;
using UnityEngine;
using Zenject;

namespace Core
{
    public class Bootstrap : MonoInstaller
    {
        [SerializeField] private BuildingManager buildingManagerPrefab;
        [SerializeField] private UIManager uiManagerPrefab;
        public override void InstallBindings()
        {
            Container.Bind<IInputService>().To<NewInputService>().AsSingle();
            Container.Bind<IGridService>().To<GridService>().AsSingle();
            Container.Bind<ISaveLoadService>().To<SaveLoadService>().AsSingle();
            Container.Bind<BuildingManager>()
                .FromComponentInNewPrefab(buildingManagerPrefab)
                .AsSingle()
                .NonLazy();

            Container.Bind<UIManager>()
                .FromComponentInNewPrefab(uiManagerPrefab)
                .AsSingle()
                .NonLazy();
        }
    }
}