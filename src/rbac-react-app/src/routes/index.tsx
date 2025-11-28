import { lazy } from 'react';
import { RouteObject } from 'react-router-dom';

// Lazy load components
const Dashboard = lazy(() => import('@/pages/Dashboard'));
const Users = lazy(() => import('@/pages/Users'));
const Groups = lazy(() => import('@/pages/Groups'));
const Roles = lazy(() => import('@/pages/Roles'));
const Permissions = lazy(() => import('@/pages/Permissions'));
const Assignments = lazy(() => import('@/pages/Assignments'));
const Export = lazy(() => import('@/pages/Export'));
const Login = lazy(() => import('@/pages/Login'));
const NotFound = lazy(() => import('@/pages/NotFound'));

export const routes: RouteObject[] = [
  {
    path: '/',
    element: lazy(() => import('@/layouts/MainLayout')),
    children: [
      {
        index: true,
        element: Dashboard,
      },
      {
        path: 'users',
        element: Users,
      },
      {
        path: 'groups',
        element: Groups,
      },
      {
        path: 'roles',
        element: Roles,
      },
      {
        path: 'permissions',
        element: Permissions,
      },
      {
        path: 'assignments',
        element: Assignments,
      },
      {
        path: 'export',
        element: Export,
      },
    ],
  },
  {
    path: '/login',
    element: Login,
  },
  {
    path: '*',
    element: NotFound,
  },
];

// Route configuration with metadata
export const routeConfig = {
  dashboard: {
    path: '/',
    title: 'Dashboard',
    icon: 'LayoutDashboard',
    showInNav: true,
  },
  users: {
    path: '/users',
    title: 'Users',
    icon: 'Users',
    showInNav: true,
  },
  groups: {
    path: '/groups',
    title: 'Groups',
    icon: 'FolderTree',
    showInNav: true,
  },
  roles: {
    path: '/roles',
    title: 'Roles',
    icon: 'Shield',
    showInNav: true,
  },
  permissions: {
    path: '/permissions',
    title: 'Permissions',
    icon: 'Key',
    showInNav: true,
  },
  assignments: {
    path: '/assignments',
    title: 'Assignments',
    icon: 'Link',
    showInNav: true,
  },
  export: {
    path: '/export',
    title: 'Export Data',
    icon: 'Download',
    showInNav: true,
  },
  login: {
    path: '/login',
    title: 'Login',
    icon: 'LogIn',
    showInNav: false,
  },
};

export type RouteKey = keyof typeof routeConfig;
