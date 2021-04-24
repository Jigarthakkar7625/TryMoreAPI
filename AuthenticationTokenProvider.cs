using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Claims;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.Infrastructure;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using System.Collections.Concurrent;

namespace TryMoreAPI
{
    public class AuthenticationTokenProvider : IAuthenticationTokenProvider
    {
        private static ConcurrentDictionary<string, AuthenticationTicket> _refreshTokens = new ConcurrentDictionary<string, AuthenticationTicket>();

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            await Task.Run(() =>
            {
                var guid = Guid.NewGuid().ToString();

                var refreshTokenProperties = new AuthenticationProperties(context.Ticket.Properties.Dictionary)
                {
                    IssuedUtc = context.Ticket.Properties.IssuedUtc,
                    ExpiresUtc = DateTime.UtcNow.AddYears(1) //expiration time of refresh_token 
                };
                var refreshTokenTicket = new AuthenticationTicket(context.Ticket.Identity, refreshTokenProperties);

                _refreshTokens.TryAdd(guid, refreshTokenTicket);

                context.SetToken(guid);
            });
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            await Task.Run(() =>
            {
                AuthenticationTicket ticket;
                if (_refreshTokens.TryRemove(context.Token, out ticket))
                {
                    context.SetTicket(ticket);
                }
            });
        }
        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }
        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }

        public string CreateRefreshToken(AuthenticationTokenCreateContext context)
        {

            var guid = Guid.NewGuid().ToString();

            var refreshTokenProperties = new AuthenticationProperties(context.Ticket.Properties.Dictionary)
            {
                IssuedUtc = context.Ticket.Properties.IssuedUtc,
                ExpiresUtc = DateTime.UtcNow.AddYears(1) //expiration time of refresh_token 
            };
            var refreshTokenTicket = new AuthenticationTicket(context.Ticket.Identity, refreshTokenProperties);

            _refreshTokens.TryAdd(guid, refreshTokenTicket);

            context.SetToken(guid);

            return context.Token;
        }
    }

    public class MyAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            await Task.Run(() =>
            {
                context.Validated();
            });
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                if (property.Key != ".issued" && property.Key != ".expires")
                {
                    context.AdditionalResponseParameters.Add(property.Key, property.Value);
                }
            }
            return Task.FromResult<object>(null);
        }
    }


}