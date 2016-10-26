using System;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using AutoMapper;
using Conreign.Core.Client.Handlers;
using Conreign.Core.Client.Handlers.Decorators;
using Conreign.Core.Contracts.Client;
using Conreign.Core.Contracts.Communication;
using MediatR;
using SimpleInjector;
using SimpleInjector.Extensions.ExecutionContextScoping;

namespace Conreign.Core.Client
{
    public class GameHandler : IGameHandler
    {
        private static readonly Type HandlerInterfaceType = typeof(IAsyncRequestHandler<,>);
        private const string HandlerContextKey = "ConreignClientContext";
        private readonly IGameConnection _connection;
        private readonly Container _container;
        private readonly ActionBlock<WorkItem> _processor;
        private bool _isDisposed;

        public GameHandler(IGameConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            _connection = connection;
            _container = new Container();
            var mapperConfiguration = new MapperConfiguration(x => x.AddProfile<MappingProfile>());
            _container.Options.DefaultScopedLifestyle = new ExecutionContextScopeLifestyle();
            _container.RegisterSingleton<IMediator, Mediator>();
            _container.RegisterSingleton(new SingleInstanceFactory(_container.GetInstance));
            _container.RegisterSingleton(new MultiInstanceFactory(_container.GetAllInstances));
            _container.Register(() => (IHandlerContext) CallContext.GetData(HandlerContextKey), Lifestyle.Scoped);
            _container.RegisterSingleton(mapperConfiguration.CreateMapper());
            _container.Register(HandlerInterfaceType, new []{ typeof(LoginHandler).Assembly }, Lifestyle.Scoped);
            _container.RegisterDecorator(HandlerInterfaceType, typeof(AuthenticationDecorator<,>), Lifestyle.Scoped);
            _processor = new ActionBlock<WorkItem>(Process);
        }

        public IObservable<IClientEvent> Events => _connection.Events;

        public async Task<T> Handle<T>(IAsyncRequest<T> command, Metadata metadata)
        {
            EnsureIsNotDisposed();
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            if (metadata == null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }
            var source = new TaskCompletionSource<object>();
            var workItem = new WorkItem(source, command, metadata);
            _processor.Post(workItem);
            var result = await source.Task;
            return (T) result;
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            _container.Dispose();
            _connection.Dispose();
            _isDisposed = true;
        }

        private void EnsureIsNotDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("GameHandler");
            }
        }

        private async Task Process(WorkItem workItem)
        {
            var traceId = Guid.NewGuid().ToString();
            var context = new HandlerContext(_connection, workItem.Metadata, traceId);
            CallContext.SetData(HandlerContextKey, context);
            try
            {
                _container.Verify();
                using (var scope = _container.BeginExecutionContextScope())
                {
                    var mediator = scope.GetInstance<IMediator>();
                    var result = await mediator.SendAsync(workItem.Request);
                    workItem.CompletionSource.SetResult(result);
                }
            }
            catch (Exception ex)
            {
                workItem.CompletionSource.SetException(ex);
            }
        }

        private class WorkItem
        {
            public WorkItem(TaskCompletionSource<object> completionSource, dynamic request, Metadata metadata)
            {
                CompletionSource = completionSource;
                Request = request;
                Metadata = metadata;
            }

            public TaskCompletionSource<object> CompletionSource { get; }
            public dynamic Request { get; }
            public Metadata Metadata { get; }
        }
    }
}
