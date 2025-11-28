/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_API_URL: string;
  readonly VITE_API_TIMEOUT: string;
  readonly VITE_API_DEBUG: string;
  readonly VITE_APP_NAME: string;
  readonly VITE_APP_VERSION: string;
  readonly VITE_APP_ENV: string;
  readonly VITE_AUTH_TOKEN_KEY: string;
  readonly VITE_SESSION_TIMEOUT: string;
  readonly VITE_REMEMBER_ME_DAYS: string;
  readonly VITE_FEATURE_EXPORT: string;
  readonly VITE_FEATURE_BULK_OPERATIONS: string;
  readonly VITE_FEATURE_USER_PERMISSIONS_VIEW: string;
  readonly VITE_FEATURE_ADVANCED_SEARCH: string;
  readonly VITE_DEFAULT_THEME: string;
  readonly VITE_DEFAULT_PAGE_SIZE: string;
  readonly VITE_MAX_PAGE_SIZE: string;
  readonly VITE_MOBILE_SIDEBAR_COLLAPSED: string;
  readonly VITE_TOAST_DURATION: string;
  readonly VITE_TOAST_POSITION: string;
  readonly VITE_LAZY_LOADING: string;
  readonly VITE_API_CACHE_DURATION: string;
  readonly VITE_ENABLE_PWA: string;
  readonly VITE_SHOW_ERROR_DETAILS: string;
  readonly VITE_REACT_STRICT_MODE: string;
  readonly VITE_HMR: string;
  readonly VITE_ENABLE_MONITORING: string;
  readonly VITE_FORCE_HTTPS: string;
  readonly VITE_DEFAULT_LOCALE: string;
  readonly VITE_AVAILABLE_LOCALES: string;
  // Add more env variables as needed
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}