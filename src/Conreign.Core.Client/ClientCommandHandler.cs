using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Conreign.Core.Contracts.Client;
using Conreign.Core.Contracts.Communication;
using MediatR;
using SimpleInjector;

namespace Conreign.Core.Client
{
    public class ClientCommandHandler : IClientCommandHandler
    {
        private readonly IClientConnection _connection;
        private readonly IMediator _mediator;
        private readonly ActionBlock<WorkItem> _processor;
        private bool _isDisposed;

        public ClientCommandHandler(IClientConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            var container = new Container();
            container.RegisterClientMediator();
            _connection = connection;
            _mediator = container.GetInstance<IMediator>();
            _processor = new ActionBlock<WorkItem>(Process);
        }

        public ClientCommandHandler(IClientConnection connection, IMediator mediator)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (mediator == null)
            {
                throw new ArgumentNullException(nameof(mediator));
            }
            _connection = connection;
            _mediator = mediator;
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
            var workItem = new WorkItem(source, command, typeof(T), metadata);
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
            _processor.Complete();
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
            try
            {
                var traceId = Guid.NewGuid().ToString();
                var context = new HandlerContext(_connection, workItem.Metadata, traceId);
                var envelopeType = typeof(CommandEnvelope<,>).MakeGenericType(workItem.Request.GetType(), workItem.ResponseType);
                var envelope = Activator.CreateInstance(envelopeType, workItem.Request, context);
                var result = await _mediator.SendAsync((dynamic)envelope);
                workItem.CompletionSource.SetResult(result);
            }
            catch (Exception ex)
            {
                workItem.CompletionSource.SetException(ex);
            }
        }

        private class WorkItem
        {
            public WorkItem(TaskCompletionSource<object> completionSource, object request, Type responseType, Metadata metadata)
            {
                CompletionSource = completionSource;
                Request = request;
                Metadata = metadata;
                ResponseType = responseType;
            }

            public TaskCompletionSource<object> CompletionSource { get; }
            public object Request { get; }
            public Type ResponseType { get; }
            public Metadata Metadata { get; }
        }
    }
}
