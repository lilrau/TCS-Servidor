using System.Text;

namespace TCS_Cliente.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Lê o corpo da requisição
            context.Request.EnableBuffering();

            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
            {
                var body = await reader.ReadToEndAsync();
                Console.WriteLine($"Requisição Recebida: {context.Request.Method} {context.Request.Path} - Corpo: {body}");
                context.Request.Body.Position = 0; // Resetando a posição do fluxo para o próximo middleware
            }

            await _next(context);
        }
    }
}
