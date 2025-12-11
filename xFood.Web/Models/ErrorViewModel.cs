namespace xFood.Web.Models;

// Modelo de erro exibido na interface.
public class ErrorViewModel
{
    // Id da requisição para rastreamento.
    public string? RequestId { get; set; }

    // Indica se o RequestId deve ser exibido.
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
