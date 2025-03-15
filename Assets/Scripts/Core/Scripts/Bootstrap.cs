using UnityEngine;
using Zenject;

namespace Core
{
    public class Bootstrap : MonoInstaller
    {
        public override void InstallBindings()
        {
            Debug.Log("Bootstrap Install");
        }
    }
}