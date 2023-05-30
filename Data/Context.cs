using System.Data.Entity;
using Teste_Frontend.Models;

namespace Teste_Frontend.Data
{
    public class Context : DbContext
    {
        public DbSet<Produto> Produtos { get; set; }
    }
}
