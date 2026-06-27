import axios, { AxiosError } from "axios";
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

// Response interceptor — normalise errors
apiClient.interceptors.response.use(
  (response) => response,
  (error: AxiosError<{ message?: string; errors?: Record<string, string[]> }>) => {
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
