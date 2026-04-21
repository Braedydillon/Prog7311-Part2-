using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Prog7311_Part2.Models;

    public class ClientContextDatabase : DbContext
    {
        public ClientContextDatabase (DbContextOptions<ClientContextDatabase> options)
            : base(options)
        {
        }

        public DbSet<Prog7311_Part2.Models.Client> Client { get; set; } = default!;

public DbSet<Prog7311_Part2.Models.Contract> Contract { get; set; } = default!;

public DbSet<Prog7311_Part2.Models.ServiceRequest> ServiceRequest { get; set; } = default!;
    }
