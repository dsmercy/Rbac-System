using Microsoft.EntityFrameworkCore;
using RbacSystem.Models.Entities;

namespace RbacSystem.Data;

public class RbacDbContext : DbContext
{
    public RbacDbContext(DbContextOptions<RbacDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<UserGroup> UserGroups { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<GroupRole> GroupRoles { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserId).HasColumnName("userid").UseIdentityAlwaysColumn();
            entity.Property(e => e.Username).HasColumnName("username").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(200).IsRequired();
            entity.Property(e => e.IsActive).HasColumnName("isactive").HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasColumnName("createdat").HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Group configuration
        modelBuilder.Entity<Group>(entity =>
        {
            entity.ToTable("groups");
            entity.HasKey(e => e.GroupId);
            entity.Property(e => e.GroupId).HasColumnName("groupid").UseIdentityAlwaysColumn();
            entity.Property(e => e.GroupName).HasColumnName("groupname").HasMaxLength(150).IsRequired();
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(300);
            entity.Property(e => e.CreatedAt).HasColumnName("createdat").HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasIndex(e => e.GroupName).IsUnique();
        });

        // Role configuration
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles");
            entity.HasKey(e => e.RoleId);
            entity.Property(e => e.RoleId).HasColumnName("roleid").UseIdentityAlwaysColumn();
            entity.Property(e => e.RoleName).HasColumnName("rolename").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(300);
            entity.Property(e => e.CreatedAt).HasColumnName("createdat").HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasIndex(e => e.RoleName).IsUnique();
        });

        // Permission configuration
        modelBuilder.Entity<Permission>(entity =>
        {
            entity.ToTable("permissions");
            entity.HasKey(e => e.PermissionId);
            entity.Property(e => e.PermissionId).HasColumnName("permissionid").UseIdentityAlwaysColumn();
            entity.Property(e => e.PermissionName).HasColumnName("permissionname").HasMaxLength(150).IsRequired();
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(300);

            entity.HasIndex(e => e.PermissionName).IsUnique();
        });

        // UserGroup configuration
        modelBuilder.Entity<UserGroup>(entity =>
        {
            entity.ToTable("usergroups");
            entity.HasKey(e => e.UserGroupId);
            entity.Property(e => e.UserGroupId).HasColumnName("usergroupid").UseIdentityAlwaysColumn();
            entity.Property(e => e.UserId).HasColumnName("userid").IsRequired();
            entity.Property(e => e.GroupId).HasColumnName("groupid").IsRequired();
            entity.Property(e => e.AssignedAt).HasColumnName("assignedat").HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasIndex(e => new { e.UserId, e.GroupId }).IsUnique();

            entity.HasOne(e => e.User)
                .WithMany(u => u.UserGroups)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Group)
                .WithMany(g => g.UserGroups)
                .HasForeignKey(e => e.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // UserRole configuration
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("userroles");
            entity.HasKey(e => e.UserRoleId);
            entity.Property(e => e.UserRoleId).HasColumnName("userroleid").UseIdentityAlwaysColumn();
            entity.Property(e => e.UserId).HasColumnName("userid").IsRequired();
            entity.Property(e => e.RoleId).HasColumnName("roleid").IsRequired();
            entity.Property(e => e.AssignedAt).HasColumnName("assignedat").HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasIndex(e => new { e.UserId, e.RoleId }).IsUnique();

            entity.HasOne(e => e.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // GroupRole configuration
        modelBuilder.Entity<GroupRole>(entity =>
        {
            entity.ToTable("grouproles");
            entity.HasKey(e => e.GroupRoleId);
            entity.Property(e => e.GroupRoleId).HasColumnName("grouproleid").UseIdentityAlwaysColumn();
            entity.Property(e => e.GroupId).HasColumnName("groupid").IsRequired();
            entity.Property(e => e.RoleId).HasColumnName("roleid").IsRequired();
            entity.Property(e => e.AssignedAt).HasColumnName("assignedat").HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasIndex(e => new { e.GroupId, e.RoleId }).IsUnique();

            entity.HasOne(e => e.Group)
                .WithMany(g => g.GroupRoles)
                .HasForeignKey(e => e.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Role)
                .WithMany(r => r.GroupRoles)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // RolePermission configuration
        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.ToTable("rolepermissions");
            entity.HasKey(e => e.RolePermissionId);
            entity.Property(e => e.RolePermissionId).HasColumnName("rolepermissionid").UseIdentityAlwaysColumn();
            entity.Property(e => e.RoleId).HasColumnName("roleid").IsRequired();
            entity.Property(e => e.PermissionId).HasColumnName("permissionid").IsRequired();

            entity.HasIndex(e => new { e.RoleId, e.PermissionId }).IsUnique();

            entity.HasOne(e => e.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(e => e.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}