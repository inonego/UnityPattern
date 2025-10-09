using System;

namespace inonego
{
    public interface ISpawnable
    {
        protected internal void OnSpawn();
        protected internal void OnDespawn();

        protected internal Action DespawnFromRegistry { get; set; }
    }
}