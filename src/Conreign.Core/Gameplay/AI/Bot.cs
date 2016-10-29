using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Conreign.Core.Contracts.Client;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Gameplay.AI.Behaviours;
using Conreign.Core.Gameplay.AI.Events;

namespace Conreign.Core.Gameplay.AI
{
    public class Bot : IDisposable
    {
        private readonly BotContext _context;
        private ActionBlock<IClientEvent> _processor;
        private readonly IDisposable _subscription;
        private bool _isDisposed;
        private readonly DispatcherBehaviour _dispatcher;

        public Bot(string readableId, IClientConnection connection, params IBotBehaviour[] behaviours)
        {
            if (string.IsNullOrEmpty(readableId))
            {
                throw new ArgumentException("Readable id cannot be null or empty.", nameof(readableId));
            }
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            _processor = new ActionBlock<IClientEvent>(Process);
            _context = new BotContext(readableId, connection, _processor.Complete, Handle);
            _subscription = connection.Events.Subscribe(Handle);
            _dispatcher = new DispatcherBehaviour(behaviours);
        }

        public Task Completion => _processor.Completion;

        public void Start()
        {
            EnsureIsNotDisposed();
            if (_processor.Completion.IsCompleted)
            {
                _processor = new ActionBlock<IClientEvent>(Process);
            }
            Handle(new BotStarted());
        }

        public void Stop()
        {
            EnsureIsNotDisposed();
            Handle(new BotStopped());
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }
            if (!disposing)
            {
                return;
            }
            _subscription?.Dispose();
            _processor?.Complete();
            _processor = null;
            _isDisposed = true;
        }

        private Task Process(IClientEvent @event)
        {
            return _dispatcher.Handle(new BotNotification<IClientEvent>(@event, _context));
        }

        private void Handle(IClientEvent @event)
        {
            if (@event == null)
            {
                return;
            }
            _processor.Post(@event);
        }

        private void EnsureIsNotDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException($"Bot[${_context.Connection.Id}]");
            }
        }
    }
}
