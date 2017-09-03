using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Conreign.Client.Contracts;
using Conreign.Contracts.Communication;
using Conreign.LoadTest.Core.Behaviours;
using Conreign.LoadTest.Core.Events;
using Serilog;
using SerilogMetrics;

namespace Conreign.LoadTest.Core
{
    // TODO: better cancellation handling, not a stateful object?
    public class Bot : IDisposable
    {
        private const string ReceivedCounterName = "Bot.EventsReceived";
        public const string QueueSizeGaugeName = "Bot.QueueSize";
        private const int ReceivedCounterResolution = 10;
        private const int MaxQueueSize = 500;
        private readonly BotContext _context;
        private readonly IBotBehaviour<IClientEvent> _entryBehaviour;
        private readonly ICounterMeasure _received;
        private readonly IDisposable _subscription;
        private CancellationToken _cancellationToken;
        private Exception _exception;
        private bool _isDisposed;
        private ActionBlock<IClientEvent> _processor;
        private IGaugeMeasure _queueSize;
        private BotState _state;
        private Subject<IBotEvent> _subject;

        public Bot(string botId, IClientConnection connection, params IBotBehaviour[] behaviours)
        {
            if (string.IsNullOrEmpty(botId))
            {
                throw new ArgumentException("Bot id cannot be null or empty.", nameof(botId));
            }
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            _state = BotState.Idle;
            _context = new BotContext(botId, connection, Stop, Notify);
            _subscription = connection.Events.Subscribe(OnClientEvent, OnClientException, OnClientCompleted);
            var allBehaviours = behaviours.ToList();
            var dispatcher = new DispatcherBehaviour(allBehaviours);
            //var retry = new RetryBehaviour(dispatcher);
            var errorHandling = new ErrorHandlingBehaviour(dispatcher);
            var diagnostics = new DiagnosticsBehaviour(errorHandling);
            _entryBehaviour = diagnostics;
            _received = _context.Logger.CountOperation(ReceivedCounterName, resolution: ReceivedCounterResolution);
            Events = Observable.Empty<IBotEvent>();
        }

        public string Id => _context.BotId;
        public IClientConnection Connection => _context.Connection;
        public IObservable<IBotEvent> Events { get; private set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task Run(CancellationToken? cancellationToken = null)
        {
            EnsureIsNotDisposed();
            _cancellationToken = cancellationToken ?? CancellationToken.None;
            var processorOptions = new ExecutionDataflowBlockOptions
            {
                BoundedCapacity = MaxQueueSize
            };
            _processor = new ActionBlock<IClientEvent>(Process, processorOptions);
            _subject?.Dispose();
            _subject = new Subject<IBotEvent>();
            _queueSize = _context.Logger.GaugeOperation(QueueSizeGaugeName, "items", () => _processor.InputCount);
            _received.Reset();
            Events = _subject.AsObservable();
            Notify(new BotStarted());
            _state = BotState.Started;
            await _processor.Completion;
            if (_exception != null)
            {
                throw _exception;
            }
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
            Stop();
            _subject?.Dispose();
            _subject = null;
            _isDisposed = true;
        }

        private void OnClientEvent(IClientEvent @event)
        {
            Handle(@event);
        }

        private void OnClientException(Exception exception)
        {
            _exception = exception;
            Stop();
        }

        private void OnClientCompleted()
        {
            if (_isDisposed)
            {
                return;
            }
            Stop(new InvalidOperationException("Client stream unexpectedly completed."));
        }

        private void Stop(Exception exception = null)
        {
            EnsureIsNotDisposed();
            if (_state != BotState.Started)
            {
                return;
            }
            _processor?.Post(new BotStopped());
            _processor?.Complete();
            _exception = _exception ?? exception;
            if (_exception == null)
            {
                _subject.OnCompleted();
            }
            else
            {
                _subject.OnError(_exception);
            }
            _state = BotState.Stopped;
        }

        private void Notify(IBotEvent @event)
        {
            _subject.OnNext(@event);
            Handle(@event);
        }

        private void Handle(IClientEvent @event)
        {
            EnsureIsNotDisposed();
            if (@event == null)
            {
                _context.Logger.Error("[Bot:{BotId}:{UserId}] Received null event.", _context.BotId, _context.UserId);
                return;
            }
            if (_state == BotState.Stopped)
            {
                throw new InvalidOperationException("Bot is stopped.");
            }
            _received.Increment();
            _queueSize.Write();
            _context.Logger.Debug("[Bot:{BotId}:{UserId}]: Received {EventType} {@Event}",
                _context.BotId,
                _context.UserId,
                @event.GetType().Name,
                @event);
            if (_cancellationToken.IsCancellationRequested)
            {
                _context.Logger.Warning("[Bot:{BotId}:{UserId}] Bot execution cancelled.");
                Stop(new TaskCanceledException("Bot execution was cancelled."));
                return;
            }
            var posted = _processor.Post(@event);
            if (!posted)
            {
                _context.Logger.Warning("[Bot:{BotId}:{UserId}] Failed to accept {EventType}. Queue length exceeded.",
                    _context.BotId,
                    _context.UserId,
                    @event.GetType().Name);
                Stop(new InvalidOperationException("Bot queue is too large."));
            }
        }

        private async Task Process(IClientEvent @event)
        {
            if (_state != BotState.Started)
            {
                return;
            }
            await _entryBehaviour.Handle(new BotNotification<IClientEvent>(@event, _context));
        }

        private void EnsureIsNotDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException($"Bot[${Id}]");
            }
        }
    }
}