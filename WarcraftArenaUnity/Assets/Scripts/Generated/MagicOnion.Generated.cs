#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 219
#pragma warning disable 168

namespace MagicOnion
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::MagicOnion;
    using global::MagicOnion.Client;

    public static partial class MagicOnionInitializer
    {
        static bool isRegistered = false;

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Register()
        {
            if(isRegistered) return;
            isRegistered = true;

            MagicOnionClientRegistry<ElleRealTimeStd.Shared.Test.Interfaces.Service.IMyFirstService>.Register((x, y, z) => new ElleRealTimeStd.Shared.Test.Interfaces.Service.IMyFirstServiceClient(x, y, z));

            StreamingHubClientRegistry<ElleRealTimeStd.Shared.Test.Interfaces.StreamingHub.IGamingHub, ElleRealTimeStd.Shared.Test.Interfaces.StreamingHub.IGamingHubReceiver>.Register((a, _, b, c, d, e) => new ElleRealTimeStd.Shared.Test.Interfaces.StreamingHub.IGamingHubClient(a, b, c, d, e));
        }
    }
}

#pragma warning restore 168
#pragma warning restore 219
#pragma warning restore 414
#pragma warning restore 612
#pragma warning restore 618
#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 219
#pragma warning disable 168

namespace MagicOnion.Resolvers
{
    using System;
    using MessagePack;

    public class MagicOnionResolver : global::MessagePack.IFormatterResolver
    {
        public static readonly global::MessagePack.IFormatterResolver Instance = new MagicOnionResolver();

        MagicOnionResolver()
        {

        }

        public global::MessagePack.Formatters.IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly global::MessagePack.Formatters.IMessagePackFormatter<T> formatter;

            static FormatterCache()
            {
                var f = MagicOnionResolverGetFormatterHelper.GetFormatter(typeof(T));
                if (f != null)
                {
                    formatter = (global::MessagePack.Formatters.IMessagePackFormatter<T>)f;
                }
            }
        }
    }

    internal static class MagicOnionResolverGetFormatterHelper
    {
        static readonly global::System.Collections.Generic.Dictionary<Type, int> lookup;

        static MagicOnionResolverGetFormatterHelper()
        {
            lookup = new global::System.Collections.Generic.Dictionary<Type, int>(4)
            {
                {typeof(global::ElleRealTimeStd.Shared.Test.Entities.StreamingHub.Player.Player[]), 0 },
                {typeof(global::MagicOnion.DynamicArgumentTuple<global::UnityEngine.Vector3, global::UnityEngine.Quaternion>), 1 },
                {typeof(global::MagicOnion.DynamicArgumentTuple<int, int>), 2 },
                {typeof(global::MagicOnion.DynamicArgumentTuple<string, string, global::UnityEngine.Vector3, global::UnityEngine.Quaternion>), 3 },
            };
        }

        internal static object GetFormatter(Type t)
        {
            int key;
            if (!lookup.TryGetValue(t, out key))
            {
                return null;
            }

            switch (key)
            {
                case 0: return new global::MessagePack.Formatters.ArrayFormatter<global::ElleRealTimeStd.Shared.Test.Entities.StreamingHub.Player.Player>();
                case 1: return new global::MagicOnion.DynamicArgumentTupleFormatter<global::UnityEngine.Vector3, global::UnityEngine.Quaternion>(default(global::UnityEngine.Vector3), default(global::UnityEngine.Quaternion));
                case 2: return new global::MagicOnion.DynamicArgumentTupleFormatter<int, int>(default(int), default(int));
                case 3: return new global::MagicOnion.DynamicArgumentTupleFormatter<string, string, global::UnityEngine.Vector3, global::UnityEngine.Quaternion>(default(string), default(string), default(global::UnityEngine.Vector3), default(global::UnityEngine.Quaternion));
                default: return null;
            }
        }
    }
}

#pragma warning restore 168
#pragma warning restore 219
#pragma warning restore 414
#pragma warning restore 612
#pragma warning restore 618
#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 219
#pragma warning disable 168

namespace ElleRealTimeStd.Shared.Test.Interfaces.Service {
    using System;
	using MagicOnion;
    using MagicOnion.Client;
    using Grpc.Core;
    using MessagePack;

    public class IMyFirstServiceClient : MagicOnionClientBase<global::ElleRealTimeStd.Shared.Test.Interfaces.Service.IMyFirstService>, global::ElleRealTimeStd.Shared.Test.Interfaces.Service.IMyFirstService
    {
        static readonly Method<byte[], byte[]> SumAsyncMethod;
        static readonly Func<RequestContext, ResponseContext> SumAsyncDelegate;

        static IMyFirstServiceClient()
        {
            SumAsyncMethod = new Method<byte[], byte[]>(MethodType.Unary, "IMyFirstService", "SumAsync", MagicOnionMarshallers.ThroughMarshaller, MagicOnionMarshallers.ThroughMarshaller);
            SumAsyncDelegate = _SumAsync;
        }

        IMyFirstServiceClient()
        {
        }

        public IMyFirstServiceClient(CallInvoker callInvoker, IFormatterResolver resolver, IClientFilter[] filters)
            : base(callInvoker, resolver, filters)
        {
        }

        protected override MagicOnionClientBase<IMyFirstService> Clone()
        {
            var clone = new IMyFirstServiceClient();
            clone.host = this.host;
            clone.option = this.option;
            clone.callInvoker = this.callInvoker;
            clone.resolver = this.resolver;
            clone.filters = filters;
            return clone;
        }

        public new IMyFirstService WithHeaders(Metadata headers)
        {
            return base.WithHeaders(headers);
        }

        public new IMyFirstService WithCancellationToken(System.Threading.CancellationToken cancellationToken)
        {
            return base.WithCancellationToken(cancellationToken);
        }

        public new IMyFirstService WithDeadline(System.DateTime deadline)
        {
            return base.WithDeadline(deadline);
        }

        public new IMyFirstService WithHost(string host)
        {
            return base.WithHost(host);
        }

        public new IMyFirstService WithOptions(CallOptions option)
        {
            return base.WithOptions(option);
        }
   
        static ResponseContext _SumAsync(RequestContext __context)
        {
            return CreateResponseContext<DynamicArgumentTuple<int, int>, int>(__context, SumAsyncMethod);
        }

        public global::MagicOnion.UnaryResult<int> SumAsync(int x, int y)
        {
            return InvokeAsync<DynamicArgumentTuple<int, int>, int>("IMyFirstService/SumAsync", new DynamicArgumentTuple<int, int>(x, y), SumAsyncDelegate);
        }
    }
}

#pragma warning restore 168
#pragma warning restore 219
#pragma warning restore 414
#pragma warning restore 612
#pragma warning restore 618
#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 219
#pragma warning disable 168

namespace ElleRealTimeStd.Shared.Test.Interfaces.StreamingHub {
    using Grpc.Core;
    using Grpc.Core.Logging;
    using MagicOnion;
    using MagicOnion.Client;
    using MessagePack;
    using System;
    using System.Threading.Tasks;

    public class IGamingHubClient : StreamingHubClientBase<global::ElleRealTimeStd.Shared.Test.Interfaces.StreamingHub.IGamingHub, global::ElleRealTimeStd.Shared.Test.Interfaces.StreamingHub.IGamingHubReceiver>, global::ElleRealTimeStd.Shared.Test.Interfaces.StreamingHub.IGamingHub
    {
        static readonly Method<byte[], byte[]> method = new Method<byte[], byte[]>(MethodType.DuplexStreaming, "IGamingHub", "Connect", MagicOnionMarshallers.ThroughMarshaller, MagicOnionMarshallers.ThroughMarshaller);

        protected override Method<byte[], byte[]> DuplexStreamingAsyncMethod { get { return method; } }

        readonly global::ElleRealTimeStd.Shared.Test.Interfaces.StreamingHub.IGamingHub __fireAndForgetClient;

        public IGamingHubClient(CallInvoker callInvoker, string host, CallOptions option, IFormatterResolver resolver, ILogger logger)
            : base(callInvoker, host, option, resolver, logger)
        {
            this.__fireAndForgetClient = new FireAndForgetClient(this);
        }
        
        public global::ElleRealTimeStd.Shared.Test.Interfaces.StreamingHub.IGamingHub FireAndForget()
        {
            return __fireAndForgetClient;
        }

        protected override void OnBroadcastEvent(int methodId, ArraySegment<byte> data)
        {
            switch (methodId)
            {
                case -1297457280: // OnJoin
                {
                    var result = LZ4MessagePackSerializer.Deserialize<global::ElleRealTimeStd.Shared.Test.Entities.StreamingHub.Player.Player>(data, resolver);
                    receiver.OnJoin(result); break;
                }
                case 532410095: // OnLeave
                {
                    var result = LZ4MessagePackSerializer.Deserialize<global::ElleRealTimeStd.Shared.Test.Entities.StreamingHub.Player.Player>(data, resolver);
                    receiver.OnLeave(result); break;
                }
                case 1429874301: // OnMove
                {
                    var result = LZ4MessagePackSerializer.Deserialize<global::ElleRealTimeStd.Shared.Test.Entities.StreamingHub.Player.Player>(data, resolver);
                    receiver.OnMove(result); break;
                }
                default:
                    break;
            }
        }

        protected override void OnResponseEvent(int methodId, object taskCompletionSource, ArraySegment<byte> data)
        {
            switch (methodId)
            {
                case -733403293: // JoinAsync
                {
                    var result = LZ4MessagePackSerializer.Deserialize<global::ElleRealTimeStd.Shared.Test.Entities.StreamingHub.Player.Player[]>(data, resolver);
                    ((TaskCompletionSource<global::ElleRealTimeStd.Shared.Test.Entities.StreamingHub.Player.Player[]>)taskCompletionSource).TrySetResult(result);
                    break;
                }
                case 1368362116: // LeaveAsync
                {
                    var result = LZ4MessagePackSerializer.Deserialize<Nil>(data, resolver);
                    ((TaskCompletionSource<Nil>)taskCompletionSource).TrySetResult(result);
                    break;
                }
                case -99261176: // MoveAsync
                {
                    var result = LZ4MessagePackSerializer.Deserialize<Nil>(data, resolver);
                    ((TaskCompletionSource<Nil>)taskCompletionSource).TrySetResult(result);
                    break;
                }
                default:
                    break;
            }
        }
   
        public global::System.Threading.Tasks.Task<global::ElleRealTimeStd.Shared.Test.Entities.StreamingHub.Player.Player[]> JoinAsync(string roomName, string userName, global::UnityEngine.Vector3 position, global::UnityEngine.Quaternion rotation)
        {
            return WriteMessageWithResponseAsync<DynamicArgumentTuple<string, string, global::UnityEngine.Vector3, global::UnityEngine.Quaternion>, global::ElleRealTimeStd.Shared.Test.Entities.StreamingHub.Player.Player[]> (-733403293, new DynamicArgumentTuple<string, string, global::UnityEngine.Vector3, global::UnityEngine.Quaternion>(roomName, userName, position, rotation));
        }

        public global::System.Threading.Tasks.Task LeaveAsync()
        {
            return WriteMessageWithResponseAsync<Nil, Nil>(1368362116, Nil.Default);
        }

        public global::System.Threading.Tasks.Task MoveAsync(global::UnityEngine.Vector3 position, global::UnityEngine.Quaternion rotation)
        {
            return WriteMessageWithResponseAsync<DynamicArgumentTuple<global::UnityEngine.Vector3, global::UnityEngine.Quaternion>, Nil>(-99261176, new DynamicArgumentTuple<global::UnityEngine.Vector3, global::UnityEngine.Quaternion>(position, rotation));
        }


        class FireAndForgetClient : global::ElleRealTimeStd.Shared.Test.Interfaces.StreamingHub.IGamingHub
        {
            readonly IGamingHubClient __parent;

            public FireAndForgetClient(IGamingHubClient parentClient)
            {
                this.__parent = parentClient;
            }

            public global::ElleRealTimeStd.Shared.Test.Interfaces.StreamingHub.IGamingHub FireAndForget()
            {
                throw new NotSupportedException();
            }

            public Task DisposeAsync()
            {
                throw new NotSupportedException();
            }

            public Task WaitForDisconnect()
            {
                throw new NotSupportedException();
            }

            public global::System.Threading.Tasks.Task<global::ElleRealTimeStd.Shared.Test.Entities.StreamingHub.Player.Player[]> JoinAsync(string roomName, string userName, global::UnityEngine.Vector3 position, global::UnityEngine.Quaternion rotation)
            {
                return __parent.WriteMessageAsyncFireAndForget<DynamicArgumentTuple<string, string, global::UnityEngine.Vector3, global::UnityEngine.Quaternion>, global::ElleRealTimeStd.Shared.Test.Entities.StreamingHub.Player.Player[]> (-733403293, new DynamicArgumentTuple<string, string, global::UnityEngine.Vector3, global::UnityEngine.Quaternion>(roomName, userName, position, rotation));
            }

            public global::System.Threading.Tasks.Task LeaveAsync()
            {
                return __parent.WriteMessageAsync<Nil>(1368362116, Nil.Default);
            }

            public global::System.Threading.Tasks.Task MoveAsync(global::UnityEngine.Vector3 position, global::UnityEngine.Quaternion rotation)
            {
                return __parent.WriteMessageAsync<DynamicArgumentTuple<global::UnityEngine.Vector3, global::UnityEngine.Quaternion>>(-99261176, new DynamicArgumentTuple<global::UnityEngine.Vector3, global::UnityEngine.Quaternion>(position, rotation));
            }

        }
    }
}

#pragma warning restore 168
#pragma warning restore 219
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612
