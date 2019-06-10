using System;

namespace Core
{
    [Flags]
    public enum EnityTypeMask
    {
        Entity = 1 << EntityType.Entity,
        Item = 1 << EntityType.Item,
        Container = 1 << EntityType.Container,
        Unit = 1 << EntityType.Unit,
        Player = 1 << EntityType.Player,
        GameEntity = 1 << EntityType.GameEntity,
        DynamicEntity = 1 << EntityType.DynamicEntity,
        AreaTrigger = 1 << EntityType.AreaTrigger,
        SceneObject = 1 << EntityType.SceneObject,
    }
}
