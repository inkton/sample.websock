using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Inkton.Nester;
using Websock;
using Websock.Model;

namespace Websock.Database
{
    public class MessageContext : DbContext
    {
        public MessageContext (DbContextOptions<MessageContext> options)
            : base(options)
        {
        }

        public DbSet<Session> Sessions { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Session>()
                .HasKey(session => session.Id);
            modelBuilder.Entity<Message>()
                .HasKey(message => message.Id);
        }
    }

    public static class MessageContextFactory
    {
        public static MessageContext Create(Runtime runtime)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MessageContext>();

            string connectionString = string.Format(@"Server={0};database={1};uid={2};pwd={3};",
                                    runtime.MySQL.Host,
                                    runtime.MySQL.Resource,
                                    runtime.MySQL.User,
                                    runtime.MySQL.Password);

            optionsBuilder.UseMySql(connectionString);
            var context = new MessageContext(optionsBuilder.Options);
            context.Database.EnsureCreated();

            return context;
        }
    }
}
