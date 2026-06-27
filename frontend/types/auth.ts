export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface UserDto {
  id: string;
  username: string;
  email: string;
  slug: string;
  createdAt: string;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  user: UserDto;
}

export interface ApiError {
  message: string;
  errors?: Record<string, string[]>;
}

export interface Result<T> {
  isSuccess: boolean;
  value?: T;
  error?: string;
}
