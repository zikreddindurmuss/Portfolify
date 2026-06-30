import { LoginForm } from "@/components/auth/LoginForm";

export const metadata = {
  title: "Giriş Yap — Portfolify",
};

export default function LoginPage() {
  return (
    <main className="auth-bg flex min-h-screen items-center justify-center px-4">
      <div className="w-full max-w-md animate-fade-in space-y-6">
        <div className="text-center">
          <a href="/" className="text-2xl font-extrabold tracking-tight text-primary">
            Portfolify
          </a>
        </div>
        <LoginForm />
      </div>
    </main>
  );
}
