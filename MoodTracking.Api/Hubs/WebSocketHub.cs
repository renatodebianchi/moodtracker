using MediatR; 
using System; 
using Microsoft.AspNetCore.SignalR; 
using System.Threading; 
using System.Threading.Tasks; 
namespace Api.Hubs 
{ 
    /// <summary>
    /// Hub SignalR para comunicação em tempo real via WebSocket.
    /// </summary>
    public class WebSocketHub : Hub 
    { 
        private readonly IMediator _mediator; 
        /// <summary>
        /// Inicializa uma nova instância do <see cref="WebSocketHub"/>.
        /// </summary>
        /// <param name="mediator">Instância do MediatR para orquestração de comandos/queries.</param>
        public WebSocketHub(IMediator mediator) 
        { 
            _mediator = mediator; 
        } 
        /// <summary>
        /// Evento chamado quando um cliente se conecta ao hub.
        /// </summary>
        public override async Task OnConnectedAsync() 
        { 
            await base.OnConnectedAsync(); 
            // This newMessage call is what is not being received on the front end 
            // This console.WriteLine does print when I bring up the component in the front end. 
            Console.WriteLine("Nova conexao: " + Context.ConnectionId); 
            await SubscribeAsync("GeneralGroup"); 
            await Clients.All.SendAsync("clientFunctionCallbackName", "TesteSocketApi"); 
            await SendMessageAsync("GeneralGroup", "clientFunctionCallbackName", "Group Message"); 
        } 
        /// <summary>
        /// Evento chamado quando um cliente se desconecta do hub.
        /// </summary>
        /// <param name="exception">Exceção lançada durante a desconexão, se houver.</param>
        public override Task OnDisconnectedAsync(Exception? exception) 
        { 
            return base.OnDisconnectedAsync(exception); 
        } 
        /// <summary>
        /// Adiciona a conexão do cliente a um grupo SignalR.
        /// </summary>
        /// <param name="group">Nome do grupo.</param>
        public async Task SubscribeAsync(string group) 
        { 
            await Groups.AddToGroupAsync(Context.ConnectionId, group); 
        } 
        /// <summary>
        /// Envia uma mensagem para todos os clientes de um grupo SignalR.
        /// </summary>
        /// <param name="group">Nome do grupo.</param>
        /// <param name="callBackName">Nome do callback do lado do cliente.</param>
        /// <param name="message">Mensagem a ser enviada.</param>
        public async Task SendMessageAsync(string group, string callBackName, string message) 
        { 
            //await Clients.All.SendAsync("test", message); 
            await Clients.Group(group).SendAsync(callBackName, message); 
        } 
        /// <summary>
        /// Executa uma tarefa de exemplo e retorna uma mensagem de status.
        /// </summary>
        /// <param name="group">Nome do grupo SignalR.</param>
        /// <param name="args">Argumentos para a tarefa.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Mensagem de status da tarefa.</returns>
        public async Task<string> ExecuteSomeTask(string group, string[] args, CancellationToken cancellationToken) 
        { 
            /*IRequest request = new ListAllPedidoRequest(); 
            var retorno = await _mediator.Send(request, cancellationToken);*/ 
            await Task.Delay(1); 
            var retorno = "Service Online"; 
            return retorno; 
        } 
    } 
}
