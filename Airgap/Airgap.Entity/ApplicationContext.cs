using Airgap.Entity.Entities;
using Airgap.Entity.Mappings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airgap.Entity
{
    public class ApplicationContext:DbContext
    {

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {

        }
        public  DbSet<Account> Accounts { get; set; }
        public  DbSet<AccountAppliance> AccountAppliances { get; set; }
        public  DbSet<AccountToken> AccountTokens { get; set; }
        public  DbSet<Appliance> Appliances { get; set; }
        public  DbSet<Event> Events { get; set; }
        public  DbSet<EventType> EventTypes { get; set; }
        public  DbSet<Notification> Notifications { get; set; }
        public  DbSet<NotificationPreference> NotificationPreferences { get; set; }
        public  DbSet<PasswordHistory> PasswordHistories { get; set; }
        public  DbSet<Setting> Settings { get; set; }
        public  DbSet<State> States { get; set; }
        public  DbSet<TimerSchedule> TimerSchedules { get; set; }
        public  DbSet<TimerType> TimerTypes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            new AccountApplianceMap(modelBuilder.Entity<AccountAppliance>());
            new AccountMap(modelBuilder.Entity<Account>());
            new AccountTokenMap(modelBuilder.Entity<AccountToken>());
            new ApplianceMap(modelBuilder.Entity<Appliance>());
            new EventMap(modelBuilder.Entity<Event>());
            new EventTypeMap(modelBuilder.Entity<EventType>());
            new NotificationMap(modelBuilder.Entity<Notification>());
            new NotificationPreferenceMap(modelBuilder.Entity<NotificationPreference>());
            new PasswordHistoryMap(modelBuilder.Entity<PasswordHistory>());
            new SettingMap(modelBuilder.Entity<Setting>());
            new StateMap(modelBuilder.Entity<State>());
            new TimerScheduleMap(modelBuilder.Entity<TimerSchedule>());
            new TimerTypeMap(modelBuilder.Entity<TimerType>());
        }
    }
}
