//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//namespace Airgap.Entity.Entities
//{
//    using System;
//    using System.Data.Entity;
//    using System.ComponentModel.DataAnnotations.Schema;
//    using System.Linq;

//    public partial class Application : DbContext
//    {
//        public Application()
//            : base("name=Applicationa")
//        {
//        }

//        public virtual DbSet<Account> Accounts { get; set; }
//        public virtual DbSet<AccountAppliance> AccountAppliances { get; set; }
//        public virtual DbSet<AccountToken> AccountTokens { get; set; }
//        public virtual DbSet<Appliance> Appliances { get; set; }
//        public virtual DbSet<Event> Events { get; set; }
//        public virtual DbSet<EventType> EventTypes { get; set; }
//        public virtual DbSet<Notification> Notifications { get; set; }
//        public virtual DbSet<NotificationPreference> NotificationPreferences { get; set; }
//        public virtual DbSet<PasswordHistory> PasswordHistories { get; set; }
//        public virtual DbSet<Setting> Settings { get; set; }
//        public virtual DbSet<State> States { get; set; }
//        public virtual DbSet<TimerSchedule> TimerSchedules { get; set; }
//        public virtual DbSet<TimerType> TimerTypes { get; set; }

//        protected override void OnModelCreating(DbModelBuilder modelBuilder)
//        {
//            modelBuilder.Entity<Appliance>()
//                .Property(e => e.ZipCode)
//                .IsFixedLength();

//            modelBuilder.Entity<Appliance>()
//                .Property(e => e.TriggerMile)
//                .HasPrecision(12, 1);

//            modelBuilder.Entity<EventType>()
//                .HasMany(e => e.NotificationPreferences)
//                .WithRequired(e => e.EventType)
//                .WillCascadeOnDelete(false);
//        }
//    }
//}
