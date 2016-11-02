using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Conreign.Client.SignalR.Proxies;
using Conreign.Core.Contracts.Client;
using Conreign.Core.Contracts.Client.Messages;
using Conreign.Core.Contracts.Communication;
using Microsoft.AspNet.SignalR.Client;

namespace Conreign.Client.SignalR
{
    public sealed class SignalRClientConnection : IClientConnection
    {
        private readonly SignalRConnectionContext _context;
        private readonly IDisposable _disposable;
        private readonly Subject<IClientEvent> _subject;
        private bool _isDisposed;

        internal SignalRClientConnection(HubConnection connection, IHubProxy hub)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (hub == null)
            {
                throw new ArgumentNullException(nameof(hub));
            }
            _subject = new Subject<IClientEvent>();
            Events = _subject.AsObservable();
            _context = new SignalRConnectionContext(hub, connection, new Metadata());
            _context.HubConnection.Error += OnConnectionError;
            var disposables = new List<IDisposable>
            {
                _context.Hub.On<MessageEnvelope>("OnNext", e => _subject.OnNext((IClientEvent) e.Payload)),
                _context.Hub.On<Exception>("OnError", _subject.OnError),
                _context.Hub.On("OnCompleted", _subject.OnCompleted),
                connection
            };
            _disposable = new CompositeDisposable(disposables);
        }

        public Guid Id => Guid.Parse(_context.HubConnection.ConnectionId);

        public IObservable<IClientEvent> Events { get; }

        public Task<LoginResult> Authenticate(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentException("Access token cannot be null or empty.", nameof(accessToken));
            }
            EnsureIsNotDisposed();
            var token = new JwtSecurityToken(accessToken);
            var userId = Guid.Parse(token.Subject);
            var meta = new Metadata {AccessToken = accessToken};
            var context = new SignalRConnectionContext(_context.Hub, _context.HubConnection, meta);
            var result = new LoginResult(new UserProxy(context), userId, accessToken);
            return Task.FromResult(result);
        }

        public async Task<LoginResult> Login()
        {
            EnsureIsNotDisposed();
            var login = new LoginCommand();
            var response = await _context.Send(login);
            return await Authenticate(response.AccessToken);
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            _context.HubConnection.Error -= OnConnectionError;
            _context.HubConnection.Stop();
            _disposable.Dispose();
            _isDisposed = true;
        }

        private void OnConnectionError(Exception ex)
        {
            _subject.OnNext(new ConnectionProblemDetected(ex));
        }

        private void EnsureIsNotDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException($"SignalRGameConnection: {Id}");
            }
        }
    }
}