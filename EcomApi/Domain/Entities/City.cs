using System.Text.Json.Serialization;

namespace Domain.Entities;

public class City
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    // Relacionamento com Estado
    public int StateId { get; set; }
    [JsonIgnore]
    public State State { get; set; } = null!;

    // Uma Cidade pode ter vários Clientes
    public ICollection<Client> Clients { get; set; } = new List<Client>();
    public ICollection<Supplier> Suppliers { get; set; } = new List<Supplier>();
}

