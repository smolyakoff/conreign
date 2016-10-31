using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Conreign.Core.AI.Behaviours;
using Conreign.Core.AI.Events;
using Conreign.Core.Contracts.Client;
using Conreign.Core.Contracts.Communication;
using Serilog;
using SerilogMetrics;

namespace Conreign.Core.AI
{
    public class Bot : IDisposable
    {
        private readonly BotContext _context;
        private readonly IBotBehaviour<IClientEvent> _entryBehaviour;
        private readonly IDisposable _subscription;
        private Subject<IBotEvent> _subject;
        private bool _isDisposed;
        private ActionBlock<IClientEvent> _processor;
        private Exception _exception;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;
        private IGaugeMeasure _queueGauge;
        private readonly ICounterMeasure _receivedEventsCounter;

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
            _context = new BotContext(readableId, connection, Complete, Notify);
            _subscription = connection.Events.Subscribe(OnClientEvent, OnClientException, OnClientCompleted);
            var allBehaviours = behaviours.ToList();
            if (!behaviours.OfType<StopBehaviour>().Any())
            {
                allBehaviours.Add(new StopBehaviour());
            }
            
            var dispatcher = new DispatcherBehaviour(allBehaviours);
            var retry = new RetryBehaviour(dispatcher);
            var errorHandling = new ErrorHandlingBehaviour(retry);
            var diagnostics = new DiagnosticsBehaviour(errorHandling);
            _entryBehaviour = diagnostics;
            Events = Observable.Empty<IBotEvent>();
            _receivedEventsCounter = _context.Logger.CountOperation("BotReceivedEvents");
        }

        public string Id => _context.ReadableId;
        public IClientConnection Connection => _context.Connection;
        public IObservable<IBotEvent> Events { get; private set; }

        public async Task Run(CancellationToken? cancellationToken = null)
        {
            EnsureIsNotDisposed();
            cancellationToken = cancellationToken ?? CancellationToken.None;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken.Value, _cancellationTokenSource.Token).Token;
            var processorOptions = new ExecutionDataflowBlockOptions
            {
                BoundedCapacity = 100
            };
            _processor = new ActionBlock<IClientEvent>(Process, processorOptions);
            _subject?.Dispose();
            _subject = new Subject<IBotEvent>();
            _queueGauge = _context.Logger.GaugeOperation("Bot processing queue size", "items", () => _processor.InputCount);
            _receivedEventsCounter.Reset();
            Events = _subject.AsObservable();
            Notify(new BotStarted());
            await _processor.Completion;
            if (_exception != null)
            {
                throw _exception;
            }
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
            _cancellationTokenSource.Cancel();
            _subject?.Dispose();
            _subscription?.Dispose();
            _processor?.Complete();
            _subject = null;
            _processor = null;
            _isDisposed = true;
        }

        private void OnClientEvent(IClientEvent @event)
        {
            Handle(@event);
        }

        private void OnClientException(Exception exception)
        {
            _exception = exception;
            Complete();
        }

        private void OnClientCompleted()
        {
            if (_isDisposed)
            {
                return;
            }
            _exception = new InvalidOperationException("Client stream unexpectedly completed.");
            Complete();
        }

        private void Complete(Exception exception = null)
        {
            _processor.Complete();
            _exception = _exception ?? exception;
            if (_exception == null)
            {
                _subject.OnCompleted();
            }
            else
            {
                _subject.OnError(_exception);
            }
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
                _context.Logger.Error("[{ReadableId}-{UserId}]: Received null event.", _context.ReadableId, _context.UserId);
                return;
            }
            _receivedEventsCounter.Increment();
            _queueGauge.Write();
            _context.Logger.Debug("[{ReadableId}-{UserId}]: Received {@Event}",
                _context.ReadableId,
                _context.UserId,
                @event);
            var posted = _processor.Post(@event);
            if (!posted)
            {
                _context.Logger.Warning("[{ReadableId}-{UserId}] Failed to handle event of type {EventType}. Queue length exceeded.",
                    _context.ReadableId, 
                    _context.UserId, 
                    @event.GetType().Name);
                _exception = new InvalidOperationException("Queue is too large");
                Complete();
            }
        }

        private async Task Process(IClientEvent @event)
        {
            if (_cancellationToken.IsCancellationRequested)
            {
                _context.Logger.Warning("[{ReadableId}-{UserId}] Bot execution cancelled.");
                _exception = new TaskCanceledException("Bot execution was cancelled.");
                Complete();
                return;
            }
            await _entryBehaviour.Handle(new BotNotification<IClientEvent>(@event, _context));
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