

using System.Reflection.Metadata.Ecma335;

namespace Domain.Entities;

public class State
{
    public int Id {get; set;}
    public string Name {get; set;}
    public string Uf {get;set;}
    public ICollection<City> Cities { get; set; } = new List<City>();
    public ICollection<Client> Clients { get; set; } = new List<Client>();
    public ICollection<Supplier> Suppliers { get; set; } = new List<Supplier>();
}