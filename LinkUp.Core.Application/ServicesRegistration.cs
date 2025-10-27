using System.Reflection;
using LinkUp.Core.Application.Interfaces;
using LinkUp.Core.Application.Services;
using LinkUp.Core.Domain.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinkUp.Core.Application
{
    public static class ServicesRegistration
    {
        //Extension method - Decorator pattern
        public static void AddApplicationLayerIoc(this IServiceCollection services, IConfiguration config)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ILikeService, LIkeService>();
            services.AddScoped<IFriendShipRequestService, FriendShipRequestService>();
            services.AddScoped<IFriendShipService, FriendShipService>();
            services.AddScoped<ICommentService, CommentService>();
            
            services.AddScoped<IBattleshipGameService, BattleshipGameService>();
            services.AddScoped<IShipService, ShipService>();
            services.AddScoped<IShipPositionService, ShipPositionService>();
            services.AddScoped<IAttackService, AttackService>();
        }
    }
}
