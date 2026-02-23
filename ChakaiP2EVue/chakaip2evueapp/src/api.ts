// api.ts
import axios, { AxiosError } from 'axios';
import type { AxiosInstance } from 'axios';
import router from './router/router.ts'

// ----- URL fallback chain -----
const BASE_URLS = [
  'http://localhost/api',
  'http://chad-desktop.local/api',
  'https://unlamentable-unscourging-santos.ngrok-free.dev/api'
];

// Load last successful URL if available
const savedUrl = localStorage.getItem('lastWorkingApiUrl');
let currentUrlIndex = savedUrl ? BASE_URLS.indexOf(savedUrl) : 0;

// Safety check
if (currentUrlIndex < 0) currentUrlIndex = 0;

// Track which URLs we have attempted for this request
let triedCount = 0;

// Create axios instance
const api: AxiosInstance = axios.create({
  baseURL: BASE_URLS[currentUrlIndex],
  headers: { 'Content-Type': 'application/json' },
});

// ---- REQUEST INTERCEPTOR ----
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('authToken');
    if (token) {
      config.headers['Authorization'] = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// ---- RESPONSE INTERCEPTOR ----
api.interceptors.response.use(
  (response) => {
    // Reset fallback counter on successful call
    triedCount = 0;

    // Save new working URL if needed
    const currentBaseUrl = api.defaults.baseURL;
    const savedBaseUrl = localStorage.getItem('lastWorkingApiUrl');

    if (currentBaseUrl !== savedBaseUrl) {
      localStorage.setItem('lastWorkingApiUrl', currentBaseUrl || '');
    }

    return response;
  },

  async (error: AxiosError) => {
    const originalRequest = error.config;

    if (!originalRequest || (originalRequest as any)._retry) {
      return Promise.reject(error);
    }

    const status = error.response?.status;
    const shouldFallback = status === 404 || status === 500 || status === 502 || status === undefined;

    // Attempt fallback *only up to total number of URLs*
    if (shouldFallback && triedCount < BASE_URLS.length - 1) {
      triedCount++;

      // Move forward in the list and wrap around
      currentUrlIndex = (currentUrlIndex + 1) % BASE_URLS.length;

      const nextUrl = BASE_URLS[currentUrlIndex];
      console.warn(
        `API failed (${status}) — switching to ${nextUrl}`
      );

      api.defaults.baseURL = nextUrl;
      localStorage.setItem('lastWorkingApiUrl', nextUrl);

      (originalRequest as any)._retry = true;
      return api(originalRequest);
    }

    // Skip redirect logic for POST /api/accounts
    if (originalRequest.url === '/api/accounts') {
      return Promise.reject(error);
    }

    // Unauthorized (401)
    if (status === 401) {
      console.log("Unauthorized! Redirecting to login...");
      localStorage.removeItem('authToken');
      router.push('/login');
    }

    return Promise.reject(error);
  }
);

export default api;
