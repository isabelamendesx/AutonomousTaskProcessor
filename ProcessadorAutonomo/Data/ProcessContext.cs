using Microsoft.EntityFrameworkCore;
using ProcessadorAutonomo.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProcessadorAutonomo.Data
{
    public class ProcessContext : DbContext
    {
        public DbSet<Process> Processes { get; set; }
        public DbSet<SubProcess> SubProcesses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Process>()
                .HasMany(process => process.SubProcesses)
                .WithOne(subprocess => subprocess.Process)
                .HasForeignKey(subprocess => subprocess.ProcessId)
                .IsRequired();

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder) =>
            builder.UseSqlite($"Data Source=C:\\Users\\isabelam\\source\\repos\\processadorTarefas\\ProcessadorAutonomo\\Data\\ProcessData.db");
        
    }
}
