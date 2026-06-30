import axios, { AxiosError, type InternalAxiosRequestConfig } from "axios";
import type { RegisterRequest, LoginRequest, AuthResponse } from "@/types/auth";

const BASE_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5000";

export const apiClient = axios.create({
  baseURL: BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
  timeout: 10_000,
});

// Request interceptor — attach JWT from localStorage if present
apiClient.interceptors.request.use((config) => {
  if (typeof window !== "undefined") {
    const token = localStorage.getItem("accessToken");
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
  }
  return config;
});

// ── 401 → refresh token → retry akışı ────────────────────────────────────────

type RetriableConfig = InternalAxiosRequestConfig & { _retry?: boolean };

const REFRESH_URL = "/api/auth/refresh";
let refreshPromise: Promise<string> | null = null;

function redirectToLogin() {
  clearTokens();
  if (typeof window !== "undefined") {
    window.location.href = "/login";
  }
}

/** Tek seferde tek refresh isteği — eşzamanlı 401'ler aynı promise'i paylaşır. */
function refreshAccessToken(): Promise<string> {
  if (!refreshPromise) {
    refreshPromise = (async () => {
      const storedRefreshToken =
        typeof window !== "undefined" ? localStorage.getItem("refreshToken") : null;

      if (!storedRefreshToken) {
        throw new Error("Refresh token bulunamadı.");
      }

      const res = await axios.post<AuthResponse>(`${BASE_URL}${REFRESH_URL}`, {
        refreshToken: storedRefreshToken,
      });

      saveTokens(res.data.accessToken, res.data.refreshToken);
      return res.data.accessToken;
    })().finally(() => {
      refreshPromise = null;
    });
  }

  return refreshPromise;
}

// Response interceptor — 401'de refresh dener, başarısızsa login'e yönlendirir
apiClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError<{ message?: string; errors?: Record<string, string[]> }>) => {
    const originalRequest = error.config as RetriableConfig | undefined;

    const isRefreshCall = originalRequest?.url?.includes(REFRESH_URL);

    if (error.response?.status === 401 && originalRequest && !originalRequest._retry && !isRefreshCall) {
      originalRequest._retry = true;
      try {
        const newAccessToken = await refreshAccessToken();
        originalRequest.headers = originalRequest.headers ?? {};
        originalRequest.headers.Authorization = `Bearer ${newAccessToken}`;
        return apiClient(originalRequest);
      } catch {
        redirectToLogin();
        return Promise.reject(new Error("Oturum süresi doldu, lütfen tekrar giriş yapın."));
      }
    }

    if (error.response?.status === 401 && isRefreshCall) {
      redirectToLogin();
    }

    const message =
      error.response?.data?.message ??
      error.message ??
      "Sunucu ile bağlantı kurulamadı.";
    return Promise.reject(new Error(message));
  }
);

// ── Auth endpoints ──────────────────────────────────────────────────────────

export async function register(data: RegisterRequest): Promise<AuthResponse> {
  const res = await apiClient.post<AuthResponse>("/api/auth/register", data);
  return res.data;
}

export async function login(data: LoginRequest): Promise<AuthResponse> {
  const res = await apiClient.post<AuthResponse>("/api/auth/login", data);
  return res.data;
}

export async function logout(): Promise<void> {
  const refreshToken = typeof window !== "undefined" ? localStorage.getItem("refreshToken") : null;
  if (!refreshToken) return;
  await apiClient.post("/api/auth/logout", { refreshToken });
}

// ── Token helpers ───────────────────────────────────────────────────────────

export function saveTokens(accessToken: string, refreshToken: string) {
  localStorage.setItem("accessToken", accessToken);
  localStorage.setItem("refreshToken", refreshToken);
}

export function clearTokens() {
  localStorage.removeItem("accessToken");
  localStorage.removeItem("refreshToken");
}

export function isAuthenticated(): boolean {
  if (typeof window === "undefined") return false;
  return !!localStorage.getItem("accessToken");
}
