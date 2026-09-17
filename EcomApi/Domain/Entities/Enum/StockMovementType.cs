namespace Domain.Enums;

public enum StockMovementType
{
    Inbound = 1,        // Entrada (compra, produção)
    Outbound = 2,       // Saída (venda, uso)
    Adjustment = 3,     // Ajuste manual (inventário)
    Return = 4,         // Devolução de cliente
    Transfer = 5,       // Transferência entre lojas/almoxarifados
    Loss = 6,           // Perda (roubo, extravio)
    Damage = 7,         // Danificado (avaria)
    Reservation = 8,    // Reserva para pedido
    Cancellation = 9    // Cancelamento de reserva/pedido
}