using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Conreign.Core.AI.Behaviours;
using Conreign.Core.AI.Events;
using Conreign.Core.Contracts.Client;
using Conreign.Core.Contracts.Communication;

namespace Conreign.Core.AI
{
    public class Bot : IDisposable
    {
        private readonly BotContext _context;
        private readonly DispatcherBehaviour _dispatcher;
        private readonly IDisposable _subscription;
        private Subject<IBotEvent> _subject;
        private bool _isDisposed;
        private ActionBlock<IClientEvent> _processor;

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
            _processor = CreateProcessor();
            _context = new BotContext(readableId, connection, Complete, Notify);
            _subscription = connection.Events.Subscribe(OnClientEvent, OnClientException, OnClientCompleted);
            var allBehaviours = behaviours.ToList();
            if (!behaviours.OfType<StopBehaviour>().Any())
            {
                allBehaviours.Add(new StopBehaviour());
            }
            _dispatcher = new DispatcherBehaviour(allBehaviours);
            _subject = new Subject<IBotEvent>();
            Events = _subject.AsObservable();
        }

        public string Id => _context.ReadableId;
        public Task Completion => _processor.Completion;
        public IClientConnection Connection => _context.Connection;
        public bool IsStarted { get; private set; }
        public bool IsStopped => !IsStarted;
        public IObservable<IBotEvent> Events { get; private set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Start()
        {
            EnsureIsNotDisposed();
            if (IsStarted)
            {
                return;
            }
            IsStarted = true;
            if (_processor.Completion.IsCompleted)
            {
                _processor = CreateProcessor();
                _subject.Dispose();
                _subject = new Subject<IBotEvent>();
                Events = _subject.AsObservable();
            }
            Notify(new BotStarted());
        }

        public void Stop()
        {
            EnsureIsNotDisposed();
            if (IsStopped)
            {
                return;
            }
            IsStarted = false;
            Notify(new BotStopped());
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

        private void OnClientEvent(IClientEvent @event)
        {
            Handle(@event);
        }

        private void OnClientException(Exception exception)
        {
            _subject.OnError(exception);
        }

        private void OnClientCompleted()
        {
            _subject.OnError(new InvalidOperationException("Client stream unexpectedly completed."));
            Complete();
        }

        private void Complete()
        {
            _processor.Complete();
            _subject.OnCompleted();
        }

        private void Notify(IBotEvent @event)
        {
            _subject.OnNext(@event);
            Handle(@event);
        }

        private void Handle(IClientEvent @event)
        {
            if (@event == null)
            {
                return;
            }
            _context.Logger.Debug("[{ReadableId}-{UserId}] Processor queue length is {QueueLength}.", 
                _context.ReadableId, 
                _context.UserId, 
                _processor.InputCount);
            var posted = _processor.Post(@event);
            if (!posted)
            {
                _context.Logger.Warning("[{ReadableId}-{UserId}] Failed to handle event of type {EventType}.",
                    _context.ReadableId, 
                    _context.UserId, 
                    @event.GetType().Name);
            }
        }

        private void EnsureIsNotDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException($"Bot[${_context.Connection.Id}]");
            }
        }

        private ActionBlock<IClientEvent> CreateProcessor()
        {
            var options = new ExecutionDataflowBlockOptions
            {
                BoundedCapacity = 100
            };
            return new ActionBlock<IClientEvent>(Process, options);
        }

        private async Task Process(IClientEvent @event)
        {
            try
            {
                await _dispatcher.Handle(new BotNotification<IClientEvent>(@event, _context));
            }
            catch (Exception ex)
            {
                _context.Logger.Error(ex, 
                    "Failed to handle [{ReadableId}-{UserId}] event with {Message}.", 
                    _context.ReadableId, 
                    _context.UserId, 
                    ex.Message);
                //TODO: not an error, just a problem :)
                //_subject.OnError(ex);
            }
        }
    }
}