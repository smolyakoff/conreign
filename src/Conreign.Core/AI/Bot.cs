﻿using System;
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
        private IGaugeMeasure _queueSize;
        private readonly ICounterMeasure _received;

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
            _context = new BotContext(botId, connection, Complete, Notify);
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
            _received = _context.Logger.CountOperation(DiagnosticsConstants.BotReceivedEventsCounterName(botId), resolution: DiagnosticsConstants.EventsCounterResolution);
        }

        public string Id => _context.BotId;
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
            _queueSize = _context.Logger.GaugeOperation(DiagnosticsConstants.BotQueueSizeGaugeName(Id), "items", () => _processor.InputCount);
            _received.Reset();
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
            _processor.Post(new BotStopped());
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
                _context.Logger.Error("[Bot:{BotId}:{UserId}] Received null event.", _context.BotId, _context.UserId);
                return;
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
                _exception = new TaskCanceledException("Bot execution was cancelled.");
                Complete();
                return;
            }
            var posted = _processor.Post(@event);
            if (!posted)
            {
                _context.Logger.Warning("[Bot:{BotId}:{UserId}] Failed to accept {EventType}. Queue length exceeded.",
                    _context.BotId, 
                    _context.UserId, 
                    @event.GetType().Name);
                _exception = new InvalidOperationException("Bot queue is too large.");
                Complete();
            }
        }

        private async Task Process(IClientEvent @event)
        {
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