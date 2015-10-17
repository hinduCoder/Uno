using Castle.DynamicProxy;

namespace WebClient.SignalR
{
    public class GameHubValidationInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            var method = invocation.Method;
            if (method.DeclaringType == typeof(GameHub) && method.GetBaseDefinition() != method)
            {
                invocation.Proceed();
                return;
            }
            

            var gameHub = invocation.InvocationTarget as GameHub;
            var gameSession = gameHub.CurrentPlayer.Room.GameSession;
            if (gameSession.CurrentPlayer.Name == gameHub.CurrentPlayer.Name)
                invocation.Proceed();
        }
    }
}