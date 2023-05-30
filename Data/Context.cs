using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using Teste_Frontend.Models;

namespace Teste_Frontend.Data
{
    public class Context : DbContext
    {
        public DbSet<Produto> Pessoas { get; set; }
    }
}
