using UnityEngine;
using Zenject;

namespace SaveData
{
    public class SaveDataInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Debug.Log("SaveData Install");
            Container.Bind<ISaveLoadService>().To<SaveLoadService>().AsSingle().NonLazy();
        }
    }
}