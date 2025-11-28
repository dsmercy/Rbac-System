# RBAC React Application

A modern, full-featured React application for Role-Based Access Control (RBAC) system with TypeScript, Zustand for state management, and Bootstrap for UI components.

## 🚀 Features

- **Modern Tech Stack**: React 18, TypeScript, Vite, Zustand, React Router v6, Bootstrap 5
- **State Management**: Centralized state management using Zustand
- **API Integration**: Generic API service with Axios for all CRUD operations
- **Routing**: Centralized routing configuration with lazy loading
- **Responsive Design**: Mobile-first design with Bootstrap
- **Authentication**: JWT-based authentication with protected routes
- **CRUD Operations**: Complete management for Users, Groups, Roles, Permissions
- **Assignments**: Manage relationships between entities
- **Data Export**: Export data in JSON, Excel, and HTML formats
- **Modern UI**: Clean, professional interface with gradient accents
- **Toast Notifications**: User-friendly feedback system
- **Type Safety**: Full TypeScript support throughout the application

## 📁 Project Structure

```
rbac-react-app/
├── src/
│   ├── components/
│   │   ├── common/
│   │   │   ├── DataTable.tsx
│   │   │   ├── LoadingSpinner.tsx
│   │   │   └── Pagination.tsx
│   │   └── layout/
│   │       ├── Navbar.tsx
│   │       ├── Sidebar.tsx
│   │       └── Sidebar.css
│   ├── layouts/
│   │   └── MainLayout.tsx
│   ├── pages/
│   │   ├── Dashboard.tsx
│   │   ├── Users.tsx
│   │   ├── Groups.tsx
│   │   ├── Roles.tsx
│   │   ├── Permissions.tsx
│   │   ├── Assignments.tsx
│   │   ├── Export.tsx
│   │   ├── Login.tsx
│   │   └── NotFound.tsx
│   ├── routes/
│   │   └── index.tsx
│   ├── services/
│   │   ├── apiService.ts (Generic API service)
│   │   ├── userService.ts
│   │   ├── groupService.ts
│   │   ├── roleService.ts
│   │   ├── permissionService.ts
│   │   ├── assignmentService.ts
│   │   └── exportService.ts
│   ├── store/
│   │   ├── authStore.ts
│   │   └── uiStore.ts
│   ├── types/
│   │   └── index.ts
│   ├── App.tsx
│   ├── main.tsx
│   └── index.css
├── package.json
├── tsconfig.json
├── vite.config.ts
└── index.html
```

## 🛠️ Installation

### Prerequisites

- Node.js >= 18.x
- npm or yarn
- Your RBAC API running on http://localhost:5225

### Steps

1. **Clone or extract the project**

2. **Install dependencies**
```bash
npm install
```

3. **Configure API endpoint** (if different from default)

Edit `vite.config.ts`:
```typescript
server: {
  port: 3000,
  proxy: {
    '/api': {
      target: 'http://localhost:5225',  // Change this if your API runs on a different port
      changeOrigin: true,
    },
  },
}
```

4. **Start the development server**
```bash
npm run dev
```

5. **Build for production**
```bash
npm run build
```

6. **Preview production build**
```bash
npm run preview
```

## 🎯 Usage

### Default Credentials
- **Username**: `admin`
- **Password**: `password`

### Key Features

#### 1. Dashboard
- Overview statistics of users, groups, roles, and permissions
- Quick access to all modules

#### 2. Users Management
- View all users with pagination
- Create new users
- Edit existing users
- Delete users
- Search and filter users
- View user's effective permissions

#### 3. Groups Management
- Manage user groups/departments
- Create, edit, delete groups
- Assign users to groups

#### 4. Roles Management
- Define roles with descriptions
- View role permissions
- Create, edit, delete roles

#### 5. Permissions Management
- Manage system permissions
- Define permission names and descriptions
- CRUD operations for permissions

#### 6. Assignments
- **User → Role**: Assign roles directly to users
- **Group → Role**: Assign roles to groups
- **User → Group**: Add users to groups
- **Role → Permission**: Assign permissions to roles
- Bulk assignment operations

#### 7. Data Export
- Export to JSON format
- Export to Excel/CSV format
- Export to HTML format
- Timestamped exports with complete data

## 🏗️ Architecture

### State Management (Zustand)

The application uses Zustand for state management with two main stores:

**Auth Store** (`src/store/authStore.ts`):
- User authentication state
- Login/logout functionality
- User profile management
- Token management

**UI Store** (`src/store/uiStore.ts`):
- Sidebar open/close state
- Loading states
- Theme preferences

### API Service Layer

**Generic API Service** (`src/services/apiService.ts`):
- Centralized Axios instance
- Request/response interceptors
- Token management
- Error handling
- Generic CRUD methods (GET, POST, PUT, DELETE, PATCH)
- File download support

**Entity Services**:
Each entity (User, Group, Role, Permission) has its own service module that uses the generic API service.

### Routing

Centralized routing configuration in `src/routes/index.tsx` with:
- Lazy loading for code splitting
- Protected routes
- Route metadata (title, icon, navigation visibility)

### Type Safety

Full TypeScript support with:
- Entity types
- DTO types
- API response types
- Filter parameter types
- Store types

## 🎨 Customization

### Change Theme Colors

Edit `src/index.css`:
```css
:root {
  --primary-gradient: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}
```

### Add New Routes

1. Create page component in `src/pages/`
2. Add route configuration in `src/routes/index.tsx`
3. Add menu item in `src/components/layout/Sidebar.tsx`

### Add New API Endpoints

1. Create service in `src/services/`
2. Use the generic `apiService` methods
3. Define TypeScript types in `src/types/`

## 📦 Key Dependencies

- **react**: ^18.2.0
- **react-router-dom**: ^6.20.0
- **zustand**: ^4.4.7
- **axios**: ^1.6.2
- **bootstrap**: ^5.3.2
- **react-bootstrap**: ^2.9.1
- **lucide-react**: ^0.294.0 (Icons)
- **react-hot-toast**: ^2.4.1 (Notifications)
- **typescript**: ^5.2.2
- **vite**: ^5.0.8

## 🔧 Configuration Files

- **tsconfig.json**: TypeScript configuration
- **vite.config.ts**: Vite build tool configuration
- **package.json**: Project dependencies and scripts

## 🚀 Deployment

### Build for Production

```bash
npm run build
```

This creates an optimized build in the `dist` folder.

### Deploy to Static Hosting

The built files can be deployed to any static hosting service:
- Vercel
- Netlify
- GitHub Pages
- AWS S3
- Azure Static Web Apps
- Any CDN or web server

### Environment Variables

For production, update the API endpoint in `vite.config.ts` or use environment variables:

```typescript
// .env.production
VITE_API_URL=https://your-api-domain.com
```

## 🧪 Testing

Add your testing framework of choice:

```bash
# For Vitest
npm install -D vitest @testing-library/react @testing-library/jest-dom

# For Jest
npm install -D jest @testing-library/react @testing-library/jest-dom
```

## 📝 API Integration Notes

The application expects the following API endpoints:

- `GET /api/users` - Get users list
- `POST /api/users` - Create user
- `PUT /api/users/:id` - Update user
- `DELETE /api/users/:id` - Delete user
- `GET /api/users/:id/permissions` - Get user permissions
- (Similar endpoints for groups, roles, permissions)
- `POST /api/assignments/*` - Create assignments
- `DELETE /api/assignments/*` - Remove assignments
- `GET /api/export/*` - Export data

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License.

## 🙏 Acknowledgments

- Bootstrap for UI components
- Lucide React for beautiful icons
- Zustand for simple state management
- React Router for routing
- Vite for blazing fast builds

## 📞 Support

For issues or questions:
- Create an issue in the repository
- Contact the development team

---

**Happy coding! 🎉**
