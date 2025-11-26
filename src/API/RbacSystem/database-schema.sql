-- ============================
-- RBAC System Database Schema
-- PostgreSQL 14+
-- ============================

-- Drop existing tables if they exist (in correct order to handle foreign keys)
DROP TABLE IF EXISTS RolePermissions CASCADE;
DROP TABLE IF EXISTS GroupRoles CASCADE;
DROP TABLE IF EXISTS UserRoles CASCADE;
DROP TABLE IF EXISTS UserGroups CASCADE;
DROP TABLE IF EXISTS Permissions CASCADE;
DROP TABLE IF EXISTS Roles CASCADE;
DROP TABLE IF EXISTS Groups CASCADE;
DROP TABLE IF EXISTS Users CASCADE;

-- ============================
-- USERS
-- ============================
CREATE TABLE Users (
    UserId          BIGINT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    Username        VARCHAR(100) NOT NULL UNIQUE,
    Email           VARCHAR(200) NOT NULL UNIQUE,
    IsActive        BOOLEAN NOT NULL DEFAULT TRUE,
    CreatedAt       TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_users_username ON Users(Username);
CREATE INDEX idx_users_email ON Users(Email);
CREATE INDEX idx_users_isactive ON Users(IsActive);

-- ============================
-- GROUPS (Department, Team, Business Unit)
-- ============================
CREATE TABLE Groups (
    GroupId         BIGINT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    GroupName       VARCHAR(150) NOT NULL UNIQUE,
    Description     VARCHAR(300),
    CreatedAt       TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_groups_groupname ON Groups(GroupName);

-- ============================
-- USER ? GROUP MANY-TO-MANY
-- ============================
CREATE TABLE UserGroups (
    UserGroupId     BIGINT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    UserId          BIGINT NOT NULL,
    GroupId         BIGINT NOT NULL,
    AssignedAt      TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(UserId, GroupId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
    FOREIGN KEY (GroupId) REFERENCES Groups(GroupId) ON DELETE CASCADE
);

CREATE INDEX idx_usergroups_userid ON UserGroups(UserId);
CREATE INDEX idx_usergroups_groupid ON UserGroups(GroupId);

-- ============================
-- ROLES
-- ============================
CREATE TABLE Roles (
    RoleId          BIGINT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    RoleName        VARCHAR(100) NOT NULL UNIQUE,
    Description     VARCHAR(300),
    CreatedAt       TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_roles_rolename ON Roles(RoleName);

-- ============================
-- PERMISSIONS
-- ============================
CREATE TABLE Permissions (
    PermissionId    BIGINT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    PermissionName  VARCHAR(150) NOT NULL UNIQUE,
    Description     VARCHAR(300)
);

CREATE INDEX idx_permissions_permissionname ON Permissions(PermissionName);

-- ============================
-- ROLE ? PERMISSION MANY-TO-MANY
-- ============================
CREATE TABLE RolePermissions (
    RolePermissionId BIGINT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    RoleId           BIGINT NOT NULL,
    PermissionId     BIGINT NOT NULL,
    UNIQUE(RoleId, PermissionId),
    FOREIGN KEY (RoleId) REFERENCES Roles(RoleId) ON DELETE CASCADE,
    FOREIGN KEY (PermissionId) REFERENCES Permissions(PermissionId) ON DELETE CASCADE
);

CREATE INDEX idx_rolepermissions_roleid ON RolePermissions(RoleId);
CREATE INDEX idx_rolepermissions_permissionid ON RolePermissions(PermissionId);

-- ============================
-- USER ? ROLE (Direct Role Assignment)
-- ============================
CREATE TABLE UserRoles (
    UserRoleId     BIGINT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    UserId         BIGINT NOT NULL,
    RoleId         BIGINT NOT NULL,
    AssignedAt     TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
    FOREIGN KEY (RoleId) REFERENCES Roles(RoleId) ON DELETE CASCADE
);

CREATE INDEX idx_userroles_userid ON UserRoles(UserId);
CREATE INDEX idx_userroles_roleid ON UserRoles(RoleId);

-- ============================
-- GROUP ? ROLE (Roles assigned to teams)
-- ============================
CREATE TABLE GroupRoles (
    GroupRoleId    BIGINT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    GroupId        BIGINT NOT NULL,
    RoleId         BIGINT NOT NULL,
    AssignedAt     TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(GroupId, RoleId),
    FOREIGN KEY (GroupId) REFERENCES Groups(GroupId) ON DELETE CASCADE,
    FOREIGN KEY (RoleId) REFERENCES Roles(RoleId) ON DELETE CASCADE
);

CREATE INDEX idx_grouproles_groupid ON GroupRoles(GroupId);
CREATE INDEX idx_grouproles_roleid ON GroupRoles(RoleId);

-- ============================
-- COMMENTS FOR DOCUMENTATION
-- ============================
COMMENT ON TABLE Users IS 'Application users';
COMMENT ON TABLE Groups IS 'User groups (departments, teams, business units)';
COMMENT ON TABLE Roles IS 'User roles';
COMMENT ON TABLE Permissions IS 'System permissions';
COMMENT ON TABLE UserGroups IS 'User to group assignments';
COMMENT ON TABLE UserRoles IS 'Direct user to role assignments';
COMMENT ON TABLE GroupRoles IS 'Group to role assignments (inherited by users)';
COMMENT ON TABLE RolePermissions IS 'Role to permission assignments';

-- ============================
-- VERIFICATION QUERIES
-- ============================

-- Count tables
SELECT 
    schemaname,
    tablename
FROM pg_tables 
WHERE schemaname = 'public'
ORDER BY tablename;

-- Verify all tables exist
SELECT 
    'Users' as table_name, COUNT(*) as row_count FROM Users
UNION ALL
SELECT 'Groups', COUNT(*) FROM Groups
UNION ALL
SELECT 'Roles', COUNT(*) FROM Roles
UNION ALL
SELECT 'Permissions', COUNT(*) FROM Permissions
UNION ALL
SELECT 'UserGroups', COUNT(*) FROM UserGroups
UNION ALL
SELECT 'UserRoles', COUNT(*) FROM UserRoles
UNION ALL
SELECT 'GroupRoles', COUNT(*) FROM GroupRoles
UNION ALL
SELECT 'RolePermissions', COUNT(*) FROM RolePermissions;

-- Show all foreign keys
SELECT
    tc.table_name, 
    kcu.column_name, 
    ccu.table_name AS foreign_table_name,
    ccu.column_name AS foreign_column_name 
FROM information_schema.table_constraints AS tc 
JOIN information_schema.key_column_usage AS kcu
  ON tc.constraint_name = kcu.constraint_name
  AND tc.table_schema = kcu.table_schema
JOIN information_schema.constraint_column_usage AS ccu
  ON ccu.constraint_name = tc.constraint_name
  AND ccu.table_schema = tc.table_schema
WHERE tc.constraint_type = 'FOREIGN KEY' 
  AND tc.table_schema = 'public'
ORDER BY tc.table_name;

-- Show all indexes
SELECT
    tablename,
    indexname,
    indexdef
FROM pg_indexes
WHERE schemaname = 'public'
ORDER BY tablename, indexname;

COMMIT;