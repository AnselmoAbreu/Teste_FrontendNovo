using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Teste_Frontend.Models;

namespace Teste_Frontend.Data
{
    public class Teste_FrontendContext : DbContext
    {
        public Teste_FrontendContext (DbContextOptions<Teste_FrontendContext> options)
            : base(options)
        {
        }

        public DbSet<Teste_Frontend.Models.Produto> Produto { get; set; }

        public DbSet<Teste_Frontend.Models.ProdutosViewModel> ProdutosViewModel { get; set; }
    }
}
