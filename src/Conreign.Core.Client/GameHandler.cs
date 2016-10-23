using System;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using AutoMapper;
using Conreign.Core.Client.Handlers;
using Conreign.Core.Client.Handlers.Decorators;
using Conreign.Core.Client.Messages;
using Conreign.Core.Contracts.Communication;
using MediatR;
using SimpleInjector;
using SimpleInjector.Extensions.ExecutionContextScoping;

namespace Conreign.Core.Client
{
    public class GameHandler
    {
        private static readonly Type HandlerType = typeof(IAsyncRequestHandler<,>);
        private const string HandlerContextKey = "ConreignClientContext";
        private readonly IGameConnection _connection;
        private readonly Container _container;
        private readonly Mediator _mediator;
        private readonly ActionBlock<WorkItem> _processor;

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
            _container.Register(() => (IHandlerContext) CallContext.GetData(HandlerContextKey), Lifestyle.Scoped);
            _container.RegisterSingleton(mapperConfiguration.CreateMapper());
            _container.Register(HandlerType, new [] {typeof(LoginHandler).Assembly}, Lifestyle.Scoped);
            _container.RegisterDecorator(HandlerType, typeof(AuthenticationDecorator<,>), Lifestyle.Scoped);
            _mediator = new Mediator(type => _container.GetInstance(type), type => _container.GetAllInstances(type));
            _processor = new ActionBlock<WorkItem>(Process);
        }

        public IObservable<IClientEvent> Events => _connection.Events;

        public async Task<T> Handle<T>(IAsyncRequest<T> command, Metadata metadata)
        {
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

        private async Task Process(WorkItem workItem)
        {
            var traceId = Guid.NewGuid().ToString();
            var context = new HandlerContext(_connection, workItem.Metadata, traceId);
            CallContext.SetData(HandlerContextKey, context);
            using (_container.BeginExecutionContextScope())
            {
                try
                {
                    var result = await _mediator.SendAsync(workItem.Request);
                    workItem.CompletionSource.SetResult(result);
                }
                catch (Exception ex)
                {
                    workItem.CompletionSource.SetException(ex);
                }
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
