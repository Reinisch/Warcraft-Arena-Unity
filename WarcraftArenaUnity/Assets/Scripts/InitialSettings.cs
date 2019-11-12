using System.Collections;
using System.Collections.Generic;
using MessagePack.Resolvers;
using MessagePack.Unity;
using UnityEngine;

namespace Assets.Scripts
{
    class InitialSettings
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void RegisterResolvers()
        {
            CompositeResolver.RegisterAndSetAsDefault
            (
                MessagePack.Resolvers.GeneratedResolver.Instance,

                MagicOnion.Resolvers.MagicOnionResolver.Instance,
                BuiltinResolver.Instance,
                PrimitiveObjectResolver.Instance,
                UnityResolver.Instance
            );
        }
    }
}

